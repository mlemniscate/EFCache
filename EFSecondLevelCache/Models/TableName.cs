namespace EFSecondLevelCache.Models;

public class TableName
{
    private TableName(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public static TableName Employees => new TableName("Employees");
}