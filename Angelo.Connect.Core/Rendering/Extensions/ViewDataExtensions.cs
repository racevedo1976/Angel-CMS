using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Angelo.Connect.Rendering
{
    public static class ViewDataExtensions
    {
        public static void SetRenderingContext(this ViewDataDictionary viewData, RenderingContext renderingContext)
        {
            viewData["RenderingContext"] = renderingContext;
        }

        public static RenderingContext GetRenderingContext(this ViewDataDictionary viewData)
        {
            var renderingContext = viewData["RenderingContext"];

            if (renderingContext != null)
                return (RenderingContext)renderingContext;

            return null;
        }

        public static void SetTreeContext(this ViewDataDictionary viewData, TreeContext treeContext)
        {
            viewData["ContentTreeId"] = treeContext.TreeId;
            viewData["ContentNodeId"] = treeContext.NodeId;
            viewData["ContentEditable"] = treeContext.Editable;
            viewData["ContentContainers"] = treeContext.AllowContainers;
            viewData["ContentZone"] = treeContext.Zone;
        }

        public static void UpdateTreeContext(this ViewDataDictionary viewData, string nodeId)
        {
            viewData["ContentNodeId"] = nodeId;
        }

        public static TreeContext GetTreeContext(this ViewDataDictionary viewData)
        {
            TreeContext treeContext = null;

            var editable = viewData["ContentEditable"];
            var treeId = viewData["ContentTreeId"];
            var nodeId = viewData["ContentNodeId"];
            var containers = viewData["ContentContainers"];
            var zone = viewData["ContentZone"];

            if(treeId != null)
            {
                treeContext = new TreeContext { TreeId = treeId.ToString() };

                if (nodeId != null)
                    treeContext.NodeId = nodeId.ToString();

                if (editable != null)
                    treeContext.Editable = (bool)editable;

                if (containers != null)
                    treeContext.AllowContainers = (bool)containers;

                if (zone != null)
                    treeContext.Zone = (ZoneContext)zone;
            }
                     
            return treeContext;
        }

        public static string GetReturnUrl(this ViewDataDictionary viewData)
        {
            if (viewData["ReturnUrl"] != null)
                return (string)viewData["ReturnUrl"];

            return "";
        }
    }
}
