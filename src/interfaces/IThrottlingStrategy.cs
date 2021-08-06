using System;
using System.Collections.Generic;

namespace DotNetThrottlingExample.interfaces
{
    /// <summary>
    /// A interface that is used for get the right throttling retry after TimeSpan for a polly policy
    /// </summary>
    public interface IThrottlingStrategy
    {
        TimeSpan GetRetryAfterTimeSpan(Exception exception);
    }
}