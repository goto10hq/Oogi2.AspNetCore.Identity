using Microsoft.AspNetCore.Identity;

namespace Oogi2.AspNetCore.Identity
{
    public class LookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key.Normalize().ToLowerInvariant();
        }
    }
}