using Oogi2.AspNetCore.Identity.Stores;
using Oogi2.AspNetCore.Identity.Tests.Fixtures;
using Xunit;
using Oogi2.AspNetCore.Identity.Tests.Entities;

namespace Oogi2.AspNetCore.Identity.Tests
{
    public abstract class StoreTestsBase : IClassFixture<DocumentDbFixture>
    {
        protected Repository<TestIdentityUser> _repoUsers;
        protected Repository<TestIdentityRole> _repoRoles;
        Connection _connection;

        protected StoreTestsBase(DocumentDbFixture documentDbFixture)
        {
            _connection = documentDbFixture.Connection;
            _repoUsers = new Repository<TestIdentityUser>(documentDbFixture.Connection);
            _repoRoles = new Repository<TestIdentityRole>(documentDbFixture.Connection);
        }

        protected void CreateDocument(TestIdentityUser user)
        {
            _repoUsers.Create(user);
        }

        protected void CreateDocument(TestIdentityRole role)
        {
            _repoRoles.Create(role);
        }

        protected DocumentDbUserStore<TestIdentityUser, TestIdentityRole> CreateUserStore()
        {
            return new DocumentDbUserStore<TestIdentityUser, TestIdentityRole>(_connection);
        }

        protected DocumentDbRoleStore<TestIdentityRole> CreateRoleStore()
        {
            return new DocumentDbRoleStore<TestIdentityRole>(_connection);
        }
    }
}