using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileSystemGlobbing;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.NekoBoiNick.CoreKeeperMods.Enums;
using Cake.NekoBoiNick.CoreKeeperMods.Exceptions;
using Cake.NekoBoiNick.CoreKeeperMods.Extensions;
using Cake.SevenZip;
using Cake.SevenZip.Builder;
using Cake.SevenZip.Parsers;
using Cake.SevenZip.Switches;
using Spectre.Console;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;

// cSpell:words Metafile
// Ignore Spelling: Metafile

namespace Cake.NekoBoiNick.CoreKeeperMods;
public class BuildContext : FrostingContext {
  [SuppressMessage("Performance", "CA1822")]
  private List<string> DrivesToCheck => [..System.IO.DriveInfo.GetDrives().Select(drive => drive.ToString()).Where(drive => drive.StartsWithAny(["A","B","C","D","E"], StringComparison.OrdinalIgnoreCase))];
  [SuppressMessage("Performance", "CA1822")]
  private List<string> GlobsToCheck  => [
    "./Steam*/SteamApps/common/Core Keeper/",
    "./Program Files (x86)/Steam/SteamApps/common/Core Keeper/",
  ];
  // ReSharper disable InconsistentNaming
  private const string SDK_ZIP_PATH       = "Assets/ModSDK/EditorAssemblies.zip";
  private const string METAFILE_ZIP_PATH  = "Assets/ModSDK/Data/MetaFiles.zip";
  private const string GAME_NAME          = "CoreKeeper";
  private const int    CURRENT_VERSION    = 2;
  private const string SETTINGS_PATH      = "Assets/ModSDK/ModSDKSettings.asset";
  private const string SDK_ASSEMBLY_PATH  = $"Assets/Plugins/{GAME_NAME}ModSDK/";
  private const string GAME_ASSEMBLY_PATH = $"Assets/Plugins/{GAME_NAME}/";
  // ReSharper restore InconsistentNaming
  public bool           Debug                 { get; }
  [SuppressMessage("Performance", "CA1822")]
  public string         ScriptsGuid           => "{10b6c8a7-f8f2-4214-8234-c079a894d4cc}";
  [SuppressMessage("Performance", "CA1822")]
  public string         SolutionItemsGuid     => "{D422557C-9B44-4447-A638-4F70D466C951}";
  [SuppressMessage("Performance", "CA1822")]
  public string         HiddenItemsGuid       => "{00914974-C1E5-4587-A22A-0EFB81E3B164}";
  public DirectoryPath  GameDirectory         { get;}
  public MSBuildTask    MSBuildTask           { get; }
  public FilePath       SdkZipPath            { get; }
  public FilePath       MetafileZipPath       { get; }
  [SuppressMessage("Performance", "CA1822")]
  public string         GameName              => GAME_NAME;
  [SuppressMessage("Performance", "CA1822")]
  public int            CurrentVersion        => CURRENT_VERSION;
  public FilePath      SettingsPath          { get; }
  public DirectoryPath SolutionDirectoryPath { get; }
  public FilePath      SolutionFilePath      { get; }
  public DirectoryPath CakeProjectRoot       { get; }
  public DirectoryPath RepositoryPath        { get; }
  public FilePath      CakeProjectFile       { get; }
  public DirectoryPath SdkAssemblyPath       { get; }
  public DirectoryPath GameAssemblyPath      { get; }

  public BuildContext(ICakeContext context) : base(context) {
    Debug         = context.Arguments.HasArgument("Debug");
    MSBuildTask   = ParseArgumentsAsEnum<MSBuildTask>(context.Arguments);
    GameDirectory = GetGamePath();
    // ReSharper disable VirtualMemberCallInConstructor
    SolutionDirectoryPath = GetSolutionFileDirectory();
    SolutionFilePath      = SolutionDirectoryPath.CombineWithFilePath(this.File(SolutionDirectoryPath.GetDirectoryName() + ".sln"));
    CakeProjectFile       = GetCakeProject();
    CakeProjectRoot       = CakeProjectFile.GetDirectory();
    RepositoryPath        = CakeProjectRoot.GetParent();
    SdkZipPath            = SolutionDirectoryPath.CombineWithFilePath(this.File(SDK_ZIP_PATH));
    MetafileZipPath       = SolutionDirectoryPath.CombineWithFilePath(this.File(METAFILE_ZIP_PATH));
    SettingsPath          = SolutionDirectoryPath.CombineWithFilePath(this.File(SETTINGS_PATH));
    SdkAssemblyPath       = SolutionDirectoryPath.Combine(this.Directory(SDK_ASSEMBLY_PATH));
    GameAssemblyPath      = SolutionDirectoryPath.Combine(this.Directory(GAME_ASSEMBLY_PATH));
  }

  public UsingChangeWorkingDirectory ChangeWorkingDirectory(DirectoryPath newDirectoryPath) {
    return new UsingChangeWorkingDirectory(newDirectoryPath, this);
  }

  private T? ParseArguments<T>(ICakeArguments arguments) {
    Type t = typeof(T);
    if (t.IsEnum) {
      throw new InvalidOperationException();
    }
    return default;
  }

  private static TEnum ParseArgumentsAsEnum<TEnum>(ICakeArguments arguments) where TEnum : struct, Enum {
    TEnum output = default;
    foreach (string argumentKey in arguments.GetArguments().Keys) {
      if (TryGetFromValue(argumentKey, out TEnum result1)) {
        return result1;
      }
      if (Enum.TryParse(arguments.GetArgument(argumentKey), out TEnum result2)) {
        return result2;
      }
    }
    return output;
  }

