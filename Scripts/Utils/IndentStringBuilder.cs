using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Ignore spelling: Unindent

namespace NekoBoiNick.Common.Utils {
  internal class IndentStringBuilder : IDisposable, IEquatable<IndentStringBuilder> {
    private readonly StringBuilder _sb;
    private int indent;
    private char indentChar;
    private int indentSize;
    // ReSharper disable ConvertToAutoPropertyWithPrivateSetter
    private int ReadOnlyIndent => indent;
    private char ReadOnlyIndentChar => indentChar;
    private int ReadOnlyIndentSize => indentSize;
    // ReSharper restore ConvertToAutoPropertyWithPrivateSetter

    /// <inheritdoc cref="StringBuilder()" />
    public IndentStringBuilder(char @char = ' ', int size = 1) {
      _sb = new StringBuilder();
      indentChar = @char;
      indentSize = size;
    }

    public IndentStringBuilder Indent(int number = 1, char? @char = null, int? size = null) {
      indent += number;
      indentChar = @char ?? indentChar;
      indentSize = size ?? indentSize;
      return this;
    }

    public IndentStringBuilder Unindent(int number = 1) {
      indent = Math.Clamp(indent - number, 0, indent);
      return this;
    }

    /// <inheritdoc cref="StringBuilder.Append(char)" />
    public IndentStringBuilder Append(char value) {
      if (_sb is [.., '\r' or '\n']) {
        _sb.Append(new string(indentChar, indentSize * indent));
      }
      _sb.Append(value);
      return this;
    }

    /// <inheritdoc cref="StringBuilder.Append(string?)" />
    public IndentStringBuilder Append(string? value) {
      if (_sb is [.., '\r' or '\n']) {
        _sb.Append(new string(indentChar, indentSize * indent));
      }
      _sb.Append(value);
      return this;
    }

    /// <inheritdoc cref="StringBuilder.Append(int)" />
    public IndentStringBuilder Append(int value) {
      if (_sb is [.., '\r' or '\n']) {
        _sb.Append(new string(indentChar, indentSize * indent));
      }
      _sb.Append(value);
      return this;
    }

    /// <inheritdoc cref="StringBuilder.Append(object?)" />
    public IndentStringBuilder Append(object? value) {
      if (_sb is [.., '\r' or '\n']) {
        _sb.Append(new string(indentChar, indentSize * indent));
      }
      _sb.Append(value);
      return this;
    }

    /// <inheritdoc cref="StringBuilder.AppendLine()" />
    public IndentStringBuilder AppendLine() {
      _sb.AppendLine();
      return this;
    }

    /// <inheritdoc cref="StringBuilder.AppendLine(string?)" />
    public IndentStringBuilder AppendLine(string? value) {
      if (_sb is [.., '\r' or '\n']) {
        _sb.Append(new string(indentChar, indentSize * indent));
      }
      _sb.AppendLine(value);
      return this;
    }

    public void Dispose() {
      indent = 0;
    }

    /// <inheritdoc cref="StringBuilder.ToString()" />
    public override string ToString() => _sb.ToString();
    /// <inheritdoc cref="StringBuilder.ToString(int, int)" />
    public string ToString(int startIndex, int length) => _sb.ToString(startIndex, length);

    public bool Equals(IndentStringBuilder? other) => other?.ToString().Equals(ToString(), StringComparison.Ordinal) == true;
    /// <inheritdoc cref="StringBuilder.Equals(StringBuilder?)" />
    public bool Equals(StringBuilder? other) => other?.ToString().Equals(ToString(), StringComparison.Ordinal) == true;
    /// <inheritdoc cref="StringBuilder.Equals(object?)" />
    public override bool Equals(object? obj) => obj switch {
      IndentStringBuilder isb => Equals(isb),
      StringBuilder sb => Equals(sb),
      _ => false,
    };
    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(ReadOnlyIndent, ReadOnlyIndentChar, ReadOnlyIndentSize, _sb.GetHashCode());
  }
}
