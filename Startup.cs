using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PoshCode.PowerShell
{
    public class Startup
    {
        public static IServiceProvider services;

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfigurationRoot>(config)
                .BuildServiceProvider();

            //configure console logging
            services.GetService<ILoggerFactory>()
                    .AddConsole(LogLevel.Debug);


            var logger = services.GetService<ILoggerFactory>().CreateLogger<Startup>();
            logger.LogDebug("Starting application");

            //do the actual work here
            //var bar = serviceProvider.GetService<IBarService>();
            //bar.DoSomeRealWork();
            BeginInvoke(logger);

            logger.LogDebug("All done!");
        }


        public static void BeginInvoke(ILogger<Startup> logger)
        {
            var config = services.GetService<IConfigurationRoot>();
            var computerName = config.GetValue<string>("ComputerName", "localhost");
            var user = config.GetValue<string>("UserName");
            var pw = config.GetValue<string>("Password");
            var password = new SecureString();
            foreach (var ch in pw.ToCharArray())
            {
                password.AppendChar(ch);
            }

            logger.LogDebug($"Connecting to {computerName}");
            var credential = new PSCredential(user, password);
            var runspace = RemotePowerShell.Connect(computerName, credential);

            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript(config.GetValue<string>("Script"));

                ManualResetEvent running = new ManualResetEvent(false);
                using (running)
                {
                    ps.InvocationStateChanged += (sender, args) =>
                    {
                        if (running != null && args.InvocationStateInfo.State == PSInvocationState.Running)
                        {
                            running.Set();
                        }
                    };

                    ps.BeginInvoke();
                    logger.LogDebug($"WaitOne");
                    running.WaitOne();
                    Thread.Sleep(500);
                    logger.LogDebug($"Disconnect");
                    runspace.Disconnect();
                }
                running = null;
            }
        }
    }
}