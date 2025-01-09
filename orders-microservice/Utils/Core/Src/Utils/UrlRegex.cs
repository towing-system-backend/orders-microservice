namespace Application.Core
{
    public static class UrlRegex
    {
        public static bool IsUrl(string url) 
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) 
                   && (uriResult.Scheme == Uri.UriSchemeHttp 
                       || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}