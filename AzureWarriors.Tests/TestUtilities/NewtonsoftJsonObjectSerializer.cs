using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;

namespace AzureWarriors.Tests.TestUtilities
{
    /// <summary>
    /// Custom implementation of IObjectSerializer using Newtonsoft.Json.
    /// </summary>
    public class NewtonsoftJsonObjectSerializer : INewtonsoftJsonObjectSerializer
    {
        /// <summary>
        /// Deserializes the JSON content from the provided stream.
        /// </summary>
        public T? Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using var reader = new StreamReader(stream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Serializes the provided value into JSON and writes it to the stream.
        /// </summary>
        public void Serialize<T>(T value, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var json = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            stream.Write(bytes, 0, bytes.Length);
        }

        public T? Deserialize<T>(string json, JsonSerializerOptions jsonSerializerOptions)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentNullException(nameof(json));

            // Cria uma instância de JsonSerializerSettings e mapeia algumas opções.
            var settings = new JsonSerializerSettings
            {
                // Exemplo: configurar a política de nomes, se estiver definida.
                ContractResolver = jsonSerializerOptions.PropertyNamingPolicy != null
                    ? new Newtonsoft.Json.Serialization.DefaultContractResolver()
                    : null
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
