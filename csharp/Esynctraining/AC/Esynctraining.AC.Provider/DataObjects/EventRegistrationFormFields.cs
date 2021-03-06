﻿using System;
using System.Collections.Generic;

namespace Esynctraining.AC.Provider.DataObjects
{
    public class EventRegistrationFormFields
    {
        public string ScoId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerifyPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Dictionary<string,string> AdditionalFields { get;set; }
    }

    public class SaveEventResponse
    {
        public string EventScoId { get; set; }

        public string EventTitle { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class SaveEventFields
    {
        public SaveEventFields(UserCredentials adminUser, string name, DateTime startDate, DateTime eventEndDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Non-empty value expected", nameof(name));
            if (startDate == DateTime.MinValue || startDate == DateTime.MaxValue)
                throw new ArgumentOutOfRangeException($"Invalid Start Event value { startDate }", nameof(startDate));
            if (eventEndDate == DateTime.MinValue || eventEndDate == DateTime.MaxValue)
                throw new ArgumentOutOfRangeException($"Invalid End Event value { eventEndDate }", nameof(eventEndDate));

            AdminUser = adminUser ?? throw new ArgumentNullException(nameof(adminUser));

            Name = name;
            StartDate = startDate;
            EndDate = eventEndDate;

            EventTemplateId = string.Empty; //will be set later in code (on 1st get with redirect)
            TimeZoneId = 4;
            EventType = "meeting";
            OwnerPermissionId = "host";
            EventCategory = "live";
            LoggedInAccess = "view";
            CatalogView = "remove";
            DefaultCatalogView = "view";
            ShowInCatalog = true;
            DefaultRegistrationType = "advance";
            Lang = "en";
            UrlPath = string.Empty;
            Description = string.Empty;
            EventInfo = string.Empty;
        }

        public UserCredentials AdminUser { get; set; }
        public string Name { get; set; }
        public string EventTemplateId { get; set; }
        public string Description { get; set; }
        public string EventCategory { get; set; }
        public string EventType { get; set; }
        public string ListScoId { get; set; }
        public string FolderScoId { get; set; }
        public string OwnerPermissionId { get; set; }
        public string UrlPath { get; set; }
        public string EventInfo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool PasswordByPass { get; set; }
        public bool ShowInCatalog { get; set; }
        public bool RegistrationLimitEnabled { get; set; }
        public string LoggedInAccess { get; set; }
        public string CatalogView { get; set; }
        public string DefaultCatalogView { get; set; }
        public string DefaultRegistrationType { get; set; }
        public int TimeZoneId { get; set; }
        public string Lang { get; set; }
        //public string Owasp { get; set; }
        public bool Tag11041 { get; set; }
        public bool Tag11038 { get; set; }
        public bool Tag11039 { get; set; }
        public bool Tag11040 { get; set; }
    }

    public class LoginAsOnUiContainer
    {
        public string BreezeCCookie { get; set; }
        public string BreezeSession { get; set; }
        public string Owasp { get; set; }
    }

    public class CreatingEventResponse
    {
        //public string Cookie { get; set; }
        public Uri CreateEventPostUrl { get; set; }
        //public string OwaspReturned { get; set; }
        public string ScoId { get; set; }
        public string EventTemplateId { get; set; }
    }

    public class CreatingEventContainer
    {
        public string Owasp { get; set; }
        public string EventScoId { get; set; }
        public string FolderScoId { get; set; }
        public Uri PostUrl { get; set; }
        public string AccountId { get; set; }
        public Dictionary<string, string> EventProperties { get; set; }
    }
}