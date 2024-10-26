using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cake.Cli;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.MSBuildSolutionParser.Project;
using Cake.NekoBoiNick.CoreKeeperMods.Enums;
using Cake.NekoBoiNick.CoreKeeperMods.Extensions;
using NekoBoiNick.CoreKeeperMods.Scripts.Cake.Constructs;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;
using Spectre.Console;

namespace Cake.NekoBoiNick.CoreKeeperMods.Tasks;

[TaskName("CsProjectFileModify")]
[IsDependentOn(typeof(SolutionFileModifyTask))]
public sealed class CsProjectFileModifyTask : AsyncFrostingTask<BuildContext> {
  public List<FilePath> ModProjects { get; } = [];
  public List<FilePath> UnityProjects { get; set; } = [];

  public override bool ShouldRun(BuildContext context) {
    return context.MSBuildTask is MSBuildTask.Restore or MSBuildTask.Rebuild;
  }

  public static async Task<ProgressReturned<ReturnRecord>> RunUnityProjectTaskAsync(BuildContext context, IFile file) {
    await Task.Delay(0); // Used to ignore CS1998
    var project = context.DeserializeProject(file.Path, context.SolutionFilePath);
    return new ProgressReturned<ReturnRecord>(new ReturnRecord());
  }

  public static async Task<ProgressReturned<ReturnRecord>> RunModProjectTaskAsync(BuildContext context, IFile file) {
    await Task.Delay(0); // Used to ignore CS1998
    var project = context.DeserializeProject(file.Path, context.SolutionFilePath);
    return new ProgressReturned<ReturnRecord>(new ReturnRecord());
  }

  public static async Task<ProgressReturned<ReturnRecord>> RunOtherProjectTaskAsync(BuildContext context, IFile file) {
    await Task.Delay(0); // Used to ignore CS1998
    var project = context.DeserializeProject(file.Path, context.SolutionFilePath);
    return new ProgressReturned<ReturnRecord>(new ReturnRecord());

  }

  /// <summary>
  /// A record for the return of all tasks in this class.
  /// </summary>
  public readonly struct ReturnRecord {
    [SuppressMessage("Naming", "AMNF0001:Awaitable (Asynchronous) methods should be suffixed with 'Async'", Justification = "Wait until fix: https://github.com/priyanshu92/AsyncMethodNameFixer/issues/26")]
    public Task         Task         { get; }
    public FilePath     ProjectPath  { get; }
    public ProgressTask ProgressTask { get; }
    public ReturnRecord(Task task, FilePath projectPath, ProgressTask progressTask) {
      this.Task         = task;
      this.ProjectPath  = projectPath;
      this.ProgressTask = progressTask;
    }
  }
 
