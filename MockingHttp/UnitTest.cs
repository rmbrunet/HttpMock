namespace MockingHttp;

public class UnitTest
{
    [Fact]
    public async Task HttpClientReturns_RequestedHttpStatusCode()
    {
        var client = HttpMock.CreateHttpClient(HttpStatusCode.OK);

        Uri? uri = null;

        var response = await client.GetAsync(uri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse_FromValue()
    {
        HttpResponseMessage expected = new(HttpStatusCode.OK);

        var client = HttpMock.CreateHttpClient(expected);

        Uri? uri = null;

        var response = await client.GetAsync(uri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse_FromFactory()
    {
        HttpResponseMessage expected = new(HttpStatusCode.OK);

        var client = HttpMock.CreateHttpClient(() => Task.FromResult(expected));

        Uri? uri = null;

        var response = await client.GetAsync(uri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse_FromFactoryWithRequest()
    {
        Func<HttpRequestMessage, Task<HttpResponseMessage>> factory = request =>
        {
            string path = request.RequestUri!.AbsoluteUri;

            return path.EndsWith("token")
                ? Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))
                : Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

        };

        var client = HttpMock.CreateHttpClient(factory);

        Uri uri = new Uri("token", UriKind.Relative);

        var response = await client.GetAsync(uri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}