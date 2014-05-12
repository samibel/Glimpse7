using Glimpse.Core.Tab.Assist;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using umbraco.MacroEngines;
using Umbraco.Web.Models;

namespace Glimpse7.Helper
{
    class UmbracoFn
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>TabSection</returns>
        public static TabSection showMethods(Type obj)
        {
            var plugin = Plugin.Create("Function", "Param", "Type");
            foreach (var method in obj.GetMethods().OrderBy(x => x.Name))
            {
                var parameters = method.GetParameters();
                var parameterDescriptions = string.Join
                    (", ", method.GetParameters()
                                 .Select(x => x.ParameterType + " " + x.Name)
                                 .ToArray());

                plugin.AddRow().Column(method.Name.Replace("get_", "")).Column(parameterDescriptions).Column(method.ReturnType.ToString());
                //plugin.AddRow().Column(method.Name).Column(parameterDescriptions).Column(method.ReturnType.ToString());

            }
            return plugin;




        }


        public static List<FnDetails> showMethodsList(Type obj)
        {
            List<FnDetails> methodsList = new List<FnDetails>();
            var plugin = Plugin.Create("Function", "Param", "Type");
            foreach (var method in obj.GetMethods().OrderBy(x => x.Name))
            {
                var fnDetails = new FnDetails();

                var parameterDescriptions = string.Join
                    ("   ,  ", method.GetParameters()
                                 .Select(x => x.ParameterType + " " + x.Name)
                                 .ToArray());
                fnDetails.FunctionName = method.Name;
                fnDetails.param = parameterDescriptions;
                fnDetails.TypeName = method.ReturnType.ToString();

                methodsList.Add(fnDetails);
            }
            return methodsList;
        }

        /// <summary>
        /// Get All Object Methods
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TabSection showMethodsValue(object obj, string type)
        {
            var plugin = Plugin.Create("Property", "Value", "Type");
            try
            {
                string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj.GetType());

                foreach (PropertyDescriptor property in properties.Sort())
                {
                    try
                    {
                        if (property.GetValue(obj).GetType().Name.ToLower().EndsWith("list") || property.Name == "Children" || property.Name == "ContentSet"
                               || property.Name == "Parent"
                                )
                        {

                            var value = getComplexTypeTabSection(property, obj, type);
                            plugin.AddRow().Column(property.Name).Column(value).Column(property.GetValue(obj).GetType().Name).Emphasis();

                        }
                        else
                        {

                            plugin.AddRow().Column(property.Name).Column(property.GetValue(obj)).Column(property.GetValue(obj).GetType().Name).Emphasis();
                        }

                    }
                    catch (Exception ex)
                    {
                        // plugin.AddRow().Column(property.Name).Column("**").Column(property.GetValue(node).GetType().Name).Warn();
                    }
                }
            }
            catch (Exception ex)
            {

            }


