namespace FileUtil.Models
{
	public class Source
	{
		public string Name { get; set; }
		public string NetworkShareUser { get; set; }
		public string NetworkSharePassword { get; set; }
		public string NetworkShareDomain { get; set; }
		public string Path { get; set; }
		public string IsLocalFileSystem { get; set; }
	}
}
