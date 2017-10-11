using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Oogi2.AspNetCore.Identity
{
    /// <summary>
    /// Represents a user in the identity system for the <see cref="Stores.DocumentDbUserStore{TUser, TRole}"/> with the role type defaulted to <see cref="DocumentDbIdentityRole"/>
    /// </summary>
    public class DocumentDbIdentityUser : DocumentDbIdentityUser<DocumentDbIdentityRole>
    {
    }

    /// <summary>
    /// Represents a user in the identity system for the <see cref="Stores.DocumentDbUserStore{TUser, TRole}"/>
    /// </summary>
    public class DocumentDbIdentityUser<TRole>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string NormalizedUserName { get; set; }
        public string NormalizedEmail { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string PhoneNumber { get; internal set; }
        public bool IsPhoneNumberConfirmed { get; internal set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsTwoFactorAuthEnabled { get; set; }
        public IList<UserLoginInfo> Logins { get; set; } = new List<UserLoginInfo>();
        public IList<TRole> Roles { get; set; } = new List<TRole>();
        public IList<Claim> Claims { get; set; } = new List<Claim>();
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEndDate { get; set; }
        public int AccessFailedCount { get; set; }
    }
}