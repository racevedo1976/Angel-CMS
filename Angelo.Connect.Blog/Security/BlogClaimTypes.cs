using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Blog.Security
{
    public class BlogClaimTypes
    {
        public const string BlogCategoryContribute = "blog-category-contribute";


        // NOTE: No "Create" claim since users can't delegate the authority to create content.
        // 		 Using the "UserContentCreate" to manage instead


        // admin level 
        public const string UserBlogsBrowse = "client-user-blogs-browse";
        public const string UserBlogsManage = "client-user-blogs-manage";
        public const string UserBlogsPublish = "client-user-blogs-publish";
       
        // author level 
        public const string PersonalBlogAuthor = "user-blog-author";
        public const string PersonalBlogPublish = "user-blogs-publish";
 

        // delegate permissions
        public const string BlogPostRead = "resource-blog-post-read";
        public const string BlogPostEdit = "resource-blog-post-edit";
    }
}
