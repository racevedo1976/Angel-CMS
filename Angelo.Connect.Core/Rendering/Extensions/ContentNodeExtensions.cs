using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Rendering
{
    public static class ContentNodeExtensions
    {

        // Content Nodes (fully stylable, support widths and other layout styles)
        public static string BuildNodeCssStyle(this ContentNode node, TreeContext treeContext)
        {
            var style = node.GetStyle();
            var css = new StringBuilder();

            if (treeContext.Zone.AllowPadding)
            {
                if (!string.IsNullOrEmpty(style.PaddingTop))
                    css.Append($"padding-top: {style.PaddingTop}; ");

                if (!string.IsNullOrEmpty(style.PaddingBottom))
                    css.Append($"padding-bottom: {style.PaddingBottom}; ");
            }

          

          
            // DEPRECATED: Still supporting column width for now
            if (node.CssColumnSize != null && node.CssColumnSize != "12")
            {
                var pct = Math.Round(12m / Decimal.Parse(node.CssColumnSize));

                css.Append("display: inline-block; ");
                css.Append($" width: {pct}%; ");
            }

            return css.ToString().Trim();
        }

        public static string BuildNodeCssClasses(this ContentNode node, TreeContext treeContext)
        {
            var css = new StringBuilder();
            var style = node.GetStyle();

            css.Append("content-node ");

            if (treeContext.Editable)
                css.Append("editable ");

            // DEPRECATED: Still supporting column width for now
            if (node.CssColumnSize != null && node.CssColumnSize != "12")
                css.Append("inline-block ");

            // add background class
            if (!string.IsNullOrEmpty(style.BackgroundClass))
                css.Append(style.BackgroundClass + " ");

            // add other classes
            if (!string.IsNullOrEmpty(style.NodeClasses))
                css.Append(style.NodeClasses + " ");


            return css.ToString().Trim();
        }

        public static string BuildWidgetCssClasses(this ContentNode node, TreeContext treeContext)
        {
            var css = new StringBuilder();
            var style = node.GetStyle();

            css.Append("content-widget ");

            if(treeContext.Editable)
                css.Append("editable ");

            // only root nodes should have width contained
            if (node.ParentId == null && style.FullWidth == false && treeContext.Zone.AllowContainers)
            {
                css.Append("container ");
            }

            return css.ToString().Trim();
        }

        public static string BuildWidgetCssStyle(this ContentNode node, TreeContext treeContext)
        {
            var css = new StringBuilder();
            var style = node.GetStyle();

            if (!string.IsNullOrEmpty(style.MaxHeight))
            {
                css.Append($"max-height: {style.MaxHeight}; ");
                css.Append($"overflow-x: hidden; ");
                css.Append($"overflow-x: scroll; ");
            }


            if (!string.IsNullOrEmpty(style.Alignment))
            {
                if (style.Alignment == "left")
                {
                    css.Append($"position: relative; ");
                    css.Append($"display: inline-block; ");
                    css.Append($"right: 0%; ");
                    css.Append($"transform: translate(0%); ");
                }
                else if (style.Alignment == "right")
                {
                    css.Append($"position: relative; ");
                    css.Append($"display: inline-block; ");
                    css.Append($"right: -100%; ");
                    css.Append($"transform: translate(-100%); ");
                }
                else if (style.Alignment == "center")
                {
                    css.Append($"position: relative; ");
                    css.Append($"display: inline-block; ");
                    css.Append($"right: -50%; ");
                    css.Append($"transform: translate(-50%); ");
                }
            }

            return css.ToString().Trim();
        }


        // Embedded Nodes (subset of styles, layout controlled by template)

        public static string BuildEmbeddedNodeCssClasses(this ContentNode node, TreeContext treeContext)
        {
            var css = new StringBuilder();
            var style = node.GetStyle();

            css.Append("content-node ");

            if (treeContext.Editable)
            {
                css.Append("content-node-embedded ");
                css.Append("editable ");
            }

            // add background class
            if (!string.IsNullOrEmpty(style.BackgroundClass))
                css.Append(style.BackgroundClass + " ");

            // add other classes
            if(!string.IsNullOrEmpty(style.NodeClasses))
                css.Append(style.NodeClasses + " ");

            return css.ToString().Trim();
        }

        public static string BuildEmbeddedWidgetCssClasses(this ContentNode node, TreeContext treeContext)
        {
            var css = new StringBuilder();

            css.Append("content-widget ");

            if (treeContext.Editable)
            {
                css.Append("content-widget-embedded ");
                css.Append("editable ");
            }

            return css.ToString().Trim();
        }   

    }
}
