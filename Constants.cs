public static class Constants
{
    public const int AccessTokenExpiration = 20;
    public const int RefreshTokenExpiration = 40;


    private const string FallbackTokenSecret = "superSecretKey@345";
    private const string FallbackDBPassword = "superSecretKey@345";

    public static string TokenSecret
    {
        get
        {
            string envSecret = Environment.GetEnvironmentVariable("TOKEN_SECRET");
            return envSecret ?? FallbackTokenSecret;
        }
    }

    public static string DBPassword
    {
        get
        {
        string envPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        return envPassword ?? FallbackDBPassword;
        }
    }

    private const string FallbackIssuer = "http://localhost:5000";

    public static string Issuer
    {
        get
        {
            string envIssuer = Environment.GetEnvironmentVariable("ISSUER");
            return envIssuer ?? FallbackIssuer;
        }
    }
    private const string FallbackAudience = "http://localhost:5000";

    public static string Audience
    {
        get
        {
            string envAudience = Environment.GetEnvironmentVariable("AUDIENCE");
            return envAudience ?? FallbackAudience;
        }
    }
}