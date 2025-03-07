using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class UserAuthentication : IUserAuthentication
{
    private readonly IDataRepo dataRepo;
    private readonly Dictionary<string, Session> _sessions;
    public UserAuthentication(IDataRepo dataRepo)
    {
        this.dataRepo = dataRepo;
        _sessions = new Dictionary<string, Session>();
    }

    public User GenerateUser(string username, string password)
    {
        var salt = GenerateSalt();
        var PasswordHash = HashPassword(password, salt);
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            PasswordHash = PasswordHash,
            PasswordSalt = salt,
            Created = DateTime.UtcNow,
            LastActive = DateTime.UtcNow
        };
    }

    public Session AuthenticateUser(string username, string password)
    {
        User user = dataRepo.GetUser(username);
        if (user == null)
        {
            throw new ArgumentException("Invalid username or password");
        }
        var PasswordHash = HashPassword(password, user.PasswordSalt);
        if (PasswordHash != user.PasswordHash)
        {
            throw new ArgumentException("Invalid username or password");
        }

        var expires = GetExpiration();
        var refreshExpires = GetRefreshExpiration();
        var tokenString = GenerateJSONWebToken(user.Username, expires);
        var refreshToken = GenerateRefreshToken();

        Session session = new Session
        {
            AccessToken = tokenString,
            RefreshToken = refreshToken,
            Username = user.Username,
            Expiration = expires,
            RefreshExpiration = refreshExpires
        };

        _sessions[user.Username] = session;
        return session;
    }

    private string HashPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = password + salt;
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    public Session RefreshToken(string username, string refreshToken)
    {
        if 
        (!IsValidRefreshRequest(username, refreshToken))
        {
            throw new ArgumentException("Invalid refresh request");
        }
        var expires = GetExpiration();
        var refreshExpires = GetRefreshExpiration();
        var tokenString = GenerateJSONWebToken(username, expires);
        var newRefreshToken = GenerateRefreshToken();
        Session session = new Session {
                AccessToken = tokenString,
                RefreshToken = newRefreshToken,
                Username = username,
                Expiration = expires,
                RefreshExpiration = refreshExpires
            };
        _sessions[username] = session;
        return session;
    }

    public void RevokeToken(string username, string token)
    {
        if (_sessions.ContainsKey(username) && _sessions[username].AccessToken == token)
        {
            _sessions.Remove(username);
        }
    }

    public Session ValidateToken(string username, string token)
    {
         var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Constants.TokenSecret);
            var TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Constants.Issuer,
                ValidAudience = Constants.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            tokenHandler.ValidateToken(token, TokenValidationParameters, out SecurityToken validatedToken);

            if (!_sessions.ContainsKey(username) || _sessions[username].AccessToken != token || _sessions[username].Expiration < DateTime.UtcNow)
            {
                throw new ArgumentException("Invalid token.");
            }

            return _sessions[username];
    }

     private string GenerateJSONWebToken(string username, DateTime expires)
    {
        var claims = new []
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.TokenSecret));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: Constants.Issuer,
            audience: Constants.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signinCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private DateTime GetExpiration()
    {
        return DateTime.UtcNow.AddMinutes(Constants.AccessTokenExpiration);
    }

    private DateTime GetRefreshExpiration()
    {
        return DateTime.UtcNow.AddMinutes(Constants.RefreshTokenExpiration);
    }

    public bool IsValidRefreshRequest(string username, string refreshToken)
    {
        if (username == null || refreshToken == null)
        {
            return false;
        }
        if (!_sessions.ContainsKey(username)) {
            return false;
        }
        if (_sessions[username].RefreshExpiration < DateTime.UtcNow) {
            return false;
        }
        if (_sessions[username].RefreshToken == refreshToken) {
            return true;
        }
        return false;
    }

    private string GenerateSalt()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}