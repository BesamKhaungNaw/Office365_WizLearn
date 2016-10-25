using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    /// <summary>
    /// Defines a folder of OneDrive for Business
    /// </summary>
    public class Folder
    {
        public Nullable<Int32> ChildCount { get; set; }
    }
}