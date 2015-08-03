using System;

namespace Esynctraining.AC.Provider.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class SkipDuringUpdateAttribute : Attribute
    {
    }

}
