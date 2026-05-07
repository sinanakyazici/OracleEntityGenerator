using System.Text;

namespace OracleEntityGenerator.CodeGeneration;

internal sealed class CodeWriter
{
    private readonly StringBuilder _builder = new();
    private int _indentLevel;
    private bool _isStartOfLine = true;

    public CodeWriter WriteLine()
    {
        _builder.AppendLine();
        _isStartOfLine = true;
        return this;
    }

    public CodeWriter WriteLine(string text)
    {
        WriteIndentIfNeeded();
        _builder.AppendLine(text);
        _isStartOfLine = true;
        return this;
    }

    public CodeWriter Indent()
    {
        _indentLevel++;
        return this;
    }

    public CodeWriter Unindent()
    {
        if (_indentLevel == 0)
        {
            throw new InvalidOperationException("Cannot unindent below zero.");
        }

        _indentLevel--;
        return this;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }

    private void WriteIndentIfNeeded()
    {
        if (!_isStartOfLine)
        {
            return;
        }

        _builder.Append(' ', _indentLevel * 4);
        _isStartOfLine = false;
    }
}
