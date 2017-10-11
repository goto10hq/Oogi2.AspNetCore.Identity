using Oogi2.AspNetCore.Identity.Stores;
using Oogi2.AspNetCore.Identity.Tests.Builder;
using Oogi2.AspNetCore.Identity.Tests.Fixtures;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Oogi2.AspNetCore.Identity.Tests.Entities;

namespace Oogi2.AspNetCore.Identity.Tests
{
    [Collection("DocumentDbCollection")]
    public class UserStoreTests : StoreTestsBase
    {
        public UserStoreTests(DocumentDbFixture documentDbFixture)
            : base(documentDbFixture)
        {
        }

        [Fact]
        public async Task ShouldSetNormalizedUserName()
        {
            TestIdentityUser user = DocumentDbIdentityUserBuilder.Create().WithNormalizedUserName();
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();

            string normalizedUserName = Guid.NewGuid().ToString();
            await store.SetNormalizedUserNameAsync(user, normalizedUserName, CancellationToken.None);

            Assert.Equal(normalizedUserName, user.NormalizedUserName);
        }

        [Fact]
        public async Task ShouldReturnAllUsersWithAdminRoleClaim()
        {
            var store = CreateUserStore();

            string adminRoleValue = Guid.NewGuid().ToString();

            TestIdentityUser firstAdmin = DocumentDbIdentityUserBuilder.Create().WithId().AddClaim(ClaimTypes.Role, adminRoleValue).AddClaim();
            TestIdentityUser secondAdmin = DocumentDbIdentityUserBuilder.Create().WithId().AddClaim(ClaimTypes.Role, adminRoleValue).AddClaim().AddClaim();
            TestIdentityUser thirdAdmin = DocumentDbIdentityUserBuilder.Create().WithId().AddClaim(ClaimTypes.Role, adminRoleValue);

            CreateDocument(firstAdmin);
            CreateDocument(secondAdmin);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddClaim().AddClaim());
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddClaim().AddClaim().AddClaim());
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddClaim().AddClaim());
            CreateDocument(thirdAdmin);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddClaim().AddClaim().AddClaim().AddClaim());

            IList<TestIdentityUser> adminUsers = await store.GetUsersForClaimAsync(new Claim(ClaimTypes.Role, adminRoleValue), CancellationToken.None);

            Assert.Collection(
                adminUsers,
                u => u.Id.Equals(firstAdmin.Id),
                u => u.Id.Equals(secondAdmin.Id),
                u => u.Id.Equals(thirdAdmin.Id));
        }

        [Fact]
        public async Task ShouldReturnUserByLoginProvider()
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().WithId().WithUserLoginInfo(amount: 3);
            UserLoginInfo targetLogin = targetUser.Logins[1];

            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithUserLoginInfo(amount: 2));
            CreateDocument(targetUser);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithUserLoginInfo(amount: 2));

            IdentityUser<TestIdentityRole> foundUser = await store.FindByLoginAsync(targetLogin.LoginProvider, targetLogin.ProviderKey, CancellationToken.None);

            Assert.Equal(targetUser.Id, foundUser.Id);
        }

        [Fact]
        public async Task ShouldReturnAllUsersWithAdminRole()
        {
            TestIdentityRole adminRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();

            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();

            TestIdentityUser firstAdmin = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName().AddRole(adminRole).AddRole();
            TestIdentityUser secondAdmin = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName().AddRole(adminRole).AddRole().AddRole();
            TestIdentityUser thirdAdmin = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName().AddRole(adminRole);

            CreateDocument(firstAdmin);
            CreateDocument(secondAdmin);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddRole().AddRole());
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddRole().AddRole().AddRole());
            CreateDocument(thirdAdmin);
            CreateDocument(DocumentDbIdentityUserBuilder.Create());
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddRole());

            IList<TestIdentityUser> adminUsers = await store.GetUsersInRoleAsync(adminRole.NormalizedName, CancellationToken.None);

            Assert.Collection(
                adminUsers,
                u => u.Id.Equals(firstAdmin.Id),
                u => u.Id.Equals(secondAdmin.Id),
                u => u.Id.Equals(thirdAdmin.Id));
        }

        [Fact]
        public async Task ShouldReturnNoUsersWithAdminRoleWhenPassingNotNormalizedRoleNameToGetUsersInRole()
        {
            TestIdentityRole adminRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();
            TestIdentityUser firstAdmin = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName().AddRole(adminRole).AddRole();

            CreateDocument(firstAdmin);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().AddRole().AddRole());

            IList<TestIdentityUser> adminUsers = await store.GetUsersInRoleAsync(adminRole.Name, CancellationToken.None);

            Assert.Empty(adminUsers);
        }

        [Fact]
        public async Task ShouldReturnUserBySpecificEmail()
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedEmail();

            CreateDocument(DocumentDbIdentityUserBuilder.Create());
            CreateDocument(targetUser);
            CreateDocument(DocumentDbIdentityUserBuilder.Create());
            CreateDocument(DocumentDbIdentityUserBuilder.Create());

            IdentityUser<TestIdentityRole> foundUser = await store.FindByEmailAsync(targetUser.NormalizedEmail, CancellationToken.None);

            Assert.Equal(targetUser.Id, foundUser.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        public async Task ShouldIncreaseAccessFailedCountBy1(int accessFailedCount)
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().WithAccessFailedCountOf(accessFailedCount);

            await store.IncrementAccessFailedCountAsync(targetUser, CancellationToken.None);

            Assert.Equal(++accessFailedCount, targetUser.AccessFailedCount);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(1)]
        [InlineData(0)]
        public async Task ShouldResetAccessFailedCountToZero(int accessFailedCount)
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> store = CreateUserStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().WithAccessFailedCountOf(accessFailedCount);

            await store.ResetAccessFailedCountAsync(targetUser, CancellationToken.None);

            Assert.Equal(0, targetUser.AccessFailedCount);
        }

        [Fact]
        public async Task ShouldAddUserToRole()
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> userStore = CreateUserStore();
            DocumentDbRoleStore<TestIdentityRole> roleStore = CreateRoleStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create();
            TestIdentityRole targetRole = DocumentDbIdentityRoleBuilder.Create("RoleName").WithId().WithNormalizedRoleName();

            // Create sample data role
            await roleStore.CreateAsync(targetRole, CancellationToken.None);

            // Add the created sample data role to the user
            await userStore.AddToRoleAsync(targetUser, targetRole.NormalizedName, CancellationToken.None);

            Assert.Contains(targetUser.Roles, r => r.NormalizedName.Equals(targetRole.NormalizedName));
        }

        [Fact]
        public async Task ShouldThrowExceptionOnAddingUserToNonexistantRole()
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> userStore = CreateUserStore();
            DocumentDbRoleStore<TestIdentityRole> roleStore = CreateRoleStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create();
            TestIdentityRole someNotTargetedRole = DocumentDbIdentityRoleBuilder.Create().WithId().WithNormalizedRoleName();

            // Create a role so there is a differently named role in the store
            await roleStore.CreateAsync(someNotTargetedRole, CancellationToken.None);

            // Add the user to a role name different than the role created before, expecting an exception
            await Assert.ThrowsAsync(typeof(ArgumentException), async () => await userStore.AddToRoleAsync(targetUser, "NotExistantRole", CancellationToken.None));
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenPassingNotNormalizedNameToAddToRole()
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> userStore = CreateUserStore();
            DocumentDbRoleStore<TestIdentityRole> roleStore = CreateRoleStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create();
            TestIdentityRole targetRole = DocumentDbIdentityRoleBuilder.Create().WithId().WithNormalizedRoleName();

            // Create sample data role
            await roleStore.CreateAsync(targetRole, CancellationToken.None);

            // Add the user to the created role, but pass the not normalized name, expecting an exception
            await Assert.ThrowsAsync(typeof(ArgumentException), async () => await userStore.AddToRoleAsync(targetUser, targetRole.Name, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldRemoveRoleFromUser()
        {
            DocumentDbUserStore<TestIdentityUser, TestIdentityRole> userStore = CreateUserStore();
            TestIdentityRole firstRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            TestIdentityRole secondRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().AddRole(firstRole).AddRole(secondRole);

            // Remove the second role
            await userStore.RemoveFromRoleAsync(targetUser, secondRole.NormalizedName, CancellationToken.None);

            // Assert second role has been removed while first one is still there
            Assert.DoesNotContain(targetUser.Roles, r => r.NormalizedName.Equals(secondRole.NormalizedName));
            Assert.Contains(targetUser.Roles, r => r.NormalizedName.Equals(firstRole.NormalizedName));
        }

        [Fact]
        public async Task ShouldNotRemoveRoleFromUserWhenPassingNotNormalizedRoleNameToRemoveFromRole()
        {
            var userStore = CreateUserStore();
            TestIdentityRole firstRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            TestIdentityRole secondRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().AddRole(firstRole).AddRole(secondRole);

            // Try remove the second role with a not normalized role name
            await userStore.RemoveFromRoleAsync(targetUser, secondRole.Name, CancellationToken.None);

            // Assert both roles are still here, as lookup without normalized name should have failed
            Assert.Collection(targetUser.Roles,
                r => r.NormalizedName.Equals(firstRole.NormalizedName),
                r => r.NormalizedName.Equals(secondRole.NormalizedName));
        }

        [Fact]
        public async Task ShouldReturnUserIsInRole()
        {
            var userStore = CreateUserStore();
            TestIdentityRole targetRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().AddRole(targetRole).AddRole();

            bool isInRole = await userStore.IsInRoleAsync(targetUser, targetRole.NormalizedName, CancellationToken.None);

            Assert.True(isInRole);
        }

        [Fact]
        public async Task ShouldReturnUserIsNotInRole()
        {
            var userStore = CreateUserStore();
            var targetUser = DocumentDbIdentityUserBuilder.Create().AddRole().AddRole();

            bool isInRole = await userStore.IsInRoleAsync(targetUser, "NonExistantRoleName", CancellationToken.None);

            Assert.False(isInRole);
        }

        [Fact]
        public async Task ShouldReturnUserIsNotInRoleWhenPassingNotNormalizedRoleNameToIsInRole()
        {
            var userStore = CreateUserStore();
            TestIdentityRole targetRole = DocumentDbIdentityRoleBuilder.Create().WithNormalizedRoleName();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().AddRole(targetRole).AddRole();

            // Pass not normalized name which should lead to not locating the target role
            bool isInRole = await userStore.IsInRoleAsync(targetUser, targetRole.Name, CancellationToken.None);

            Assert.False(isInRole);
        }

        [Fact]
        public async Task ShouldReturnUserByUserName()
        {
            var userStore = CreateUserStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName();

            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName());
            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName());
            CreateDocument(targetUser);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedUserName());

            TestIdentityUser foundUser = await userStore.FindByNameAsync(targetUser.NormalizedUserName, CancellationToken.None);

            Assert.Equal(targetUser.Id, foundUser.Id);
        }

        [Fact]
        public async Task ShouldReturnUserByEmail()
        {
            var userStore = CreateUserStore();
            TestIdentityUser targetUser = DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedEmail();

            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedEmail());
            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedEmail());
            CreateDocument(targetUser);
            CreateDocument(DocumentDbIdentityUserBuilder.Create().WithId().WithNormalizedEmail());

            IdentityUser<TestIdentityRole> foundUser = await userStore.FindByEmailAsync(targetUser.NormalizedEmail, CancellationToken.None);

            Assert.Equal(targetUser.Id, foundUser.Id);
        }
    }
}