namespace SmartInventory.Model;

internal interface IEntity<T1>
{
    T1 Id { get; set; }
    T1 CreatedBy { get; set; }
    DateTime CreatedTime { get; set; }
    T1 UpdatedBy { get; set; }
    DateTime UpdatedAt { get; set; }
}

public class Entity : IEntity<int>
{
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Entity2 : IEntity<string>
{
    public string Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public string UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}