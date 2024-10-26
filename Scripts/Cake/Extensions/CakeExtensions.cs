using System;
using System.Diagnostics;
using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.IO.Paths;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.NekoBoiNick.CoreKeeperMods.Exceptions;
using LibGit2Sharp;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;

namespace Cake.NekoBoiNick.CoreKeeperMods.Extensions;

internal static class CakeExtensions {
  /// <summary>
  /// Single method to combine multiple directory path segments into a full <see cref="DirectoryPath" />.
  /// </summary>
  /// <param name="directoryPath">The original directory path.</param>
  /// <param name="directoryPaths">The other directory path segments.</param>
  /// <returns>The output and resulting <see cref="DirectoryPath" />.</returns>
  [DebuggerStepThrough]
  public static DirectoryPath Combine(this DirectoryPath directoryPath, params string[] directoryPaths) {
    DirectoryPath output = new DirectoryPath(directoryPath.FullPath);
    // ReSharper disable once LoopCanBeConvertedToQuery
    foreach (string path in directoryPaths) {
      output = output.Combine(path);
    }
    return output;
  }

  /// <summary>
  /// Single method to combine multiple directory path segments ending with what should be a file path segment into a full <see cref="FilePath" />.
  /// </summary>
  /// <param name="directoryPath">The original directory path.</param>
  /// <param name="paths">The other directory path segments ending with what should be a file path.</param>
  /// <returns>The output and resulting <see cref="FilePath" />.</returns>
  [DebuggerStepThrough]
  public static FilePath CombineWithFilePath(this DirectoryPath directoryPath, params string[] paths) {
    Path output = new DirectoryPath(directoryPath.FullPath);
    foreach ((int index, string path) in paths.Select((x, i) => (i, x))) {
      if (index >= paths.Length - 1) {
        output = ((DirectoryPath)output).CombineWithFilePath(path);
      }
      if (index < paths.Length - 1) {
        output = ((DirectoryPath)output).Combine(path);
      }
    }
    if (output is FilePath filePath) {
      return filePath;
    }
    throw new InvalidOperationException($"Failed to compute directory path into type of {nameof(FilePath)}.");
  }

  public static void Debug(this BuildContext context, bool conditional, string message) {
    if (context.Debug) {
      context.Debug(message);
    }
  }
  public static void Debug(this BuildContext context, bool conditional, string message, params object[] args) {
    if (context.Debug) {
      context.Debug(message, args);
    }
  }
  public static void Debug(this BuildContext context, bool conditional, LogAction action) {
    if (context.Debug) {
      context.Debug(action);
    }
  }
  public static void Debug(this ICakeLog log, BuildContext context, string message) {
    if (context.Debug) {
      log.Debug(message);
    }
  }
  public static void Debug(this ICakeLog log, BuildContext context, string message, params object[] args) {
    if (context.Debug) {
      log.Debug(message, args);
    }
  }
  public static void Debug(this ICakeLog log, BuildContext context, bool conditional, LogAction action) {
    if (context.Debug) {
      log.Debug(action);
    }
  }


  public static FilePath MakeAbsolute(this ConvertableFilePath filePath, DirectoryPath parent) {
    if (!filePath.Path.IsRelative) {
      return filePath.Path;
    }

    var output = filePath.Path.MakeAbsolute(parent);
    if (output.IsRelative) {
      throw new InvalidOperationException($"Failed to make FilePath of {filePath} absolute with root DirectoryPath of {parent}");
    }

    return output;
  }

  public static bool Contains(this DirectoryPath directoryPath, ICakeContext context, FilePath filePath) {
    if (directoryPath.IsRelative && !filePath.IsRelative) {
      directoryPath = context.MakeAbsolute(directoryPath);
    }
    if (filePath.IsRelative && !directoryPath.IsRelative) {
      filePath = context.MakeAbsolute(filePath);
    }

    return directoryPath.Segments.All((string segment, int index) => filePath.Segments[index] == segment);
  }

  public static bool Contains(this DirectoryPath directoryPath, ICakeContext context, string filePath) {
    return directoryPath.Segments.All((string segment, int index) => filePath.Split('\\', '/')[index] == segment);
  }

  public static DirectoryPath MakeAbsolute(this ConvertableDirectoryPath directoryPath, DirectoryPath parent) {
    if (!directoryPath.Path.IsRelative) {
      return directoryPath.Path;
    }

    var output = directoryPath.Path.MakeAbsolute(parent);
    if (output.IsRelative) {
      throw new InvalidOperationException($"Failed to make DirectoryPath of {directoryPath} absolute with root DirectoryPath of {parent}");
    }

    return output;
  }

