using MatrixResponsibility.Services;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

[TestFixture]
public class AdAuthenticationServiceIntegrationTests
{
    private HttpClient _httpClient;
    private LDAPAuthenticationService _adAuthenticationService;

    [SetUp]
    public void SetUp()
    {
        // Настройка HttpClient
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://ldapservice.olimproekt.ru/"),
            Timeout = TimeSpan.FromSeconds(30)
        };

        // Инициализация сервиса
        _adAuthenticationService = new LDAPAuthenticationService(_httpClient);
    }

    [Test]
    public async Task Validate_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        string username = "ivanov";
        string password = "1Max242202366";

        // Act
        var result = await _adAuthenticationService.Validate(username, password);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task Validate_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        string username = "invaliduser";
        string password = "invalidpassword";

        // Act
        var result = await _adAuthenticationService.Validate(username, password);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task Validate_EmptyCredentials_HandlesGracefully()
    {
        // Arrange
        string username = "";
        string password = "";

        // Act
        var result = await _adAuthenticationService.Validate(username, password);

        // Assert
        Assert.IsFalse(result);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient?.Dispose();
    }
}