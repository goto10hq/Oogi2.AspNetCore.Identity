using Oogi2.Attributes;

namespace Oogi2.AspNetCore.Identity.Tests.Entities
{
    [EntityType("entity", "oogi2/user")]
    public class TestIdentityUser : IdentityUser<TestIdentityRole>
    {
    }
}