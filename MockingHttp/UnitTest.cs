using static MockingHttp.HttpMock;
namespace MockingHttp;

public class UnitTest
{
    [Fact]
    public async Task HttpClientReturns_RequestedHttpStatusCode()
    {
        // Arrange
        Uri? uri = null;

        var sut = CreateHttpClient(HttpStatusCode.OK);


        // Act
        var response = await sut.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse_FromValue()
    {
        // Arrange
        HttpResponseMessage expected = new(HttpStatusCode.OK);

        Uri? uri = null;

        var sut = CreateHttpClient(expected);

        // Act
        var response = await sut.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse_FromFactory()
    {
        // Arrange
        HttpResponseMessage expected = new(HttpStatusCode.OK);

        Uri? uri = null;

        var client = CreateHttpClient(() => Task.FromResult(expected));

        // Act
        var response = await client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse_FromFactoryWithRequest()
    {
        // Arrange

        static Task<HttpResponseMessage> factory(HttpRequestMessage request)
        {
            string path = request.RequestUri!.AbsoluteUri;

            return path.EndsWith("token")
                ? Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))
                : Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

        }

        Uri uri = new("token", UriKind.Relative);

        var sut = CreateHttpClient(factory);

        // Act

        var response = await sut.GetAsync(uri);

        // Assert
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}