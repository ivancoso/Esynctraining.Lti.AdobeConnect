using System;

namespace Esynctraining.AC.Provider.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal sealed class SkipAttribute : Attribute
    {
    }

}
