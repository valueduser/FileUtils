namespace FileUtil.Models
{
	public class Source
	{
		public string NetworkShareUser { get; set; }
		public string NetworkSharePassword { get; set; }
		public string NetworkShareDomain { get; set; }
		public string NetworkShareUncPath { get; set; }
		public string IsLocalFileSystem { get; set; }
	}
}
