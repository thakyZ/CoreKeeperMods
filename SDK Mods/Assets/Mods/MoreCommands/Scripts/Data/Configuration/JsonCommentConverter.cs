using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoreCommands.Data.Configuration {
#nullable enable
  /*
   * public class JsonCommentContractResolver<T> : DefaultContractResolver where T : Attribute
   * {
   *   protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
   *   {
   *     var list = base.CreateProperties(type, memberSerialization);
   *
   *     foreach (var prop in list)
   *     {
   *       var pi = type.GetProperty(prop.UnderlyingName);
   *       if (pi != null)
   *       {
   *         // if the property has any attribute other than
   *         // the specific one we are seeking, don't serialize it
   *         if (pi.GetCustomAttributes().Any() && pi.GetCustomAttribute<T>() == null)
   *         {
   *           prop.ShouldSerialize = obj => false;
   *         }
   *       }
   *     }
   *
   *     return list;
   *   }
   * }
   */

  public class JsonCommentConverter : JsonConverter<string> {
    private readonly string comment;
    public JsonCommentConverter(string comment) {
      this.comment = comment;
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      return typeToConvert == typeof(string) ? reader.GetString() : null;
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options) {
      writer.WriteCommentValue(comment);
    }
  }

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true)]
  public class JsonCommentAttribute : Attribute {
    /// <summary>
    /// Gets or sets the comment of the property.
    /// </summary>
    /// <value>The text of the comment</value>
    public string? Comment { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommentAttribute"/> class with the specified comment.
    /// </summary>
    /// <param name="comment">Text contents of the comment.</param>
    public JsonCommentAttribute(string comment) {
      Comment = comment;
    }
  }
#nullable disable
}
