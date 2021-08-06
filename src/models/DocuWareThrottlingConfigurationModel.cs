using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetThrottlingExample.models
{
    public class DocuWareThrottlingConfigurationModel
    {
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public int SecondsToWait { get; set; }
    }
}
