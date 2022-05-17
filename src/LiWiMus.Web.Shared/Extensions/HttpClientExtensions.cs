﻿using System.Net.Http.Formatting;
using System.Text;
using Newtonsoft.Json;

namespace LiWiMus.Web.Shared.Extensions;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value) 
        => client.PatchAsync(requestUri, new ObjectContent(typeof(T), value, new JsonMediaTypeFormatter(), "application/json"));

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