  private static bool TryGetFromValue<TEnum>(string name, out TEnum result) where TEnum : struct, Enum {
    result = default;
    List<TEnum> values = [..Enum.GetValues<TEnum>().Where((TEnum @enum) => @enum.ToString().Equals(name, StringComparison.OrdinalIgnoreCase))];

    if (values.Count == 0) {
      return false;
    }

    result = values[0];
    return true;
  }

  private DirectoryPath GetGamePath() {
    var matcher = new Matcher();
    matcher.AddIncludePatterns(GlobsToCheck);
    DirectoryPath? output = DrivesToCheck.SelectMany(a => matcher.GetResultsInFullPath(a))
      .Select(b => (FilePath)this.File(b))
      .Select(c => c.GetDirectory())
      .Where(d => d.GetDirectoryName().Equals("Core Keeper", StringComparison.OrdinalIgnoreCase))
      .FirstOrDefault(e => VerifyPath(e));
    string? prompt;
    if (output is null) {
      prompt = AnsiConsole.Ask<string?>("Input path to game directory:", null);
    } else {
      prompt = AnsiConsole.Ask<string?>("Input path to game directory:", output.FullPath);
      if (prompt is null) {
        InvalidGamePathDirectoryException.ThrowPromptNull(nameof(prompt));
      }
      output = this.Directory(prompt);
      DirectoryNotFoundException.ThrowIfNotFound(output, this);
    }
    return output ?? throw new NullReferenceException("soup");
  }

  private bool VerifyPath(DirectoryPath path, bool silent = false) {
    if (!this.DirectoryExists(path))  {
      if (!silent) {
        Log.Warning($"{path} doesn't exist");
      }
      return false;
    }

    if (this.FileExists(path.CombineWithFilePath("CoreKeeper.exe"))) {
      return true;
    }

    if (this.FileExists(path.CombineWithFilePath("CoreKeeperServer.exe"))) {
      return true;
    }

    if (this.DirectoryExists(path.Combine("Assets"))) {
      // Path looks like Unity project, accept this for dev work
      return true;
    }

    if (!silent) {
      Log.Error("Chosen path does not contain the Core Keeper game.");
    }

    return false;
  }

  public List<FilePath> ListFilesInZip(FilePath zipPath) {
    List<FilePath> result = [];
    this.SevenZip((CommandBuilder s)
      => s.InListMode()
          .WithArchive(zipPath)
          .WithCommandOutput((IListOutput output)
            => result = output.Archives
                              .SelectMany((IArchiveListOutput archive) => archive.Files)
                              .Select((IArchivedFileListOutput files) => (FilePath)this.File(files.Name)).ToList()));
    return result;
  }

  public void ExtractFileTo(FilePath zipPath, FilePath internalFile, FilePath destinationPath, bool recurse = false) {
    if (!ListFilesInZip(zipPath).Any(x => x.GetFilename().Equals(internalFile.GetFilename()))) {
      Log.Warning($"Zip file, {zipPath.MakeRelative(SolutionDirectoryPath)} does not contain file {internalFile}");
      return;
    }
    this.SevenZip((CommandBuilder s)
      => s.InExtractMode()
          .WithArchive(zipPath)
          .WithOutputDirectory(destinationPath.GetDirectory())
          .WithOverwriteMode(OverwriteMode.Overwrite)
          .WithIncludeArchiveFilenames(internalFile.GetFilename().ToString()));
    this.MoveFile(destinationPath.GetDirectory().CombineWithFilePath(internalFile.GetFilename()), destinationPath);
  }

  private FilePath WalkDirectoryUpUntilFile(DirectoryPath directory, string fileName, int depth = -1) {
    var count = 0;
    while (true) {
      if (depth != -1 && count >= depth) {
        throw new OverflowException($"The iterations of the method {nameof(WalkDirectoryUpUntilFile)} went past iteration max of {depth} with a total of {count}");
      }

      FilePath potentialFilePath = directory.CombineWithFilePath(fileName);

      if (FileSystem.Exist(potentialFilePath)) {
        return potentialFilePath;
      }

      count++;
      directory = directory.GetParent();
    }
  }

  private DirectoryPath WalkDirectoryUpUntilDirectory(DirectoryPath directory, string directoryName, int depth = -1) {
    var count = 0;
    while (true) {
      if (depth != -1 && count >= depth) {
        throw new OverflowException($"The iterations of the method {nameof(WalkDirectoryUpUntilDirectory)} went past iteration max of {depth} with a total of {count}");
      }

      DirectoryPath potentialDirectoryPath = directory.Combine(directoryName);

      if (FileSystem.Exist(potentialDirectoryPath)) {
        return potentialDirectoryPath;
      }

      count++;
      directory = directory.GetParent();
    }
  }

  private DirectoryPath GetAssemblyDirectory() {
    return this.File(Assembly.GetExecutingAssembly().Location).Path.GetDirectory() ?? throw new System.IO.DirectoryNotFoundException();
  }

  private FilePath GetCakeProject() {
    var assemblyLocation = GetAssemblyDirectory();
    return WalkDirectoryUpUntilFile(assemblyLocation, "scripts.csproj", 4);
  }

  private DirectoryPath GetSolutionFileDirectory() {
    var cakeProjectRoot = GetCakeProject().GetDirectory().GetParent();
    return WalkDirectoryUpUntilDirectory(cakeProjectRoot, "SDK Mods", 2);
  }
}
