using Dropbox.Api;



namespace DropboxConnect
{
    public abstract class DropboxBase
    {
        #region "Properties"
        string _authorisationCode ="";
        protected  string AuthorisationCode {
            get { return _authorisationCode; }
            set { _authorisationCode = value; }
        }
        #endregion
       
        #region "Properties"
        protected Dropbox.Api.DropboxClient DropboxClientAccess()
        {
            if(_authorisationCode == null) { return null; }
            return new DropboxClient(_authorisationCode);
        }
        #endregion
    }
}
