using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class IdentitySet
    {
        public Identity Application { get; set; }
        public Identity Device { get; set; }
        public Identity User { get; set; }
    }
}