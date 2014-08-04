OAuthUI
=======

A portable class library that provides a UI for OAuth authentication on desktop and mobile platforms.

Note that OAuthUI handles *only* the UI aspects of OAuth, i.e. it displays a page in an embedded web browser and waits for redirection to the specified URI. Constructing the start URI and parsing the result URI is application-specific and outside the scope of this library.

Usage
-----

```csharp
var handler = new OAuthUIHandler();
var result = await handler.AuthorizeAsync(startUri, redirectUri);
if (result.Status == OAuthStatus.Success)
{
    // Parse the URI in result.Data to get the authorization code
}
```
