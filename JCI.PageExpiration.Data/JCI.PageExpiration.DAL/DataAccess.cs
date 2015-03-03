#region Namespace
using System;
using System.Collections.Generic;
using System.Text;
using JCI.PageExpiration.Data.Entities;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Utilities;
using JCI.PageExpiration.CommonUtitilities;
using JCI.PageExpiration.Data.Common;
using System.Configuration;
using System.Web;
#endregion

namespace JCI.PageExpiration.Data
{
    public class DataAccess : DAOBase
    {
        /// <summary>
        /// Retrieves Configuration details
        /// </summary>
        /// <returns></returns>
        public List<ConfigurationList> GetConfigurationDetails()
        {
            List<ConfigurationList> results = null;
            try
            {
                
                string siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
                Trace.TraceInformation("Site Url obtained from config :  " + siteUrl);
                Console.WriteLine("Site Url obtained from config :  " + siteUrl);
                results = new List<ConfigurationList>();
                using (var clientContext = this.GetClientContextWithAccessToken(siteUrl))
                {
                    CamlQuery caml = new CamlQuery();
                    Trace.TraceInformation("GetConfigurationDetails Items retrieval Start :  " + siteUrl);
                    Console.WriteLine("GetConfigurationDetails Items retrieval Start :  " + siteUrl);
                   
                    ListItemCollection lstItemCollection = CommonUtilities.GetListItemCollection(clientContext, ConfigurationManager.AppSettings["ConfigurationList"], caml);

                    Trace.TraceInformation("GetConfigurationDetails Items retrieval End :  " + siteUrl);
                    Console.WriteLine("GetConfigurationDetails Items retrieval End :  " + siteUrl);

                    Trace.TraceInformation("GetConfigurationDetails Items Loop Start :  " + siteUrl);
                    Console.WriteLine("GetConfigurationDetails Items Loop Start :  " + siteUrl);
                    // Iterate through all items in the List
                    foreach (ListItem item in lstItemCollection)
                    {
                        ConfigurationList peList = new ConfigurationList();
                        peList.BusinessUnit = CommonUtilities.ToSafeString(item[PageConfigurationList.Title]);
                        peList.EmailTemplateUrl = CommonUtilities.ToSafeString(((Microsoft.SharePoint.Client.FieldUrlValue)(item[PageConfigurationList.EmailtemplateList])).Url);
                        Console.WriteLine(peList.EmailTemplateUrl);
                        peList.ConfigurationUrl = CommonUtilities.ToSafeString(((Microsoft.SharePoint.Client.FieldUrlValue)(item[PageConfigurationList.ConfigurationList])).Url);
                        results.Add(peList);

                    }

                    Trace.TraceInformation("GetConfigurationDetails Items Loop End :  " + siteUrl);
                    Console.WriteLine("GetConfigurationDetails Items Loop End :  " + siteUrl);
                }

                return results;

            }
            catch (Exception ex)
            {
                Trace.TraceError("GetConfigurationDetails Error", ex);
                Console.WriteLine(Trace.GetDetailedError("GetConfigurationDetails Error ", ex));
                return null;
            }
        }

