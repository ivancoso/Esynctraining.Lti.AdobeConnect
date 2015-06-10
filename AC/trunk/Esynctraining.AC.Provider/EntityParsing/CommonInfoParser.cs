using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    public static class CommonInfoParser
    {
        public static CommonInfo Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            try
            {
                return new CommonInfo()
                {
                    AccountUrl = xml.SelectSingleNodeValue("host/text()")
                };
            }
            catch (Exception ex)
            {
                TraceTool.TraceException(ex);
            }

            return null;
        }
    }
}
