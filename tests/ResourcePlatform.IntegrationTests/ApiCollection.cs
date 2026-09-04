namespace ResourcePlatform.IntegrationTests;


/// <summary>
/// One container and one migrated database shared by every test class.
/// Tests stay independent by creating their own users and organizations.
/// </summary>
[CollectionDefinition(nameof(ApiCollection))]
public class ApiCollection : ICollectionFixture<ApiFactory>;