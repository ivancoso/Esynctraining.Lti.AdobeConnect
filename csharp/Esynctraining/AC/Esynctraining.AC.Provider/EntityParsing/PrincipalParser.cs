﻿namespace Esynctraining.AC.Provider.EntityParsing
{
    using System;
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Principal parser.
    /// </summary>
    public static class PrincipalParser
    {
        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>Principal object.</returns>
        public static Principal Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                if (xml != null && xml.NodeExists("//principal"))
                {
                    return Parse(xml.SelectSingleNode("//principal"));
                }
                return null;
            }

            try
            {
                return new Principal
                {
                    PrincipalId = xml.SelectAttributeValue("principal-id"),
                    AccountId = xml.SelectAttributeValue("account-id"),
                    IsHidden = xml.ParseAttributeBool("is-hidden"),
                    IsPrimary = xml.ParseAttributeBool("is-primary"),
                    HasChildren = xml.ParseAttributeBool("has-children"),
                    Type = xml.SelectAttributeValue("type"),
                    Login = xml.SelectSingleNodeValue("login/text()"),
                    Name = xml.SelectSingleNodeValue("name/text()"),
                    Email = xml.SelectSingleNodeValue("email/text()"),
                    DisplayId = xml.SelectSingleNodeValue("display-uid/text()"),
                    Description = xml.SelectSingleNodeValue("description/text()"),
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
