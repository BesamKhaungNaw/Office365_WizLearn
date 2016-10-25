using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class ThumbnailSet : BaseModel
    {
        public Thumbnail Large { get; set; }
        public Thumbnail Medium { get; set; }
        public Thumbnail Small { get; set; }
        public Thumbnail Source { get; set; }
    }
}