using DropboxConnect.Authorisation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DropboxConnect
{
    public static class DropboxV2
    {
        // AuthorisationFlowPart1 funtion returns string array: redirect url string placed in first element and other element is error message (if any).
        public static string[] AuthorisationFlowPart1(string appKey, string appSecret, string redirectUrl, string strCrossSiteScriptCheck) {
            redirectUrl = GetValidRedirectURL(redirectUrl);

            AuthorisationSettings authSettings = new AuthorisationSettings(appKey, appSecret, new Uri(redirectUrl), strCrossSiteScriptCheck);
            return AuthorizeApp.GetAuthorisationURL(authSettings);
            

        }
        private static string GetValidRedirectURL(string redirectUrl)
        {
            // dropbox complete redirect url must match with redirect url details registered by us in their developer site.
            if (redirectUrl != string.Empty)
            {
                Uri strRedirect = new Uri(redirectUrl);
                if (strRedirect.Authority.ToUpper().Contains("LOCALHOST"))
                {
                    redirectUrl = strRedirect.Scheme + "://" + strRedirect.Authority;
                }
                else
                {
                    redirectUrl = strRedirect.Scheme + "://" + strRedirect.Authority + strRedirect.Segments[0] + strRedirect.Segments[1];
                }
                return redirectUrl;

            }
            return "";
        }
        public static string AuthorisationFlowPart2(string straccessToken,string appKey, string appSecret, string redirectUrl, string strCrossSiteScriptCheck)
        {
            try
            {
                redirectUrl = GetValidRedirectURL(redirectUrl);
                AuthorisationSettings authSettings = new AuthorisationSettings(appKey, appSecret, new Uri(redirectUrl), strCrossSiteScriptCheck);

                var authTask = AuthorizeApp.GetUserAuthorisationCode(straccessToken, authSettings);
                if (!authTask.IsCompleted) { authTask.Wait(); }
                if (authTask.Result != null)
                {
                    return authTask.Result.ToString();
                }
               
            }
            catch (AggregateException ex)
            {

                foreach (Exception ie in ex.InnerExceptions)
                {
                    throw;
                }

            }

            return "";
        }
        public static string UploadFile(string localPCFileLocation, string dropboxFileName, string accessToken)
        {

            var dropboxHelper = new DropboxFile(accessToken);
            if (dropboxHelper != null)
            {
                // Task(Of String)fileUploadTask As Task(Of String)


                var fileUploadTask = dropboxHelper.UploadFile(localPCFileLocation, dropboxFileName);

                 fileUploadTask.Wait();

                if (!fileUploadTask.Result.ToUpper().Contains("FAILED"))
                {
                    return "Success";
                }
                
            }
            return "";
        }
        public static bool DownloadFile(string localPCFileLocation, string dropboxLocation , string accessToken)
        {
            try
            {
                var dropboxHelper = new DropboxFile(accessToken);
                if (dropboxHelper != null)
                {
                    var fileTask = dropboxHelper.DownloadFile(dropboxLocation, localPCFileLocation);
                    fileTask.Wait();

                    if(! fileTask.IsCompleted)
                     {
                         fileTask.Wait();
                     }

                    
                    if (fileTask.Result)
                     {
                        return true;
                     }

                }
                    
           
            }
            catch (Exception)
            {

                throw;
            }

            return false;
        }

        public static string RequestFileList(string dropboxFolder, string accessToken)
        {

            StringBuilder strfiles= new StringBuilder();

           // FolderContent folderContent = new FolderContent();
            List<FolderContent> folderContentList;
            try
            {
                var dropboxHelper = new DropboxFolder(accessToken);
                if (dropboxHelper != null)
                {
                    var folderTask = dropboxHelper.ListFolderContent(dropboxFolder);
                    folderTask.Wait();
                    if (folderTask.Result != null) {
                        folderContentList = folderTask.Result;
                        // Files were obtained from dropbox so send tham back to the client to populate the combo box
                        foreach (var folderContent in folderContentList)
                        {
                            if (!folderContent.IsFolder) {
                                if (folderContent.Path.StartsWith(("\\"))) {


                                    strfiles.Append(folderContent.Path + "¬");

                                }
                                else
                                {

                                    strfiles.Append(folderContent.Path.Substring(1) + "¬");


                                }
                            }
                        }




                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            
            
           




        return strfiles.ToString();
        }

      

  

        public static string CreateFolder(string folderName, string accessToken )
        {
            var dropboxFolder = new DropboxFolder(accessToken);

            if (dropboxFolder != null) {
                var folderTask = dropboxFolder.CreateFolder("/" + folderName);
                folderTask.Wait();
                if (folderTask.Result !=null) {

                    return folderTask.Result.Name;
            }
            }

            return "";

        }

        



    }
}
