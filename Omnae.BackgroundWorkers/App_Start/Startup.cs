using System;
using System.Globalization;
using System.Reflection;

namespace Omnae.BackgroundWorkers
{
    internal static class Startup
    {
        public static void RedirectAssembies()
        {
            RedirectAssembly("Newtonsoft.Json", new Version("12.0.0.0"), "30ad4fe6b2a6aeed");
            RedirectAssembly("System.Diagnostics.DiagnosticSource", new Version("5.0.0.1"), "cc7b13ffcd2ddd51");
            RedirectAssembly("System.Runtime.CompilerServices.Unsafe", new Version("5.0.0.0"), "b03f5f7f11d50a3a");
        }

        private static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            Assembly Handler(object sender, ResolveEventArgs args)
            {
                var requestedAssembly = new AssemblyName(args.Name)
                {
                    Version = targetVersion,
                    CultureInfo = CultureInfo.InvariantCulture
                };

                if (requestedAssembly.Name != shortName)
                    return null;

                var targetPublicKeyToken = new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken();
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);

                AppDomain.CurrentDomain.AssemblyResolve -= Handler;

                return Assembly.Load(requestedAssembly);
            }

            AppDomain.CurrentDomain.AssemblyResolve += Handler;
        }
    }
}