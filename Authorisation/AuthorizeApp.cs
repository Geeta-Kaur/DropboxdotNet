using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dropbox.Api;
using System.Threading.Tasks;

namespace DropboxConnect.Authorisation
{
    public static class AuthorizeApp
    {
        //#region "properties"

        //public string DropboxAppKey { get; set; }
        //public string DropboxSecretKey { get; set; }
        //public Uri DropboxRedirectURI { get; set; }
        //public string SecurityToken { get; set; }
        //#endregion

        #region "constructors"
        //public AuthorizeApp(string dropboxAppKey, string dropboxSecretKey, Uri dropboxRedirectUri, string securityToken)
        //{
        //    this.DropboxAppKey = dropboxAppKey;
        //    this.DropboxSecretKey = dropboxSecretKey;
        //    this.DropboxRedirectURI = dropboxRedirectUri;
        //    this.SecurityToken = securityToken;
        //}

        #endregion

        #region "Authorisation Flow 1"
        /// <summary>
        /// Gets the URI used to start the OAuth2.0 authorization flow.
        /// </summary>
        /// <param name="oauthResponseType">The grant type requested, either <c>Token</c> or <c>Code</c>.</param>
        /// <param name="appKey">The apps key, found in the
        /// <a href="https://www.dropbox.com/developers/apps">App Console</a>.</param>
        /// <param name="redirectUri">Where to redirect the user after authorization has completed. This must be the exact URI
        /// registered in the <a href="https://www.dropbox.com/developers/apps">App Console</a>; even <c>localhost</c>
        /// must be listed if it is used for testing. A redirect URI is required for a token flow, but optional for code. 
        /// If the redirect URI is omitted, the code will be presented directly to the user and they will be invited to enter
        /// the information in your app.</param>
        /// <param name="state">Up to 500 bytes of arbitrary data that will be passed back to <paramref name="redirectUri"/>.
        /// This parameter should be used to protect against cross-site request forgery (CSRF).</param>
        /// <returns>String array - First element: The uri of a web page which must be displayed to the user in order to authorize the app and second element: error message, if any error occured..</returns>
        public static string[] GetAuthorisationURL(AuthorisationSettings authSettings)
        {
            string[] resulturi = new string[2];
            try
            {
                var uri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, authSettings.DropboxAppKey, authSettings.DropboxRedirectURI.ToString(), authSettings.SecurityToken, true);
                resulturi[0] = uri.ToString();
                resulturi[1] = string.Empty;
            }
            catch (ArgumentException ex)
            {


               
                resulturi[0] = string.Empty;
                resulturi[1] = string.Format("Could not retrieve dropbox url due to exception: {0}", ex.ToString());
                return resulturi;
            }

            return resulturi;
        }
        public static string[] GetAuthorisationWithNoRedirectURL(AuthorisationSettings authSettings)
        {
            string[] resulturi = new string[2];
            try
            {
                var uri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Code, authSettings.DropboxAppKey, string.Empty, authSettings.SecurityToken, true);
                resulturi[0] = uri != null ? uri.ToString() : string.Empty;
                resulturi[1] = string.Empty;
            }
            catch (Exception ex)
            {
                resulturi[0] = string.Empty;
                resulturi[1] = string.Format("Could not retrieve dropbox url due to exception: {0}", ex.ToString());
                
            }

            return resulturi;
        }

        #endregion

        #region "Authorisation Flow 2"

        public static async Task<string> GetUserAuthorisationCode(string userAccessCode, AuthorisationSettings authSettings)
        {
            var message = "";
            try
            {
                OAuth2Response response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(userAccessCode,
                authSettings.DropboxAppKey,
                authSettings.DropboxSecretKey,
                authSettings.DropboxRedirectURI.ToString());
                if (response != null)
                {
                    //if (response.State != null && response.State.Equals(authSettings.SecurityToken))
                    //{
                    return response.AccessToken;
                    //}
                }
                else
                {
                    message = "Failed: Access code is not secure.";
                }


            }
            catch (ArgumentException ex)
            {
                message = "Failed: " + ex.Message;
                
            }

            return message;
        }
        #endregion

        //public DropboxClient IsAuthenticateUser()
        //{

        //   // return new DropboxClient(accessCode);
        //}


    }

    public class AuthorisationSettings
    {

        public string DropboxAppKey { get; set; }
        public string DropboxSecretKey { get; set; }
        public Uri DropboxRedirectURI { get; set; }
        public string SecurityToken { get; set; }
        #region "constructors"
        public AuthorisationSettings(string dropboxAppKey, string dropboxSecretKey, Uri dropboxRedirectUri, string securityToken)
        {
            this.DropboxAppKey = dropboxAppKey;
            this.DropboxSecretKey = dropboxSecretKey;
            this.DropboxRedirectURI = dropboxRedirectUri;
            this.SecurityToken = securityToken;
        }

        #endregion
    }
}