        /// <summary>
        /// Retrieves site Expiration Config details
        /// </summary> 
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public List<PageExpirationSites> GetSiteExpirationDetails(string siteUrl)
        {
            List<PageExpirationSites> results = null;
            try
            {
                string siteCollection = getSiteCollectionUrl(siteUrl);
                results = new List<PageExpirationSites>();
                using (var clientContext = this.GetClientContextWithAccessToken(siteCollection))
                {
                    CamlQuery caml = new CamlQuery();
                    // Sets row limit here
                    caml.ViewXml = @"<View Scope='RecursiveAll'><RowLimit>"+ConfigurationManager.AppSettings["ResultItems"]+"</RowLimit></View>";

                    Trace.TraceInformation("GetSiteExpirationDetails rowlimit count obtained from App.config");
                    Console.WriteLine("GetSiteExpirationDetails rowlimit count obtained from App.config");

                    Trace.TraceInformation("GetSiteExpirationDetails Listname to be obtained from SiteURL : " + siteUrl);
                    Console.WriteLine("GetSiteExpirationDetails Listname to be obtained from SiteURL" + siteUrl);

                    var spLlist = clientContext.Web.GetList(siteUrl);
                    clientContext.Load(spLlist, list => list.Title);
                    clientContext.ExecuteQuery();
                    Trace.TraceInformation("GetSiteExpirationDetails successfully obtained from SiteURL : " + siteUrl);
                    Console.WriteLine("GetSiteExpirationDetails successfully obtained from SiteURL :" + siteUrl);

                    Trace.TraceInformation("GetSiteExpirationDetails Items retrieval Start : " + siteUrl);
                    Console.WriteLine("GetSiteExpirationDetails Items retrieval Start : " + siteUrl);

                    ListItemCollection lstItemCollection = CommonUtilities.GetListItemCollection(clientContext, spLlist.Title, caml);
                    Console.WriteLine("TotalCount:" + lstItemCollection.Count);
                    Trace.TraceInformation("GetSiteExpirationDetails Items retrieval End : " + siteUrl);
                    Console.WriteLine("GetSiteExpirationDetails Items retrieval End : " + siteUrl);

                    Trace.TraceInformation("GetSiteExpirationDetails Items Loop Start : " + siteUrl);
                    Console.WriteLine("GetSiteExpirationDetails Items Loop Start : " + siteUrl);

                    // Iterate through all items in the List
                    foreach (ListItem item in lstItemCollection)
                    {
                        PageExpirationSites pageExpSites = new PageExpirationSites();
                        pageExpSites.Title = CommonUtilities.ToSafeString(item[PageExpirationSitesList.Title]);
                        pageExpSites.SiteUrl = CommonUtilities.ToSafeString(((Microsoft.SharePoint.Client.FieldUrlValue)(item[PageExpirationSitesList.SiteUrl])).Url);
                        Console.WriteLine(CommonUtilities.ToSafeString(((Microsoft.SharePoint.Client.FieldUrlValue)(item[PageExpirationSitesList.SiteUrl])).Url));
                        Console.WriteLine(CommonUtilities.ToSafeString(((Microsoft.SharePoint.Client.FieldUrlValue)(item[PageExpirationSitesList.SiteUrl])).Description));
                        pageExpSites.NotificationGroup = CommonUtilities.ToSafeString(item[PageExpirationSitesList.NotificationGroup]);
                        pageExpSites.SiteName = clientContext.Web.Title;
                        results.Add(pageExpSites);
                    }

                    Trace.TraceInformation("GetSiteExpirationDetails Items Loop End : " + siteUrl);
                    Console.WriteLine("GetSiteExpirationDetails Items Loop End : " + siteUrl);

                }
                return results;
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetSiteExpirationDetails Error ", ex);
                Console.WriteLine(Trace.GetDetailedError("GetSiteExpirationDetails Error ", ex));
                return null;
            }

        }
        /// <summary>
        /// Retrieves list of pages which are going to expire withing 45 days
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public List<ExpiringPages> GetPublishingPages(string siteUrl)
        {
            Trace.TraceInformation("GetPublishingPages URL: "+siteUrl);
            Console.WriteLine("GetPublishingPages URL: "+siteUrl);
            string endDate = string.Empty;
            List<ExpiringPages> results = null;
            try
            {
                results = new List<ExpiringPages>();
                using (var clientContext = this.GetClientContextWithAccessToken(siteUrl))
                {
                    CamlQuery caml = new CamlQuery();
                    DateTime dt = DateTime.Now.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["Reminderdays"]));
                    Trace.TraceInformation("GetPublishingPages reminderdays obtained from App.config");
                    Console.WriteLine("GetPublishingPages reminderdays obtained from App.config");
                    string schedulingEndDate = dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    Trace.TraceInformation("GetPublishingPages End Date : " + schedulingEndDate);
                    Console.WriteLine("GetPublishingPages End Date :" + schedulingEndDate);

                    caml.ViewXml = @"<View Scope='RecursiveAll'>" +
                   "<Query>" + "<Where>" +
                        "<Leq><FieldRef Name='PublishingExpirationDate'  IncludeTimeValue='FALSE'/><Value Type='PublishingScheduleEndDateFieldType'>" + schedulingEndDate + "</Value></Leq>"
                        + "</Where><OrderBy><FieldRef Name='PublishingExpirationDate' Ascending='True' /></OrderBy></Query>" +
                   "</View>";

                    Trace.TraceInformation("GetPublishingPages items retrieval Start : " + siteUrl);
                    Console.WriteLine("GetPublishingPages items retrieval Start :" + siteUrl);

                    ListItemCollection lstItemCollection = CommonUtilities.GetListItemCollection(clientContext, ConfigurationManager.AppSettings["Pages"], caml);

                    Trace.TraceInformation("GetPublishingPages Pages Loop Start : " + siteUrl);
                    Console.WriteLine("GetPublishingPages Pages Loop Start : " + siteUrl);
                    // Iterate through all items in the List
                    foreach (ListItem item in lstItemCollection)
                    {
                        ExpiringPages expPages = new ExpiringPages();
                        expPages.Name = item[ExpirationPagesList.PageName].ToSafeString();
                        expPages.Url = item.File.Context.Url.ToString() + "/Pages/" + item[ExpirationPagesList.PageName].ToSafeString();
                        expPages.ExpirationDate = item[ExpirationPagesList.SchedulingEndDate].ToSafeString();
                        expPages.SiteName = clientContext.Web.Title;
                        Console.WriteLine("Name: " + expPages.Name + " Url: " + expPages.Url + " ExpirationDate: " + expPages.ExpirationDate + " SiteName: " + expPages.SiteName);
                        results.Add(expPages);
                    }
                    Trace.TraceInformation("GetPublishingPages Pages Loop End : " + siteUrl);
                    Console.WriteLine("GetPublishingPages Pages Loop End : " + siteUrl);
                }
                return results;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Page Expiration {0}", ex);
                Console.WriteLine(Trace.GetDetailedError("Get Publishing Pages Error ", ex));
                return null;
            }

        }

        /// <summary>
        /// Retrieves Email template values
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public List<EmailTemplate> GetEmailDetails(string siteUrl)
        {
            string siteCollection = getSiteCollectionUrl(siteUrl);

            List<EmailTemplate> results = null;
            try
            {
                results = new List<EmailTemplate>();
                using (var clientContext = this.GetClientContextWithAccessToken(siteCollection))
                {

                    var spLlist = clientContext.Web.GetList(siteUrl);
                    clientContext.Load(spLlist, list => list.Title);
                    clientContext.ExecuteQuery();
                    Trace.TraceInformation("GetEmailDetails List Name obtained from  : " + siteUrl);
                    Console.WriteLine("GetEmailDetails List Name obtained from : " + siteUrl);
                    CamlQuery caml = new CamlQuery();
                    Trace.TraceInformation("GetEmailDetails Items retrival Start : " + siteUrl);
                    Console.WriteLine("GetEmailDetails Items retrival Start : : " + siteUrl);
                    ListItemCollection lstItemCollection = CommonUtilities.GetListItemCollection(clientContext, spLlist.Title, caml);
                    Trace.TraceInformation("GetEmailDetails Items retrival Start : " + siteUrl);
                    Console.WriteLine("GetEmailDetails Items retrival Start : : " + siteUrl);
                    Trace.TraceInformation("GetEmailDetails Items Loop Start: " + siteUrl);
                    Console.WriteLine("GetEmailDetails Items Loop Start : : " + siteUrl);
                    // Iterate through all items in the List
                    foreach (ListItem item in lstItemCollection)
                    {
                        EmailTemplate emailTemplate = new EmailTemplate();
                        emailTemplate.AdminSubject = CommonUtilities.ToSafeString(item[EmailTemplateList.AdminSubject]);
                        Console.WriteLine(emailTemplate.AdminSubject);
                        emailTemplate.AdminTemplate = CommonUtilities.ToSafeString(item[EmailTemplateList.AdminTemplate]);
                        Console.WriteLine(emailTemplate.AdminTemplate);
                      
                        FieldUserValue[] adminGroups = item[EmailTemplateList.AdminGroup] as FieldUserValue[];
                        if (adminGroups!=null)
                           emailTemplate.Admins = adminGroups;
                        
                        emailTemplate.AuthorTemplate = CommonUtilities.ToSafeString(item[EmailTemplateList.AuthorTemplate]);
                        Console.WriteLine(emailTemplate.AuthorTemplate);
                        emailTemplate.AuthorSubject = CommonUtilities.ToSafeString(item[EmailTemplateList.AuthorSubject]);
                        Console.WriteLine(emailTemplate.AuthorSubject);
                        results.Add(emailTemplate);
                    }
                    Trace.TraceInformation("GetEmailDetails Items Loop  End: " + siteUrl);
                    Console.WriteLine("GetEmailDetails Items Loop End : " + siteUrl);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Page Expiration {0}", ex);
                Console.WriteLine(Trace.GetDetailedError("GetEmailDetails Error ", ex));
                return null;
            }
            return results;
        }


     /// <summary>
     /// Sends Email to Multiple Users
     /// </summary>
     /// <param name="siteUrl"></param>
     /// <param name="Users">Multiple users</param>
     /// <param name="subject"></param>
     /// <param name="body"></param>
        public void SendEmail(string siteUrl, FieldUserValue[] Users, string subject, string body)
        {

            Trace.TraceInformation("Email sending to multiple Users started");
            Console.WriteLine("Email sending to multiple Users started " );
            string siteCollection = getSiteCollectionUrl(siteUrl);
            try
            {
                using (var clientContext = this.GetClientContextWithAccessToken(siteCollection))
                {
                    EmailProperties properties = new EmailProperties();
                    // Sends mail to multiple Users
                    foreach (FieldUserValue userValue in Users)
                    {
                      
                        string emailId = isUser(clientContext, userValue.LookupValue);
                        if (emailId != string.Empty)
                        {
                            properties.To = new string[] { emailId };
                        }
                        else
                        {
                            clientContext.Load(clientContext.Web.SiteGroups);
                            clientContext.ExecuteQuery();
                            Group group = clientContext.Web.SiteGroups.GetByName(userValue.LookupValue);
                            clientContext.Load(group);
                            clientContext.Load(group.Users);
                            clientContext.ExecuteQuery();
                            UserCollection users = group.Users;
                            string[] tolist = new string[users.Count];
                            int i = 0;
                            foreach (User groupUser in users)
                            {
                                tolist[i] = groupUser.Email;
                                i++;
                            }
                            properties.To = new string[] { };
                            properties.To = tolist;
                        }

                        properties.Subject = subject;
                        properties.Body = body;
                        try
                        {
                            Trace.TraceInformation("Email sending to " + userValue.LookupValue + " started");
                            Console.WriteLine("Email sending to " + userValue.LookupValue + " started");
                            
                            Utility.SendEmail(clientContext, properties);
                            clientContext.ExecuteQuery();
                        
                            Trace.TraceInformation("Email Sent to " + userValue.LookupValue);
                            Console.WriteLine("Email Sent to " + userValue.LookupValue);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(string.Format("Unable to deliver email to  {0} ", userValue.ToString()));
                            Trace.TraceError("Unable to deliver Email {0}", ex);
                            Console.WriteLine(Trace.GetDetailedError("SendEmail Error ", ex));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError(string.Format("SendEmail error  {0} ", ex));
                Trace.TraceError("SendEmail error  {0}", ex);

            }
        }

        /// <summary>
        /// Sends email to single user
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="To">Single User or Group</param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void SendEmail(string siteUrl, string To, string subject, string body)
        {
            string siteCollection = getSiteCollectionUrl(siteUrl);
            try
            {
                using (var clientContext = this.GetClientContextWithAccessToken(siteCollection))
                {
                    EmailProperties properties = new EmailProperties();
                    string emailId = isUser(clientContext, To);
                    if (emailId != string.Empty)
                    {
                        properties.To = new string[] { emailId };
                    }
                    else
                    {
                        clientContext.Load(clientContext.Web.SiteGroups);
                        clientContext.ExecuteQuery();
                        Group group = clientContext.Web.SiteGroups.GetByName(To.Trim());
                        clientContext.Load(group);
                        clientContext.Load(group.Users);
                        clientContext.ExecuteQuery();
                        UserCollection users = group.Users;
                        string[] tolist = new string[users.Count];
                        int i = 0;
                        foreach (User groupUser in users)
                        {
                            Console.WriteLine("Group member's Email Id : " + groupUser.Email);
                            tolist[i] = groupUser.Email;
                            i++;
                        }
                        properties.To = new string[] { };
                        properties.To = tolist;
                    }

                    properties.Subject = subject;
                    properties.Body = body;
                    try
                    {
                        Trace.TraceInformation("Email sending to " + To + " started");
                        Console.WriteLine("Email sending to " + To + " started");
                      
                      
                        Utility.SendEmail(clientContext, properties);
                        clientContext.ExecuteQuery();
                     
                        Trace.TraceInformation("Email Sent to " + To);
                        Console.WriteLine("Email Sent to " + To);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(string.Format("Unable to deliver email to  {0} ", To));
                        Trace.TraceError("Unable to deliver Email {0}", ex);
                        Console.WriteLine(Trace.GetDetailedError("SendEmail Error ", ex));
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.TraceError(string.Format("Unable to deliver email to  {0} ", To));
                Trace.TraceError("Unable to deliver Email {0}", ex);

            }
        }
        /// <summary>
        /// Root method which executes the functionality
        /// </summary>
        public void pageExpirationWebJob()
        {
            try
            {
                #region Variables
                DataAccess dataAccess = new DataAccess();
                List<ConfigurationList> lstConfiguration = null;
                List<PageExpirationSites> lstPageExpirationSites = null;
                List<ExpiringPages> lstExpiringPages = null;
                List<EmailTemplate> lstEmailTemplate = null;
                string body = string.Empty;
                string expirationDate = string.Empty;
                StringBuilder peBuilder = null;
                StringBuilder peAdminBuilder = null;
                StringBuilder peAdminSiteBuilder = new StringBuilder();
                string siteName = string.Empty;
                string siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
                FieldUserValue[] admins  = null;
                string AdminSubject = string.Empty;
                string expirationSites = string.Empty;
                string expirationPages = string.Empty;
                string expirationPagesTotal = string.Empty;
                string adminTemplate = string.Empty;
                #endregion

                #region MainFunctionality

                Trace.TraceInformation("GetConfigurationDetails start");
                Console.WriteLine("GetConfigurationDetails start");

                lstConfiguration = dataAccess.GetConfigurationDetails();

                Trace.TraceInformation("GetConfigurationDetails End");
                Console.WriteLine("GetConfigurationDetails End");

                Trace.TraceInformation("GetConfigurationDetails Loop Start");
                Console.WriteLine("GetConfigurationDetails Loop Start");
                //Content Hub Configuration List
                foreach (var config in lstConfiguration)
                {
                    try
                    {
                        Console.WriteLine(config.ConfigurationUrl);
                        peAdminBuilder = new StringBuilder();
                        Trace.TraceInformation("GetSiteExpirationDetails Start");
                        Console.WriteLine("GetSiteExpirationDetails Start");

                        lstPageExpirationSites = dataAccess.GetSiteExpirationDetails(config.ConfigurationUrl);

                        Trace.TraceInformation("GetSiteExpirationDetails End");
                        Console.WriteLine("GetSiteExpirationDetails End");

                        Trace.TraceInformation("GetEmailDetails Start");
                        Console.WriteLine("GetEmailDetails Start");

                        lstEmailTemplate = dataAccess.GetEmailDetails(config.EmailTemplateUrl);

                        Trace.TraceInformation("GetEmailDetails End");
                        Console.WriteLine("GetEmailDetails End");

                        Trace.TraceInformation("GetSiteExpirationDetails Loop Start");
                        Console.WriteLine("GetSiteExpirationDetails Start");
                        // Page Expiration Sites List
                        foreach (var exp in lstPageExpirationSites)
                        {
                            try
                            {
                                peBuilder = new StringBuilder();
                                Trace.TraceInformation("GetPublishingPages Start");
                                Console.WriteLine("GetPublishingPages Start");
                                // Pages Libary
                                 lstExpiringPages = dataAccess.GetPublishingPages(exp.SiteUrl);
                                 Trace.TraceInformation("GetPublishingPages End");
                                 Console.WriteLine("GetPublishingPages End");
                                if(lstExpiringPages.Count>0)
                                {
                                    peBuilder.Append("<div sytle=width:100%;font-size: 10px!important;font-family: sans-serif!important;>");
                                    peBuilder.Append("#######");
                                    peBuilder.Append(" - ");
                                    peBuilder.Append("<a href='");
                                    peBuilder.Append(exp.SiteUrl);
                                    peBuilder.Append("'>");
                                    peBuilder.Append(exp.SiteUrl);
                                    peBuilder.Append("</a>");
                                    peBuilder.Append("<br>");
                                    peBuilder.Append("<b>Note:</b> This list is sorted by Expiration Date with the Pages set to Expire first on top.<br><br>");
                                    siteName = exp.SiteName;
                                  
                                    peBuilder.Append("<table border='1' style='width:100%'><th><b>Page Name</b></th><th><b>Links to Page</b></th><th><b>Expiration Date</b></th>");

                                    Trace.TraceInformation("GetPublishingPages Loop Start");
                                    Console.WriteLine("GetPublishingPages Start");

                                    foreach (var page in lstExpiringPages)
                                    {
                                        try
                                        {
                                            peBuilder.Replace("#######", "<b>" + page.SiteName + "</b>");
                                            peBuilder.Append("<tr><td style='width:20%'>");
                                            peBuilder.Append(page.Name);
                                            peBuilder.Append("</td>" + "<td style='width:20%'>");
                                            peBuilder.Append("<a href='");
                                            peBuilder.Append(page.Url);
                                            peBuilder.Append("'>");
                                            peBuilder.Append(page.Url);
                                            peBuilder.Append("</a>");
                                            peBuilder.Append("</td>" + "<td style='width:20%'>");
                                            peBuilder.Append(page.ExpirationDate);
                                        }
                                        catch (Exception ex)
                                        {
                                            Trace.TraceError("GetPublishingPages Loop Error Occured {0}", ex);
                                            Console.WriteLine(Trace.GetDetailedError("GetPublishingPages Loop Error Occured ", ex));
                                        }
                                    }
                                    Trace.TraceInformation("GetPublishingPages Loop End");
                                    Console.WriteLine("GetPublishingPages End");

                                    peBuilder.Append("</td></tr></table>");
                                    peBuilder.Append("</div>");
                                    // Email Template List
                                    foreach (var email in lstEmailTemplate)
                                    {
                                        try
                                        {
                                            AdminSubject = email.AdminSubject;
                                            adminTemplate = email.AdminTemplate;
                                            admins = email.Admins;
                                            peAdminBuilder.Append(peBuilder.ToString());
                                            peAdminBuilder.Append("<br>");
                                            peBuilder.Insert(0, email.AuthorTemplate + "<br>");
                                            string subject =  string.Format(email.AuthorSubject, DateTime.Now.AddDays(Convert.ToInt32(ConfigurationManager.AppSettings["Reminderdays"])).Date.ToShortDateString());
                                            Trace.TraceInformation("SendEmail Start");
                                            Console.WriteLine("SendEmail Start");
                                            dataAccess.SendEmail(config.ConfigurationUrl, exp.NotificationGroup, subject, peBuilder.ToString());
                                            Trace.TraceInformation("SendEmail End");
                                            Console.WriteLine("SendEmail End");
                                            peBuilder = null;
                                            expirationPages = string.Empty;
                                            expirationSites = string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            Trace.TraceError("GetEmailDetails Loop Error Occured {0} ", ex);
                                            Console.WriteLine(Trace.GetDetailedError("GetEmailDetails Loop Error Occured ", ex));
                                        }
                                    }
                                }
                              
                              
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError("GetSiteExpirationDetails Loop Error Occured {0}", ex);
                                Console.WriteLine(Trace.GetDetailedError("GetSiteExpirationDetails Loop Error Occured ", ex));
                            }
                        }
                        Trace.TraceInformation("GetSiteExpirationDetails Loop End");
                        Console.WriteLine("GetSiteExpirationDetails End");
                        peAdminBuilder.Insert(0, adminTemplate);

                        Trace.TraceInformation("SendEmail to Admin Start");
                        Console.WriteLine("SendEmail to Admin Start");
                        // Sends mail to Admin
                        dataAccess.SendEmail(config.ConfigurationUrl, admins, AdminSubject, peAdminBuilder.ToString());
                        Trace.TraceInformation("SendEmail to Admin End");
                        Console.WriteLine("SendEmail to Admin End");
                        peAdminBuilder = null;

                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("GetConfigurationDetails Loop Error Occured {0}", ex);
                        Console.WriteLine(Trace.GetDetailedError("GetConfigurationDetails Loop Error Occured ", ex));
                    }
                }
                // Config Loop End
                Trace.TraceInformation("GetConfigurationDetails Loop End");
                Console.WriteLine("GetConfigurationDetails Loop End");
                #endregion
            }
            catch (Exception ex)
            {
                Trace.TraceError("Page Expiration {0}", ex);
                Console.WriteLine(Trace.GetDetailedError("pageExpirationWebJob Error ", ex));
            }
        }

        /// <summary>
        /// Checks user existing or not
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="To"></param>
        /// <returns></returns>
        private string isUser(ClientContext clientContext, string To)
        {
            User user = null;
            string email = string.Empty;
            try
            {
                user = clientContext.Web.EnsureUser(To);

                if (user != null)
                {
                    clientContext.Load(user);
                    clientContext.ExecuteQuery();
                    email = user.Email;
                }
                return user.Email;
            }
            catch (Exception ex)
            {
                //Trace.TraceError("isUser Error Occured {0}", ex);
                //Console.WriteLine(Trace.GetDetailedError("isUser Error Occured", ex));
                return email;
            }

        }

        /// <summary>
        /// Separates listname and site collection name
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string getSiteCollectionUrl(string url)
        {
            try{
            string[] listUrl = url.Split(new string[] { "Lists" }, StringSplitOptions.None);
            Trace.TraceError("getSiteCollectionUrl Mothod Called");
            Console.WriteLine("getSiteCollectionUrl Mothod Called");
            return listUrl[0];
        }
             catch (Exception ex)
            {
                Trace.TraceError("getSiteCollectionUrl Error Occured {0}", ex);
                Console.WriteLine(Trace.GetDetailedError("getSiteCollectionUrl Error Occured ", ex));
                return string.Empty;
            }
        }
    }
}




