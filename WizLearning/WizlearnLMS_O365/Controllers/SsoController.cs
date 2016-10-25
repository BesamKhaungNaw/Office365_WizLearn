using Edulearn.Vcube.Libraries;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WizlearnLMS_O365.Controllers
{
    public class SsoController : Controller
    {
        public async Task<ActionResult> Index()
        {
            if(Request.IsAuthenticated)
            {

                var _factory = new ServiceClientFactory();

                var accessToken = await _factory.GetAccessToken();
                var jwt = new JwtSecurityToken(accessToken);
                var name = jwt.Claims.Where(q => q.Type == "unique_name").FirstOrDefault().Value;
                var msoId = (object)this.Server.UrlEncode(Crypo.EncryptToBase64(name, "SSOUser", "asknlearn"));

                return Redirect($"{OAuthSettings.LmsHost}{OAuthSettings.LmsUrl}msoID={msoId}");
            }

            var content = new ContentResult();
            content.Content = "For some reason, the app thinks you're unauthenticated.";

            return content;

        }
    }
}