using System;
using System.Net;
using System.Threading.Tasks;
using DocuWare.Platform.ServerClient;
using DocuWare.Services.Http.Client;
using DotNetThrottlingExample.classes;
using DotNetThrottlingExample.interfaces;
using DotNetThrottlingExample.models;
using Microsoft.Extensions.Configuration;
using Polly;

namespace DotNetThrottlingExample
{
    class Program
    {
        // Stores the configuration which includes AppSettings, command line arguments and environment variables
        public static IConfigurationRoot configuration;

        // Contains the Factory to get the correct throttling strategy.
        public static ThrottlingStrategyFactory ThrottlingStrategyFactory;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello and Welcome to DocuWares platform throttling example based on polly!");

            string serverName = @"YOUR-COMPANY-NAME.docuware.cloud";
            string serverAddress = @"https://" + serverName + @"/DocuWare/Platform/";
            string userName = "USERNAME";
            string userPassword = "PASSWORD";

            // Build a configuration which includes everything needed to run the application
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            // Get the app settings from the configuration
            AppSettingsModel appSettings =
                configuration.Get<AppSettingsModel>();

            // Create a throttling strategy factory which gets all app settings
            ThrottlingStrategyFactory = new ThrottlingStrategyFactory(appSettings);

            // Create a polly policy which reacts to the DocuWare platform throttling
            Policy throttlingPolicy = getPollyThrottlingPolicy();

            // Login example which forces the platform to throttle
            throttlingPolicy.Execute(() => loginDocuWareWithUserNameAndPassword(serverAddress, userName, userPassword));
        }

        /// <summary>
        /// Get a polly throttling policy.
        /// </summary>
        /// <returns>A policy which reacts at throttling exceptions.</returns>
        static Policy getPollyThrottlingPolicy()
        {
            // Create and return a polly throttling policy.
            return Policy
                .Handle<HttpClientRequestException>(httpClientRequestException => httpClientRequestException.StatusCode == HttpStatusCode.TooManyRequests)
                //.Or<Exception>() with that you are able to add multiple exceptions.
                .WaitAndRetry(retryCount: 5,
                    sleepDurationProvider: (retryCount, exception, context) =>
                    {
                        // Get the strategy relied on the exception type.
                        IThrottlingStrategy strategy = ThrottlingStrategyFactory.GetStrategy(exception);

                        // Retrieve retry after value as a TimeSpan from the chosen strategy.
                        TimeSpan retryAfterTimeSpan = strategy.GetRetryAfterTimeSpan(exception);
                        return retryAfterTimeSpan;
                    },
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine("Let's retry it for the {0} time after waiting for {1}", retryCount,
                            timeSpan.ToString());
                    }
                );
        }

        /// <summary>
        /// Login to DocuWare system and forces it to throttle
        /// </summary>
        /// <param name="serverAddress">The DocuWare server platform endpoint.</param>
        /// <param name="userName">The used DocuWare user.</param>
        /// <param name="userPassword">The user password.</param>
        private static void loginDocuWareWithUserNameAndPassword(string serverAddress, string userName, string userPassword)
        {
            for (int i = 0; i < 2000; i++)
            {
                // Open a connection to the DocuWare system
                ServiceConnection serviceConnection = ServiceConnection.Create(new Uri(serverAddress),
                    userName,
                    userPassword);

                Console.WriteLine(string.Format("Iteration: {0} Organization count: {1}", i, serviceConnection.Organizations.Length));

                // Free used connection
                serviceConnection.Disconnect();
            }
        }
    }
}
