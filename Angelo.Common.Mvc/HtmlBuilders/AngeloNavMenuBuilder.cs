using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Encodings.Web;

namespace Angelo.Common.Mvc
{
    public class AngeloNavMenuBuilder<TModel> : IHtmlContent where TModel : class
    {
        protected IHtmlHelper htmlHelper;
        protected string controlId;
        protected TModel viewModel;
        protected IEnumerable<NavMenuNode> nodes;
        protected NavMenuOrientation orientation;
        protected string rootNodeId;


        public AngeloNavMenuBuilder(IHtmlHelper helper, TModel model)
        {
            viewModel = model;
            htmlHelper = helper;
            nodes = new List<NavMenuNode>();
            orientation = NavMenuOrientation.Horizontal;
        }

        public AngeloNavMenuBuilder<TModel> Id(string val)
        {
            controlId = val;
            return this;
        }

        public AngeloNavMenuBuilder<TModel> BindTo(IEnumerable<NavMenuNode> navMenuNodes)
        {
            nodes = navMenuNodes;
            return this;
        }

        public AngeloNavMenuBuilder<TModel> BindTo(string modelPropertyName)
        {
            PropertyInfo pi = viewModel.GetType().GetProperty(modelPropertyName);
            if (pi != null)
            {
                if (typeof(IEnumerable<NavMenuNode>).IsAssignableFrom(pi.PropertyType))
                {
                    nodes = (IEnumerable<NavMenuNode>)pi.GetValue(viewModel, null);
                }
            }
            return this;
        }

        public AngeloNavMenuBuilder<TModel> BindTo(Expression<Func<TModel, object>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;
            if (me == null)
                throw new ArgumentException("Invalid lambda expression defined in Bind method.");
            var modelPropertyName = me.Member.Name;
            return BindTo(modelPropertyName);
        }

        public AngeloNavMenuBuilder<TModel> Orientation(NavMenuOrientation val)
        {
            orientation = val;
            return this;
        }

        public AngeloNavMenuBuilder<TModel> RootNodeId(string nodeId)
        {
            rootNodeId = nodeId;
            return this;
        }

        //  Build the header tag -------------------------------------------------------------------------------
        //
        //  <div class="navbar-header">
        //     <button type = "button" class="navbar-toggle" data-toggle="collapse" data-target="#toggleTargetId">
        //          <span class="sr-only">Toggle navigation</span>
        //          <span class="icon-bar"></span>
        //          <span class="icon-bar"></span>
        //          <span class="icon-bar"></span>
        //      </button>
        //  </div>
        //
        protected TagBuilder BuildHeaderTag(string toggleTargetId)
        {
            var header = new TagBuilder("div");
            header.AddCssClass("navbar-header");

            var button = new TagBuilder("button");
            button.AddCssClass("navbar-toggle");
            button.MergeAttribute("type", "button");
            button.MergeAttribute("data-toggle", "collapse");
            button.MergeAttribute("data-target", "#" + toggleTargetId);

            var spanT = new TagBuilder("span");
            spanT.AddCssClass("sr-only");
            spanT.InnerHtml.Append("Toggle navigation");

            button.InnerHtml.AppendHtml(spanT);
            for(int count1 = 0; count1 < 3; count1++)
            {
                var spanB = new TagBuilder("span");
                spanB.AddCssClass("icon-bar");
                button.InnerHtml.AppendHtml(spanB);
            }

            header.InnerHtml.AppendHtml(button);
            return header;
        }

        //  Build menu link tag -------------------------------------------------------------------------------
        //
        //  <li><a href="{Link}">{Title}</a></li>
        //
        protected TagBuilder BuildMenuLinkTag(NavMenuNode node)
        {
            var li = new TagBuilder("li");
            var a = new TagBuilder("a");
            a.MergeAttribute("href", node.Link);
            a.InnerHtml.Append(node.Title);
            li.InnerHtml.AppendHtml(a);
            return li;
        }

        //  Build level 2 sub-menu tag -------------------------------------------------------------------------------
        //
        //    <li>
        //        <a href = "#" > Dropdown </ a >
        //        <ul style="width:480px;">
        //            { Lever 3+ menu items }
        //        </ul>
        //    </li>
        //
        protected TagBuilder BuildLevel2SubMenuTag(NavMenuNode node)
        {
            var li = new TagBuilder("li");

            var a = new TagBuilder("a");
            a.MergeAttribute("href", "#");
            a.InnerHtml.Append(node.Title);
            li.InnerHtml.AppendHtml(a);

            var ul = new TagBuilder("ul");
            ul.MergeAttribute("style", "width:480px;");
            var list = nodes.Where(x => x.ParentId == node.Id).OrderBy(x => x.Order).ToList();
            foreach (var item in list)
            {
                if (item.HasChildren)
                    ul.InnerHtml.AppendHtml(BuildLevel2SubMenuTag(item));
                else
                    ul.InnerHtml.AppendHtml(BuildMenuLinkTag(item));
            }

            li.InnerHtml.AppendHtml(ul);
            return li;
        }

