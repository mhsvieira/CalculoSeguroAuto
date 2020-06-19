using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SeguroAutomovel.Provider
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            try
            {
                var user = context.UserName;
                var password = context.Password;

                // validação simples das credenciais, poderia buscar no banco de dados
                if (user != "userauto" || password != "zA504yJ!")
                {
                    context.SetError("invalid_grant", "Usuário ou senha inválidos.");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim(ClaimTypes.Name, user));

                var roles = new List<string>();

                roles.Add("Admin");

                roles.Add("User");

                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                GenericPrincipal principal = new GenericPrincipal(identity, roles.ToArray());

                Thread.CurrentPrincipal = principal;

                context.Validated(identity);
            }
            catch (System.Exception e)
            {
                context.SetError("invalid_grant", "Falha ao autenticar. " + e.Message);
            }
        }
    }
}