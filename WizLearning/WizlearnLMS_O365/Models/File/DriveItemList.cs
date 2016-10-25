using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;


namespace WizlearnLMS_O365.Models.File
{
    /// <summary>
    /// Defines a list of DriveItem objects
    /// </summary>
    public class DriveItemList
    {
        /// <summary>
        /// The list of contacts
        /// </summary>
        [JsonProperty("value")]
        public List<DriveItem> DriveItems { get; set; }
        public String parentDriveId { get; set; }
    }
}