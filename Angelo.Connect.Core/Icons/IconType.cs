using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Icons
{
    public class IconType : IEquatable<string>
    {
        // public properties mimic an enumerated type
        public static IconType Error { get { return new IconType("glyphicon glyphicon-exclamation-sign"); } }
        public static IconType Warning {  get { return new IconType("glyphicon glyphicon-warning-sign"); } }
        public static IconType Success { get { return new IconType("glyphicon glyphicon-ok-sign"); } }
        public static IconType Info { get { return new IconType("glyphicon glyphicon-info-sign"); } }
        public static IconType Contact { get { return new IconType("glyphicon glyphicon-phone-alt"); } }
        public static IconType Email { get { return new IconType("glyphicon glyphicon-envelope"); } }
        public static IconType Message { get { return new IconType("glyphicon glyphicon-envelope"); } }
        public static IconType Question { get { return new IconType("glyphicon glyphicon-question-sign"); } }
        public static IconType User { get { return new IconType("glyphicon glyphicon-user"); } }
        public static IconType Login { get { return new IconType("glyphicon glyphicon-log-in"); } }
        public static IconType Logout { get { return new IconType("glyphicon glyphicon-log-out"); } }
        public static IconType List { get { return new IconType("glyphicon glyphicon-list"); } }
        public static IconType List2 { get { return new IconType("fa fa-list"); } }
        public static IconType Check { get { return new IconType("glyphicon glyphicon-check"); } }
        public static IconType Fire { get { return new IconType("glyphicon glyphicon-fire"); } }
        public static IconType Cloud { get { return new IconType("glyphicon glyphicon-cloud"); } }
        public static IconType Globe { get { return new IconType("glyphicon glyphicon-globe"); } }
        public static IconType Bookmark { get { return new IconType("fa fa-bookmark"); } }
        public static IconType Briefcase { get { return new IconType("fa fa-briefcase"); } }
        public static IconType TaskList { get { return new IconType("glyphicon glyphicon-tasks"); } }
        public static IconType Pencil { get { return new IconType("fa fa-pencil"); } }
        public static IconType Add { get { return new IconType("glyphicon glyphicon-plus"); } }
        public static IconType Edit { get { return new IconType("glyphicon glyphicon-edit"); } }
        public static IconType Remove { get { return new IconType("glyphicon glyphicon-remove"); } }
        public static IconType Save { get { return new IconType("fa fa-floppy-o"); } }
        public static IconType CaretRight { get { return new IconType("glyphicon glyphicon-triangle-right"); } }
        public static IconType CaretDown { get { return new IconType("glyphicon glyphicon-triangle-bottom"); } }
        public static IconType CaretUp { get { return new IconType("glyphicon glyphicon-triangle-top"); } }
        public static IconType ChevronRight { get { return new IconType("glyphicon glyphicon-chevron-right"); } }
        public static IconType ChevronDown { get { return new IconType("glyphicon glyphicon-chevron-down"); } }
        public static IconType ChevronUp { get { return new IconType("glyphicon glyphicon-chevron-up"); } }
        public static IconType File { get { return new IconType("glyphicon glyphicon-file"); } }
        public static IconType Attachment { get { return new IconType("glyphicon glyphicon-paperclip"); } }
        public static IconType Download { get { return new IconType("glyphicon glyphicon-download-alt"); } }
        public static IconType Search { get { return new IconType("glyphicon glyphicon-search"); } }
        public static IconType Reports { get { return new IconType("glyphicon glyphicon-scale"); } }
        public static IconType Usage { get { return new IconType("glyphicon glyphicon-stats"); } }
        public static IconType Console { get { return new IconType("glyphicon glyphicon-console"); } }
        public static IconType Export { get { return new IconType("glyphicon glyphicon-export"); } }
        public static IconType SortAsc { get { return new IconType("glyphicon glyphicon-sort-by-alphabet"); } }
        public static IconType SortDesc { get { return new IconType("glyphicon glyphicon-sort-by-alphabet-alt"); } }
        public static IconType Dashboard { get { return new IconType("glyphicon glyphicon-dashboard"); } }
        public static IconType Calendar { get { return new IconType("icon-calendar"); } }
        public static IconType Slideshow { get { return new IconType("icon-Slideshow"); } }
        public static IconType Product { get { return new IconType("glyphicon glyphicon-gift"); } }
        public static IconType Module { get { return new IconType("fa fa-cubes"); } }
        public static IconType Tags { get { return new IconType("glyphicon glyphicon-tags"); } }
        public static IconType Trashcan { get { return new IconType("fa fa-trash"); } }
        public static IconType Home { get { return new IconType("fa fa-home"); } }
        public static IconType HomeAdmin { get { return new IconType("fa fa-institution"); } }
        public static IconType Network { get { return new IconType("fa fa-plug"); } }
        public static IconType Settings { get { return new IconType("fa fa-cog"); } }
        public static IconType Wrench { get { return new IconType("fa fa-wrench"); } }
        public static IconType HealthCheck { get { return new IconType("fa fa-medkit"); } }
        public static IconType Sites { get { return new IconType("fa fa-sitemap"); } }
        public static IconType Code { get { return new IconType("fa fa-code"); } }
        public static IconType Ldap { get { return new IconType("fa fa-users"); } }
        public static IconType UserGroup { get { return new IconType("fa fa-users"); } }
        public static IconType Shield { get { return new IconType("fa fa-shield"); } }
        public static IconType Collections { get { return new IconType("fa fa-cubes"); } }
        public static IconType Roles { get { return new IconType("fa fa-id-badge"); } }
        public static IconType Theme { get { return new IconType("fa fa-newspaper-o"); } }
        public static IconType Folder { get { return new IconType("fa fa-folder"); } }
        public static IconType VideoCamera { get { return new IconType("fa fa-video-camera"); } }
        public static IconType Design { get { return new IconType("fa fa-desktop"); } }
        public static IconType SiteMap { get { return new IconType("fa fa-sitemap"); } }
        public static IconType Crop { get { return new IconType("fa fa-crop"); } }
        public static IconType Alert { get { return new IconType("fa fa-bell"); } }
        public static IconType Announcement { get { return new IconType("fa fa-bullhorn"); } }
        public static IconType Windows { get { return new IconType("fa fa-windows"); } }
        public static IconType GooglePlus { get { return new IconType("fa fa-google-plus"); } }
        public static IconType Column1 { get { return new IconType("icon-column"); } }
        public static IconType Column2 { get { return new IconType("icon-column1"); } }
        public static IconType Column3 { get { return new IconType("icon-column2"); } }
        public static IconType Column4 { get { return new IconType("icon-column3"); } }
        public static IconType Column5 { get { return new IconType("icon-column4"); } }
        public static IconType Column6 { get { return new IconType("icon-column5"); } }
        public static IconType Column80Pct { get { return new IconType("icon-80column"); } }
        public static IconType Column75Pct { get { return new IconType("icon-75column"); } }
        public static IconType Column66Pct { get { return new IconType("icon-66column"); } }
        public static IconType Column33Pct { get { return new IconType("icon-33column"); } }
        public static IconType Column25Pct { get { return new IconType("icon-25column"); } }
        public static IconType Open { get { return new IconType("fa fa-external-link"); } }

        // instances should behave like a string
        public static IconType FromString(string type)
        {
            return new IconType(type);
        }

        public override string ToString()
        {
            //if (!string.IsNullOrEmpty(_iconType) && !_iconType.StartsWith("fa "))
            //    return "glyphicon glyphicon-" + _iconType;

            return _iconType;
        }

        public bool Equals(string other)
        {
            return this.ToString() == other;
        }
        
        // privately manage class instances
        private string _iconType;

        private IconType(string type)
        {
            _iconType = type;
        }

    }
}