            return plugin;


        }


        /// <summary>
        /// getComplexTypeTabSection
        /// </summary>
        /// <param name="property"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>

        private static object getComplexTypeTabSection(PropertyDescriptor property, object obj, string type)
        {
            if (property.GetValue(obj).GetType().Name.IndexOf("OrderedEnumerable") > -1)
            {
                IEnumerable<object> myEnumerable = (IEnumerable<object>)property.GetValue(obj);


                switch (myEnumerable.GetType().GetGenericArguments()[0].ToString())
                {
                    case "Umbraco.Core.Models.IPublishedContent":
                        return getIPublishedContentIEnumerable(myEnumerable, type);
                    default:
                        return myEnumerable.GetType().GetGenericArguments()[0].ToString();

                }
            }

            switch (property.GetValue(obj).GetType().Name)
            {
                case "DynamicPublishedContentList":

                    return getDynamicPublishedContentList(property.GetValue(obj), type);
                case "DynamicPublishedContent":
                    return getDynamicPublishedContent(property.GetValue(obj), type);
                case "DynamicNodeList":

                    return getDynamicNodeList(property.GetValue(obj), type);
                case "DynamicNode":

                    return getDynamicNode(property.GetValue(obj), type);
                case "IPublishedContent[]":

                    return getIPublishedContentArray(property.GetValue(obj), type);

                default:
                    return property.GetValue(obj).GetType().Name.IndexOf("OrderedEnumerable") + "+++" + property.GetValue(obj).GetType().Name;

            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getIPublishedContentIEnumerable(object collection, string type)
        {
            var tabSection = new TabSection("Id", "Name", "NodeTypeAlias", "");
            bool isEmpty = true;
            foreach (var item in ((IEnumerable<Umbraco.Core.Models.IPublishedContent>)collection))
            {

                isEmpty = false;
                string url = "";
                if (type == "media")
                {
                    url = string.Format("<a href='{0}' >Details</a>", getMediaUrl(item.Id.ToString()));
                }
                else
                {
                    url = string.Format("<a href='{0}' >Details</a>", getContentUrl(item.Id.ToString()));
                }

                tabSection.AddRow()
                       .Column(item.Id.ToString())
                       .Column(item.Name)
                       .Column(item.DocumentTypeAlias)
                       .Column(url).Raw();


            }
            if (isEmpty == true)
            {
                return "** Empty **";
            }
            return tabSection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getIPublishedContentArray(object collection, string type)
        {
            var tabSection = new TabSection("Id", "Name", "NodeTypeAlias", "");
            bool isEmpty = true;
            foreach (var item in ((Umbraco.Core.Models.IPublishedContent[])collection))
            {

                isEmpty = false;
                string url = "";
                if (type == "media")
                {
                    url = string.Format("<a href='{0}' >Details</a>", getMediaUrl(item.Id.ToString()));
                }
                else
                {
                    url = string.Format("<a href='{0}' >Details</a>", getContentUrl(item.Id.ToString()));
                }

                tabSection.AddRow()
                       .Column(item.Id.ToString())
                       .Column(item.Name)
                       .Column(item.DocumentTypeAlias)
                       .Column(url).Raw();


            }
            if (isEmpty == true)
            {
                return "** Empty **";
            }
            return tabSection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getDynamicNodeList(object collection, string type)
        {
            var tabSection = new TabSection("Id", "Name", "NodeTypeAlias", "");
            bool isEmpty = true;
            foreach (var item in (DynamicNodeList)collection)
            {
                isEmpty = false;

                string url = "";
                if (type == "media")
                {
                    url = string.Format("<a href='{0}' >Details</a>", getMediaUrl(item.Id.ToString()));
                }
                else
                {
                    url = string.Format("<a href='{0}' >Details</a>", getContentUrl(item.Id.ToString()));
                }
                tabSection.AddRow()
                       .Column(item.Id.ToString())
                       .Column(item.Name)
                       .Column(item.NodeTypeAlias).
                        Column(url).Raw();


            }
            if (isEmpty == true) return "** Empty **";
            return tabSection;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getDynamicNode(object obj, string type)
        {

            var tabSection = new TabSection("Id", "Name", "NodeTypeAlias", "");
            if (obj == null)
            {
                return "** Empty **";
            }
            else
            {
                var item = (DynamicNode)obj;
                string url = "";
                if (type == "media")
                {
                    url = string.Format("<a href='{0}' >Details</a>", getMediaUrl(item.Id.ToString()));
                }
                else
                {
                    url = string.Format("<a href='{0}' >Details</a>", getContentUrl(item.Id.ToString()));
                }
                tabSection.AddRow()
                       .Column(item.Id.ToString())
                       .Column(item.Name)
                       .Column(item.NodeTypeAlias).
                        Column(url).Raw();

                return tabSection;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getDynamicPublishedContentList(object collection, string type)
        {

            var tabSection = new TabSection("Id", "Name", "DocumentTypeAlias", "");
            bool isEmpty = true;
            foreach (var item in (DynamicPublishedContentList)collection)
            {
                isEmpty = false;
                string url = "";
                if (type == "media")
                {
                    url = string.Format("<a href='{0}' >Details</a>", getMediaUrl(item.Id.ToString()));
                }
                else
                {
                    url = string.Format("<a href='{0}' >Details</a>", getContentUrl(item.Id.ToString()));
                }
                tabSection.AddRow()
                       .Column(item.Id.ToString())
                       .Column(item.Name)
                       .Column(item.DocumentTypeAlias).
                        Column(url).Raw();

            }
            if (isEmpty == true) return "** Empty **";
            return tabSection;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object getDynamicPublishedContent(object obj, string type)
        {

            var tabSection = new TabSection("Id", "Name", "NodeTypeAlias", "");
            if (obj == null)
            {
                return "** Empty **";
            }
            else
            {
                var item = (DynamicPublishedContent)obj;
                string url = "";
                if (type == "media")
                {
                    url = string.Format("<a href='{0}' >Details</a>", getMediaUrl(item.Id.ToString()));
                }
                else
                {
                    url = string.Format("<a href='{0}' >Details</a>", getContentUrl(item.Id.ToString()));
                }
                tabSection.AddRow()
                       .Column(item.Id.ToString())
                       .Column(item.Name)
                       .Column(item.DocumentTypeAlias)
                       .Column(url).Raw();

                return tabSection;
            }
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string getContentUrl(string id)
        {
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string query = "glimpse7GetContentById=" + id;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["glimpse7GetContentById"]))
            {
                url = url.Replace("glimpse7GetContentById=" + System.Web.HttpContext.Current.Request.QueryString["glimpse7GetContentById"], query);
                return url;
            }
            else
            {
                if (System.Web.HttpContext.Current.Request.QueryString.Count > 0)
                {
                    url += "&" + query;
                }
                else
                {
                    url += "?" + query;
                }
            }
            return url;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string getMediaUrl(string id)
        {
            string url = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            string query = "glimpse7GetMediaById=" + id;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.QueryString["glimpse7GetMediaById"]))
            {

                url = url.Replace("glimpse7GetMediaById=" + System.Web.HttpContext.Current.Request.QueryString["glimpse7GetMediaById"], query);
            }
            else
            {
                if (System.Web.HttpContext.Current.Request.QueryString.Count > 0)
                {
                    url += "&" + query;
                }
                else
                {
                    url += "?" + query;
                }
            }
            return url;
        }


    }
}


/// <summary>
/// 
/// </summary>
class FnDetails
{
    public string FunctionName { get; set; }
    public string param { get; set; }
    public string TypeName { get; set; }
}