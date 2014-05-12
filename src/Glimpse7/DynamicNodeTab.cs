using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glimpse.AspNet.Extensibility;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using System.Dynamic;
using System.ComponentModel;
using System.Xml.Linq;
using umbraco.MacroEngines;
using Glimpse7.Helper;

namespace Glimpse7
{
    class DynamicNodeTab : AspNetTab
    {
        public override object GetData(ITabContext context)
        {
            var plugin = Plugin.Create("Function", "Param", "Type");
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                int NodeId = 0;
                if (System.Web.HttpContext.Current.Request["glimpse7GetCheatSheet"] == "true")
                {
                    return UmbracoFn.showMethods(typeof(umbraco.MacroEngines.DynamicNode));
                }
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["glimpse7GetContentById"]))
                {
                    Int32.TryParse(System.Web.HttpContext.Current.Request["glimpse7GetContentById"], out NodeId);
                }
                else if (umbraco.presentation.UmbracoContext.Current.PageId != null)
                {
                    NodeId = umbraco.presentation.UmbracoContext.Current.PageId.Value;
                }
                else
                {
                    return UmbracoFn.showMethodsValue(typeof(umbraco.MacroEngines.DynamicNode), "content");
                }

                dynamic node = new umbraco.MacroEngines.DynamicNode(NodeId);
                return UmbracoFn.showMethodsValue(node, "content");
            }

            catch (Exception ex)
            {
                plugin.AddRow().Column(umbraco.presentation.UmbracoContext.Current.PageId);
            }

            return plugin;
        }

        public override string Name
        {
            get { return "DynamicNode"; }
        }


    }
}