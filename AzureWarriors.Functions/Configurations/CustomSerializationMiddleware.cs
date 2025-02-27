using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Text.Json;

namespace AzureWarriors.Functions.Configurations
{
    public class CustomSerializationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly JsonSerializerOptions _options;

        public CustomSerializationMiddleware()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            // Exemplo: Adicionar um conversor de Enum para string
            _options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            // Intercepta a requisição antes da execução da função
            var inputData = await context.GetHttpRequestDataAsync();
            if (inputData != null)
            {
                using var reader = new StreamReader(inputData.Body);
                var requestBody = await reader.ReadToEndAsync();

                // Desserializa a entrada com opções customizadas
                var deserialized = JsonSerializer.Deserialize<object>(requestBody, _options);
                context.Items["DeserializedInput"] = deserialized;
            }

            // Executa a função
            await next(context);
        }
    }
}
