using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetThrottlingExample.models
{
   public class DocuWarePlatformConfigurationModel
    {
        public int ThrottlingDefaultRetryAfterSeconds { get; set; }
        public IEnumerable<DocuWareThrottlingConfigurationModel> Throttling { get; set; }
    }
}
