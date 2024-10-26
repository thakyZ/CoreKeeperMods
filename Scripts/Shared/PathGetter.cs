using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;

namespace NekoBoiNick.CoreKeeperMods.Scripts.Shared {
  public static partial class PathGetter {
    private static Matcher GetMatcher(List<string> pathsToCheck) {
      Matcher matcher = new();
      matcher.AddIncludePatterns(pathsToCheck);
      return matcher;
    }

    [SupportedOSPlatform("windows")]
    public static string[] GetDrivesWindows() {
      return [..DriveInfo.GetDrives().Select(drive => drive.ToString()).Where(drive => drive.StartsWithAny(["A","B","C","D","E"], StringComparison.OrdinalIgnoreCase))];
    }

    [SupportedOSPlatform("linux")]
    public static string[] GetDrivesLinux() {
      return ["/"];
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static string[] GetDrives() {
      if (OperatingSystem.IsLinux()) {
        return GetDrivesLinux();
      }
      if (OperatingSystem.IsWindows()) {
        return GetDrivesWindows();
      }
      throw new PlatformNotSupportedException();
    }

    public static string? GetPath(List<string> pathsToCheck) {
      List<string> drives = ["/"];
      List<string> globs = [];
      foreach (var path in pathsToCheck.Select((string path) => path.Replace('\\', '/')).Where(path => DriveRegex().IsMatch(path))) {
        drives.Add(path[..3]);
        globs.Add($"./{path[3..].Replace("Steam/","Steam*/")}");
      }
      var matcher = GetMatcher(globs);
      List<string> found = [];
      foreach (string drive in drives) {
        found.AddRange(
          matcher.GetResultsInFullPath(drive).Select(
            (string result) => result.SplitAfter("Core Keeper", StringComparison.OrdinalIgnoreCase)[0]
          ).Distinct()
        );
      }
      return found.Count switch {
        1 => found[0],
        0 => null,
        _ => found[0],
      };
    }

    [GeneratedRegex(@"^[A-Z]:\\")]
    private static partial Regex DriveRegex();
  }
}
