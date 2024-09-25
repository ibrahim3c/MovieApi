using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Movie.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) return Task.FromResult(AuthenticateResult.NoResult());
            if (!Request.Headers["Authorization"].ToString().StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.NoResult());

            var EncodedCredential = (Request.Headers["Authorization"].ToString())["Basic ".Length..];
            var DecodedCredential = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(EncodedCredential));
            var userNamePassword = DecodedCredential.Split(":");
            var userName = userNamePassword[0];
            var password = userNamePassword[1];

            if (userName != "Ibrahim" || password != "ibrahim1020") return Task.FromResult(AuthenticateResult.Fail("Invalid UserName of Password"));


            ClaimsIdentity claims = new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier,"1")

                },"Basic");

            var principal =new ClaimsPrincipal(claims);
            var ticket = new AuthenticationTicket(principal,"Basic");
            return Task.FromResult(AuthenticateResult.Success(ticket));



        }
    }
}
