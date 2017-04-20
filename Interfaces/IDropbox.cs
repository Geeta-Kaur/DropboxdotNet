using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
namespace DropboxConnect.Interfaces
{
    public interface IDropbox

    {
        DropboxClient IsAuthenticateUser();
        // void DropboxSettings (string applicationKey,string applicationSecretKey,Uri redirectURI);
        // void DropboxSettings(string applicationKey, string applicationSecretKey);
    }
}
