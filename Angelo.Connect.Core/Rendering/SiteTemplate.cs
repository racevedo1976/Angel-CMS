using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    [NotMapped]
    public class SiteTemplate
    {
        public const string CONFIG_FILE_NAME = "config.json";
        public const string DATA_FOLDER_NAME = "data";
        public const string THEMES_FOLDER_NAME = "themes";
        public const string THEMES_DEFAULT_NAME = "default";
        public const string THEME_CSS_FILE_NAME = "theme.css";
        public const string TEMPLATE_CSS_FILE_NAME = "common.css";

        public SiteTemplate()
        { 
        }

        public string Schema { get; set; }

        public string Id { get; set; }

        public string Title { get; set; }
       
        public string Description { get; set; }

        public IList<PageTemplate> PageTemplates { get; set; }

        public IList<SiteTemplateMasterPage> MasterPages { get; set; }

        public IList<SiteTemplatePage> Pages { get; set; }

        public IList<SiteTemplateNav> NavMenus { get; set; }

        public IList<SiteTemplateTheme> Themes { get; set; }

        public IEnumerable<string> Scripts { get; set; }

        public string TemplateFolder
        {
            get {
                return "wwwroot\\templates\\" + Id.ToLower();
            }
        }

        public string DataFolder {
            get {
                return TemplateFolder + "\\" + DATA_FOLDER_NAME;
            }
        }

        public string ThemesFolder
        {
            get {
                return TemplateFolder + "\\" + THEMES_FOLDER_NAME;
            }
        }

        public string Stylesheet
        {
            get { return "/templates/" + Id.ToLower() + "/" + TEMPLATE_CSS_FILE_NAME; }
        }

        public string PreviewImage
        {
            get {
                return $"/templates/{Id}/{THEMES_FOLDER_NAME}/{THEMES_DEFAULT_NAME}/preview.png";
            }
        }
    }

    public class SiteTemplateTheme
    {
        public string Id { get; set; }

        public string SiteTemplateId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> CoreStyles { get; set; }
       
        public bool IsDefault
        {
            get {
                return Id.ToLower() == "default";
            }
        }

        public string PreviewImage
        {
            get {
                return $"/templates/{SiteTemplateId}/themes/{Id}/preview.png";
            }
        }

        public string Stylesheet
        {
            get {
                return $"/templates/{SiteTemplateId}/{SiteTemplate.THEMES_FOLDER_NAME}/{Id}/{SiteTemplate.THEME_CSS_FILE_NAME}";
            }
        }
    }
}
