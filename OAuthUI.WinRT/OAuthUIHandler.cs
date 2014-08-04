using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace OAuthUI
{
    public class OAuthUIHandler : IOAuthUIHandler
    {
        public async Task<OAuthResult> AuthorizeAsync(Uri startUri, Uri redirectUri, OAuthUIOptions options = null)
        {
            var result =
                await WebAuthenticationBroker.AuthenticateAsync(
                    WebAuthenticationOptions.None,
                    startUri,
                    redirectUri);

            OAuthStatus status;
            switch (result.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    status = OAuthStatus.Success;
                    break;
                case WebAuthenticationStatus.UserCancel:
                    status = OAuthStatus.Cancelled;
                    break;
                case WebAuthenticationStatus.ErrorHttp:
                    status = OAuthStatus.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unexpected ResponseStatus value: " + result.ResponseStatus);
            }

            return new OAuthResult(result.ResponseData, status);
        }
    }
}
