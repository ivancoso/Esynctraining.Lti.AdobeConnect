namespace Esynctraining.Core.Weborb.Business.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    using global::Weborb.Client;

    /// <summary>
    /// The web orb client model.
    /// </summary>
    /// <typeparam name="T">
    /// Any service response
    /// </typeparam>
    public class WebOrbClientWrapper<T>
    {
        /// <summary>
        /// The gate way.
        /// </summary>
        private readonly string gateway;

        /// <summary>
        /// The class.
        /// </summary>
        private readonly string @class;

        /// <summary>
        /// The method.
        /// </summary>
        private readonly string method;

        /// <summary>
        /// The DTO converter.
        /// </summary>
        private readonly Func<Hashtable, T> dtoConverter;

        /// <summary>
        /// The destination.
        /// </summary>
        private readonly string destination;

        /// <summary>
        /// The executed call back.
        /// </summary>
        private readonly AutoResetEvent executedCallBack = new AutoResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="WebOrbClientWrapper{T}"/> class.
        /// </summary>
        /// <param name="gateway">
        /// The gateway.
        /// </param>
        /// <param name="class">
        /// The class.
        /// </param>
        /// <param name="method">
        /// The method.
        /// </param>
        /// <param name="dtoConverter">
        /// The DTO Converter.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        public WebOrbClientWrapper(string gateway, string @class, string method, Func<Hashtable, T> dtoConverter, string destination = null)
        {
            this.gateway = gateway;
            this.@class = @class;
            this.method = method;
            this.dtoConverter = dtoConverter;
            this.destination = destination;
        }

        /// <summary>
        /// Gets or sets the res.
        /// </summary>
        private Domain.Contracts.ServiceResponse<T> result { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        private Fault fault { get; set; }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Exception when error is returned
        /// </exception>
        public Domain.Contracts.ServiceResponse<T> Invoke(params object[] args)
        {
            var lookupService = new WeborbClient(this.gateway, this.destination ?? "GenericDestination");

            lookupService.Invoke(this.@class, this.method, args, new Responder<ServiceResponse>(this.GotResult, this.GotError));
            this.executedCallBack.WaitOne();
            if (this.fault != null)
            {
                throw new ApplicationException(string.Format("Message: " + this.fault.Message + ", Details: " + this.fault.Detail));
            }

            return this.result;
        }

        /// <summary>
        /// The got result.
        /// </summary>
        /// <param name="res">
        /// The res.
        /// </param>
        private void GotResult(ServiceResponse res)
        {
            this.result = this.Convert(res);
            this.executedCallBack.Set();
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="res">
        /// The res.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        private Domain.Contracts.ServiceResponse<T> Convert(ServiceResponse res)
        {
            var response = new Domain.Contracts.ServiceResponse<T>();
            if (this.dtoConverter != null)
            {
                if (res.@object != null)
                {
                    response.@object = this.dtoConverter(res.@object);
                }

                if (res.@objects != null)
                {
                    response.@objects = res.objects.Select(x => this.dtoConverter(x)).ToList();
                }
            }

            if (res.error != null)
            {
                response.SetError(this.ConvertError(res.error.ToExpando()));
            }

            return response;
        }

        /// <summary>
        /// The got error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        private void GotError(Fault error)
        {
            this.fault = error;
            this.executedCallBack.Set();
        }

        /// <summary>
        /// The convert error.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="Error"/>.
        /// </returns>
        private Error ConvertError(dynamic obj)
        {
            var objEx = (IDictionary<string, object>)obj;
            var error = new Error
            {
                errorCode = obj.errorCode,
                errorDetail = obj.errorDetail,
                errorMessage = obj.errorMessage,
                errorType = obj.errorType,
            };

            if (objEx.ContainsKey("faultEntities"))
            {
                error.faultEntities = new List<string>();
            }

            return error;

        }
    }
}