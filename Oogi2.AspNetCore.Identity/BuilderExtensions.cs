using Oogi2.AspNetCore.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Oogi2.AspNetCore.Identity
{
    public static class BuilderExtensions
    {
        public static IdentityBuilder AddDocumentDbStores(this IdentityBuilder builder)
        {
            builder.Services.AddSingleton(
                typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                typeof(DocumentDbRoleStore<>).MakeGenericType(builder.RoleType));

            builder.Services.AddSingleton(
                typeof(IUserStore<>).MakeGenericType(builder.UserType),
                typeof(DocumentDbUserStore<,>).MakeGenericType(builder.UserType, builder.RoleType));

            builder.Services.AddTransient<ILookupNormalizer, LookupNormalizer>();

            return builder;
        }
    }
}