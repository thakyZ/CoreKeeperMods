using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System;

#nullable enable
namespace NekoBoiNick.CoreKeeperMods
{
  /// <summary>
  /// Update solution file to contain development systems.
  /// </summary>
  public partial class UpdateSolution
  {
    public static class Constants
    {
      public static readonly string ScriptsProject = new StringBuilder()
                                                     .AppendLine("Project(\"{{{0}}}\") = \"scripts\", \"../Scripts/scripts.csproj\", \"{{10b6c8a7-f8f2-4214-8234-c079a894d4cc}}\"")
                                                     .Append("EndProject")
                                                     .ToString();
      public static readonly string SolutionItems = new StringBuilder()
                                                    .AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Solution Items\", \"Solution Items\", \"{D422557C-9B44-4447-A638-4F70D466C951}\"")
                                                    .AppendLine("\tProjectSection(SolutionItems) = preProject")
                                                    .AppendLine("\t\t.editorconfig = .editorconfig")
                                                    .AppendLine("\tEndProjectSection")
                                                    .Append("EndProject")
                                                    .ToString();
      public static readonly string HiddenItems = new StringBuilder()
                                                  .AppendLine("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"Hidden\", \"Hidden\", \"{00914974-C1E5-4587-A22A-0EFB81E3B164}\"")
                                                  .Append("EndProject")
                                                  .ToString();
      public static readonly string ProjectConfigurationPlatforms = new StringBuilder()
                                                                    .AppendLine("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Debug|Any CPU.ActiveCfg = Debug|Any CPU")
                                                                    .AppendLine("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Debug|Any CPU.Build.0 = Debug|Any CPU")
                                                                    .AppendLine("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Release|Any CPU.ActiveCfg = Release|Any CPU")
                                                                    .Append("\t\t{10b6c8a7-f8f2-4214-8234-c079a894d4cc}.Release|Any CPU.Build.0 = Release|Any CPU")
                                                                    .ToString();
      public static readonly string HiddenNestedProjectPrefix = new StringBuilder()
                                                                .Append("\tGlobalSection(NestedProjects) = preSolution")
                                                                .ToString();
      public static readonly string HiddenNestedProjectTemplate = new StringBuilder()
                                                                  .Append("\t\t{{{0}}} = {{00914974-C1E5-4587-A22A-0EFB81E3B164}}")
                                                                  .ToString();
      public static readonly string HiddenNestedProjectSuffix = new StringBuilder()
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
      filePath = string.IsNullOrEmpty(filePath) || string.IsNullOrWhiteSpace(filePath) || filePath == "" || filePath == " " ? Path.GetFullPath(Path.Combine("..", "SDK Mods", "SDK Mods.sln")) : filePath;

      if (!Path.Exists(filePath))
      {
        return new Hashtable(new Dictionary<string, object?>() { { "Error", true }, { "Message", $"Path at {filePath} does not exist." }, { "Data", null } });
      }

      string mainProjectUuidCollection = "";

      List<string> ProjectUuids = [];

      using var fileStream = new FileStream(filePath, FileMode.Open);
      using var streamReader = new StreamReader(fileStream);

      string[] fileTextLines = NewLineRegex.Split(streamReader.ReadToEnd());
      List<string> outFileTextLines = [];

      foreach ((int index, string line) in fileTextLines.Select((value, i) => (i, value)))
      {
        if (ProjectRegex.IsMatch(line))
        {
          var matches = ProjectRegex.Match(line);
          if (matches.Groups.Count > 1)
          {
            ProjectUuids.Add(matches.Groups[2].Value);
          }
          if (string.IsNullOrEmpty(mainProjectUuidCollection) && matches.Groups.Count > 0)
          {
            mainProjectUuidCollection = matches.Groups[1].Value;
          }
        }

        if (index < fileTextLines.Length - 2 &&
            SolutionConfigurationPlatformsRegex.IsMatch(fileTextLines[index + 1]) &&
            GlobalRegex.IsMatch(fileTextLines[index]) &&
            EndProjectRegex.IsMatch(fileTextLines[index - 1]))
        {
          if (string.IsNullOrEmpty(mainProjectUuidCollection))
          {
            return new Hashtable(new Dictionary<string, object?>() { { "Error", true }, { "Message", $"Failed to fetch the main project uuid collection." }, { "Data", null } });
          }
          else
          {
            outFileTextLines.Add(string.Format(Constants.ScriptsProject, mainProjectUuidCollection));
          }
          outFileTextLines.Add(Constants.SolutionItems);
          outFileTextLines.Add(Constants.HiddenItems);
        }
        else if (index < fileTextLines.Length - 2 &&
                 EndGlobalSectionRegex.IsMatch(fileTextLines[index]) &&
                 SolutionPropertiesRegex.IsMatch(fileTextLines[index + 1]) &&
                 HideSolutionNodeRegex.IsMatch(fileTextLines[index + 2]))
        {
          outFileTextLines.Add(Constants.ProjectConfigurationPlatforms);
        }
        else if (index < fileTextLines.Length - 1 &&
                 EndGlobalRegex.IsMatch(fileTextLines[index]) &&
                 EndGlobalSectionRegex.IsMatch(fileTextLines[index - 1]))
        {
          outFileTextLines.Add(Constants.HiddenNestedProjectPrefix);
          foreach (var uuid in ProjectUuids)
          {
            outFileTextLines.Add(string.Format(Constants.HiddenNestedProjectTemplate, uuid));
          }
          outFileTextLines.Add(Constants.HiddenNestedProjectSuffix);
        }

        outFileTextLines.Add(line);
      }

      return new Hashtable(new Dictionary<string, object?>() { { "Error", false }, { "Message", "Success" }, { "Data", string.Join("\n", outFileTextLines) } });
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex SolutionConfigurationPlatformsRegex = new(@"^\tGlobalSection\(SolutionConfigurationPlatforms\) = preSolution$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex GlobalRegex = new(@"^Global$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex EndProjectRegex = new(@"^EndProject$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex ProjectRegex = new("^Project\\(\"\\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\\}\"\\) = \"[\\w\\.]+\", \"[\\w\\.]+\\.csproj\", \"\\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\\}\"$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex NewLineRegex = new(@"\r?\n");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex EndGlobalSectionRegex = new(@"^\tEndGlobalSection$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex SolutionPropertiesRegex = new(@"^\tGlobalSection\(SolutionProperties\) = preSolution$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex HideSolutionNodeRegex = new(@"^\t\tHideSolutionNode = FALSE$");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
    private static Regex EndGlobalRegex = new(@"^EndGlobal$");
  }
}
#nullable restore