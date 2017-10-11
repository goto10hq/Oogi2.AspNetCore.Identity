using System.Collections.Generic;

namespace Oogi2.AspNetCore.Identity.Tests.Comparer
{
    public class DocumentDbIdentityRoleComparer : IEqualityComparer<IdentityRole>
    {
        public bool Equals(IdentityRole x, IdentityRole y)
        {
            return x.Id.Equals(y.Id)
                && string.Equals(x.Name, y.Name)
                && string.Equals(x.NormalizedName, y.NormalizedName);
            //&& x.DocumentType.Equals(y.DocumentType);
        }

        public int GetHashCode(IdentityRole obj)
        {
            return 1;
        }
    }
}