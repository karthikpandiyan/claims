using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using JCI.PageExpiration.WebJob.Security;
namespace JCI.PageExpiration.WebJob.Security
{
    public class SecurityContext
    {
        /// <summary>
        /// The key to get the security token out of the current context item collection
        /// </summary>
        internal const string CONTEXTITEM_KEY_SECURITYTOKEN = "__ciSecurityToken";

        /// <summary>
        /// The key to get the user out of the current context item collection
        /// </summary>
        internal const string CONTEXTITEM_KEY_USER = "__ciUser";

        /// <summary>
        /// The key to get the user groups out of the current context item collection
        /// </summary>
        internal const string CONTEXTITEM_KEY_GROUPS = "__ciGroups";



        /// <summary>
        /// Gets the security token associated to the current request or null.
        /// </summary>
        public static SPSecurityToken CurrentSecurityToken
        {
            get
            {
                try
                {
                    var context = HttpContext.Current;
                    if (context == null)
                        return null;

                    if (context.Items.Contains(CONTEXTITEM_KEY_SECURITYTOKEN))
                    {
                        return (SPSecurityToken)context.Items[CONTEXTITEM_KEY_SECURITYTOKEN];
                    }

                    if (HttpContext.Current.Session["SPSecurityToken"] != null)
                    {
                        return (SPSecurityToken)HttpContext.Current.Session["SPSecurityToken"];
                    }

                    return null;

                }
                catch
                {
                    return null;
                }
            }
            set  // Only used by Reports so that Telerik REST API can set and use the SPSEcurityToken since there is no session
            {
                var context = HttpContext.Current;
                if (context == null)
                    return;

                context.Items[CONTEXTITEM_KEY_SECURITYTOKEN] = value;

            }
        }

        /// <summary>
        /// Gets the user associated to the current request or null.
        /// </summary>
        //public static IUser CurrentUser
        //{
        //    get
        //    {
        //        try
        //        {
        //            var context = HttpContext.Current;
        //            if (context == null)
        //                return null;

        //            if (context.Items.Contains(CONTEXTITEM_KEY_USER))
        //            {
        //                return (IUser)context.Items[CONTEXTITEM_KEY_USER];
        //            }

        //            return null;

        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}

        /// <summary>
        /// Determines if the user associated to the current request is part of the specified group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool IsInGroup(string groupName)
        {
            try
            {
                var context = HttpContext.Current;
                if (context == null)
                    return false;

                if (context.Items.Contains(CONTEXTITEM_KEY_GROUPS))
                {
                    var groups = (string[])(context.Items[CONTEXTITEM_KEY_GROUPS]);
                    if (groups == null)
                        return false;

                    return groups.Contains(groupName);
                }

                return false;

            }
            catch
            {
                return false;
            }
        }


    }
}
