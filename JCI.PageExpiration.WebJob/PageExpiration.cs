#region Namespace
using JCI.PageExpiration.Data;
using System;
#endregion

namespace JCI.PageExpiration.WebJob
{
    class PageExpirationJob
    {
        static void Main(string[] args)
        {
            DataAccess dataAccess = new DataAccess();
            // Calls Root function
            dataAccess.pageExpirationWebJob();

        }
    }
}
