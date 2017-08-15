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
			throw new NotImplementedException();
		}

		public void WalkFilePaths(FindDuplicatesJob job)
		{
			throw new NotImplementedException();
		}
	}
}
