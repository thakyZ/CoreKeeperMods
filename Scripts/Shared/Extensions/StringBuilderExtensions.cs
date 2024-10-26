#nullable enable
using System.Runtime.CompilerServices;
using System.Text;

namespace NekoBoiNick.CoreKeeperMods.Shared.Extensions {
  internal static class StringBuilderExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder Append(this StringBuilder sb, string value, int indent, char indentChar = ' ') {
      return sb.Append(new string(indentChar, indent) + value);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendLine(this StringBuilder sb, string value, int indent, char indentChar = ' ') {
      return sb.AppendLine(new string(indentChar, indent) + value);
    }
  }
}
