using System;
using System.Linq.Expressions;

namespace Esynctraining.ImportExport.Excel.Import
{
    public sealed class ImportPropertyDescriptor<TContainer> : IImportPropertyDescriptor
    {
        private readonly string _propertyName;
        private readonly Func<TContainer, object> _getValue;
        private readonly Action<TContainer, object> _setValue;


        public string Name { get { return _propertyName; } }

        public string DisplayName { get; private set; }

        // TODO: we can set it via [Required] attribute. Is it OK for import??
        public bool Required { get; private set; }

        public int ExcelFileColumnIndex { get; set; }


        public ImportPropertyDescriptor(Expression<Func<TContainer, object>> property, string displayName, bool required)
        {
            //Check.Argument.IsNotNull(property, "property");
            //Check.Argument.IsNotEmpty(displayName, "displayName");

            _getValue = property.Compile();
            _setValue = ExpressionUtility.GetPropertySetter<TContainer>(property).Compile();
            _propertyName = ExpressionUtility.GetPropertyName<TContainer>(property);

            DisplayName = displayName;
            // TRICK: default value
            // TODO: use any const ImportBatchControllerHelper.NotMarkedExcelFileColumnIndex
            ExcelFileColumnIndex = -1;
            Required = required;
        }


        public void SetValue(TContainer entity, object value)
        {
            // TODO: check on null?

            //TODO: check boxing!!!
            _setValue(entity, value);
        }

        public object GetValue(TContainer entity)
        {
            // TODO: check on null?

            return _getValue(entity);
        }

    }

}
