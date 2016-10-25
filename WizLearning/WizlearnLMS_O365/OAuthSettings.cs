using System.Configuration;

namespace WizlearnLMS_O365
{
    public class OAuthSettings
    {

        private static string _clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string _clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string _tenantId = ConfigurationManager.AppSettings["ida:TenantID"];
        private static string _lmsHost = ConfigurationManager.AppSettings["ida:LmsHost"];
        private static string _lmsUrl = ConfigurationManager.AppSettings["ida:LmsUrl"];
        private static string _signInRedirect = ConfigurationManager.AppSettings["ida:SignInRedirect"];
        private static string _signOutRedirect = ConfigurationManager.AppSettings["ida:SignOutRedirect"];

        public static string ClientId { get { return _clientId; } }

        public static string TenantId { get { return _tenantId; } }

        public static string ClientSecret { get { return _clientSecret; } }

        public static string LmsUrl { get { return _lmsUrl; } }

        public static string LmsHost { get { return _lmsHost; } }

        public static string SignOutRedirect { get { return _signOutRedirect; } }

        public static string SignInRedirect { get { return _signInRedirect; } }
    }
}