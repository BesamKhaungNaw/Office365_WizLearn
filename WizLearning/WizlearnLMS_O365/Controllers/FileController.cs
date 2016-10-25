using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WizlearnLMS_O365.Models.OneDrive;
using Microsoft.Office365.SharePoint.FileServices;
using Microsoft.OData.Core;
using Microsoft.OData.Client;
using System.Diagnostics;
using System.IO;

namespace WizlearnLMS_O365.Controllers
{
    [Authorize]
    [HandleError(ExceptionType = typeof(Office365AssertFailedException))]
    public class FileController : Controller
    {
        private ServiceClientFactory _factory;


        public async Task<ActionResult> Index(string id, string nexturl)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("SignIn", "Account");

            _factory = new ServiceClientFactory();

            var sharepointClient = await _factory.CreateSharepointServicesClientWithAsync("MyFiles");

            List<FileObject> returnResults = new List<FileObject>();

            try
            {
                // Performs a search of the default Documents folder.
                // You could also specify other folders using the syntax: var filesResults = await _client.Files["folder_name"].ExecuteAsync();
                // This results in a call to the service.
                var filesResults = await sharepointClient.Files.ExecuteAsync();

                var files = filesResults.CurrentPage;

                foreach (IItem fileItem in files)
                {
                    // The item to add to the result set.
                    FileObject modelFile = new FileObject(fileItem);

                    if(modelFile.DisplayName == "File")
                        returnResults.Add(modelFile);
                }
            }
            catch (ODataErrorException)
            {
                return null;
            }
            catch (DataServiceQueryException)
            {
                return null;
            }
            catch (MissingMethodException e)
            {
                Debug.Write(e.Message);
            }

            ViewBag.LmsParams = Server.UrlEncode(id);
            ViewBag.NextUrl = Server.UrlEncode(nexturl);

            return View(returnResults);
        }

        public async Task<ActionResult> Download(string fileId, string lmsParams, string nexturl)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("SignIn", "Account");

            _factory = new ServiceClientFactory();

            var sharepointClient = await _factory.CreateSharepointServicesClientWithAsync("MyFiles");

            IItemFetcher thisItemFetcher = sharepointClient.Files.GetById(fileId);
            IFileFetcher thisFileFetcher = thisItemFetcher.ToFile();
            var myFile = await thisFileFetcher.ExecuteAsync();
            var file = myFile as Microsoft.Office365.SharePoint.FileServices.File;

            var filename = HttpUtility.UrlEncode(file.Name.Replace(' ','_'));

            var generatedFileName = $"{Server.MapPath("/_Tools/O365_OneDrive_Download")}\\{filename}";

            if (Debugger.IsAttached)
            {
                generatedFileName = $"{Server.MapPath("~/")}Content\\downloads\\{filename}";
            }

            using (Stream stream = await file.DownloadAsync())
            {
                using (Stream saveFile = System.IO.File.Create(generatedFileName))
                {
                    CopyStream(stream, saveFile);
                }

            }

            var redirectUrl = $"/{Server.UrlDecode(lmsParams)}{filename}&nexturl={Server.UrlEncode(nexturl)}";

            return Redirect(redirectUrl);
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

    }
}