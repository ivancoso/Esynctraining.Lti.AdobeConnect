using System;

namespace Esynctraining.AC.Provider.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class SkipDuringUpdateAttribute : Attribute
    {
    }
}
