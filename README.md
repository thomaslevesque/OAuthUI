OAuthUI
=======

A portable class library that provides a UI for OAuth authentication on desktop and mobile platforms.

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
