using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;

namespace PoshCode.PowerShell
{
    public static class RemotePowerShell
    {
        public static Runspace Connect(string computerName = "localhost", PSCredential credential = null, string name = null, string scheme = "http", int port = 5985, string appName = "wsman", string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell")
        {
            var connection = new WSManConnectionInfo(scheme, computerName, port, appName, shellUri, credential);
            connection.IdleTimeout = 3 * 1000 * 60; // three minutes
            var runspace = RunspaceFactory.CreateRunspace(connection);

            if (!string.IsNullOrEmpty(name))
            {
                runspace.Name = name;
            }
            runspace.Open();

            return runspace;
        }

        public static Runspace Connect(string computerName = "localhost", string shellName = "Microsoft.PowerShell", PSCredential credential = null, string name = null, bool useSsl = false, int port = 5985, string appName = "wsman")
        {
            return Connect(computerName, credential, name, useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, port, appName, $"http://schemas.microsoft.com/powershell/{shellName}");
        }

        public static System.Management.Automation.PowerShell Create(string computerName = "localhost", PSCredential credential = null, string name = null, string scheme = "http", int port = 5985, string appName = "wsman", string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell")
        {
            var connection = new WSManConnectionInfo(scheme, computerName, port, appName, shellUri, credential);
            var runspace = RunspaceFactory.CreateRunspace(connection);

            if (!string.IsNullOrEmpty(name))
            {
                runspace.Name = name;
            }

            runspace.Open();
            var powershell = System.Management.Automation.PowerShell.Create();
            powershell.Runspace = runspace;

            return powershell;
        }

        public static System.Management.Automation.PowerShell Create(string computerName = "localhost", string shellName = "Microsoft.PowerShell", PSCredential credential = null, string name = null, bool useSsl = false, int port = 5985, string appName = "wsman")
        {
            return Create(computerName, credential, name, useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, port, appName, $"http://schemas.microsoft.com/powershell/{shellName}");
        }

        public static Runspace[] Get(string computerName = "localhost", PSCredential credential = null, string scheme = "http", int port = 5985, string appName = "wsman", string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell")
        {
            var connection = new WSManConnectionInfo(scheme, computerName, port, appName, shellUri, credential);
            return Runspace.GetRunspaces(connection);
        }


        /// <summary>
        /// Set it and forget it, when you don't need the results
        /// </summary>
        public static Task<PSDataCollection<PSObject>> RunScript(string script, IDictionary parameters = null, string computerName = "localhost", PSCredential credential = null)
        {
            var runspace = Connect(computerName, credential);
            var task = runspace.InvokeAsync(script, parameters);
            runspace.Disconnect();
            return task;
        }

    }
}
