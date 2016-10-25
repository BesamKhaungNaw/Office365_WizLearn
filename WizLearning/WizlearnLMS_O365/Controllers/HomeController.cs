using Microsoft.Office365.OutlookServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using WizlearnLMS_O365.Models.File;

namespace WizlearnLMS_O365.Controllers
{
    public class HomeController : Controller
    {
   
    
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("ViewDetails");
                //return RedirectToAction("Index", "Sso");
            }
            else
                return RedirectToAction("SignIn", "Account");
        }

        

        public async Task<ActionResult> ViewDetails(string id="")
        {
            if (id == "")
            {
                Utility.root = new DriveItem();
                Utility.root = JsonConvert.DeserializeObject<DriveItem>(await SendRequestToAPI("_api/v2.0/me/drive/root","GET"));
                id = Utility.root.Id;
            }
             
            DriveItemList list = await getData(id);
            //list.parentDriveId = id;
            Utility.root.Id = id;
            //  return View(getData(id));

            //  ContentResult result = new ContentResult();
            //    result.Content = $"TenantId: {tenantId}<br><br>Url Root: {urlRoot}<br><br>accessToken: {accessToken}<br><br><h3> making a call to {driveEndpoint}</h3>: <br>Result: {list} ";
            //   var driveEndpoint = $"_api/v2.0/me/drive";
            // return result;
            return View(list);
        }


        public ActionResult ViewUploadFile()
        {
            return View();
        }
      
        public async Task<ActionResult> UploadFile(String model)
        {
          
            //var drive = JsonConvert.DeserializeObject<Drive>(await SendRequestToAPI("_api/v2.0/me/drive","GET"));
            //var root = JsonConvert.DeserializeObject<DriveItem>(await SendRequestToAPI("_api/v2.0/me/drive/root","GET"));

            Stream memPhoto = null;
             String filename = null;
         
            foreach (string upload in Request.Files)
            {

                filename = Path.GetFileName(Request.Files[upload].FileName);

                memPhoto = Request.Files[upload].InputStream;
                // fileStream.Read();
            }

         
            Models.File.DriveItem result = null;

          //  Stream memPhoto = getFileContent(filePath);
            if (memPhoto.Length > 0)
            {
                String contentType = "image/png";
                await UploadFileDirect(Utility.drive.Id, Utility.root.Id,
                     new Models.File.DriveItem
                     {
                         File = new Models.File.File { },
                         //   Name = filePath.Substring(filePath.LastIndexOf("\\") + 1),
                         Name = filename+1,
                         ConflictBehavior = "rename",
                     },
                     memPhoto,
                     contentType);
            }

            return new RedirectResult(Request.UrlReferrer.ToString());

        }


        public async Task<String> UploadFileDirect(String driveId, String parentFolderId,
            DriveItem file, Stream content, String contentType)
        {
            var endPoint = String.Format("_api/v2.0/drives/{0}/items/{1}/children/{2}/content",
                    driveId,
                    parentFolderId,
                    file.Name);

               String list = await SendRequestToAPI(endPoint,"PUT", contentType, content);
            return list;
        }

        public async Task<String> SendRequestToAPI(String driveEndpoint,String requestType,String contentType=null, Stream content=null)
        {
            var _factory = new ServiceClientFactory();

            var accessToken = await _factory.GetAccessToken();
            var jwt = new JwtSecurityToken(accessToken);
            var name = jwt.Claims.Where(q => q.Type == "unique_name").FirstOrDefault().Value;
            var tenantId = jwt.Claims.Where(q => q.Type == "tid").FirstOrDefault().Value;
            var urlRoot = jwt.Claims.Where(q => q.Type == "aud").FirstOrDefault().Value;
            var list = "";

            // Currently using HttpClient but you can check out RestRequest library from Nuget
            // that will help make things easier. 
            HttpClient client = new HttpClient();

            // Base address points to {tenant}-my.sharepoint.com
            client.BaseAddress = new Uri(urlRoot);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Make call to Drive
            //     HttpResponseMessage response = await client.GetAsync(driveEndpoint);
            HttpRequestMessage request = new HttpRequestMessage(
                  new HttpMethod(requestType), driveEndpoint);
            if (content != null)
            {
                HttpContent requestContent = null;
                System.IO.Stream streamContent = content as System.IO.Stream;
                if (streamContent != null)
                {
                    requestContent = new StreamContent(streamContent);
                 //   requestContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                //    request.Content = requestContent;
                }

            }
           
            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                // Return json response as string
                list = await response.Content.ReadAsStringAsync();
            }

            //    return (drive);

            return list;
        }



        public async Task<DriveItemList> getData(string rootid)
        {
            Utility.drive = new Drive();
            Utility.drive = JsonConvert.DeserializeObject<Drive>(await SendRequestToAPI("_api/v2.0/me/drive","GET"));
            
           
            String childenEndPoint = String.Format("_api/v2.0/drives/{0}/items/{1}/children?$top={2}",
                    Utility.drive.Id,
                    rootid,
                    100);
            var childrenItems = JsonConvert.DeserializeObject<DriveItemList>(await SendRequestToAPI(childenEndPoint,"GET"));
            return childrenItems;
          
        }

        //public async Task<String> getStringData(String driveEndpoint)
        //{
        //    var _factory = new ServiceClientFactory();

        //    var accessToken = await _factory.GetAccessToken();
        //    var jwt = new JwtSecurityToken(accessToken);
        //    var name = jwt.Claims.Where(q => q.Type == "unique_name").FirstOrDefault().Value;
        //    var tenantId = jwt.Claims.Where(q => q.Type == "tid").FirstOrDefault().Value;
        //    var urlRoot = jwt.Claims.Where(q => q.Type == "aud").FirstOrDefault().Value;
        //    var list = "";


        //    //     var driveEndpoint = $"_api/v2.0/drive/root/children";
         
        //    // Currently using HttpClient but you can check out RestRequest library from Nuget
        //    // that will help make things easier. 
        //    HttpClient client = new HttpClient();

        //    // Base address points to {tenant}-my.sharepoint.com
        //    client.BaseAddress = new Uri(urlRoot);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    // Add Authorization header
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //    // Make call to Drive
        //   HttpResponseMessage response = await client.GetAsync(driveEndpoint);
          
           

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Return json response as string
        //        list = await response.Content.ReadAsStringAsync();
        //    }
          
        //    //    return (drive);

        //    return list;
        //}

    }

}