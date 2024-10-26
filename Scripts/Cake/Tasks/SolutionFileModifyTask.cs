using System.Linq;
using System.Text;
using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.FileHelpers;
using Cake.Frosting;
using Cake.NekoBoiNick.CoreKeeperMods.Enums;
using Cake.MSBuildSolutionParser;
using Cake.MSBuildSolutionParser.Enums;
using System;
using Cake.Core.IO;
using Cake.Cli;

namespace Cake.NekoBoiNick.CoreKeeperMods.Tasks;
[TaskName("SolutionFileModify")]
[IsDependentOn(typeof(RestoreTask))]
public sealed class SolutionFileModifyTask : FrostingTask<BuildContext> {
  public bool      HasErrored         { get; private set; }
  public FilePath? SolutionFile       { get; private set; }
  public FilePath? SolutionBackupFile { get; private set; }

  public override bool ShouldRun(BuildContext context) {
    return context.MSBuildTask is MSBuildTask.Restore or MSBuildTask.Rebuild;
  }

  public override void Run(BuildContext context) {
    SolutionFile = context.SolutionDirectoryPath.CombineWithFilePath($"{context.SolutionDirectoryPath.GetDirectoryName()}.sln");
    if (!context.FileExists(SolutionFile)) {
      context.Log.Warning($"Solution file at {SolutionFile} does not exist.");
      return;
    }
    SolutionBackupFile = SolutionFile.GetDirectory().CombineWithFilePath(context.File(SolutionFile.GetFilename() + ".bak"));
    if (!context.FileSystem.GetFile(SolutionBackupFile).Exists) {
      context.CopyFile(SolutionFile, SolutionBackupFile);
    }
    var solution = context.DeserializeSolution(SolutionFile);
    var solutionItems = new SolutionFolder(context.SolutionItemsGuid, "Solution Items", "Solution Items");
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath(".gitignore"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath(".gitattributes"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath(".gitmodules"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath("cspell.yaml"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath("LICENSE"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath("README.md"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath("OpenProject.sh"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath("OpenProject.bat"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.GetParent().CombineWithFilePath("OpenProject.ps1"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.CombineWithFilePath(".editorconfig"));
    solutionItems.AddSolutionItem(context.SolutionDirectoryPath.CombineWithFilePath("Directory.Build.props"));
    var modProjects = context.FileSystem.GetDirectory(context.SolutionDirectoryPath.Combine("Assets").Combine("Mods")).GetDirectories("*", Core.IO.SearchScope.Current).Select(x => x.Path).Distinct().ToList();
    var hiddenItems = new SolutionFolder(context.HiddenItemsGuid, "Hidden", "Hidden");
    var projectsToHide = solution.Projects.Where(projectItem => !modProjects.Any(x => x.GetDirectoryName().Equals(projectItem.Name, System.StringComparison.OrdinalIgnoreCase))).ToList();
    context.Log.Information($"projectsToHide.Count = {projectsToHide.Count}");
    solution.NestProjectsIn(projectsToHide, hiddenItems);
    solution.AddProject(new SolutionProject(context.ScriptsGuid, "Scripts", context.CakeProjectFile, VisualStudioProjectTypeGuid.CSharp_Net_Core));
    solution.AddProject(solutionItems);
    solution.AddProject(hiddenItems);
    var solutionText = context.SerializeSolution(solution, SolutionFile);
    // context.Log.Information(solutionText);
    context.FileWriteText(SolutionFile, Encoding.UTF8, solutionText);
  }

  public override void OnError(Exception exception, BuildContext context) {
    HasErrored = true;
    context.Log.LogException(exception);
  }

  public override void Finally(BuildContext context) {
    if (HasErrored && SolutionFile is not null && SolutionBackupFile is not null) {
      context.Log.Warning("Restoring backup of solution file.");
      context.MoveFile(SolutionBackupFile, SolutionFile);
    } else if (!HasErrored) {
      context.DeleteFile(SolutionBackupFile);
    }
  }
}
