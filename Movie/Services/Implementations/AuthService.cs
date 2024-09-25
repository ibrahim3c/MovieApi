using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movie.Configs;
using Movie.Data;
using Movie.DTOs;
using Movie.Models;
using Movie.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Movie.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IMapper mapper;
        private readonly IOptionsMonitor<JwtConfigs> jWT;
        private readonly AppDbContext appDbContext;


        public AuthService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            IMapper mapper, IOptionsMonitor<JwtConfigs> JWT, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            jWT = JWT;
            this.appDbContext = appDbContext;
        }




        public async Task<AuthResult> LoginAsync(UserLoginDTO userLoginDTO)
        {
            var appUser = await userManager.FindByEmailAsync(userLoginDTO.Email);
            if (appUser == null) return new AuthResult
            {
                Errors = new List<string> { "Email or Password is incorrect!" },
                Success = false
            };


            bool checkPassword = await userManager.CheckPasswordAsync(appUser, userLoginDTO.Password);
            if (!checkPassword) return new AuthResult
            {
                Errors = new List<string> { "Email or Password is incorrect!" },
                Success = false
            };




            //generate toke
            return await GenerateJwtToken(appUser);


        }




        public async Task<AuthResult> RegisterAsync(UserRegisterationDTO userRegisterationDTO)
        {
            if (await userManager.FindByEmailAsync(userRegisterationDTO.Email) is not null)
                return new AuthResult
                {
                    Errors = new List<string> { "Email is already Registered!" },
                    Success = false
                };
            if (await userManager.FindByNameAsync(userRegisterationDTO.UserName) is not null)
                return new AuthResult
                {
                    Errors = new List<string> { "User Name is already Registered!" },
                    Success = false
                };


            //assign dto to user
            AppUser appUser = mapper.Map<AppUser>(userRegisterationDTO);


            var result = await userManager.CreateAsync(appUser, userRegisterationDTO.Password);
            if (!result.Succeeded)
            {
                // add errors
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }


                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string>(errors)
                };
            }


            // add roles to user => if u have this role
            //await userManager.AddToRoleAsync(appUser, "Admin");




            // addPermissions
            var up = new UserPermission
            {
                UserId = appUser.Id,
                PermissionId = (int)PermissionEnum.GetMovies
            };
            appDbContext.userPermissions.Add(up);
            appDbContext.SaveChanges();


            //generate token and refreshToken
            return await GenerateJwtToken(appUser);
        }




        private async Task<AuthResult> GenerateJwtToken(AppUser appUser)
        {
            // create claims
            var userClaims = await userManager.GetClaimsAsync(appUser);
            var roleClaims = new List<Claim>();

            var roles = await userManager.GetRolesAsync(appUser);
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));


            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,appUser.Id),
                new Claim(ClaimTypes.Name,appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email,appUser.Email),


                // to test policy
                new Claim("Gender","Male"),
                new Claim("Degree","51")
            }.Union(userClaims).Union(roleClaims);


            // we use symantic security key
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jWT.CurrentValue.SecretKey));

            // signCredintial
            // it will take (key,algorithm )
            SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //JWTSecurity responsible for hold token (header ,payload,singnature)
            JwtSecurityToken JwtToken = new JwtSecurityToken(
                issuer: jWT.CurrentValue.Issuer
                ,
                audience: jWT.CurrentValue.Audience
                ,
                claims: claims
                ,
                expires: DateTime.Now.AddMinutes(jWT.CurrentValue.Expire)
                ,
                signingCredentials: signingCredentials
            );


            var AuthResult = new AuthResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JwtToken),
                ExpiresOn = JwtToken.ValidTo
            };



            // if he has refresh token return it as result
            if (appUser.RefreshTokens != null && appUser.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = appUser.RefreshTokens.FirstOrDefault(t => t.IsActive);
                AuthResult.RefreshToken = activeRefreshToken.Token;
                AuthResult.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }

            // if he has not we will generate one and assing it to result
            else
            {
                var refreshToken = GenerateRefreshToken();
                AuthResult.RefreshToken = refreshToken.Token;
                AuthResult.RefreshTokenExpiration = refreshToken.ExpiresOn;


                //save refreshToken in db
                appUser.RefreshTokens.Add(refreshToken);
                await userManager.UpdateAsync(appUser);

                // or                
                // appDbContext.RefreshTokens.Add(refreshToken);
                //await appDbContext.SaveChangesAsync();




            }




            return AuthResult;


        }


        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);


            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.Now.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };



        }


        public async Task<AuthResult> RefreshTokenAsync(string token)
        {
            
            // ensure this token is for user
            var appUser = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token==token));
            if (appUser == null) return new AuthResult
            {
                Success = false,
                Errors=new List<string> { "Invalid Token"}
                // Errors = new List<string> { "No user has this Token" }

            };

            //ensure this token is active
            var refreshToken=appUser.RefreshTokens.SingleOrDefault(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new List<string> { "Invalid Token" }
                    // Errors = new List<string> { "InActive Token" }

                };
            }

            //here this token has user and active

            // we need to revoke this old token
            //refreshToken.RevokedOn = DateTime.UtcNow;
            appUser.RefreshTokens.SingleOrDefault(t => t.Token == token).RevokedOn=DateTime.UtcNow;

             return  await GenerateJwtToken(appUser); // here we generate new jwtToken and it will assgin new refreshToken for this user

        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            // ensure this token is for user
            var appUser = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (appUser == null) return false;

            //ensure this token is active
            var refreshToken = appUser.RefreshTokens.SingleOrDefault(t => t.Token == token);
            if (!refreshToken.IsActive) return false;

            //here this token has user and active

            // we need to revoke this old token
           appUser.RefreshTokens.SingleOrDefault(t => t.Token == token).RevokedOn = DateTime.UtcNow;

            // update user
            await userManager.UpdateAsync(appUser);

            return true;
        }


        public async Task<string> GetRefreshTokenOfUser(string email)
        {
            var appUser = await userManager.FindByEmailAsync(email);
            var refreshToken=appUser.RefreshTokens.FirstOrDefault(t => t.IsActive);
            if (refreshToken is null) return string.Empty;
            return refreshToken.Token;
        }


        // this for role and users => u can add rest of operation like delete role or delete user and so on
        public async Task<RoleResult> AddRoleToUserAsync(AddRoleToUserDTO addRoleDTO)
        {
            //check if user exists
            var user = await userManager.FindByIdAsync(addRoleDTO.UserID);
            if (user == null) return new RoleResult
            {
                Success = false,
                Errors = new List<string> { "Invalid User ID or Role" }
            };


            //check if role exists
            var role = await roleManager.FindByNameAsync(addRoleDTO.RoleName);
            if (role == null) return new RoleResult
            {
                Success = false,
                Errors = new List<string> { "Invalid User ID or Role" }
            };


            //check if user doesnot have this role
            if (await userManager.IsInRoleAsync(user, addRoleDTO.RoleName)) return new RoleResult
            {
                Success = false,
                Errors = new List<string> { "User already assigned to this role" }
            };


            var result = await userManager.AddToRoleAsync(user, addRoleDTO.RoleName);
            if (!result.Succeeded)
            {
                // add errors
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }


                return new RoleResult
                {
                    Success = false,
                    Errors = new List<string>(errors)
                };
            }


            return new RoleResult
            {
                Success = true
            };


        }
        public async Task<RoleResult> CreateRoleAsync(string roleName)
        {
            // Check if the role already exists
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return new RoleResult
                {
                    Success = false,
                    Errors = new List<string> { "Role already exists" }
                };
            }


            // Create the role
            var roleResult = await roleManager.CreateAsync(new AppRole { Name = roleName });


            if (!roleResult.Succeeded)
            {
                // Add errors
                var errors = new List<string>();
                foreach (var error in roleResult.Errors)
                {
                    errors.Add(error.Description);
                }


                return new RoleResult
                {
                    Success = false,
                    Errors = errors
                };
            }


            return new RoleResult
            {
                Success = true
            };
        }


        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await userManager.Users.ToListAsync();
        }


        public async Task<List<AppRole>> GetAllRoles()
        {
            return await roleManager.Roles.ToListAsync();
        }

    }
}
