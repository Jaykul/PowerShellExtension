using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace PoshCode.PowerShell
{
    public static class RunspaceExtensions
    {

        public static System.Management.Automation.PowerShell CreatePipeline(this Runspace runspace, string script, IDictionary parameters = null, Action<PSDataStreams> psDataStreamAction = null)
        {

            if (script == null)
            {
                throw new ArgumentOutOfRangeException("script");
            }

            if (runspace == null)
            {
                throw new ArgumentOutOfRangeException("runspace");
            }

            var ps = System.Management.Automation.PowerShell.Create();
            ps.Runspace = runspace;
            ps.AddScript(script);

            if (parameters != null && parameters.Count > 0) ps.AddParameters(parameters);
            psDataStreamAction?.Invoke(ps.Streams);

            return ps;
        }




        public static Task<PSDataCollection<PSObject>> InvokeAsync(this Runspace runspace, PSCommand command, Action<PSDataStreams> psDataStreamAction = null)
        {
            if (command == null)
            {
                throw new ArgumentOutOfRangeException("command");
            }

            if (runspace == null)
            {
                throw new ArgumentOutOfRangeException("runspace");
            }

            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                ps.Commands = command;
                ps.Runspace = runspace;

                psDataStreamAction?.Invoke(ps.Streams);

                return ps.InvokeAsync();
            }
        }

        public static Task<PSDataCollection<PSObject>> InvokeAsync(this Runspace runspace, string script, IDictionary scriptParameters = null, Action<PSDataStreams> psDataStreamAction = null)
        {

            if (script == null)
            {
                throw new ArgumentOutOfRangeException("script");
            }

            if (runspace == null)
            {
                throw new ArgumentOutOfRangeException("runspace");
            }

            using (System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript(script);

                if (scriptParameters != null && scriptParameters.Count > 0) ps.AddParameters(scriptParameters);

                psDataStreamAction?.Invoke(ps.Streams);

                return ps.InvokeAsync();
            }
        }


        public static Collection<T> Invoke<T>(this Runspace runspace, PSCommand command, Action<PSDataStreams> psDataStreamAction = null)
        {
            if (command == null)
            {
                throw new ArgumentOutOfRangeException("command");
            }

            if (runspace == null)
            {
                throw new ArgumentOutOfRangeException("runspace");
            }

            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                ps.Commands = command;
                ps.Runspace = runspace;

                psDataStreamAction?.Invoke(ps.Streams);

                return ps.Invoke<T>();
            }
        }

        public static Collection<T> Invoke<T>(this Runspace runspace, string script, IDictionary parameters = null, Action<PSDataStreams> psDataStreamAction = null)
        {

            if (script == null)
            {
                throw new ArgumentOutOfRangeException("script");
            }

            if (runspace == null)
            {
                throw new ArgumentOutOfRangeException("runspace");
            }

            using (System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript(script);

                if (parameters != null && parameters.Count > 0) ps.AddParameters(parameters);

                psDataStreamAction?.Invoke(ps.Streams);

                return ps.Invoke<T>();
            }
        }

        public static Collection<PSObject> Invoke(this Runspace runspace, string script, IDictionary parameters = null, Action<PSDataStreams> psDataStreamAction = null)
        {

            if (script == null)
            {
                throw new ArgumentOutOfRangeException("script");
            }

            if (runspace == null)
            {
                throw new ArgumentOutOfRangeException("runspace");
            }

            using (System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create())
            {
                ps.Runspace = runspace;
                ps.AddScript(script);

                if (parameters != null && parameters.Count > 0) ps.AddParameters(parameters);

                psDataStreamAction?.Invoke(ps.Streams);

                return ps.Invoke();
            }
        }

    }
}
