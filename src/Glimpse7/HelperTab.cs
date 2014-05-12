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
using System.Reflection;

namespace Glimpse7
{
    class HelperTab : AspNetTab
    {
        public override object GetData(ITabContext context)
        {
            var plugin = Plugin.Create("#", "Function", "Param");
            List<Type[]> typeList = new List<Type[]>();
            try
            {

                var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            from type in assembly.GetTypes()
                            where type.IsSubclassOf(typeof(System.Web.WebPages.HelperPage))
                            select type;
                foreach (var type in types)
                {
                    if (type.Name != "WebGridRenderer")
                        foreach (var item in UmbracoFn.showMethodsList(type).Where(t => t.TypeName == "System.Web.WebPages.HelperResult"))
                        {
                            plugin.AddRow().Column("@" + type.Name).Column(item.FunctionName).Column(item.param);
                        }
                }
                return plugin;

            }

            catch (Exception ex)
            {
                plugin.AddRow().Column(ex.ToString());
            }

            return plugin;
        }

        public override string Name
        {
            get { return "Helper"; }
        }

        private Type[] getTypesArray(Type obj)
        {
            return Assembly.GetAssembly(obj).GetTypes();
        }

        /*
        public override RuntimeEvent ExecuteOn
        {
            get { return RuntimeEvent.EndRequest; }
        }*/



    }
}