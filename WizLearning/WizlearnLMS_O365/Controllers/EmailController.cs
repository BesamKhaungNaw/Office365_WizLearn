using WizlearnLMS_O365.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.IdentityModel.Tokens;
using WizlearnLMS_O365.Models.OneDrive;
using Microsoft.Office365.SharePoint.FileServices;
using Microsoft.OData.Core;
using Microsoft.OData.Client;
using System.Diagnostics;

namespace WizlearnLMS_O365.Controllers
{

    [Authorize]
    [HandleError(ExceptionType = typeof(Office365AssertFailedException))]
    public class EmailController : Controller
    {
        private ServiceClientFactory _factory;

        public EmailController()
        {
            _factory = new ServiceClientFactory();
        }

        public async Task<ActionResult> File()
        {
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

            return View(returnResults);

    }

        public async Task<ActionResult> Index()
        {
            return RedirectToAction("File");

            var outlookServicesClient = await _factory.CreateOutlookServicesClientWithAsync("Mail");
            //var great = await LookupOneDriveUrl();

            var accessToken = await _factory.GetAccessToken();

            var jwt = new JwtSecurityToken(accessToken);

            var endpointUri = _factory.ServiceEndpointUri;


            Assert.ThrowExceptionIfNull(outlookServicesClient, "Failed to create outlook service client, please ensure you have capbility to access mails");

            var result = await (from i in outlookServicesClient.Me.Folders.GetById("Inbox").Messages
                                     orderby i.DateTimeReceived descending
                                     select i).Take(10).ExecuteAsync();

            var mailItems = new List<MailItem>();

            foreach(var mail in result.CurrentPage)
            {
                mailItems.Add(new MailItem(mail));
            }

            return View(mailItems);
        }
    }
}