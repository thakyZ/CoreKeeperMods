#nullable enable
using System.Text.Json;
using MoreCommands.Data.Converter;

namespace MoreCommands.Data {
  public static class JsonBase {
    public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions {
      ReadCommentHandling = JsonCommentHandling.Skip,
      AllowTrailingCommas = true,
      MaxDepth = 1000,
      Converters = {
				new Float3JsonConverter(),
        new Float2JsonConverter(),
			}
    };
  }
}
