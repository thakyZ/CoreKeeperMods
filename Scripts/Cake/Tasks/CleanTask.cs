using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cake.Cli;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.Git;
using Cake.NekoBoiNick.CoreKeeperMods.Enums;
using Cake.NekoBoiNick.CoreKeeperMods.Extensions;
using LibGit2Sharp;
using NekoBoiNick.CoreKeeperMods.Scripts.Cake.Constructs;
using Spectre.Console;
using LogLevel = Cake.Core.Diagnostics.LogLevel;

namespace Cake.NekoBoiNick.CoreKeeperMods.Tasks;

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext> {
  public override bool ShouldRun(BuildContext context) {
    return context.MSBuildTask is MSBuildTask.Clean or MSBuildTask.Rebuild;
  }

  /// <summary>
  /// GLobs of directories to clean
  /// </summary>
  private static List<string> DirectoriesToClean => [
    "SDK Mods/Library/",
    "SDK Mods/DTAR_*/",
    "SDK Mods/Logs/",
    "SDK Mods/.vs/",
    "SDK Mods/obj/",
    "SDK Mods/Temp/",
    "SDK Mods/Assets/Plugins/CoreKeeper/",
  ];

  /// <summary>
  ///
  /// </summary>
  [MemberNotNullWhen(true, nameof(RepositoryStatus))]
  private bool ValidRepository { get; set; }

  /// <summary>
  ///
  /// </summary>
  private RepositoryStatus? RepositoryStatus { get; set; } = null;

  private IEnumerable<Path> CleanDirectory(BuildContext context, IDirectory directory) {
    if (ValidRepository && RepositoryStatus.IsDirectoryIgnored(directory, context)) {
      yield return directory.Path;
    } else {
      foreach (var innerDirectory in directory.GetDirectories("", SearchScope.Current)) {
        if (ValidRepository && RepositoryStatus.IsDirectoryIgnored(innerDirectory, context)) {
          yield return innerDirectory.Path;
        } else {
          foreach (var path in CleanDirectory(context, innerDirectory)) {
            yield return path;
          }
        }
      }

      foreach (var file in directory.GetFiles("", SearchScope.Current)) {
        if (ValidRepository && RepositoryStatus.IsFileIgnored(file, context)) {
          yield return file.Path;
        }
      }
    }
  }

  public override void Run(BuildContext context) {
    // Check if the directory is a valid repository.
    ValidRepository = context.GitIsValidRepository(context.RepositoryPath);
    // Get the statuses of all files in a directory.
    RepositoryStatus = ValidRepository ? context.GitGetRepositoryStatus(context.RepositoryPath) : null;
    context.Log.Information("Gettings paths to delete...");
    List<Path> pathsToDelete = [];
    var repositoryDirectory = context.FileSystem.GetDirectory(context.RepositoryPath);
    using (var _ = context.ChangeWorkingDirectory(context.RepositoryPath)) {
      foreach (var glob in DirectoriesToClean) {
        foreach (var directory in context.Globber.GetDirectories(new GlobPattern(glob)).Select((directoryPath) => context.FileSystem.GetDirectory(directoryPath))) {
          pathsToDelete.AddRange(CleanDirectory(context, directory));
        }
      }
    }
    context.Log.Information("Found {0} paths to delete...", pathsToDelete.Count);
    var progress = AnsiConsole.Progress();
    progress = progress.Columns(
      new TaskDescriptionColumn(),
      new ProgressBarColumn(),
      new PercentageColumn(),
      new ElapsedTimeColumn()
    );
    var output = progress.Start((ProgressContext pContext) => {
      var task = pContext.AddTask("Deleting Files", true, pathsToDelete.Count);
      var output = new ProgressReturnedCollection<Path>("Failed to delete \"{0}\"", context.Log);
      foreach (var path in pathsToDelete) {
        IProgressReturned<Path> entry = output.AddEntry(path);
        try {
          path.Delete(context);
          entry.AddMessage($"Deleted \"{path}\"...");
        } catch (Exception exception) {
          entry.SetException(exception);
        } finally {
          task.Increment(1);
        }
      }
      return output;
    });
    output.PrintFailed();
    output.PrintExceptions();
    output.PrintMessages();
  }
}
