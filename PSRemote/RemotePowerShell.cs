using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PoshCode
{
    public static class RemotePowerShell
    {
        public static PowerShell Create(string computerName = "localhost", PSCredential credential = null, string name = null, string scheme = "http", int port = 5985, string appName = "wsman", string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell")
        {
            var connection = new WSManConnectionInfo(scheme, computerName, port, appName, shellUri, credential);
            var runspace = RunspaceFactory.CreateRunspace(connection);

            if (!string.IsNullOrEmpty(name))
            {
                runspace.Name = name;
            }

            runspace.Open();
            var powershell = PowerShell.Create();
            powershell.Runspace = runspace;

            return powershell;
        }

        public static PowerShell Create(string computerName = "localhost", string shellName = "Microsoft.PowerShell", PSCredential credential = null, string name = null, bool useSsl = false, int port = 5985, string appName = "wsman")
        {
            return Create(computerName, credential, name, useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, port, appName, $"http://schemas.microsoft.com/powershell/{shellName}");
        }

        public static Runspace[] Get(string computerName = "localhost", PSCredential credential = null, string scheme = "http", int port = 5985, string appName = "wsman", string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell")
        {
            var connection = new WSManConnectionInfo(scheme, computerName, port, appName, shellUri, credential);
            return Runspace.GetRunspaces(connection);
        }
    }
}
