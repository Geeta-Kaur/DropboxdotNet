using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace DropboxConnect
{
   public class DropboxFolder : DropboxBase
    {
        #region "Variables"
            Dropbox.Api.DropboxClient dropboxClient = null;
        #endregion

        #region "Constructor"
        public DropboxFolder(string authorisationCode)
        {
            base.AuthorisationCode = authorisationCode;
            dropboxClient = DropboxClientAccess();
        }
        #endregion

        #region "Create Functions"
        /// <summary> 
        /// Creates the specified folder. 
        /// </summary> 
        /// <remarks>This demonstrates calling an rpc style api in the Files namespace.</remarks> 
        /// <param name="path">The path of the folder to create.</param> 
        /// <param name="client">The Dropbox client.</param> 
        /// <returns>The result from the ListFolderAsync call.</returns> 
        public async Task<DropboxFolderMetaData> CreateFolder(string path)
        {
            if (dropboxClient != null)
            {
                DropboxFolderMetaData dropboxFolder = new DropboxFolderMetaData();
                var folderArg = new CreateFolderArg(path);
                var folder = await dropboxClient.Files.CreateFolderAsync(folderArg);
                if (folder != null)
                {
                    dropboxFolder.Name = folder.Name;
                }
               
                return dropboxFolder;
            }

            return null;
        }
        #endregion


        #region "Get Functions"
        public async Task<List<FolderContent>> ListFolderContent(string folderPath)
        {
            List<FolderContent> folderContent = new List<FolderContent>();
            if (dropboxClient != null)
            {
                try
                {
                    var folderResult = await dropboxClient.Files.ListFolderAsync(folderPath);
                    if (folderResult != null)
                    {
                        foreach (var entry in folderResult.Entries)
                        {
                            folderContent.Add(new FolderContent { FileName = entry.Name, IsFolder = entry.IsFolder, IsFile = entry.IsFile, Path = entry.PathDisplay });

                        }


                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }
            return folderContent;
        }
        #endregion

    }
    public class FolderContent
    {
        public string FileName { get; set; }

        public bool IsFile { get; set; }
        public bool IsFolder { get; set; }
        public string Path { get; set; }
    }
    public class DropboxFolderMetaData {
        public string Name { get; set; }
    }
}
