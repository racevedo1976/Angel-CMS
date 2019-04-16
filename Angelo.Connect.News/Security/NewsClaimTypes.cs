namespace Angelo.Connect.News.Security
{
    public class NewsClaimTypes
    {
        public const string NewsCategoryContribute = "News-category-contribute";


        // NOTE: No "Create" claim since users can't delegate the authority to create content.
        // 		 Using the "UserContentCreate" to manage instead


        // admin level 
        public const string UserNewsBrowse = "client-user-news-browse";
        public const string UserNewsManage = "client-user-news-manage";
        public const string UserNewsPublish = "client-user-news-publish";
       
        // author level 
        public const string PersonalNewsAuthor = "user-news-author";
        public const string PersonalNewsPublish = "user-news-publish";
 

        // delegate permissions
        public const string NewsPostRead = "resource-news-post-read";
        public const string NewsPostEdit = "resource-news-post-edit";
    }
}
