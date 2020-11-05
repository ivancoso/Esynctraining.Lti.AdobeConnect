namespace Esynctraining.Core.Extensions
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    ///     The xml extensions.
    /// </summary>
    public static class XmlExtensions
    {
        #region Static Fields

        /// <summary>
        /// The known lists.
        /// </summary>
        private static List<string> KnownLists;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="knownLists">
        /// The known lists.
        /// </param>
        public static void Parse(dynamic parent, XElement node, List<string> knownLists = null)
        {
            if (knownLists != null)
            {
                KnownLists = knownLists;
            }

            IEnumerable<XElement> sorted = from XElement elt in node.Elements()
                                           orderby node.Elements(elt.Name.LocalName).Count() descending
                                           select elt;

            if (node.HasElements)
            {
                int nodeCount = node.Elements(sorted.First().Name.LocalName).Count();
                bool foundNode = false;
                if (KnownLists != null && KnownLists.Count > 0)
                {
                    foundNode = (from XElement el in node.Elements() where KnownLists.Contains(el.Name.LocalName) select el).Any();
                }

                if (nodeCount > 1 || foundNode)
                {
                    // At least one of the child elements is a list
                    var item = new ExpandoObject();
                    List<dynamic> list = null;
                    string elementName = string.Empty;
                    foreach (XElement element in sorted)
                    {
                        if (element.Name.LocalName != elementName)
                        {
                            list = new List<dynamic>();
                            elementName = element.Name.LocalName;
                        }

                        if (element.HasElements || (KnownLists != null && KnownLists.Contains(element.Name.LocalName)))
                        {
                            Parse(list, element);
                            AddProperty(item, element.Name.LocalName, list);
                        }
                        else
                        {
                            Parse(item, element);
                        }
                    }

                    foreach (XAttribute attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    var item = new ExpandoObject();

                    foreach (XAttribute attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    // element
                    foreach (XElement element in sorted)
                    {
                        Parse(item, element);
                    }

                    AddProperty(parent, node.Name.ToString(), item);
                }
            }
            else
            {
                if (node.HasAttributes)
                {
                    var item = new ExpandoObject();
                    foreach (XAttribute attribute in node.Attributes())
                    {
                        AddProperty(item, attribute.Name.ToString(), attribute.Value.Trim());
                    }

                    AddProperty(item, "value", node.Value.Trim());
                    AddProperty(parent, node.Name.ToString(), item);
                }
                else
                {
                    AddProperty(parent, node.Name.ToString(), node.Value.Trim());    
                }
            }
        }

        /// <summary>
        /// The to dynamic.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="dynamic"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", 
            Justification = "Reviewed. Suppression is OK here.")]
        public static dynamic ToDynamic(this string xml)
        {
            XDocument xDoc = XDocument.Parse(xml);
            dynamic root = new ExpandoObject();
            Parse(root, xDoc.Elements().First());
            return root;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add property.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        private static void AddProperty(dynamic parent, string name, object value)
        {
            if (parent is List<dynamic>)
            {
                (parent as List<dynamic>).Add(value);
            }
            else
            {
                (parent as IDictionary<string, object>)[name] = value;
            }
        }

        #endregion
    }
}