  public static Path MakeRelative(this Path path, ICakeEnvironment environment, DirectoryPath parent) {
    Path? output = (path as FilePath)?.MakeRelative(environment, parent.FullPath)
                 ??
                   (path as DirectoryPath)?.MakeRelative(environment, parent.FullPath) as Path;
    if (output?.IsRelative != true) {
      throw new InvalidOperationException($"Failed to make Path of {path} relative to DirectoryPath of {parent}");
    }

    return output;
  }

  public static FilePath MakeRelative(this FilePath filePath, ICakeEnvironment environment, DirectoryPath parent) {
    var fileUri = new Uri(filePath.MakeAbsolute(environment).FullPath);
    var rootUri = new Uri($"{parent.MakeAbsolute(environment).FullPath}/");

    var relativeUri  = rootUri.MakeRelativeUri(fileUri);
    var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

    var output = new FilePath(relativePath);
    if (!output.IsRelative) {
      throw new InvalidOperationException($"Failed to make FilePath of {filePath} relative to DirectoryPath of {parent}");
    }

    return output;
  }

  public static DirectoryPath MakeRelative(this DirectoryPath directoryPath, ICakeEnvironment environment, DirectoryPath parent) {
    var fileUri = new Uri(directoryPath.MakeAbsolute(environment).FullPath);
    var rootUri = new Uri($"{parent.MakeAbsolute(environment).FullPath}/");

    var relativeUri  = rootUri.MakeRelativeUri(fileUri);
    var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

    var output = new DirectoryPath(relativePath);
    if (!output.IsRelative) {
      throw new InvalidOperationException($"Failed to make DirectoryPath of {directoryPath} relative to DirectoryPath of {parent}");
    }

    return output;
  }

  public static FilePath MakeRelative(this FilePath filePath, DirectoryPath parent) {
    var output = new FilePath(System.IO.Path.GetRelativePath(parent.FullPath, filePath.FullPath));
    if (!output.IsRelative) {
      throw new InvalidOperationException($"Failed to make FilePath of {filePath} relative to DirectoryPath of {parent}");
    }

    return output;
  }

  public static DirectoryPath MakeRelative(this DirectoryPath directoryPath, DirectoryPath parent) {
    var output = new DirectoryPath(System.IO.Path.GetRelativePath(parent.FullPath, directoryPath.FullPath));
    if (!output.IsRelative) {
      throw new InvalidOperationException(
        $"Failed to make DirectoryPath of {directoryPath} relative to DirectoryPath of {parent}");
    }

    return output;
  }

  internal static void UseRepository(this ICakeContext context, DirectoryPath repositoryPath,  Action<Repository> repositoryAction) {
    var absoluteRepositoryPath = repositoryPath.MakeAbsolute(context.Environment);

    DirectoryNotFoundException.ThrowIfNotFound(absoluteRepositoryPath, context);

    using (var repository = new Repository(absoluteRepositoryPath.FullPath)) {
      repositoryAction(repository);
    }
  }

  internal static TResult UseRepository<TResult>(this ICakeContext context, DirectoryPath repositoryPath, Func<Repository, TResult> repositoryFunc) {
    var absoluteRepositoryPath = repositoryPath.MakeAbsolute(context.Environment);

    DirectoryNotFoundException.ThrowIfNotFound(absoluteRepositoryPath, context);

    using (var repository = new Repository(absoluteRepositoryPath.FullPath)) {
      return repositoryFunc(repository);
    }
  }

  internal static RepositoryStatus GitGetRepositoryStatus(this ICakeContext context, DirectoryPath repositoryDirectoryPath) {
    return context.UseRepository(
      repositoryDirectoryPath,
      repository => repository.RetrieveStatus());
  }

  public static void Delete(this Path path, ICakeContext context) {
    switch (path) {
      case FilePath filePath:
        filePath.Delete(context);
        break;
      case DirectoryPath directoryPath:
        directoryPath.Delete(context);
        break;
    }
  }

  public static void Delete(this DirectoryPath directoryPath, ICakeContext context) {
    context.DeleteDirectory(directoryPath, new DeleteDirectorySettings {
      Recursive = true,
      Force     = true,
    });
  }

  public static void Delete(this FilePath filePath, ICakeContext context) {
    context.DeleteFile(filePath);
  }
}
