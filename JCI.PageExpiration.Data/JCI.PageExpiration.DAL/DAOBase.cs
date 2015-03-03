using JCI.PageExpiration.WebJob;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCI.PageExpiration.WebJob.Security;
using System.Configuration;



namespace JCI.PageExpiration.Data
{

    public  abstract class DAOBase
    {
        private SPSecurityToken _token = null;

        private ClientContext _clientContext = null;


        public DAOBase(ClientContext ctx)
        {
            _clientContext = ctx;
        }


        public DAOBase()
        {
            
        }

/*
        public DAOBase()
        {
            this._token = SecurityContext.CurrentSecurityToken;
            if (this._token == null)
                throw new InvalidOperationException("Cannot initialize Business layer without a security token, could not get one from the Security Context");
        }

        public DAOBase(SPSecurityToken token)
        {
           
            if (token == null)
                throw new InvalidOperationException("Cannot initialize Business layer without a security token");

            this._token = token;
        }
        */
       
        /// <summary>
        /// Creates a new sharepoint context, this is IDisposable.
        /// </summary>
        /// <returns></returns>
        public ClientContext CreateClientContext()
        {
            if (_clientContext != null)
                return _clientContext;

            return TokenHelper.GetClientContextWithContextToken(this._token.SPHostURL, this._token.ContextToken, this._token.AppWeb_DNSHostName);
        }


        /// <summary>
        /// returns an app only client context
        /// </summary>
        /// <returns></returns>
        public ClientContext CreateAppOnlyContext()
        {
            SharePointContextToken contextToken =
                    TokenHelper.ReadAndValidateContextToken(this._token.ContextToken, this._token.AppWeb_DNSHostName);

            string appOnlyAccessToken =   TokenHelper.GetAppOnlyAccessToken(contextToken.TargetPrincipalName,
                    this._token.SPHostURL_Authority, contextToken.Realm).AccessToken;

            return TokenHelper.GetClientContextWithAccessToken(this._token.SPHostURL, appOnlyAccessToken);

        }


        /// <summary>
        /// Returns an access token to the host web
        /// </summary>
        /// <returns></returns>
        public string GetHostWebAccessToken()
        {
            var contextToken = TokenHelper.ReadAndValidateContextToken(this._token.ContextToken, this._token.AppWeb_DNSHostName);

           return TokenHelper.GetAccessToken(contextToken, this._token.SPHostURL_Authority).AccessToken;
        }


        /// <summary>
        /// gets access token and returns clientcontext
        /// </summary>
        /// <returns></returns>
        public ClientContext GetClientContextWithAccessToken(string url)
        {
            Uri siteUri = new Uri(url);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);

            //Get the access token for the URL.  
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                siteUri.Authority, realm).AccessToken;

            //Get client context with access token
            ClientContext clientcontext = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken);
            return clientcontext;
        }


     

    }
}
