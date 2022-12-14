# HttpMock

Convenient methods to mock HttpClient and HttpMessageHandler.

The solution consists of a MockingHandler class derived from HttpMessageHandler. MockingHandler returns a response from the provided http status codes, response messages, or factory methods provided in the constructors. 

It is also included corresponding static "CreateHttpClient" factory methods to facilitate its use.    

Examples can be found in the Unit Tests, from the simpler one: returning a given HttpStatusCode:

``` C#
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
```

to the more complex one where the response depends on the request:

``` C#
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
