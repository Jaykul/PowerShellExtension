using System;
using System.Management.Automation;
using System.Threading.Tasks;

namespace PoshCode
{
    public static class PowerShellExtensions
    {
        public async static Task<PSDataCollection<PSObject>> InvokeAsync(this PowerShell shell)
        {
            return await Task.Factory.FromAsync(shell.BeginInvoke(), shell.EndInvoke);
        }

        public async static Task<PSDataCollection<PSObject>> InvokeAsync<T1>(this PowerShell shell, PSDataCollection<T1> input = null)
        {
            return await Task.Factory.FromAsync(shell.BeginInvoke(input), shell.EndInvoke);
        }

        public async static Task<PSDataCollection<T2>> InvokeAsync<T1, T2>(this PowerShell shell, PSDataCollection<T1> input = null, PSInvocationSettings invocationSettings = null)
        {
            var output = new PSDataCollection<T2>();
            object state = "none";

            await Task.Factory.FromAsync(
                (t1, t2, callback, obj) => shell.BeginInvoke(t1, t2, invocationSettings ?? new PSInvocationSettings(), callback, obj),
                (Action<IAsyncResult>)((a) => shell.EndInvoke(a)), input, output, state);

            return output;
        }

        public async static Task StopAsync(this PowerShell shell)
        {
            object state = "state";
            await Task.Factory.FromAsync(shell.BeginStop, shell.EndStop, state);
        }
    }
}