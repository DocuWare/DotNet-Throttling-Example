using DocuWare.Platform.ServerClient;
using DocuWare.Platform.ServerClient.Exceptions;
using Polly;
using Polly.Retry;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetThrottlingExample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello and Welcome to DocuWares platform throttling example based on polly!");

            var serverName = @"YOUR-COMPANY-NAME.docuware.cloud";
            var serverAddress = @"https://" + serverName + @"/DocuWare/Platform/";
            var userName = "USERNAME";
            var userPassword = "PASSWORD";

            // Create a polly policy which reacts to the DocuWare platform throttling
            var throttlingPolicy = getPollyThrottlingPolicy();

            // Login example which forces the platform to throttle
            await throttlingPolicy.ExecuteAsync(() => loginDocuWareWithUserNameAndPassword(serverAddress, userName, userPassword));
        }

        /// <summary>
        ///     Get a async polly throttling policy with use of RetryAfterInterval from the
        ///     DocuWare.Platform.ServerClient.Exceptions.ClientThrottleException.
        /// </summary>
        /// <returns>A policy which reacts at throttling exceptions.</returns>
        private static AsyncRetryPolicy getPollyThrottlingPolicy()
        {
            // Create and return a polly throttling policy.
            return Policy.Handle<ClientThrottleException>(clientThrottleException => clientThrottleException.RetryAfterInterval >= TimeSpan.Zero)
                         .WaitAndRetryAsync(5,
                                            (retryCount, exception, context) =>
                                            {
                                                if (exception is ClientThrottleException usedException)
                                                {
                                                    // Check if the waiting time is negative.
                                                    if (usedException.RetryAfterInterval <= TimeSpan.Zero)
                                                    {
                                                        // Return a zero when it is negative.
                                                        return TimeSpan.Zero;
                                                    }

                                                    // Return the waiting time from the exception as a TimeSpan.
                                                    return usedException.RetryAfterInterval;
                                                }

                                                // Return a default waiting time of 60 seconds.
                                                return TimeSpan.FromSeconds(60);
                                            },
                                            async (exception, timeSpan, retryCount, context) =>
                                            {
                                                Console.WriteLine("Let's retry it for the {0} time after waiting for {1}", retryCount, timeSpan.ToString());
                                            });
        }

        /// <summary>
        ///     Login to DocuWare system and forces it to throttle. Attention this is only for test purposes!
        /// </summary>
        /// <param name="serverAddress">The DocuWare server platform endpoint.</param>
        /// <param name="userName">The used DocuWare user.</param>
        /// <param name="userPassword">The user password.</param>
        private static async Task loginDocuWareWithUserNameAndPassword(string serverAddress, string userName, string userPassword)
        {
            var intList = Enumerable.Range(1, 80);

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 20
            };

            await Parallel.ForEachAsync(intList,
                                        options,
                                        (i, cancellationToken) =>
                                        {
                                            var serviceConnection = ServiceConnection.Create(new Uri(serverAddress), userName, userPassword);

                                            Console.WriteLine("Iteration: {0} Organization count: {1}", i, serviceConnection.Organizations.Length);

                                            return ValueTask.CompletedTask;
                                        });
        }
    }
}