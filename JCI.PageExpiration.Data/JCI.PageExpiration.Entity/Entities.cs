using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCI.PageExpiration.Data.Entities
{
    public class ConfigurationList
    {
        public ConfigurationList()
        { 

        }
     
        public string BusinessUnit { get; set; }
        public string ConfigurationUrl { get; set; }
        public string EmailTemplateUrl { get; set; }
    }


    public class PageExpirationSites
    {
        public PageExpirationSites()
        {

        }
        public string Title { get; set; }
        public string SiteUrl { get; set; }
        public string NotificationGroup { get; set; }
        public string SiteName { get; set; }
       // public FieldUserValue[] NotificationGroup { get; set; }
    }

    public class ExpiringPages
    {
        public ExpiringPages()
        {

        }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ExpirationDate { get; set; }
        public string SiteName { get; set; }
    }
    public class EmailTemplate
    {
        public EmailTemplate()
        {

        }
        public string Name { get; set; }
        public string AdminTemplate { get; set; }
        public string AdminSubject { get; set; }
        public string AuthorTemplate { get; set; }
        public string AuthorSubject { get; set; }
       // public string AdminGroup { get; set; }
        public FieldUserValue[] Admins { get; set; }
     
    }

    public class EmailCollection
    {
        public string Name { get; set; }
        public string ConfigurationURL { get; set; }
  


    }


}
