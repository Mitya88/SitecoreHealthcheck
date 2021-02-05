using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.Service.Remote
{
    public class AssemblyResolver : IServicesConfigurator
    {
        public void Process(PipelineArgs args)
        {
            var assemblyNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            var assemblyName = $"{assemblyNamespace}.VersionSpecific";

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                if (eventArgs.Name.Contains(","))
                {
                    if (eventArgs.Name.Substring(0, eventArgs.Name.IndexOf(",", StringComparison.Ordinal)) != assemblyName)
                        return null;
                }
                else
                {
                    if (string.Compare(eventArgs.Name, assemblyName, StringComparison.OrdinalIgnoreCase) != 0)
                        return null;
                }

                return LoadVersionAssembly(assemblyNamespace, assemblyName);
            };

            LoadVersionAssembly(assemblyNamespace, assemblyName);
        }

        private static Assembly LoadVersionAssembly(string assemblyNamespace, string assemblyName)
        {
            Sitecore.Diagnostics.Log.Info($"Assembly Resolve: {assemblyName}, {assemblyNamespace}", new object());
            return null;
        }

        public void Configure(IServiceCollection serviceCollection)
        {
            var assemblyNamespace = Assembly.GetExecutingAssembly().GetName().Name;
            var assemblyName = $"{assemblyNamespace}.VersionSpecific";

            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                if (eventArgs.Name.Contains(","))
                {
                    if (eventArgs.Name.Substring(0, eventArgs.Name.IndexOf(",", StringComparison.Ordinal)) != assemblyName)
                        return null;
                }
                else
                {
                    if (string.Compare(eventArgs.Name, assemblyName, StringComparison.OrdinalIgnoreCase) != 0)
                        return null;
                }

                return LoadVersionAssembly(assemblyNamespace, assemblyName);
            };

            LoadVersionAssembly(assemblyNamespace, assemblyName);
        }
    }
}
