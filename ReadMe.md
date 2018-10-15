# PowerShellExtension

A set of helper classes and extension methods for PowerShell and PS Remoting.

I just wrote these this afternoon to answer for myself the question of whether it was hard, in C# to create a session, invoke a command, and then check on the session from another process later.

While I was at it, I included some `InvokeAsync` extension methods for PowerShell to wrap the Asynchronous Programming Model pattern exposed by PowerShell with an awaitable Task pattern method.