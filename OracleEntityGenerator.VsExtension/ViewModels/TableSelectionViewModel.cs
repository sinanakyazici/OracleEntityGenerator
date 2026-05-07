using OracleEntityGenerator.Oracle;

namespace OracleEntityGenerator.VsExtension.ViewModels;

public sealed class TableSelectionViewModel : ObservableObject
{
    private bool _isSelected;

    public TableSelectionViewModel(OracleTableSummary table)
    {
        SchemaName = table.SchemaName;
        TableName = table.TableName;
        Comment = table.Comment;
        ColumnCount = table.ColumnCount;
        HasPrimaryKey = table.HasPrimaryKey;
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string SchemaName { get; }

    public string TableName { get; }

    public string? Comment { get; }

    public int ColumnCount { get; }

    public bool HasPrimaryKey { get; }
}
