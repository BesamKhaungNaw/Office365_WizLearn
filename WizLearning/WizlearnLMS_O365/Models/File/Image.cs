using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    /// <summary>
    /// Defines an image file in OneDrive for Business
    /// </summary>
    public class Image
    {
        public Int32 Height { get; set; }
        public Int32 Width { get; set; }
    }
}