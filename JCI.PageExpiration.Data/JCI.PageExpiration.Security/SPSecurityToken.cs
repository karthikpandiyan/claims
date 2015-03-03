using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCI.PageExpiration.WebJob.Security
{
    //future; datcontract
    [Serializable()]
    public class SPSecurityToken
    {
        public string ContextToken { get; set; }
        public string SPHostURL { get; set; }
        public string AppWeb_DNSHostName { get; set; }

        public string SPAppWebUrl { get; set; }

        public string SPHostURL_Authority { get; set; }
    }
}
