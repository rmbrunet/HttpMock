# HttpMock

Convinient methods to mock HttpClient and HttpMessageHandler.

The solution consists of a HttpMessageHandler derived class (MockingHandler) that allows to provides expected http status codes, response messages, or a factory method. 
It also provides corresponding static "CreateHttpClient" methods.    

Example of its use:


``` CSharp
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
```
