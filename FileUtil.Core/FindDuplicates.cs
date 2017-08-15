using System;
using FileUtil.Models;

namespace FileUtil.Core
{
	public interface IFindDuplicates
	{
		void Run(FindDuplicatesJob job);
		void WalkFilePaths(FindDuplicatesJob job);
	}


	public class FindDuplicates
	{
		public void Run(FindDuplicatesJob job)
		{
			ValidateJob(job);
			//switch on Local / Remote FS
			if (job.Options.IsLocalFileSystem)
			{
				
			}
			else
			{
				
			}
			throw new NotImplementedException();
		}

		public void WalkFilePaths(FindDuplicatesJob job)
		{
			throw new NotImplementedException();
		}

		public void ValidateJob(FindDuplicatesJob job)
		{
			if(!job.Options.IsLocalFileSystem && (String.IsNullOrEmpty(job.Options.User)|| String.IsNullOrEmpty(job.Options.Pass)))
			{
				throw new ArgumentException("Remote Filesystem selected but credentials were invalid.");
			}
		}
	}
}
