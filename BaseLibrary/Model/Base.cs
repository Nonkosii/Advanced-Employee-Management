
using System.Text.Json.Serialization;

namespace BaseLibrary.Model;

public class Base
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public List<Employee>? Employees { get; set; }
}
