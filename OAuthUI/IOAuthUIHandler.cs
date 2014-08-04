using System;
using System.Threading.Tasks;

namespace OAuthUI
{
    public interface IOAuthUIHandler
    {
        Task<OAuthResult> AuthorizeAsync(Uri startUri, Uri redirectUri, OAuthUIOptions options = null);
    }
}
