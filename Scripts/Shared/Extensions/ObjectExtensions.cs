using System;
using System.Linq;
using System.Reflection;
using System.Text;
using NekoBoiNick.Common.Utils;

namespace NekoBoiNick.CoreKeeperMods.Shared.Extensions {
  public static class ObjectExtensions {
    public static string GetObjectInformation<T>(this T @object, bool fullName = false) where T : notnull {
      Type t = @object.GetType();
      var sb = new IndentStringBuilder('\t')
        .Append('[')
        .Append(t.Name)
        .Append(']')
        .AppendLine()
        .Indent()
        .AppendLine("Fields:")
        .Indent();
      foreach (FieldInfo field in t.GetFields()) {
        sb.Append('[')
          .Append(fullName ? field.FieldType.FullName : field.FieldType.Name)
          .Append("] ")
          .Append(field.Name)
          .Append(" = ")
          .AppendLine(field.GetValue(@object)?.ToString() ?? "null");
      }
      sb.Unindent()
        .AppendLine("Properties:")
        .Indent();
      foreach (PropertyInfo property in t.GetProperties()) {
        sb.Append('[')
          .Append(fullName ? property.PropertyType.FullName : property.PropertyType.Name)
          .Append("] ")
          .Append(property.Name)
          .Append(" = ")
          .AppendLine(property.GetValue(@object)?.ToString() ?? "null");
      }
      sb.Unindent()
        .AppendLine("Methods:")
        .Indent();
      foreach (MethodInfo method in t.GetMethods()) {
        sb.Append('[')
          .Append(fullName ? method.ReturnType.FullName : method.ReturnType.Name)
          .Append("] ")
          .Append(method.Name);
        Type[] genericArguments = method.GetGenericArguments();
        if (genericArguments.Length > 0) {
          sb.Append('<')
            .Append(string.Join(", ", genericArguments.Select((Type genArg) => fullName ? genArg.FullName : genArg.Name)))
            .Append('>');
        }
        sb.Append('(')
          .Append(string.Join(", ", method.GetParameters().Select((ParameterInfo param) => new StringBuilder().Append('[').Append(fullName ? param.ParameterType.FullName : param.ParameterType.Name).Append("] ").Append(param.Name).ToString())));
      }
      return sb.Unindent(2).ToString();
    }
  }
}
