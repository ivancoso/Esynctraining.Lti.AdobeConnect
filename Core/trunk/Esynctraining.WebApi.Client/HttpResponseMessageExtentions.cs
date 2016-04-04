using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace Esynctraining.WebApi.Client
{
    public static class HttpResponseMessageExtentions
    {
        public static ApiException CreateApiException(this HttpResponseMessage response)
        {
            var httpErrorObject = response.Content.ReadAsStringAsync().Result;
            
            ApiError deserializedErrorObject =
                JsonConvert.DeserializeObject<ApiError>(httpErrorObject);

            // Now wrap into an exception which best fullfills the needs of your application:
            //ApiException ex = (deserializedErrorObject != null)
            //    ? new ApiException(deserializedErrorObject.ToString(), response, deserializedErrorObject)
            //    : new ApiException(response, deserializedErrorObject);

            string responseUri = response.RequestMessage.RequestUri.ToString();
            ApiException ex = (deserializedErrorObject != null)
                ? new ApiException(responseUri, response, deserializedErrorObject)
                : new ApiException(responseUri, response, deserializedErrorObject);

            // Sometimes, there may be Model Errors:
            if (deserializedErrorObject.modelState != null)
            {
                var errors =
                    deserializedErrorObject.modelState
                                            .Select(kvp => string.Join(". ", kvp.Value));
                for (int i = 0; i < errors.Count(); i++)
                {
                    // Wrap the errors up into the base Exception.Data Dictionary:
                    ex.Data.Add(i, errors.ElementAt(i));
                }
            }
            // Othertimes, there may not be Model Errors:
            else
            {
                var error =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);
                foreach (var kvp in error)
                {
                    // Wrap the errors up into the base Exception.Data Dictionary:
                    ex.Data.Add(kvp.Key, kvp.Value);
                }
            }
            return ex;
        }

    }

}
