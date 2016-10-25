using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class SharingLink
    {
        public Identity Application { get; set; }
        public SharingLinkType Type { get; set; }
        public String WebUrl { get; set; }
    }
}