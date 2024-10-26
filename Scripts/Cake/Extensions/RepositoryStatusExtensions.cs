using System;
using System.Linq;
using Cake.Core.IO;
using LibGit2Sharp;

namespace Cake.NekoBoiNick.CoreKeeperMods.Extensions;

/// <summary>
/// Extensions for the class <see cref="RepositoryStatus"/>.
/// </summary>
internal static class RepositoryStatusExtensions {
  public static bool IsFileTracked(this RepositoryStatus? statuses, FilePath? file, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(file);
    ArgumentNullException.ThrowIfNull(context);

    return !statuses.IsFileIgnored(file, context);
  }
  public static bool IsFileTracked(this RepositoryStatus? statuses, IFile? file, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(file);

    return !statuses.IsFileIgnored(file, context);
  }
  public static bool IsFileIgnored(this RepositoryStatus? statuses, FilePath? file, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(file);
    ArgumentNullException.ThrowIfNull(context);

    return statuses.IsFileIgnored(context.FileSystem.GetFile(file), context);
  }
  public static bool IsFileIgnored(this RepositoryStatus? statuses, IFile? file, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(file);
    ArgumentNullException.ThrowIfNull(context);

    return statuses.Ignored.Any(x => x.FilePath == file.Path.MakeRelative(context.RepositoryPath));
  }
  public static bool IsDirectoryTracked(this RepositoryStatus? statuses, DirectoryPath? directory, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(directory);
    ArgumentNullException.ThrowIfNull(context);

    return !statuses.IsDirectoryIgnored(directory, context);
  }
  public static bool IsDirectoryTracked(this RepositoryStatus? statuses, IDirectory? directory, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(directory);

    return !statuses.IsDirectoryIgnored(directory, context);
  }
  public static bool IsDirectoryIgnored(this RepositoryStatus? statuses, DirectoryPath? directory, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(directory);
    ArgumentNullException.ThrowIfNull(context);

    return statuses.IsDirectoryIgnored(context.FileSystem.GetDirectory(directory), context);
  }
  public static bool IsDirectoryIgnored(this RepositoryStatus? statuses, IDirectory? directory, BuildContext? context) {
    if (statuses is null) {
      return false;
    }

    ArgumentNullException.ThrowIfNull(directory);
    ArgumentNullException.ThrowIfNull(context);

    return statuses.Ignored.Any(x =>
      directory.Path.MakeRelative(context.RepositoryPath).Contains(context, x.FilePath)
    );
  }
}
