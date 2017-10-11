using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Oogi2.AspNetCore.Identity.Tests.Fixtures
{
    [CollectionDefinition("DocumentDbCollection")]
    public class DocumentDbFixtureCollection : ICollectionFixture<DocumentDbFixture>
    {
    }
}
