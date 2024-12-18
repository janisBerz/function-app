using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;

namespace Transactions
{
    public class Function1
    {
        [Function("Function1")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            var name = req.Query["name"];

            string requestBody = string.Empty;
            using (var reader = new StreamReader(req.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(requestBody))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<JsonElement>(requestBody);
                    if (data.TryGetProperty("name", out JsonElement nameElement))
                    {
                        name = nameElement.GetString();
                    }
                }
                catch
                {
                    // Invalid JSON, ignore and continue with empty name
                }
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            await response.WriteStringAsync(responseMessage);
            return response;
        }
    }
}
