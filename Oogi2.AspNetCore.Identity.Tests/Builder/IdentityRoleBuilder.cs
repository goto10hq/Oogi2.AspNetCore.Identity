using System;
using System.Security.Claims;
using Sushi2;
using Oogi2.AspNetCore.Identity.Tests.Entities;

namespace Oogi2.AspNetCore.Identity.Tests.Builder
{
    public class DocumentDbIdentityRoleBuilder
    {
        protected TestIdentityRole identityRole;

        public DocumentDbIdentityRoleBuilder(TestIdentityRole identityRole)
        {
            this.identityRole = identityRole;
        }

        public static implicit operator TestIdentityRole(DocumentDbIdentityRoleBuilder builder)
        {
            return builder.identityRole;
        }

        public static DocumentDbIdentityRoleBuilder Create(string roleName = null)
        {
            if (roleName == null)
            {
                roleName = Guid.NewGuid().ToString().ToUpper();
            }

            return new DocumentDbIdentityRoleBuilder(new TestIdentityRole
            {
                Name = roleName
            });
        }

        public DocumentDbIdentityRoleBuilder WithId(string id = null)
        {
            identityRole.Id = id ?? Guid.NewGuid().ToString().ToUpper();
            return this;
        }

        public DocumentDbIdentityRoleBuilder WithNormalizedRoleName(string normalizedRoleName = null)
        {
            identityRole.NormalizedName = normalizedRoleName.ToNormalizedString();
            return this;
        }

        public DocumentDbIdentityRoleBuilder AddClaim(string type, string value = null)
        {
            Claim claim = new Claim(type ?? Guid.NewGuid().ToString(), value ?? Guid.NewGuid().ToString());
            identityRole.Claims.Add(claim);

            return this;
        }

        public DocumentDbIdentityRoleBuilder AddClaim(Claim claim = null)
        {
            if (claim == null)
            {
                claim = new Claim(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }

            identityRole.Claims.Add(claim);

            return this;
        }
    }
}