namespace MockingHttp;

public static class HttpMock
{
    static readonly Uri FakeUri = new("http://fakeurl.com");
    public static HttpClient WithBaseUrl(this HttpClient @this, Uri uri)
    {
        @this.BaseAddress = uri;
        return @this;
    }
    /// <summary>
    /// Creates HttpClient that returns the provided response
    /// </summary>
    /// <param name="response">Expected HttpResponseMessage</param>
    /// <returns></returns>
    public static HttpClient CreateHttpClient(HttpResponseMessage response)
        => new HttpClient(new MockingHandler(response)).WithBaseUrl(FakeUri);

    /// <summary>
    /// Creates HttpClient that returns a response with the provided HttpStatusCode
    /// </summary>
    /// <param name="code">Expected HttpStatusCode</param>
    /// <returns></returns>
    public static HttpClient CreateHttpClient(HttpStatusCode code)
        => new HttpClient(new MockingHandler(code)).WithBaseUrl(FakeUri);

    /// <summary>
    /// Creates HttpClient that returns a response with the requested HttpResponseMessage
    /// </summary>
    /// <param name="responseFactory">HttpResponseMessage Factory</param>
    /// <returns></returns>
    public static HttpClient CreateHttpClient(Func<Task<HttpResponseMessage>> responseFactory)
        => new HttpClient(new MockingHandler(responseFactory)).WithBaseUrl(FakeUri);

    /// <summary>
    /// Creates HttpClient that returns a response with the requested HttpResponseMessage
    /// </summary>
    /// <param name="responseFactory">HttpResponseMessage Factory (with request)</param>
    /// <returns></returns>
    public static HttpClient CreateHttpClient(Func<HttpRequestMessage, Task<HttpResponseMessage>> responseFactory)
        => new HttpClient(new MockingHandler(responseFactory)).WithBaseUrl(FakeUri);

}

/// <summary>
/// Mocking DelegatingHandler to use in Unit Tests requiring HttpClient mmocking.
/// </summary>
public class MockingHandler : HttpMessageHandler
{
    readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _f;

    /// <summary>
    /// Use this constructor when jus an Status Code ie expected
    /// </summary>
    /// <param name="expected">Expected Status code</param>
    public MockingHandler(HttpResponseMessage response)
        => _f = r => Task.FromResult(response);


    /// <summary>
    /// Use this constructor when jus an Status Code ie expected
    /// </summary>
    /// <param name="expected">Expected Status code</param>
    public MockingHandler(System.Net.HttpStatusCode expected) : this(new HttpResponseMessage(expected))
    { }

    /// <summary>
    /// Use this constructor when the response does not depend on the request.
    /// </summary>
    /// <param name="f">HttpResponseMessage factory function that returns the expected response.</param>
    public MockingHandler(Func<Task<HttpResponseMessage>> f)
        => _f = r => f();

    /// <summary>
    /// Use this constructor when the response depends on some request values.
    /// </summary>
    /// <param name="f">HttpResponseMessage factory function that return the expected response</param>
    public MockingHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> f)
        => _f = f;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => _f(request);
}
