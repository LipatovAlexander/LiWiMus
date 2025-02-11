﻿using System.Net.Http.Formatting;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace LiWiMus.Web.Shared.Extensions;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(
        this HttpClient client,
        string? requestUri,
        TValue value,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }
        
        var content = JsonContent.Create(value, options: options);
        return client.PatchAsync(requestUri, content, cancellationToken);
    }
    
    public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data)
        => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data!) });

    public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data, CancellationToken cancellationToken)
        => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data!) }, cancellationToken);

    public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this HttpClient httpClient, Uri requestUri, T data)
        => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data!) });

    public static Task<HttpResponseMessage> DeleteAsJsonAsync<T>(this HttpClient httpClient, Uri requestUri, T data, CancellationToken cancellationToken)
        => httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = Serialize(data!) }, cancellationToken);

    private static HttpContent Serialize(object data) => new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
}