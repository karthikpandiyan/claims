using System;
using Microsoft.SharePoint.Client;
using System.Web;
namespace JCI.PageExpiration.CommonUtitilities
{
    public  static class CommonUtilities 
    {
     /// <summary>
     /// Retrieves List Items
     /// </summary>
     /// <param name="listName">list name</param>
     /// <param name="caml">caml query</param>
     /// <param name="clientContext">clientcontext</param>
     /// <returns>List item collections</returns>
        public static ListItemCollection GetListItemCollection( ClientContext clientContext,string listName, CamlQuery caml)
        {
            ListItemCollection items = null;
            if (clientContext != null)
            {
                try
                {

                    Web site = clientContext.Web;
                    List docLib = site.Lists.GetByTitle(listName);
                    clientContext.Load(clientContext.Web, web => web.Title);
                    clientContext.Load(docLib);
                    items = docLib.GetItems(caml);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Trace.TraceError("GetListItemCollection {0}", ex);
                    Console.WriteLine(Trace.GetDetailedError("GetListItemCollection Error", ex));
                }
            }
            return items;
        }


      
        /// <summary>
        /// This extension will try and convert the object to a string. If not, an empty string is returned to ensure the value is not null
        /// </summary>
        /// <param name="o"></param>
        /// <returns>string</returns>
        public static string ToSafeString(this object o)
        {
            try
            {
                if (o == null) // it's null don't bother
                    return String.Empty;

                return o.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

       
    }

    public static class Trace
    {

        public static void TraceInformation(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                System.Diagnostics.Trace.TraceInformation(message);

            }
            catch { }
        }

        public static void TraceWarning(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                System.Diagnostics.Trace.TraceWarning(message);

            }
            catch { }
        }


        public static void TraceError(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                System.Diagnostics.Trace.TraceError(message);
            }
            catch { }
        }

        public static void TraceError(string message, Exception ex)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                    return;

                if (ex == null)
                    return;

                message = GetDetailedError(message, ex);

                TraceError(message);
            }
            catch { }
        }

        public static string GetDetailedError(string message, Exception ex)//, bool multiLine)
        {
            if (ex.InnerException != null)
            {
                return string.Format("{0} {1}{2}{3}{4}{5}{6}",DateTime.Now,  message, ex.Message, System.Environment.NewLine, ex.InnerException.Message, System.Environment.NewLine, ex.InnerException.StackTrace);
            }
            else
            {
                return string.Format("{0} {1}{2}{3}{4}",DateTime.Now, message, ex.Message, System.Environment.NewLine, ex.StackTrace);
            }


        }
    }
}
