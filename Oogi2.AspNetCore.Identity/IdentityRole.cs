using System.Collections.Generic;
using System.Security.Claims;

namespace Oogi2.AspNetCore.Identity
{
    public class DocumentDbIdentityRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public IList<Claim> Claims { get; set; } = new List<Claim>();
    }
}