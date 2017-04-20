using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DropboxConnect
{
    public class DropboxFile : DropboxBase
    {

        #region "Variables"
        Dropbox.Api.DropboxClient dropboxClient = null;
        #endregion

        //#region "Properties"
        //public string AuthorisationCode { get; set; }
        //#endregion

        #region "Constructor"
        public DropboxFile(string authorisationCode)
        {
            base.AuthorisationCode = authorisationCode;
            dropboxClient = DropboxClientAccess();

        }
        #endregion

        #region "Upload Tasks"
        public async Task<string> UploadFile(string filePath, string dropboxFilePath)
        {
            int fileSize = (4096 * 1024);

            var message = "";
            try
            {
                FileStream fileStream;
                //= new FileStream(fileNametoUpload, System.IO.FileMode.Open,FileAccess.Read)
                using (fileStream = new FileStream(filePath, System.IO.FileMode.Open, FileAccess.Read))
                {

                    if (fileStream.Length <= fileSize)
                    {
                        FileMetadata uploadResult = await dropboxClient.Files.UploadAsync(
                                                                     "/"+ dropboxFilePath,
                                                                      WriteMode.Overwrite.Instance,
                                                                      body: fileStream);
                        if (uploadResult.IsFile)
                        {

                            return uploadResult.Name;
                        }
                        else
                        {
                            message = "Failed: Could not upload the file " + dropboxFilePath;
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
                
            }

            return message;

        }

        //private async Task<bool> UploadAsync(string fileBaseEncoded, string fileNameAndLocationPath)
        //{
        //    //  var dropboxClient = IsAuthenticateUser();
        //    if (dropboxClient != null)
        //    {
        //        try
        //        {
        //            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileBaseEncoded)))
        //            {
        //                var updated = await dropboxClient.Files.UploadAsync(
        //                    fileNameAndLocationPath,
        //                    WriteMode.Overwrite.Instance,
        //                    body: memoryStream);

        //            }
        //        }
        //        catch (ArgumentException ex)
        //        {

        //            Linetime.Logging.Debugging.DebugLog(ex);
        //        }

        //    }

        //    return false;
        //}

        public async Task<string> ChunkUpload(string dropboxFileName, string fileBaseEncoded)
        {
            var message = "";
            // var dropboxClient = IsAuthenticateUser();
            if (dropboxClient != null)
            {
                // Chunk size is 128KB.
                const int chunkSize = 128 * 1024;

                // Create a random file of 1MB in size.
                byte[] byteArray = Convert.FromBase64String(fileBaseEncoded);

                //new Random().NextBytes(fileContent);
                try
                {
                using (var stream = new MemoryStream(byteArray))
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
                    message = string.Format("Failed: Exception - {0} ", ex.Message);
                    
                }

            }

            return message;
        }

        #endregion

        #region "Download Task"
        public async Task<Byte[]> DownloadFile(string fromPath)
        {
            //  var dropboxClient = IsAuthenticateUser();
            if (dropboxClient != null)
            {
                try
                {
                    byte[] downloadedFileContent;
                    using (var fileMetaData = await dropboxClient.Files.DownloadAsync(fromPath))
                    {


                        downloadedFileContent = await fileMetaData.GetContentAsByteArrayAsync();
                        
                       return downloadedFileContent;
                    }
                    //if (downloadedFileContent != null)
                    //{

                    //    // byte[] buffer = new byte[200];
                    //    File.WriteAllBytes(toPath, downloadedFileContent);
                    //}
                }
                catch (ArgumentException)
                {

                    throw;
                }

            }

            return null;

        }
        public async Task<bool> DownloadFile(string fromPath,string toPath)
        {
            //  var dropboxClient = IsAuthenticateUser();
            if (dropboxClient != null)
            {
                try
                {
                    byte[] downloadedFileContent;

                    if (!fromPath.Contains("/")){
                        fromPath = "/" + fromPath;
                     }
                    using (var fileMetaData = await dropboxClient.Files.DownloadAsync(fromPath))
                    {


                        downloadedFileContent = await fileMetaData.GetContentAsByteArrayAsync();

                       
                    }
                    if (downloadedFileContent != null)
                    {

                        try
                        {
                            File.WriteAllBytes(toPath, downloadedFileContent);
                            return true;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        
                    }
                }
                catch (ArgumentException)
                {

                    throw;
                }

            }

            return false;

        }
        #endregion



        //#region "Common Interface Methods"
        //public DropboxClient IsAuthenticateUser()
        //{

        //    return new DropboxClient(this.AuthorisationCode);
        //}
        //#endregion

    }


    //public class FolderContent
    //{
    //    public string FileName { get; set; }

    //    public bool IsFile { get; set; }
    //    public bool IsFolder { get; set; }
    //    public string Path { get; set; }
    //}
}
