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
        public static ILogger logger;

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

            // configure console logging
            services.GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);


            logger = services.GetService<ILoggerFactory>().CreateLogger<Startup>();
            logger.LogTrace("Starting application");

            // do the actual work here
            BeginInvoke();

            logger.LogTrace("Exiting application");
        }


        public static void BeginInvoke()
        {
            var config = services.GetService<IConfigurationRoot>();
            var computerName = config.GetValue("ComputerName", "localhost");
            var script = config.GetValue("Script", "1..60 | % { \"Received $_ $(Get-Date)\"; Start-Sleep 5 } | Tee ~\\log.txt");

            PSCredential credential = GetCredential(config);

            logger.LogDebug($"Connecting to {computerName}");

            var connection = new WSManConnectionInfo("http", computerName, 5985, "wsman", $"http://schemas.microsoft.com/powershell/PowerShell.6", credential);
            connection.IdleTimeout = 30 * 1000 * 60; // thirty minutes

            if (!string.IsNullOrWhiteSpace(script))
            {
                logger.LogDebug($"Create runspace, and run {{{script}}}");
                // When given a script, run it
                var runspace = RunspaceFactory.CreateRunspace(connection);
                runspace.Open();

                using (var powershell = System.Management.Automation.PowerShell.Create())
                {
                    powershell.Runspace = runspace;
                    powershell.AddScript(script);
                    powershell.BeginInvoke();
                    Thread.Sleep(250);
                    runspace.Disconnect();
                }
            }
            else
            {
                // Without a script, look for existing runspaces
                foreach (var runspace in Runspace.GetRunspaces(connection))
                {
                    if (runspace.RunspaceStateInfo.State == RunspaceState.Disconnected)
                    {
                        logger.LogDebug($"Connect runspace {runspace.Name} which is {runspace.RunspaceStateInfo.State}");

                        using (var powershell = runspace.CreateDisconnectedPowerShell())
                        {
                            //var powershell = System.Management.Automation.PowerShell.Create();
                            //powershell.Runspace = runspace;

                            logger.LogDebug($"Connect PowerShell {powershell.InstanceId} which is {powershell.InvocationStateInfo.State}");

                            foreach (var result in powershell.Connect())
                            {
                                Console.WriteLine(LanguagePrimitives.ConvertTo(result, typeof(string)));
                            }
                        }
                    }
                    else
                    {
                        logger.LogDebug($"Cannot reconnect runspace {runspace.Name} which is {runspace.RunspaceStateInfo.State}");
                    }
                }
            }

            //ManualResetEvent running = new ManualResetEvent(false);
            //using (running)
            //{
            //    ps.InvocationStateChanged += (sender, args) =>
            //    {
            //        logger.LogDebug($"InvocationState: {args.InvocationStateInfo.State}");
            //        if (running != null && args.InvocationStateInfo.State == PSInvocationState.Running)
            //        {
            //            running.Set();
            //        }
            //    };

            //    ps.BeginInvoke();
            //    logger.LogDebug($"WaitOne");
            //    running.WaitOne();
            //    Thread.Sleep(500);
            //    logger.LogDebug($"Disconnect");
            //    runspace.Disconnect();
            //}
            //running = null;
        }

        private static PSCredential GetCredential(IConfigurationRoot config)
        {
            var username = config.GetValue<string>("UserName");
            var secret = config.GetValue<string>("Password");
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(secret))
            {
                return null;
            }

            var password = new SecureString();
            foreach (var ch in secret.ToCharArray())
            {
                password.AppendChar(ch);
            }
            return new PSCredential(username, password);
        }
    }
}