namespace SmartInventory.Contract.Request;

public class DataTablesRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public DataTablesSearch Search { get; set; } = new();
    public List<DataTablesOrder> Order { get; set; } = new();
    public List<DataTablesColumn> Columns { get; set; } = new();
}

public class DataTablesSearch
{
    public string Value { get; set; } = string.Empty;
}

public class DataTablesOrder
{
    public int Column { get; set; }
    public string Dir { get; set; } = "asc";
}

public class DataTablesColumn
{
    public string Data { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Searchable { get; set; }
    public bool Orderable { get; set; }
    public DataTablesSearch Search { get; set; } = new();
}
