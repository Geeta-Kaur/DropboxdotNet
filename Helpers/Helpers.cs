using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DropboxConnect.Interfaces;
using System.IO;
using Dropbox.Api;
using Dropbox.Api.Files;
using Linetime.Cryptography;

namespace DropboxAccess
{
public class Helpers
    {

        #region "Variables"
        DropboxClient dropboxClient = null;
        #endregion

        #region "Properties"
        public string AuthorisationCode { get; set; }
        #endregion

        #region "Constructor"
        public Helpers(string authorisationCode)
        {
            AuthorisationCode = AES.Decrypt(authorisationCode, "L1n3t1m3");
            dropboxClient = IsAuthenticateUser();
           
        }
        #endregion

        #region "Upload Tasks"
        public async Task<string> UploadFile(string filePath, string dropboxFileName)
        {
            int fileSize = (4096 * 1024);
            
          var message = "";
            try
            {
                FileStream fileStream;
                    //= new FileStream(fileNametoUpload, System.IO.FileMode.Open,FileAccess.Read)
                using (fileStream = new FileStream(filePath, System.IO.FileMode.Open, FileAccess.Read)) {

                    if (fileStream.Length <= fileSize)
                    {
                        FileMetadata uploadResult = await dropboxClient.Files.UploadAsync(
                                                                      filePath + "/" + dropboxFileName,
                                                                      WriteMode.Overwrite.Instance,
                                                                      body: fileStream);
                        if (uploadResult.IsFile)
                        {

                            return uploadResult.Name;
                        }
                        else {
                            message = "Failed: Could not upload the file " + dropboxFileName;
                        }

                    }
                       


                }
               
                //if (fileSize > 150)
                //{
                //    await ChunkUpload(dropboxFilename, fileBase64Encoded);
                //}
                //else
                //{
                    //var uploadResult = await dropboxClient.Files.UploadAsync(
                    //       dropboxFileName,
                    //       WriteMode.Overwrite.Instance,
                    //       body: memoryStream);
                    //  await UploadAsync(fileBase64Encoded, (fileLocation + "/" + dropboxFilename));

                //}
            }
            catch (ArgumentException ex)
            {

                message = string.Format("Failed: Exception - {0} ", ex.Message);
                Linetime.Logging.Debugging.DebugLog(ex);
            }

            return message;

        }

        private  async Task<bool> UploadAsync (string fileBaseEncoded, string fileNameAndLocationPath)
        {
          //  var dropboxClient = IsAuthenticateUser();
            if (dropboxClient != null)
            {
                try
                {
                    using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileBaseEncoded)))
                    {
                        var updated = await dropboxClient.Files.UploadAsync(
                            fileNameAndLocationPath,
                            WriteMode.Overwrite.Instance,
                            body: memoryStream);

                    }
                }
                catch (ArgumentException ex)
                {

                    Linetime.Logging.Debugging.DebugLog(ex);
                }

            }
            
            return false;
        }

        private  async Task ChunkUpload(string dropboxFileName, string fileBaseEncoded)
        {
            // var dropboxClient = IsAuthenticateUser();
            if (dropboxClient != null)
            {
                // Chunk size is 128KB.
                const int chunkSize = 128 * 1024;

                // Create a random file of 1MB in size.
                //var fileContent = new byte[1024 * 1024];
                //new Random().NextBytes(fileContent);
                try
                {
                    using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileBaseEncoded)))
                    {
                        int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);

                        byte[] buffer = new byte[chunkSize];
                        string sessionId = null;

                        for (var idx = 0; idx < numChunks; idx++)
                        {
                            //   Console.WriteLine("Start uploading chunk {0}", idx);
                            var byteRead = stream.Read(buffer, 0, chunkSize);

                            using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                            {
                                if (idx == 0)
                                {
                                    var result = await dropboxClient.Files.UploadSessionStartAsync(body: memStream);
                                    sessionId = result.SessionId;
                                }

                                else
                                {
                                    UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                                    if (idx == numChunks - 1)
                                    {
                                        await dropboxClient.Files.UploadSessionFinishAsync(cursor, new CommitInfo(dropboxFileName), memStream);
                                    }

                                    else
                                    {
                                        await dropboxClient.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (ArgumentException ex)
                {

                    Linetime.Logging.Debugging.DebugLog(ex);
                }

            }
           

        }

        #endregion

        #region "Download Task"
        public async Task<string> DownloadFile(string fileNameAndLocationPath)
        {
          //  var dropboxClient = IsAuthenticateUser();
            if (dropboxClient != null)
            {
                try
                {
                    using (var response = await dropboxClient.Files.DownloadAsync(fileNameAndLocationPath))
                    {

                        var downloadedFileContent = await response.GetContentAsByteArrayAsync();
                        return Convert.ToBase64String(downloadedFileContent);
                    }
                }
                catch (ArgumentException ex)
                {

                    Linetime.Logging.Debugging.DebugLog(ex);
                }

            }

            return null;

        }
        #endregion

        #region "Get List of files"
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
                                folderContent.Add(new FolderContent {FileName=entry.Name, IsFolder= entry.IsFolder,IsFile=entry.IsFile,Path = entry.PathDisplay});
                            
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        Linetime.Logging.Debugging.DebugLog(ex);
                    }

                }
                return folderContent;
            }
        #endregion

        #region "Common Interface Methods"
        public DropboxClient IsAuthenticateUser()
        {
           
            return new DropboxClient(this.AuthorisationCode);
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
}
