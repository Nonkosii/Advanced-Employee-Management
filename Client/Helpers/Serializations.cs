
using System.Text.Json;

namespace Client.Helpers;

public class Serializations()
{
    public static string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject);
    public static T? DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString);
    //public static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList>;
}
