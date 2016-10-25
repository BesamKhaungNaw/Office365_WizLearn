using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class Thumbnail
    {
        public System.IO.Stream Content { get; set; }
        public Int32 Height { get; set; }
        public Int32 Width { get; set; }
        public String Url { get; set; }
    }
}