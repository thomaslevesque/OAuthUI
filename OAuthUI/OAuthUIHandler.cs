using System;
using System.Threading.Tasks;

namespace OAuthUI
{
    public class OAuthUIHandler : IOAuthUIHandler
    {
        public Task<OAuthResult> AuthorizeAsync(Uri startUri, Uri redirectUri, OAuthUIOptions options = null)
        {
            throw BadTargetPlatformException();
        }

        public Task LogoutAsync(Uri logoutUri)
        {
            throw BadTargetPlatformException();
        }

        private Exception BadTargetPlatformException()
        {
            return new NotImplementedException("Oops, this shouldn't be happening; are you targeting an unsupported platform?");
        }
    }
}
