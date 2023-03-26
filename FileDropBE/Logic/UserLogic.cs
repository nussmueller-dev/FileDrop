using FileDropBE.Database;
using FileDropBE.Database.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FileDropBE.Logic {
  public class UserLogic {
    private const double EXPIRY_DURATION_HOURS = 744;
    private static string JwtKey;

    private readonly IConfiguration _configuration;
    private readonly DB_Context _context;
    private readonly CurrentUserHelper _currentUserHelper;

    public UserLogic(IConfiguration configuration, DB_Context context, CurrentUserHelper currentUserHelper) {
      _configuration = configuration;
      _context = context;
      _currentUserHelper = currentUserHelper;

      if (JwtKey == null) {
        JwtKey = GenerateRandomString(104);
      }
    }

    public string HashPassword(string password, string salt) {
      var hashedPassword = KeyDerivation.Pbkdf2(
          password: password,
          salt: Convert.FromBase64String(salt),
          prf: KeyDerivationPrf.HMACSHA256,
          iterationCount: 100000,
          numBytesRequested: 256 / 8);

      return Convert.ToBase64String(hashedPassword);
    }

    public string GetSalt() {
      byte[] salt;
      do {
        salt = RandomNumberGenerator.GetBytes(128 / 8);
      } while (salt.Any(x => x == 0));

      return Convert.ToBase64String(salt);
    }

    public User GetUserFromToken(string token) {
      var tokenHandler = new JwtSecurityTokenHandler();
      var tokenValidationParameters = GetTokenValidationParameters();

      if (token is null || token.Length == 0) {
        return null;
      }

      try {
        tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
      } catch {
        return null;
      }

      var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
      var username = securityToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
      var user = _context.Users.FirstOrDefault(x => x.Username == username);

      if (user != null) {
        _currentUserHelper.SetToken(securityToken);
      }

      return user;
    }

    public string BuildToken(User user) {
      var key = JwtKey;
      var issuer = _configuration["Jwt:Issuer"];

      var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
      var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
          expires: DateTime.Now.AddHours(EXPIRY_DURATION_HOURS), signingCredentials: credentials);

      return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private TokenValidationParameters GetTokenValidationParameters() {
      var key = JwtKey;
      var issuer = _configuration["Jwt:Issuer"];

      var secret = Encoding.UTF8.GetBytes(key);
      var securityKey = new SymmetricSecurityKey(secret);

      return new TokenValidationParameters {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = issuer,
        ValidAudience = issuer,
        IssuerSigningKey = securityKey,
      };
    }

    private string GenerateRandomString(int length) {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!-.?,;:_$";
      var random = new Random();
      var randomChars = Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray();
      return new string(randomChars);
    }
  }
}
