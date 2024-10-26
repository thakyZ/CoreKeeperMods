using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;

// cSpell:ignoreRegexp SYSLIB\d{3,4}
// cSpell:ignore Hashtable

namespace NekoBoiNick.CoreKeeperMods.Scripts
{
  /// <summary>
  /// Update solution file to contain development systems.
  /// </summary>
  public static partial class UpdateSolution
  {
    public static class Constants
    {
      public static readonly string scriptsProject = new StringBuilder()
                                                     .AppendLine("Project(\"{{{0}}}\") = \"scripts\", \"../Scripts/scripts.csproj\", \"{{10b6c8a7-f8f2-4214-8234-c079a894d4cc}}\"")
                                                     .Append("EndProject")
                                                     .ToString();
      public static readonly string solutionItems = new StringBuilder()
                                                    .AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"{D422557C-9B44-4447-A638-4F70D466C951}\"")
                                                    .AppendLine("\tProjectSection(SolutionItems) = preProject")
                                                    .AppendLine("\t\t.editorconfig = .editorconfig")
                                                    .AppendLine("\tEndProjectSection")
                                                    .Append("EndProject")
                                                    .ToString();
      public static readonly string hiddenItems = new StringBuilder()
                                                  .AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Hidden\", \"Hidden\", \"{00914974-C1E5-4587-A22A-0EFB81E3B164}\"")
                                                  .Append("EndProject")
                                                  .ToString();
      public static readonly string projectConfigurationPlatforms = new StringBuilder()
                                                                    .AppendLine("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Debug|Any CPU.ActiveCfg = Debug|Any CPU")
                                                                    .AppendLine("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Debug|Any CPU.Build.0 = Debug|Any CPU")
                                                                    .AppendLine("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Release|Any CPU.ActiveCfg = Release|Any CPU")
                                                                    .Append("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Release|Any CPU.Build.0 = Release|Any CPU")
                                                                    .ToString();
      public static readonly string hiddenNestedProjectPrefix = new StringBuilder()
                                                                .Append("\tGlobalSection(NestedProjects) = preSolution")
                                                                .ToString();
      public static readonly string hiddenNestedProjectTemplate = new StringBuilder()
                                                                  .Append("\t\t{{{0}}} = {{00914974-C1E5-4587-A22A-0EFB81E3B164}}")
                                                                  .ToString();
      public static readonly string hiddenNestedProjectSuffix = new StringBuilder()
                                                                .Append("\tEndGlobalSection")
                                                                .ToString();
    }

    /// <summary>
    /// Tries to update the solution file to contain development systems.
    /// </summary>
    /// <param name="filePath">The path to the solution file.</param>
    /// <returns>A boolean determining whether the task completed successfully.</returns>
    public static Hashtable TryUpdateSolution(string? filePath)
    {
      filePath = filePath.IsNullOrEmptyOrWhiteSpace() ? Path.GetFullPath(Path.Combine("..", "SDK Mods", "SDK Mods.sln")) : filePath;

      if (!Path.Exists(filePath))
      {
        return new Hashtable(new Dictionary<string, object?> { { "Error", true }, { "Message", $"Path at {filePath} does not exist." }, { "Data", null } });
      }

      string mainProjectUuidCollection = "";

      List<string> projectUuids = [];

      using var fileStream = new FileStream(filePath, FileMode.Open);
      using var streamReader = new StreamReader(fileStream);

      string[] fileTextLines = NewLineRegex().Split(streamReader.ReadToEnd());
      List<string> outFileTextLines = [];

      foreach ((int index, string line) in fileTextLines.Select((value, i) => (i, value)))
      {
        if (ProjectRegex().IsMatch(line))
        {
          var matches = ProjectRegex().Match(line);
          if (matches.Groups.Count > 1)
          {
            projectUuids.Add(matches.Groups[2].Value);
          }
          if (string.IsNullOrEmpty(mainProjectUuidCollection) && matches.Groups.Count > 0)
          {
            mainProjectUuidCollection = matches.Groups[1].Value;
          }
        }

        if (index < fileTextLines.Length - 2 &&
            SolutionConfigurationPlatformsRegex().IsMatch(fileTextLines[index + 1]) &&
            GlobalRegex().IsMatch(fileTextLines[index]) &&
            EndProjectRegex().IsMatch(fileTextLines[index - 1]))
        {
          if (string.IsNullOrEmpty(mainProjectUuidCollection))
          {
            return new Hashtable(new Dictionary<string, object?> { { "Error", true }, { "Message", "Failed to fetch the main project uuid collection." }, { "Data", null } });
          }
          outFileTextLines.Add(string.Format(Constants.scriptsProject, mainProjectUuidCollection));
          outFileTextLines.Add(Constants.solutionItems);
          outFileTextLines.Add(Constants.hiddenItems);
        }
        else if (index < fileTextLines.Length - 2 &&
                 EndGlobalSectionRegex().IsMatch(fileTextLines[index]) &&
                 SolutionPropertiesRegex().IsMatch(fileTextLines[index + 1]) &&
                 HideSolutionNodeRegex().IsMatch(fileTextLines[index + 2]))
        {
          outFileTextLines.Add(Constants.projectConfigurationPlatforms);
        }
        else if (index < fileTextLines.Length - 1 &&
                 EndGlobalRegex().IsMatch(fileTextLines[index]) &&
                 EndGlobalSectionRegex().IsMatch(fileTextLines[index - 1]))
        {
          outFileTextLines.Add(Constants.hiddenNestedProjectPrefix);
          outFileTextLines.AddRange(projectUuids.Select(uuid => string.Format(Constants.hiddenNestedProjectTemplate, uuid)));
          outFileTextLines.Add(Constants.hiddenNestedProjectSuffix);
        }

        outFileTextLines.Add(line);
      }

      return new Hashtable(new Dictionary<string, object?> { { "Error", false }, { "Message", "Success" }, { "Data", string.Join("\n", outFileTextLines) } });
    }

    [GeneratedRegex(@"^\tGlobalSection\(SolutionConfigurationPlatforms\) = preSolution$")] private static partial Regex SolutionConfigurationPlatformsRegex();
    [GeneratedRegex("^Project\\(\"\\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\\}\"\\) = \"[\\w\\.]+\", \"[\\w\\.]+\\.csproj\", \"\\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\\}\"$")] private static partial Regex GlobalRegex();
    [GeneratedRegex("^Global$")] private static partial Regex ProjectRegex();
    [GeneratedRegex("^EndProject$")] private static partial Regex EndProjectRegex();
    [GeneratedRegex(@"\r?\n")] private static partial Regex NewLineRegex();
    [GeneratedRegex(@"^\tEndGlobalSection$")] private static partial Regex EndGlobalSectionRegex();
    [GeneratedRegex(@"^\tGlobalSection\(SolutionProperties\) = preSolution$")] private static partial Regex SolutionPropertiesRegex();
    [GeneratedRegex(@"^\t\tHideSolutionNode = FALSE$")] private static partial Regex HideSolutionNodeRegex();

    [GeneratedRegex("^EndGlobal$")] private static partial Regex EndGlobalRegex();
  }
}
