using Oogi2.AspNetCore.Identity;
using Oogi2.Attributes;

namespace Oogi2.AspNetCore.SampleWeb.Models
{
    [EntityType("entity", "oogi2/user")]
    public class ApplicationUser : IdentityUser<ApplicationRole>
    {
    }

    [EntityType("entity", "oogi2/role")]
    public class ApplicationRole : IdentityRole
    {
    }
}