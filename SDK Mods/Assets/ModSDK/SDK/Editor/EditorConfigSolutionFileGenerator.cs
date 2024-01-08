// cSpell:ignoreRegexp SYSLIB(?=\d{3,4})
// cSpell:ignoreRegexp  (?<=ENABLE_)VSTU
// cSpell:ignore Postprocessor

using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class Extensions {
  public static void AddWhenMissing(this List<string> list, string input, string baseContent) {
    var done = false;
    foreach (var item in list) {
      if (item == input) {
        done = true;
      }
    }
    if (baseContent.Contains(input)) {
      done = true;
    } else if (baseContent.Contains(input.Replace("\n", "\r\n"))) {
      done = true;
    }
    if (!done) {
      list.Add(input);
    }
  }
}

/// <summary>
/// This class hooks into the Visual Studio .sln generation step and modifies the file
/// to include .editorconfig, which enforces consistent formatting standards and naming
/// conventions.
/// https://docs.microsoft.com/en-us/visualstudio/ide/editorconfig-code-style-settings-reference?view=vs-2017
/// </summary>
public class ProjectFilePostprocessor : AssetPostprocessor {
  public static class Constants {
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
    public static readonly string ExtensibilityGlobals = new StringBuilder()
                                                .AppendLine("\tGlobalSection(ExtensibilityGlobals) = postSolution")
                                                .AppendLine("\t\tSolutionGuid = {FD87994B-C032-4821-BD72-E057C33083EF}")
                                                .Append("\tEndGlobalSection")
                                                .ToString();
  }

  private static List<string> GetWhitelistedProjects(string path) {
    List<string> output = new();
    var modsDir = Path.Join(Path.GetDirectoryName(path), "Assets", "Mods");

    if (Directory.Exists(modsDir)) {
      foreach (var directory in Directory.EnumerateDirectories(modsDir)) {
        output.Add(Path.GetFileName(directory));
      }
    }

    return output;
  }

  public static string OnGeneratedSlnSolution(string path, string content) {
    string[] fileTextLines = NewLineRegex.Split(content);
    List<string> outFileTextLines = new();
    List<string> ProjectUuids = new();
    string mainProjectUuidCollection = "";
    var whitelisted = GetWhitelistedProjects(path);

    foreach ((int index, string line) in fileTextLines.Select((value, i) => (i, value))) {
      if (ProjectRegex.IsMatch(line)) {
        var matches = ProjectRegex.Match(line);

        if (matches.Groups.Count > 2) {
          if (!whitelisted.Contains(matches.Groups[2].Value)) {
            ProjectUuids.Add(matches.Groups[3].Value);
          }
        }

        if (string.IsNullOrEmpty(mainProjectUuidCollection) && matches.Groups.Count > 0) {
          mainProjectUuidCollection = matches.Groups[1].Value;
        }
      }

      if (index < fileTextLines.Length - 2 && SolutionConfigurationPlatformsRegex.IsMatch(fileTextLines[index + 1]) && GlobalRegex.IsMatch(fileTextLines[index]) && EndProjectRegex.IsMatch(fileTextLines[index - 1])) {
        if (string.IsNullOrEmpty(mainProjectUuidCollection)) {
          return content;
        } else {
          outFileTextLines.AddWhenMissing(string.Format(Constants.ScriptsProject, mainProjectUuidCollection), content);
        }

        outFileTextLines.AddWhenMissing(Constants.SolutionItems, content);
        outFileTextLines.AddWhenMissing(Constants.HiddenItems, content);
      } else if (index < fileTextLines.Length - 2 && EndGlobalSectionRegex.IsMatch(fileTextLines[index]) && SolutionPropertiesRegex.IsMatch(fileTextLines[index + 1]) && HideSolutionNodeRegex.IsMatch(fileTextLines[index + 2])) {
        outFileTextLines.AddWhenMissing(Constants.ProjectConfigurationPlatforms, content);
      } else if (index < fileTextLines.Length - 1 && EndGlobalRegex.IsMatch(fileTextLines[index]) && EndGlobalSectionRegex.IsMatch(fileTextLines[index - 1])) {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Constants.HiddenNestedProjectPrefix);

        foreach (var uuid in ProjectUuids) {
          stringBuilder.AppendLine(string.Format(Constants.HiddenNestedProjectTemplate, uuid));
        }

        stringBuilder.Append(Constants.HiddenNestedProjectSuffix);
        outFileTextLines.AddWhenMissing(stringBuilder.ToString(), content);
        outFileTextLines.AddWhenMissing(Constants.ExtensibilityGlobals, content);
      }

      outFileTextLines.Add(line);
    }

    return string.Join("\r\n", outFileTextLines);
  }

  public static string OnGeneratedCSProject(string path, string content) {
    return content;
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
  private static Regex SolutionConfigurationPlatformsRegex = new(@"^\tGlobalSection\(SolutionConfigurationPlatforms\) = preSolution$");
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
  private static Regex GlobalRegex = new(@"^Global$");
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
  private static Regex EndProjectRegex = new(@"^EndProject$");
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
  private static Regex ProjectRegex = new("^Project\\(\"\\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\\}\"\\) = \"([\\w\\.]+)\", \"[\\w\\.]+\\.csproj\", \"\\{([A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12})\\}\"$");
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