# HttpMock

Convinient methods to mock HttpClient and HttpMessageHandler.

The solution consists of a HttpMessageHandler derived class (MockingHandler) that allows to provides expected http status codes, response messages, or a factory method. 
It also provides corresponding static "CreateHttpClient" methods.    
