using Castle.Windsor;
using Castle.MicroKernel.Registration;
using FileUtil.Service.ServiceInterfaces;

namespace FileUtils
{
	class Program
	{
		static void Main(string[] args)
		{
		    var container = new WindsorContainer();

		    //container.Register(Component.For<ICompositionRoot>().ImplementedBy<CompositionRoot>());
            FindDuplicateFilesService dupe = new FindDuplicateFilesService();
			dupe.FindDuplicates();
		}
	}
}
