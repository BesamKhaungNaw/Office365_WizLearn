using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class Photo
    {
        public String CameraMake;
        public String CameraModel;
        public Double ExposureDenominator;
        public Double ExposureNumerator;
        public Double FocalLength;
        public Double FNumber;
        public DateTimeOffset TakenDateTime;
        public Int32 ISO;   
    }
}