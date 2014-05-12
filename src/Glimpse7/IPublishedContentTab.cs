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
using Umbraco.Core.Models;
using Glimpse7.Helper;
using Umbraco.Web;

namespace Glimpse7
{
    class IPublishedContentTab : AspNetTab
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
                    return UmbracoFn.showMethods(typeof(IPublishedContent));
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
                    return UmbracoFn.showMethodsValue(typeof(IPublishedContent), "content");
                }

                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);
                IPublishedContent content = umbracoHelper.TypedContent(NodeId);
                return UmbracoFn.showMethodsValue(content, "content");
            }

            catch (Exception ex)
            {
                plugin.AddRow().Column(ex.ToString());
            }

            return plugin;
        }


        public override string Name
        {
            get { return "IPublishedContent"; }
        }
    }
}