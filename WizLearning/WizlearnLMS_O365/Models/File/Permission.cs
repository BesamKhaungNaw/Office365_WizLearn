using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WizlearnLMS_O365.Models.File;
namespace WizlearnLMS_O365.Models.File
{
    public class Permission: BaseModel
    {
        public IdentitySet GrantedTo;
        public SharingInvitation Invitation;
        public ItemReference InheritedFrom;
        public SharingLink Link;
        public List<String> Roles;
        public String ShareId;
    }
}