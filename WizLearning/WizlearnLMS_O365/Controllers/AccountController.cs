using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Web;
using System.Web.Mvc;

namespace WizlearnLMS_O365.Controllers
{
    /// <summary>
    /// Controls sign-in and sign-out.
    /// </summary>

    public class AccountController : Controller
    {

        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                var redirectUri = "/";

                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    redirectUri = OAuthSettings.SignInRedirect;
                }

                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = redirectUri }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }


        public ActionResult ConsentApp()
        {
            string resourceId = ServiceConstants.OutlookServiceId;

            string authorizationRequest = String.Format(
                "https://login.windows.net/common/oauth2/authorize?response_type=code&client_id={0}&resource={1}&redirect_uri={2}",
                    Uri.EscapeDataString(OAuthSettings.ClientId),
                    Uri.EscapeDataString(resourceId),
                    Uri.EscapeDataString(Request.Url.GetLeftPart(UriPartial.Authority))
                    );

            return new RedirectResult(authorizationRequest);
        }

        public ActionResult AdminConsentApp()
        {
            string resourceId = ServiceConstants.OutlookServiceId;

            string authorizationRequest = String.Format(
                "https://login.windows.net/common/oauth2/authorize?response_type=code&client_id={0}&resource={1}&redirect_uri={2}&prompt={3}",
                    Uri.EscapeDataString(OAuthSettings.ClientId),
                    Uri.EscapeDataString(resourceId),
                    Uri.EscapeDataString(Request.Url.GetLeftPart(UriPartial.Authority)),
                    Uri.EscapeDataString("admin_consent")
                    );

            return new RedirectResult(authorizationRequest);
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public void EndSession()
        {
            // If AAD sends a single sign-out message to the app, end the user's session, but don't redirect to AAD for sign out.
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
        }

        // Called inside Startup.Auth for post-redirect-url
        public ActionResult SignOutCallback()
        {
            return Redirect(OAuthSettings.SignOutRedirect);

        }

        public void ClearCacheDatabase()
        {
            var context = new TokenCacheDbContext();

            foreach(var cacheItem in context.UserTokenCaches)
            {
                context.UserTokenCaches.Remove(cacheItem);
            }

            context.SaveChanges();

            string strRedirectController = Request.QueryString["redirect"];

            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = String.Format("/{0}", strRedirectController) }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}