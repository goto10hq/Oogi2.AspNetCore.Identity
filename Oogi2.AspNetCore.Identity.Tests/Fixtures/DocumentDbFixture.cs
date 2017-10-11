﻿using System;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;

namespace Oogi2.AspNetCore.Identity.Tests.Fixtures
{
    public class DocumentDbFixture : IDisposable
    {
        public DocumentClient Client { get; private set; }
        public ILookupNormalizer Normalizer { get; private set; }
        public string Database { get; } = "AspNetCore.Identity.DocumentDb.Tests";
        public string UserStoreDocumentCollection { get; private set; } = "AspNetCore.Identity";
        public string RoleStoreDocumentCollection { get; private set; } = "AspNetCore.Identity";
        public Uri DatabaseLink { get; private set; }
        public Uri UserStoreCollectionLink { get; private set; }
        public Uri RoleStoreCollectionLink { get; private set; }

        public DocumentDbFixture()
        {
            Client = new DocumentClient(
                serviceEndpoint: new Uri("https://localhost:8081", UriKind.Absolute),
                authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
                connectionPolicy: new ConnectionPolicy() { EnableEndpointDiscovery = false });

            DatabaseLink = UriFactory.CreateDatabaseUri(this.Database);
            UserStoreCollectionLink = UriFactory.CreateDocumentCollectionUri(this.Database, this.UserStoreDocumentCollection);
            RoleStoreCollectionLink = UriFactory.CreateDocumentCollectionUri(this.Database, this.RoleStoreDocumentCollection);

            CreateTestDatabase();
        }

        private void CreateTestDatabase()
        {
            CleanupTestDatabase();

            Database db = this.Client.CreateDatabaseAsync(new Database() { Id = this.Database }).Result;

            DocumentCollection userCollection = this.Client.CreateDocumentCollectionAsync(
                DatabaseLink,
                new DocumentCollection() { Id = this.UserStoreDocumentCollection }).Result;

            if (this.UserStoreDocumentCollection != this.RoleStoreDocumentCollection)
            {
                DocumentCollection roleCollection = this.Client.CreateDocumentCollectionAsync(
                    DatabaseLink,
                    new DocumentCollection() { Id = this.RoleStoreDocumentCollection }).Result;
            }
        }

        private void CleanupTestDatabase()
        {
            try
            {
                var existingDb = this.Client.ReadDatabaseAsync(this.DatabaseLink).Result;
            }
            catch (Exception)
            {
                // Suppose DB does not exist (anymore)
                return;
            }

            var db = this.Client.DeleteDatabaseAsync(this.DatabaseLink).Result;
        }

        public void Dispose()
        {
            CleanupTestDatabase();
        }
    }
}