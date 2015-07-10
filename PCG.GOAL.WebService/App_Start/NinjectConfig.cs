using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Ninject;
using Ninject.Extensions.Conventions;
using PCG.GOAL.ExternalDataService.Interface;
using PCG.GOAL.ExternalDataService.Service;

namespace PCG.GOAL.WebService
{
    public class NinjectConfig
    {
        public static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            RegisterServices(kernel);
            return kernel;
        }

        public static Lazy<IKernel> CreateLazyKernel()
        {
            Func<IKernel> funcKernel = (() =>
            {
                var kernel = new StandardKernel();
                kernel.Load(Assembly.GetExecutingAssembly());

                RegisterServices(kernel);
                return kernel;
            });


            return new Lazy<IKernel>(funcKernel);
        }
        private static void RegisterServices(KernelBase kernel)
        {
            kernel.Bind(x =>
            {
                x.FromThisAssembly() // Scans currently assembly
                    .SelectAllClasses() // Retrieve all non-abstract classes
                    .BindDefaultInterface(); // Binds the default interface to them;
            });
            kernel.Bind<IGoalService>().To<RethinkGoalService>();
            kernel.Bind(typeof(IWebServiceClient<>)).To(typeof(WebServiceClient<>));
        }
    }
}