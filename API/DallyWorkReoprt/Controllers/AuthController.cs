using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using DallyWorkReoprt.Utilities.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DallyWorkReoprt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        IAuthenticateRepository _Authenticate;
        IConfiguration _Config;
        private ICompanyMasterRepository _company;
        private IConfiguration _configuration;
        private IEmployeeRepository _employee;
        public AuthController(IAuthenticateRepository user, IMapper mapper, IConfiguration config, IEmployeeRepository
            employee, IConfiguration configuration, ICompanyMasterRepository Company)
        {
            _Authenticate = user;
            _Config = config;
            _employee = employee;
            _configuration = configuration;
            _company = Company;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {

            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }
            var IsValidUser = await _Authenticate.IsValidEmployeeAsync(login.Email, new EncryptionHelper().Encrypt(login.Password), ActiveStatus: Convert.ToByte(1));
            if (!IsValidUser)
            {
                return ValidationErrorResponse("Invalid Credentials");
            }
            var employee = _Authenticate.GetEmployeeQueryable(login.Email, new EncryptionHelper().Encrypt(login.Password), ActiveStatus: Convert.ToByte(1))
                .Select(s => new
                {
                    s.EmployeeName,
                    s.EmployeeId,
                    s.Email,
                    s.RoleMaster!.RoleName,
                    RoleType = s.RoleMaster.RoleType.LookupName,
                    s.Tenant,
                    s.IsAllowLogin,
                    s.CompanyId,
                    s.RoleMasterId,
                    s.DefaultBreakDuration
                }).FirstOrDefault();

            if (employee == null)
            {
                return ValidationErrorResponse("unable to find user data");
            }
            if (Convert.ToInt16(employee?.IsAllowLogin) == 0)
            {
                return ValidationErrorResponse("your account disabled contact administrator");
            }
            var tokenUser = new UserBasicDTO
            {
                EmployeeID = employee!.EmployeeId,
                UserName = employee.EmployeeName ?? "",
                Email = employee.Email,
                RoleName = employee.RoleName,
                RoleType = employee.RoleType,
                RoleId = employee.RoleMasterId ?? 0,
                CompanyId = employee.CompanyId,
                IsTenant = employee.Tenant,
                DefaultBreakDuration = employee.DefaultBreakDuration
            };
            TokenResponse tokenResponse = GenerateTokens(tokenUser);
            return Ok(ApiResponse<TokenResponse>.SuccessResponse(tokenResponse, "Token Generated"));
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrorResponse();
            }

            if (!_configuration.GetValue<bool>("AllowSignUp"))
            {
                return ValidationErrorResponse("Sign up is currently disabled.");
            }

            try
            {
                string encryptedPassword = new EncryptionHelper().Encrypt(model.Password);

                var companyEntity = new CompanyMaster
                {
                    CompanyName = model.CompanyName,
                    Email = model.Email,
                    ActiveStatus = 1,
                    IsEmailVerified = 0,
                    IsMobileNoVerified = 0,
                    CreateDate = DateTime.UtcNow,
                    Guids = Guid.NewGuid(),
                    CountryId = 1,
                    StateId = 1
                };

                await _company.AddAsync(companyEntity);
                var employeeEntity = new EmployeeMaster
                {
                    Tenant = true,
                    CompanyId = companyEntity.CompanyId,
                    ActiveStatus = 1,
                    CreateDate = DateTime.UtcNow,
                    CreatedById = "S",
                    FirstName = model.FullName,
                    SignInAttempt = 0,
                    Passwords = encryptedPassword,
                    Email = model.Email,
                    IsAllowLogin = 0,
                    Guids = Guid.NewGuid()
                };

                await _employee.AddAsync(employeeEntity);

                return Ok(ApiResponse<string>.SuccessResponse(null, "Signup successful."));
            }
            catch (Exception ex)
            {
                var fullMessage = ex.Message + (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "");
                return ValidationErrorResponse("Unable to complete signup: " + fullMessage);
            }
        }

        [NonAction]
        private TokenResponse GenerateTokens(UserBasicDTO user)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_Config["Jwt:Key"]!));
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.EmployeeID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.UserName))
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            if (!string.IsNullOrWhiteSpace(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            if (!string.IsNullOrWhiteSpace(user.RoleName))
                claims.Add(new Claim(ClaimTypes.Role, user.RoleName));

            if (!string.IsNullOrWhiteSpace(user.RoleType))
                claims.Add(new Claim("RoleType", user.RoleType));
            claims.Add(new Claim("CompanyId", user.CompanyId.ToString()));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                Issuer = _Config["Jwt:Issuer"],
                Audience = _Config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = jwtHandler.CreateToken(tokenDescriptor);
            var jwt = jwtHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                JwtId = token.Id,
                UserId = user.EmployeeID,
                ExpiryDate = DateTime.UtcNow.AddYears(1)
            };

            _Authenticate.Add(refreshToken);

            return new TokenResponse
            {
                AccessToken = jwt,
                RefreshToken = refreshToken.Token,
                User = user
            };
        }
    }
}

