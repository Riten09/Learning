using System.Text.Json;
using API.Helpers;

namespace API.Extension;

public static class HttpExtensions
{
    public static void AddPagonationHeader(this HttpResponse response, PaginationHeader header)
    {
        var jsonOption = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        response.Headers.Append("Pagination", JsonSerializer.Serialize(header, jsonOption));
        response.Headers.Append("Access-COntrol-Expose-Headers", "Pagination");
    }

}
