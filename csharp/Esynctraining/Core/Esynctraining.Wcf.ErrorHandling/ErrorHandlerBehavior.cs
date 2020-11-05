namespace Esynctraining.Wcf.ErrorHandling
{
    using System;
    using System.ServiceModel.Configuration;

    public sealed class ErrorHandlerBehavior : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(ErrorServiceBehavior); }
        }


        protected override object CreateBehavior()
        {
            return new ErrorServiceBehavior();
        }

    }

}