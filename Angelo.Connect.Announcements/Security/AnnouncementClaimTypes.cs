using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Announcement.Security
{
    public class AnnouncementClaimTypes
    {
        public const string AnnouncementCategoryContribute = "announcement-category-contribute";


        // NOTE: No "Create" claim since users can't delegate the authority to create content.
        // 		 Using the "UserContentCreate" to manage instead


        // admin level 
        public const string UserAnnouncementsBrowse = "client-user-announcements-browse";
        public const string UserAnnouncementsManage = "client-user-announcements-manage";
        public const string UserAnnouncementsPublish = "client-user-announcements-publish";
       
        // author level 
        public const string PersonalAnnouncementAuthor = "user-announcement-author";
        public const string PersonalAnnouncementPublish = "user-announcements-publish";
 

        // delegate permissions
        public const string AnnouncementPostRead = "resource-announcement-post-read";
        public const string AnnouncementPostEdit = "resource-announcement-post-edit";
    }
}
