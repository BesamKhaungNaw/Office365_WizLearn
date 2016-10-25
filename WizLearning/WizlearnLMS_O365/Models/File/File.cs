using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    /// <summary>
    /// Defines a file in OneDrive for Business
    /// </summary>
    public class File
    {
        public Hashes Hashes { get; set; }
        public String MimeType { get; set; }
    }
}