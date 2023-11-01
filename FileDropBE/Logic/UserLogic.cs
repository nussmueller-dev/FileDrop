using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Database.Entities;
using FileDropBE.Hubs;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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
    private readonly IHubContext<LoginHub> _loginHub;

    private static IList<string> QrLoginTokens = new List<string>();
    private static IList<AcceptQrLoginBindingModel> AcceptedQrLoginModels = new List<AcceptQrLoginBindingModel>();

    public UserLogic(IConfiguration configuration, DB_Context context, CurrentUserHelper currentUserHelper, IHubContext<LoginHub> loginHub) {
      _configuration = configuration;
      _context = context;
      _currentUserHelper = currentUserHelper;
      _loginHub = loginHub;

      if (JwtKey == null) {
        JwtKey = GenerateRandomString(104);
      }
    }

    public string CreateQrLogin() {
      var token = GenerateRandomString(56)
        .Replace(" ", "_")
        .Replace("+", "_")
        .Replace("&", "-");
      QrLoginTokens.Add(token);

      return token;
    }

    public bool AcceptQrLogin(AcceptQrLoginBindingModel model) {
      if (!QrLoginTokens.Contains(model.Token)) {
        return false;
      }

      model.User = _currentUserHelper.CurrentUser;

      QrLoginTokens.Remove(model.Token);
      AcceptedQrLoginModels.Add(model);
      InformAboutQrLoginAccepted();

      return true;
    }

    public AcceptQrLoginBindingModel GetAcceptedQrLoginModelFromToken(string token) {
      var model = AcceptedQrLoginModels.FirstOrDefault(x => x.Token == token);

      if (model != null) {
        AcceptedQrLoginModels.Remove(model);
      }

      return model;
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

    public string BuildToken(User user, double? validForHours = null) {
      var key = JwtKey;
      var issuer = _configuration["Jwt:Issuer"];

      var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };

      if (validForHours is null) {
        validForHours = EXPIRY_DURATION_HOURS;
      }

      DateTime? tokenExpireDate = validForHours == 0 ? DateTime.Now.AddYears(10) : DateTime.Now.AddHours(validForHours.Value);
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
      var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims, expires: tokenExpireDate, signingCredentials: credentials);

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
      return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
    }

    private void InformAboutQrLoginAccepted() {
      _loginHub.Clients.All.SendAsync("QrLoginAccepted");
    }
  }
}
