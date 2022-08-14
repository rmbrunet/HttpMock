using static MockingHttp.HttpMock;
namespace MockingHttp;

public class UnitTest
{
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task HttpClientReturns_RequestedHttpStatusCode(HttpStatusCode code)
    {
        // Arrange
        Uri? uri = null;

        var sut = CreateHttpClient(code);


        // Act
        var response = await sut.GetAsync(uri);

        // Assert
        Assert.Equal(code, response.StatusCode);
    }

    [Fact]
    public async Task HttpClientReturns_RequestedHttpResponse()
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

    [Theory]
    [InlineData("token", HttpStatusCode.OK)]
    [InlineData("no-token", HttpStatusCode.BadRequest)]
    public async Task HttpClientReturns_RequestedHttpResponse_FromFactoryWithRequest(string url, HttpStatusCode code)
    {
        // Arrange

        Uri uri = new(url, UriKind.Relative);

        static Task<HttpResponseMessage> factory(HttpRequestMessage request)
        {
            string path = request.RequestUri!.AbsoluteUri;

            return path.EndsWith("/token")
                ? Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))
                : Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));

        }

        var sut = CreateHttpClient(factory);

        // Act

        var response = await sut.GetAsync(uri);

        // Assert

        Assert.Equal(code, response.StatusCode);
    }
}