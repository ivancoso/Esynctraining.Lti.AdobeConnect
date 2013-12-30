namespace EdugameCloud.MVC.ModelBinders
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// The time span model binder.
    /// </summary>
    public class TimeSpanModelBinder : BaseModelBinder<TimeSpan, TimeSpan?>
    {
        #region Public Methods and Operators

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="controllerContext">
        /// The controller context.
        /// </param>
        /// <param name="bindingContext">
        /// The binding context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object Bind(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            IValueProvider valueProvider = bindingContext.ValueProvider;
            string value = valueProvider.GetValueByName(bindingContext.ModelName);
            string[] hoursMins = value.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int hours = 0;
            int mins = 0;
            if (hoursMins.Length > 0)
            {
                int.TryParse(hoursMins.FirstOrDefault(), out hours);
            }

            if (hoursMins.Length > 1)
            {
                int.TryParse(hoursMins.ElementAtOrDefault(1), out mins);
            }

            return new TimeSpan(hours, mins, 0);
        }

        #endregion
    }
}