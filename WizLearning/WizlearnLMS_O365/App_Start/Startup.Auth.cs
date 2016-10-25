using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace WizlearnLMS_O365
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieManager = new SystemWebCookieManager()
            });

            var authority = string.Format("{0}/{1}", ServiceConstants.AzureADEndPoint, "common");

            var options = new OpenIdConnectAuthenticationOptions {
                ClientId = OAuthSettings.ClientId,
                Authority = authority,
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters {
                    ValidateIssuer = false
                }
            };

            options.Notifications = new OpenIdConnectAuthenticationNotifications
            {
                AuthorizationCodeReceived = (context) => {

                    ClientCredential credential = new ClientCredential(OAuthSettings.ClientId, OAuthSettings.ClientSecret);

                    string nameIdentifier = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    string tenantId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

                    AuthenticationContext authContext = new AuthenticationContext(string.Format("{0}/{1}", ServiceConstants.AzureADEndPoint, tenantId), new SqlDBTokenCache(nameIdentifier));

                    authContext.AcquireTokenByAuthorizationCode(
                        authorizationCode: context.Code, 
                        redirectUri: new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), 
                        clientCredential: credential, 
                        resource: ServiceConstants.WindowsGraphResourceId);

                    return Task.FromResult(0);
                },

                RedirectToIdentityProvider = (context) =>
                {
                    string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;

                    context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                    //context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                    context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl + "/Account/SignOutCallback";

                    return Task.FromResult(0);
                },

                AuthenticationFailed = (context) =>
                {
                    context.HandleResponse();

                    return Task.FromResult(0);
                }
            };

            app.UseOpenIdConnectAuthentication(options);
        }
    }

    public class SystemWebCookieManager : ICookieManager
    {
        public string GetRequestCookie(IOwinContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);
            var cookie = webContext.Request.Cookies[key];
            return cookie == null ? null : cookie.Value;
        }

        public void AppendResponseCookie(IOwinContext context, string key, string value, CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            var webContext = context.Get<HttpContextBase>(typeof(HttpContextBase).FullName);

            bool domainHasValue = !string.IsNullOrEmpty(options.Domain);
            bool pathHasValue = !string.IsNullOrEmpty(options.Path);
            bool expiresHasValue = options.Expires.HasValue;

            var cookie = new HttpCookie(key, value);
            if (domainHasValue)
            {
                cookie.Domain = options.Domain;
            }
            if (pathHasValue)
            {
                cookie.Path = options.Path;
            }
            if (expiresHasValue)
            {
                cookie.Expires = options.Expires.Value;
            }
            if (options.Secure)
            {
                cookie.Secure = true;
            }
            if (options.HttpOnly)
            {
                cookie.HttpOnly = true;
            }

            webContext.Response.AppendCookie(cookie);
        }

        public void DeleteCookie(IOwinContext context, string key, CookieOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            AppendResponseCookie(
                context,
                key,
                string.Empty,
                new CookieOptions
                {
                    Path = options.Path,
                    Domain = options.Domain,
                    Expires = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                });
        }
    }
}