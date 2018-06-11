using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

namespace Esynctraining.Lti.Zoom.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        // These OAuth parameters are required
        private static readonly string[] RequiredOauthParameters =
        {
            OAuthConstants.ConsumerKeyParameter,
            OAuthConstants.NonceParameter,
            OAuthConstants.SignatureParameter,
            OAuthConstants.SignatureMethodParameter,
            OAuthConstants.TimestampParameter,
            OAuthConstants.VersionParameter
        };

        // These LTI launch parameters are required
        private static readonly string[] RequiredBasicLaunchParameters =
        {
            LtiConstants.LtiMessageTypeParameter,
            LtiConstants.LtiVersionParameter,
            LtiConstants.ResourceLinkIdParameter,
//                LtiConstants.UserIdParameter
        };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemLaunchParameters =
        {
            LtiConstants.AcceptMediaTypesParameter,
            LtiConstants.AcceptPresentationDocumentTargetsParameter,
            LtiConstants.ContentItemReturnUrlParameter,
            LtiConstants.LtiMessageTypeParameter,
            LtiConstants.LtiVersionParameter,
            LtiConstants.UserIdParameter
        };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemResponseParameters =
        {
            LtiConstants.ContentItemPlacementParameter
        };

        public static string GenerateOAuthSignature(this HttpRequest request, string consumerSecret)
        {
            OAuthRequest oAuthRequest = new OAuthRequest();
            oAuthRequest.HttpMethod = request.Method;
            oAuthRequest.Url = request.GetUri();
            //oAuthRequest.Parameters. = request.UnvalidatedParameters();
            return oAuthRequest.GenerateSignature(consumerSecret);
            //return OAuthUtility.GenerateSignature(request.HttpMethod, request.Url, request.UnvalidatedParameters(), consumerSecret);
        }

        //        public static string GenerateOAuthSignatureBase(this HttpRequestBase request)
        //        {
        //            return OAuthUtility.GenerateSignatureBase(request.HttpMethod, request.Url, request.UnvalidatedParameters());
        //        }

        //public static LisContextType? GetLisContextType(this HttpRequest request, string key)
        //{
        //    var stringValue = request.GetUnvalidatedString(key);
        //    LisContextType contextTypeEnum;
        //    return Enum.TryParse(stringValue, out contextTypeEnum)
        //        ? contextTypeEnum
        //        : default(LisContextType?);
        //}

        //public static DocumentTarget? GetPresentationTarget(this HttpRequest request, string key)
        //{
        //    var stringValue = request.GetUnvalidatedString(key);
        //    DocumentTarget presentationTargetEnum;
        //    return Enum.TryParse(stringValue, out presentationTargetEnum)
        //        ? presentationTargetEnum
        //        : default(DocumentTarget?);
        //}

        //private static string GetUnvalidatedString(this HttpRequest request, string key)
        //{
        //    return request.Unvalidated[key];
        //}

        /// <summary>
        /// Get a value indicating whether the current request is authenticated
        /// using LTI.
        /// </summary>
        public static bool IsAuthenticatedWithLti(this HttpRequest request)
        {
            var messageType = request.Form[LtiConstants.LtiMessageTypeParameter].Count == 0
                ? string.Empty
                : request.Form[LtiConstants.LtiMessageTypeParameter].ToString();
            //var messageType = request[LtiConstants.LtiMessageTypeParameter] ?? string.Empty;
            return request.Method.Equals(WebRequestMethods.Http.Post)
                   && messageType.Equals(LtiConstants.BasicLaunchLtiMessageType, StringComparison.OrdinalIgnoreCase);
        }

        public static NameValueCollection UnvalidatedParameters(this HttpRequest request)
        {
            var parameters = new NameValueCollection();
            parameters.Add(HttpUtility.ParseQueryString(request.QueryString.Value));
            //parameters.Add(request.Form);
            return parameters;
        }

        /// <summary>
        /// Parse the HttpRequest and return a filled in LtiRequest.
        /// </summary>
        /// <param name="request">The HttpRequest to parse.</param>
        /// <returns>An LtiInboundRequest filled in with the OAuth and LTI parameters
        /// sent by the consumer.</returns>
        public static void CheckForRequiredLtiParameters(this HttpRequest request)
        {
            if (!request.IsAuthenticatedWithLti())
            {
                throw new LtiException("Invalid request method or LTI message type.");
            }

            // Make sure the request contains all the required parameters
            request.RequireAllOf(RequiredOauthParameters);
            switch (request.Form[LtiConstants.LtiMessageTypeParameter].ToString())
            {
                case LtiConstants.BasicLaunchLtiMessageType:
                {
                    request.RequireAllOf(RequiredBasicLaunchParameters);
                    break;
                }
                case LtiConstants.ContentItemSelectionRequestLtiMessageType:
                {
                    request.RequireAllOf(RequiredContentItemLaunchParameters);
                    break;
                }
                case "ContentItemSelectionResponse":
                {
                    request.RequireAllOf(RequiredContentItemResponseParameters);
                    break;
                }
            }
        }

        public static void RequireAllOf(this HttpRequest request, IEnumerable<string> parameters)
        {
            var missing = parameters.Where(parameter => string.IsNullOrEmpty(request.Form[parameter].ToString())).ToList();

            if (missing.Count > 0)
            {
                throw new LtiException($"Missing parameters: {string.Join(", ", missing.ToArray())}");
            }
        }

    }
}

