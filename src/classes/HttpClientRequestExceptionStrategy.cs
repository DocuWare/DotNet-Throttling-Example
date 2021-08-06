using System;
using System.Collections.Generic;
using System.Linq;
using DocuWare.Services.Http.Client;
using DotNetThrottlingExample.interfaces;
using DotNetThrottlingExample.models;

namespace DotNetThrottlingExample.classes
{
    class HttpClientRequestExceptionStrategy: IThrottlingStrategy
    {
        // Contains the app settings with all necessary configurations for the strategy.
        private readonly AppSettingsModel _appSettings;

        // Is the default waiting time that will be used.
        private const int DefaultRetryAfterSecondsConst = 300;

        // Contains the seconds to retry after from the configuration in the app settings or uses the default const value of 300 seconds for it.
        public int DefaultRetryAfterSeconds =>
            _appSettings?.DocuWarePlatform?.ThrottlingDefaultRetryAfterSeconds ?? DefaultRetryAfterSecondsConst;

        /// <summary>
        /// Creates a strategy to get a TimeSpan from seconds which are stored in a throttling configuration or the default waiting time.
        /// </summary>
        /// <param name="appSettings">Contains the app settings with the needed configuration.</param>
        public HttpClientRequestExceptionStrategy(AppSettingsModel appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            _appSettings = appSettings;
        }

        /// <summary>
        /// Returns a TimeSpan for the retry waiting period.
        /// </summary>
        /// <returns>Is a TimeSpan with the waiting time for retry.</returns>
        public TimeSpan GetRetryAfterTimeSpan(Exception exception)
        {
            if (exception is HttpClientRequestException typedException)
            {
                // Get the needed throttling config by endpoint uri and used method.
                DocuWareThrottlingConfigurationModel docuWareThrottlingConfiguration =
                    GetThrottlingConfiguration(typedException.Uri.ToString(), typedException.Method.ToString());

                // Return the default retry after in form of a TimeSpan when no throttling config was found.
                if (docuWareThrottlingConfiguration == null)
                {
                    return TimeSpan.FromSeconds(DefaultRetryAfterSeconds);
                }

                // Return the waiting time from the throttling config as a TimeSpan.
                return TimeSpan.FromSeconds(docuWareThrottlingConfiguration.SecondsToWait);
            }

            // Return default retry after value in form of a TimeSpan.
            return TimeSpan.FromSeconds(DefaultRetryAfterSeconds);
        }

        /// <summary>
        /// Return a the first throttling configuration filtered by uri and method.
        /// </summary>
        /// <param name="uri">Is the uri to the endpoint.</param>
        /// <param name="method">Is the used method for the endpoint.</param>
        /// <returns>Contains the throttling configuration filtered by uri and method.</returns>
        private DocuWareThrottlingConfigurationModel GetThrottlingConfiguration(string uri, string method)
        {
            // Return null if there are both filter strings not able to get.
            if (string.IsNullOrWhiteSpace(uri) || string.IsNullOrWhiteSpace(method))
            {
                return null;
            }

            // Return the throttling config by endpoint uri and used method, null gets returned when it is not found.
            return _appSettings.DocuWarePlatform?.Throttling?.FirstOrDefault(tc =>
                uri.Contains(tc.Endpoint) && tc.Method == method);
        }
    }
}
