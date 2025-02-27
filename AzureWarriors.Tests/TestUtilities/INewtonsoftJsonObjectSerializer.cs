
using System.Text.Json;

namespace AzureWarriors.Tests.TestUtilities
{
    public interface INewtonsoftJsonObjectSerializer
    {
        T? Deserialize<T>(Stream stream);
        void Serialize<T>(T value, Stream stream);
        T? Deserialize<T>(string json, JsonSerializerOptions jsonSerializerOptions);
    }
}