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
    class DynamicMediaTab : AspNetTab
    {
        public override object GetData(ITabContext context)
        {

            var plugin = Plugin.Create("Function", "Param", "Type");
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                int NodeId = -1;

                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request["glimpse7GetMediaById"]))
                {
                    Int32.TryParse(System.Web.HttpContext.Current.Request["glimpse7GetMediaById"], out NodeId);
                }
                if (NodeId > 0)
                {
                    dynamic mediaItem = new DynamicMedia(NodeId);
                    return UmbracoFn.showMethodsValue(mediaItem, "media");
                }
                else
                {
                    return UmbracoFn.showMethods(typeof(DynamicMedia));
                }
            }

            catch (Exception ex)
            {
                plugin.AddRow().Column(ex.ToString());
            }

            return plugin;
        }

        public override string Name
        {
            get { return "DynamicMedia"; }
        }



    }
}