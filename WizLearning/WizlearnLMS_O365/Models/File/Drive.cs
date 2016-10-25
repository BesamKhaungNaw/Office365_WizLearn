using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;

namespace WizlearnLMS_O365.Models.File
{
    /// <summary>
    /// Defines a drive of OneDrive for Business
    /// </summary>
    public class Drive : BaseModel
    {
        /// <summary>
        /// The type of the current Drive
        /// </summary>
        public String DriveType { get; set; }

        /// <summary>
        /// The drive's owner
        /// </summary>
        public IdentitySet Owner { get; set; }

        /// <summary>
        /// The storage quota of the drive
        /// </summary>
        public Quota Quota { get; set; }
    }
}