using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Common.IO;
using Cake.FileHelpers;
using NekoBoiNick.CoreKeeperMods.Shared.Extensions;
using YamlDotNet.Serialization;

namespace Cake.NekoBoiNick.CoreKeeperMods.Constructs;
public static partial class PseudoAssetDatabase {
  public static PseudoImporterSettings? LoadAssetAtPath(FilePath path, BuildContext buildContext) {
    try {
      if (!buildContext.FileExists(path)) {
        throw new FileNotFoundException($"File not found at path \"{path}\".");
      }

      string fileContents = buildContext.FileReadText(path);
      string[] splitString = NewLineRegex().Split(fileContents);

      splitString = ["---", // Add new YAML document start
        ..splitString.Where((string line) =>
          // Remove invalid tags since we are only loading this file.
          !line.StartsWith("%TAG", StringComparison.OrdinalIgnoreCase)
          && !line.StartsWith("%YAML", StringComparison.OrdinalIgnoreCase)
          && !line.StartsWith("--- ", StringComparison.OrdinalIgnoreCase)
          && !line.StartsWith("MonoBehaviour:", StringComparison.OrdinalIgnoreCase)
        ).Select((string line) => line.TrimStart(' ')),
      ];

      fileContents = string.Join('\n', splitString);

      using TextReader textReader = new StringReader(fileContents);

      var deserializer = new DeserializerBuilder().Build();
      return deserializer.Deserialize<PseudoImporterSettings>(textReader);
    } catch (Exception exception) {
      buildContext.Log.Error(Verbosity.Normal, $"Failed to load asset as {typeof(PseudoImporterSettings).FullName} from path \"{path.FullPath}\".\n{exception.GetFullyQualifiedExceptionText()}");
      return default;
    }
  }

  [GeneratedRegex("\r?\n")]
  private static partial Regex NewLineRegex();
}
