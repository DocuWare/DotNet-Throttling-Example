using System;
using System.Collections.Generic;
using System.Text;
using DocuWare.Services.Http.Client;
using DotNetThrottlingExample.interfaces;
using DotNetThrottlingExample.models;

namespace DotNetThrottlingExample.classes
{
    public class ThrottlingStrategyFactory
    {
        // Contains all known throttling exceptions and the strategy for that.
        private readonly IDictionary<Type, IThrottlingStrategy>
            _strategies;

        /// <summary>
        /// Creates a throttling strategy factory to be able to choose the right strategy.
        /// </summary>
        /// <param name="appSettings">Contains the whole app settings.</param>
        public ThrottlingStrategyFactory(AppSettingsModel appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            // Define the known throttling exceptions and their strategy.
            _strategies = new Dictionary<Type, IThrottlingStrategy>()
            {
                { typeof(HttpClientRequestException), new HttpClientRequestExceptionStrategy(appSettings)}
            };
        }

        /// <summary>
        /// Get the right strategy for the known throttling exception type or throws that exception if unknown.
        /// </summary>
        /// <param name="exception">Contains a exception which is used to choose the right strategy.</param>
        /// <returns>The right strategy for the exception type.</returns>
        public IThrottlingStrategy GetStrategy(Exception exception)
        {
            Type exceptionType = exception.GetType();

            // Return when the exception type is known the right strategy for it.
            if (_strategies.ContainsKey(exceptionType))
            {
                return _strategies[exceptionType];
            }

            throw exception;
        }
    }
}
