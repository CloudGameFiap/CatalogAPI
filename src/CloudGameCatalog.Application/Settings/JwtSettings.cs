namespace CloudGameCatalog.Application.Settings;

public class JwtSettings
{
    public string EncriptKey { get; set; }
    public int ExpiresInMinutes { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
}
