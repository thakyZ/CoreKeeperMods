using System;
using Cake.Core;
using Cake.Core.IO;

// cSpell:words Metafile
// Ignore Spelling: Metafile

namespace Cake.NekoBoiNick.CoreKeeperMods;

/// <summary>
/// A disposable system to change the current working directory.
/// </summary>
public sealed class UsingChangeWorkingDirectory : IDisposable {
  /// <summary>
  /// The <see cref="DirectoryPath"/> of what the original working directory is set as.
  /// </summary>
  public DirectoryPath OldDirectoryPath { get; }

  /// <summary>
  /// The <see cref="ICakeContext"/> for which to switch working directories.
  /// </summary>
  private ICakeContext  Context          { get; }

  /// <summary>
  /// Constructs a disposable using system.
  /// </summary>
  /// <param name="newDirectoryPath">The path to the new directory</param>
  /// <param name="context">The <see cref="ICakeContext"/> for which to switch working directories.</param>
  public UsingChangeWorkingDirectory(DirectoryPath newDirectoryPath, ICakeContext context) {
    Context                              = context;
    OldDirectoryPath                     = Context.Environment.WorkingDirectory;
    Context.Environment.WorkingDirectory = newDirectoryPath;
  }

  /// <inheritdoc cref="UsingChangeWorkingDirectory(DirectoryPath, ICakeContext)"/>
  public UsingChangeWorkingDirectory(IDirectory newDirectory, ICakeContext context) {
    Context                              = context;
    OldDirectoryPath                     = Context.Environment.WorkingDirectory;
    Context.Environment.WorkingDirectory = newDirectory.Path;
  }

  /// <inheritdoc />
  public void Dispose() {
    Context.Environment.WorkingDirectory = OldDirectoryPath;
  }
}
