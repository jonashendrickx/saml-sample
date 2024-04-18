using System;

namespace SamlSample.Extensions
{
    public static class ConfigurationExtensions
    {
        public static Uri GetRoot(this string uri) => new Uri(new Uri(uri).GetLeftPart(UriPartial.Authority));
    }
}
