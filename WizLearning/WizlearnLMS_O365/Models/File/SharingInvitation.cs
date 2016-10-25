using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class SharingInvitation
    {
        public String Email { get; set; }
        public String RedeemedBy { get; set; }
        public Boolean SignInRequired { get; set; }
    }
}