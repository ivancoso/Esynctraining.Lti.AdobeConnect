﻿using System.Configuration;

namespace Esynctraining.AdobeConnect.Api.Configuration
{
    public static class Config
    {
        // TODO: implement generic method with 'name' parameter
        public static IAccessSettings GetConfig()
        {
            var result = (IAccessSettings)ConfigurationManager.GetSection("adobeConnectAccess");

            if (result == null)
                throw new ConfigurationErrorsException("Configuration section 'adobeConnectAccess' was not found.");

            return result;
        }
        
    }

}
