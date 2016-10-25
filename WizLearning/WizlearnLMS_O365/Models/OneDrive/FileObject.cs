using Microsoft.Office365.SharePoint.FileServices;
using System.ComponentModel.DataAnnotations;

namespace WizlearnLMS_O365.Models.OneDrive
{
    public class FileObject
    {
        public string Name;
        public string DisplayName;
        public string ID;
        public string WebUrl;

        [DataType(DataType.MultilineText)]
        public string FileText { get; set; }

        [DataType(DataType.MultilineText)]
        public string UpdatedText { get; set; }


        public FileObject(IItem fileItem)
        {

            ID = fileItem.Id;

            Name = fileItem.Name;

            WebUrl = fileItem.WebUrl;

            DisplayName = (fileItem is Folder) ? "Folder" : "File";

        }
    }
}