  /// <summary>
  /// Runs the task using the specified context.
  /// </summary>
  /// <param name="context">The context.</param>
  /// <returns>A <see cref="System.Threading.Tasks.Task" /> representing the asynchronous operation.</returns>
  public override async Task RunAsync(BuildContext context) {
    await Task.Delay(0); // Used to ignore CS1998
    // The completion source of the tasks.
    var completionSource = new TaskCompletionSource();
    // List of cached tasks.
    List<(Task Task, ProgressTask ProgressTask)> tasks = [];
    // TODO: remove Pug.UnityExtensions.dll from Unity.ShaderGraph.Editor.csproj
    // TODO: remove Animation.Components.dll from Unity.RenderPipelines.Universal.Editor.csproj
    // TODO: remove Pug.ECS.Extensions.dll from Unity.Physics.Custom.csproj
    // TODO: remove Pug.Objects.dll from Mischief.MDV.Editor.csproj
    // TODO: remove all references to libraries in Assets/Plugins/CoreKeeper/ from any unity-related file project

    try {
      context.Log.Information("Determining project types.");
      List<string> modDirectories = [..context.FileSystem.GetDirectory(context.SolutionDirectoryPath.Combine("Assets", "Mods")).GetDirectories("", SearchScope.Current).Select(x => x.Path.GetDirectoryName())];
      foreach (IFile file in context.FileSystem.GetDirectory(context.SolutionDirectoryPath).GetFiles("*.csproj", SearchScope.Current)) {
        string fileBaseName = file.Path.GetFilenameWithoutExtension().ToString();
        if (modDirectories.Any(x => x.Equals(fileBaseName, StringComparison.OrdinalIgnoreCase))) {
          ModProjects.Add(file.Path);
        }
        if (fileBaseName.ContainsAny(["Unity" /*, "Assembly-CSharp-Editor", "Mischief.MDV.Editor", "ModSDK.Editor" */], StringComparison.OrdinalIgnoreCase)) {
          UnityProjects.Add(file.Path);
        }
      }

      if (context.Debug) {
        context.Log.Information("Verify that these are in the correct categories:");
        var table = new Table();
        table.AddColumns(new TableColumn("Property"), new TableColumn("Values"));
        var modProjectsRow = new TableRow([new Text(nameof(ModProjects)), new Columns(ModProjects.Select((FilePath file) => new Text(file.FullPath)))]);
        table.AddRow(modProjectsRow);
        var unityProjectsRow = new TableRow([new Text(nameof(UnityProjects)), new Columns(UnityProjects.Select((FilePath file) => new Text(file.FullPath)))]);
        table.AddRow(unityProjectsRow);
        AnsiConsole.Write(table);
        if (!AnsiConsole.Prompt(new ConfirmationPrompt("Are these correct?"))) {
          completionSource.SetCanceled();
        }
      }

      /*
      var files = context.FileSystem.GetDirectory(context.SolutionDirectoryPath).GetFiles("*.csproj", SearchScope.Current).ToList();
      total = files.Count;
      var progress = AnsiConsole.Progress();
      progress = progress.Columns(
        new TaskDescriptionColumn(),
        new ProgressBarColumn(),
        new PercentageColumn(),
        new ElapsedTimeColumn()
      );
      await progress.StartAsync(async (ProgressContext pContext) => {
        var output = new ProgressReturnedCollection<(FilePath ProjectPath, Task Task, ProgressTask ProgressTask)>("Failed to modify project at \"{0}\"...", context.Log);
        // ReSharper disable once LoopCanBeConvertedToQuery
        var populating = pContext.AddTask("Populating tasks...", false);
        populating.IsIndeterminate = true;
        foreach (IFile file in files) {
          if (!file.Path.GetExtension().Equals(".csproj", StringComparison.OrdinalIgnoreCase)) {
            context.Log.Warning("Tried parsing a non C# Project file.");
            continue;
          }
          Task task;
          var  progressTask = pContext.AddTask($"Modifying {file.Path.GetFilenameWithoutExtension()}...", false);
          if (UnityProjects.Contains(file.Path)) {
            task = new Task(async () => {
              await RunUnityProjectTaskAsync(context, file);
            });
          } else if (!ModProjects.Contains(file.Path)) {
            task = new Task(async () => {
              await RunModProjectTaskAsync(context, file);
            });
          } else {
            task = new Task(async () => {
              await RunOtherProjectTaskAsync(context, file);
            });
          }
          tasks.Add((task, progressTask));
          tasks.ForEach(x => {
            x.Task.Start();
          });
        }

        while (tasks.Count > 0)
        {
          i = Task.WaitAny(tasks.ToArray());
          Task task = tasks[i];
          tasks.RemoveAt(i);
          if (task.Exception is not null)
          {
            context.Log.LogException(task.Exception);
          }
          progress++;
          var percentage = GetProgressPercentage(total, progress);
          if (percentage != lastPercentage)
          {
            await messageBus.PublishAsync(new ModReportExportEvent(1, percentage));
          }
          lastPercentage = percentage;
        }
        for (i=0; i < reports.Count; i++)
        {
          reports[i].Reports = reports2[i].ToList();
        }
      }
      */
    } catch (Exception exception) {
      completionSource.SetException(exception);
    }
  }
}
