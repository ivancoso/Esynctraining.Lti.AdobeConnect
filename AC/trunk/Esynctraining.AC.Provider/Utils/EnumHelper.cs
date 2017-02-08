using System;

namespace Esynctraining.AC.Provider.Utils
{
    public static class EnumHelper
    {
        public static string GetACEnum(this Enum shortcut)
        {
            return shortcut.ToString().Replace("_", "-");
        }
    }
}