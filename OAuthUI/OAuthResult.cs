namespace OAuthUI
{
    public class OAuthResult
    {
        private readonly OAuthStatus _status;
        private readonly string _data;

        public OAuthResult(string data, OAuthStatus status)
        {
            _data = data;
            _status = status;
        }

        public string Data
        {
            get { return _data; }
        }

        public OAuthStatus Status
        {
            get { return _status; }
        }
    }
}