        //  Build level 1 sub-menu tag -------------------------------------------------------------------------------
        //
        //  <li>
        //     <a href="#" class="dropdown-toggle" data-toggle="dropdown">Title <b class="caret"></b></a>
        //     <ul class="dropdown-menu">
        //       { Level 2 menu items }
        //     </ul>
        //  </li>
        //
        protected TagBuilder BuildLevel1SubMenuTag(NavMenuNode node)
        {
            var li = new TagBuilder("li");

            var a = new TagBuilder("a");
            a.AddCssClass("dropdown-toggle");
            a.MergeAttribute("href", "#");
            a.MergeAttribute("data-toggle", "dropdown");
            a.InnerHtml.Append(node.Title);
            var b = new TagBuilder("b");
            b.AddCssClass("caret");
            a.InnerHtml.AppendHtml(b);
            li.InnerHtml.AppendHtml(a);

            var ul = new TagBuilder("ul");
            ul.AddCssClass("dropdown-menu");
            var list = nodes.Where(x => x.ParentId == node.Id).OrderBy(x => x.Order).ToList();
            foreach (var item in list)
            {
                if (item.HasChildren)
                    ul.InnerHtml.AppendHtml(BuildLevel2SubMenuTag(item));
                else
                    ul.InnerHtml.AppendHtml(BuildMenuLinkTag(item));
            }

            li.InnerHtml.AppendHtml(ul);
            return li;
        }

        //  Build the menu tag -------------------------------------------------------------------------------
        //
        //  <div class="collapse navbar-collapse" id="toggleTargetId">
        //    <ul class="nav navbar-nav"> { or class="nav navbar-nav nva-pills nav-stacked"> }
        //       { Menu Items }
        //    </ul>
        //  </div>
        //
        protected TagBuilder BuildBodyTag(string toggleTargetId)
        {
            var body = new TagBuilder("div");
            body.AddCssClass("collapse");
            body.AddCssClass("navbar-collapse");
            body.MergeAttribute("id", toggleTargetId);

            var ul = new TagBuilder("ul");
            if (orientation == NavMenuOrientation.Vertical)
            {
                ul.AddCssClass("nav-stacked");
                ul.AddCssClass("nav-pills");
                ul.AddCssClass("nav");
            }
            else
            {
                ul.AddCssClass("navbar-nav");
                ul.AddCssClass("nav");
            }

            List<NavMenuNode> items;
            if (string.IsNullOrEmpty(rootNodeId))
                items = nodes.Where(x => string.IsNullOrEmpty(x.ParentId)).OrderBy(x => x.Order).ToList();
            else
                items = nodes.Where(x => x.ParentId == rootNodeId).OrderBy(x => x.Order).ToList();
            foreach(var item in items)
            {
                if (item.HasChildren)
                    ul.InnerHtml.AppendHtml(BuildLevel1SubMenuTag(item));
                else
                    ul.InnerHtml.AppendHtml(BuildMenuLinkTag(item));
            }

            body.InnerHtml.AppendHtml(ul);
            return body;
        }

        //  Build navbar tag -------------------------------------------------------------------------------
        //
        //  <div class="navbar navbar-default" role="navigation">
        //    <div class="container">
        //       { header }
        //       { body }
        //    </div>
        //  </div>
        //
        protected TagBuilder BuildNavBarTag()
        {
            var mainTag = new TagBuilder("div");
            mainTag.AddCssClass("navbar");
            mainTag.AddCssClass("navbar-default");
            mainTag.MergeAttribute("role", "nabigation");
            if (!string.IsNullOrEmpty(controlId))
                mainTag.MergeAttribute("id", controlId);

            var containerTag = new TagBuilder("div");
            containerTag.AddCssClass("container");

            var toggleTargetId = Guid.NewGuid().ToString("N");
            var headerTag = BuildHeaderTag(toggleTargetId);
            var bodyTag = BuildBodyTag(toggleTargetId);

            containerTag.InnerHtml.AppendHtml(headerTag);
            containerTag.InnerHtml.AppendHtml(bodyTag);
            mainTag.InnerHtml.AppendHtml(containerTag);
            return mainTag;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            var navbar = BuildNavBarTag();
            navbar.WriteTo(writer, encoder);
        }
    }





}
