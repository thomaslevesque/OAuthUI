using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.CSharp.RuntimeBinder;

namespace OAuthUI
{
    public class OAuthUIHandler : IOAuthUIHandler
    {
        public Task<OAuthResult> AuthorizeAsync(Uri startUri, Uri redirectUri, OAuthUIOptions options = null)
        {
            var window = new OAuthWindow(startUri, redirectUri, options);
            var tcs = new TaskCompletionSource<OAuthResult>();
            if (window.ShowDialog() == true)
            {
                tcs.SetResult(window.Result);
            }
            else
            {
                tcs.SetResult(new OAuthResult(string.Empty, OAuthStatus.Cancelled));
            }
            return tcs.Task;
        }

        class OAuthWindow : Window
        {
            private readonly Uri _startUri;
            private readonly Uri _redirectUri;
            private readonly OAuthUIOptions _options;

            private readonly WebBrowser _browser;

            public OAuthWindow(Uri startUri, Uri redirectUri, OAuthUIOptions options)
            {
                _startUri = startUri;
                _redirectUri = redirectUri;

                _browser = new WebBrowser();
                _browser.Navigating += BrowserOnNavigating;
                _browser.LoadCompleted += BrowserOnLoadCompleted;
                
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.WindowStyle = WindowStyle.ToolWindow;
                this.Loaded += OnLoaded;
                this.Content = _browser;

                _options = options ?? new OAuthUIOptions();
                if (_options.Width != null)
                    Width = _options.Width.Value;
                if (_options.Height != null)
                    Height = _options.Height.Value;
            }

            public OAuthResult Result { get; set; }

            protected override void OnSourceInitialized(EventArgs e)
            {
                base.OnSourceInitialized(e);
                if (_options.Owner != null)
                {
                    var helper = new WindowInteropHelper(this);
                    helper.Owner = _options.Owner.Value;
                }
            }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                _browser.Navigate(_startUri);
            }

            private void BrowserOnNavigating(object sender, NavigatingCancelEventArgs e)
            {
                if (e.Uri.AbsoluteUri.StartsWith(_redirectUri.AbsoluteUri))
                {
                    e.Cancel = true;
                    Result = new OAuthResult(e.Uri.AbsoluteUri, OAuthStatus.Success);
                    DialogResult = true;
                }
                else if (e.Uri.Scheme == "res")
                {
                    e.Cancel = true;
                    Result = new OAuthResult(string.Empty, OAuthStatus.Error);
                    DialogResult = true;
                }
            }

            private void BrowserOnLoadCompleted(object sender, NavigationEventArgs e)
            {
                try
                {
                    if (_browser.Document != null)
                    {
                        dynamic doc = _browser.Document;
                        this.Title = doc.Title;
                    }
                }
                catch (RuntimeBinderException)
                {
                }
            }
        }
    }
}
