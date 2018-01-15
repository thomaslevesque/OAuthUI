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

        public Task LogoutAsync(Uri logoutUri)
        {
            var frame = new Frame();
            var tcs = new TaskCompletionSource<bool>();
            LoadCompletedEventHandler loadCompletedHandler = null;
            NavigationFailedEventHandler failedHandler = null;
            Action unsubscribe = () =>
            {
                // ReSharper disable AccessToModifiedClosure
                frame.LoadCompleted -= loadCompletedHandler;
                frame.NavigationFailed -= failedHandler;
                // ReSharper restore AccessToModifiedClosure
            };
            loadCompletedHandler =
                (sender, e) =>
                {
                    unsubscribe();
                    tcs.SetResult(true);
                };
            failedHandler =
                (sender, e) =>
                {
                    unsubscribe();
                    tcs.SetException(e.Exception);
                };
            frame.LoadCompleted += loadCompletedHandler;
            frame.NavigationFailed += failedHandler;
            frame.Navigate(logoutUri);
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

                if (_options.Owner != null)
                {
                    var helper = new WindowInteropHelper(this);
                    helper.Owner = _options.Owner.Value;
                }
            }

            public OAuthResult Result { get; private set; }

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
