namespace FileUtils
{
    public interface IAppConfig
    {
    }

    public class AppConfig
    {
		public string User;
		public string Pass;
		public string Domain;
		public string Path;
        public bool IsLocalFileSystem;
        public string DBConnectionString;
    }
}
