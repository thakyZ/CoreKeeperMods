using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.NekoBoiNick.CoreKeeperMods.Constructs;
using Cake.NekoBoiNick.CoreKeeperMods.Enums;

namespace Cake.NekoBoiNick.CoreKeeperMods.Tasks;
[TaskName("Restore")]
public sealed class RestoreTask : FrostingTask<BuildContext> {
  public override bool ShouldRun(BuildContext context) {
    return context.MSBuildTask is MSBuildTask.Restore or MSBuildTask.Rebuild;
  }

  private const string ListCountsNotEqual = "List count of {0} ({1}) does not equal {2} ({3}), this was unexpected.";

  public override void Run(BuildContext context) {
    // Copy the method 'UpdateFromGamePath' from %MSBuildProjectDirectory%\..\SDK Mods\Assets\ModSDK\SDK\Importer\ImporterWindow.cs
    var importerSettings = PseudoImporterSettings.Init(context);
    UpdateFromGamePath(importerSettings, context.GameDirectory, context, out var addedAssemblies, out var assemblyDestPaths);
    if (addedAssemblies.Count != assemblyDestPaths.Count) {
      throw new ArgumentOutOfRangeException(string.Format(ListCountsNotEqual, nameof(addedAssemblies), addedAssemblies.Count, nameof(assemblyDestPaths), assemblyDestPaths.Count), (Exception?)null);
    }
    for (var i = 0; i < addedAssemblies.Count; i++) {
      var addedAssembly = addedAssemblies[i];
      var assemblyDestPath = assemblyDestPaths[i];
      context.Log.Information($"Added Assembly: {addedAssembly.GetFilename()} to {assemblyDestPath.FullPath}");
    }
  }

  /// <summary>
  /// Updates the dlls by snatching them from the game files.
  /// </summary>
  /// <param name="settings">Instance of <see cref="PseudoImporterSettings"/>.</param>
  /// <param name="gamePath">Path to game install folder</param>
  /// <param name="context">The <see cref="BuildContext"/> of cake builder.</param>
  /// <param name="addedAssemblies">Outputs a list of added assembly names.</param>
  /// <param name="assemblyDestPaths">Outputs a list of added assembly destination paths.</param>
  private static void UpdateFromGamePath(PseudoImporterSettings settings, DirectoryPath? gamePath, BuildContext context, out List<FilePath> addedAssemblies, out List<FilePath> assemblyDestPaths) {
    addedAssemblies = [];
    assemblyDestPaths = [];

    if (gamePath is null || string.IsNullOrEmpty(gamePath.FullPath)) {
      return;
    }

    if (!context.DirectoryExists(gamePath)) {
      context.Log.Error(Verbosity.Normal, $"{gamePath.FullPath} doesn't exist");
      return;
    }

    if (!context.FileExists(gamePath.CombineWithFilePath($"{context.GameName}.exe"))) {
      context.Log.Error(Verbosity.Normal, $"failed to found game executable at {gamePath.FullPath}, skipping import");
      return;
    }

    if (!context.DirectoryExists(context.GameAssemblyPath)) {
      context.CreateDirectory(context.GameAssemblyPath);
    }

    var assemblyDir = gamePath.Combine($"{context.GameName}_Data").Combine("Managed");

    var includeRegex = settings.includeGameAssemblies?.ConvertAll(x => new Regex(x)) ?? [];
    var excludeRegex = settings.excludeGameAssemblies?.ConvertAll(x => new Regex(x)) ?? [];

    var assembliesFromSDK = new HashSet<string>();

    if (context.FileExists(context.SdkZipPath)) {
      // Install some non-editor assemblies where we just need the version compiled with UNITY_EDITOR
      // instead of the one pulled from the game files.
      var listTask = context.ListFilesInZip(context.SdkZipPath);
      foreach (var file in listTask) {
        if (file.ToString().EndsWith(".Editor.dll")) {
          continue;
        }

        var destPath = context.GameAssemblyPath.CombineWithFilePath(file);
        context.ExtractFileTo(context.SdkZipPath, file, destPath, true);
        assembliesFromSDK.Add(file.ToString());
      }
    }

    Dictionary<string, FilePath> metaFileLookup = [];

    if (context.FileExists(context.MetafileZipPath)) {
      // Install some non-editor assemblies where we just need the version compiled with UNITY_EDITOR
      // instead of the one pulled from the game files.
      foreach (var file in context.ListFilesInZip(context.MetafileZipPath)) {
        metaFileLookup.Add(file.ToString()[..(file.ToString().Length - ".meta".Length)], file);
      }
    }

    foreach (var assembly in context.GetFiles(assemblyDir.CombineWithFilePath("*.dll").FullPath)) {
      if (assembliesFromSDK.Contains(assembly.GetFilename().ToString())) {
        // We already installed this from SDK, which probably means we need the editor version or things will break in editor
        continue;
      }

      var includeMatch = (from regex in includeRegex where regex.IsMatch(assembly.GetFilename().ToString()) select regex.Match(assembly.GetFilename().ToString())).FirstOrDefault();
      var excludeMatch = (from regex in excludeRegex where regex.IsMatch(assembly.GetFilename().ToString()) select regex.Match(assembly.GetFilename().ToString())).FirstOrDefault();

      var include = settings.includeAllAssemblies == 1;

      if (includeMatch is not null && excludeMatch is not null) {
        // both matches, take closest
        include = includeMatch.Length > excludeMatch.Length;
      } else if (includeMatch is not null) {
        include = true;
      } else if (excludeMatch is not null) {
        include = false;
      }

      if (!include) {
        context.Log.Warning($"Skipping {assembly}");
        continue;
      }

      var sourcePath = assembly.FullPath;
      var destPath = context.GameAssemblyPath.CombineWithFilePath(assembly.GetFilename());
      context.CopyFile(sourcePath, destPath);
      context.Log.Information($"Loaded {assembly} from game files");
      if (metaFileLookup.ContainsKey(assembly.GetFilename().ToString())) {
        var metaFilePath = destPath.FullPath + ".meta";
        if (!File.Exists(metaFilePath)) {
          context.ExtractFileTo(context.SdkZipPath, metaFileLookup[assembly.GetFilename().ToString()], metaFilePath, true);
        }
      }
      assemblyDestPaths.Add(destPath);
      addedAssemblies.Add(assembly.GetFilenameWithoutExtension());
    }

    if (context.FileExists(context.SdkZipPath)) {
      foreach (var file in context.ListFilesInZip(context.MetafileZipPath)) {
        if (!file.GetFilename().ToString().EndsWith(".Editor.dll") ||
          // Special case where this isn't pulled by default and name doesn't match editor dll
          file.GetFilename().ToString().EndsWith("PugText.Editor.dll") && !addedAssemblies.Contains("Pug.Other")) {
          continue;
        }

        // Install everything for now, might want to filter no game files later, but needs smarter check
        //var matchingAssemblyIndex = addedAssemblies.FindIndex(x => zipFile.Name.StartsWith(x));
        //if (matchingAssemblyIndex != -1)
        {
          context.EnsureDirectoryExists(context.SdkAssemblyPath);

          // Found matching editor assembly, install this
          context.ExtractFileTo(context.SdkZipPath, file, context.SdkAssemblyPath.CombineWithFilePath(file.GetFilename()), true);
        }
      }
    }
  }
}
