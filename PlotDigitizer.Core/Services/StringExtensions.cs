using System;

#nullable enable
namespace PlotDigitizer.Core
{
    public static class StringExtensions
    {
        public static Uri? ToUri(this string url)
        {
            var check = Uri.TryCreate(url, UriKind.Absolute, out var uri);
            if (!check || uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeFtp) {
                return null;
            }
            return uri;
        }
    }
}
