﻿
// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591
#region T4

namespace EdugameCloud.MVC.Resources {
using System.Diagnostics;
using System.CodeDom.Compiler;
using Esynctraining.Core.Providers;
using Esynctraining.Core.Utils;

	[GeneratedCode("T4MVC", "2.0"), DebuggerNonUserCode]
	public static class WebResources
	{
	   private static IResourceProvider ResourceProvider 
	   {
	      get 
		  {   
		      return LazyNested.Instance;
		  }
       }
		  
	 
		public static class Common 
	    { 
			public static string AppEmail 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("AppEmail", "Common");
			   }
		    }
		
			public static string AppEmailName 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("AppEmailName", "Common");
			   }
		    }
		
			public static string ApplicationName 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ApplicationName", "Common");
			   }
		    }
		
			public static string CompanyName 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("CompanyName", "Common");
			   }
		    }
		
			public static string DateFormatString 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("DateFormatString", "Common");
			   }
		    }
		
			public static string EmailAlreadyExists 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("EmailAlreadyExists", "Common");
			   }
		    }
		
			public static string EmailInvalid 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("EmailInvalid", "Common");
			   }
		    }
		
			public static string EmailValidationRegexp 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("EmailValidationRegexp", "Common");
			   }
		    }
		
			public static string PageTitlePrefix 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("PageTitlePrefix", "Common");
			   }
		    }
		
			public static string XSDNamespaceName 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("XSDNamespaceName", "Common");
			   }
		    }
		}
	 
		public static class Emails 
	    { 
			public static string ActivationSubject 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ActivationSubject", "Emails");
			   }
		    }
		
			public static string ChangePasswordSubject 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ChangePasswordSubject", "Emails");
			   }
		    }
		
			public static string LicenseUpgradeRequested 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("LicenseUpgradeRequested", "Emails");
			   }
		    }
		
			public static string TrialSubject 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("TrialSubject", "Emails");
			   }
		    }
		
			public static string UserCreatedSubject 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("UserCreatedSubject", "Emails");
			   }
		    }
		
			public static string UserDeletedSubject 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("UserDeletedSubject", "Emails");
			   }
		    }
		
			public static string UserEmailEditedSubject 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("UserEmailEditedSubject", "Emails");
			   }
		    }
		}
	 
		public static class Errors 
	    { 
			public static string AccessDeniedException 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("AccessDeniedException", "Errors");
			   }
		    }
		
			public static string DeletedObjectMessage 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("DeletedObjectMessage", "Errors");
			   }
		    }
		
			public static string EmailErrorMessage 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("EmailErrorMessage", "Errors");
			   }
		    }
		
			public static string EmailErrorMessageWithoutRetry 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("EmailErrorMessageWithoutRetry", "Errors");
			   }
		    }
		
			public static string EntityNotFoundException 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("EntityNotFoundException", "Errors");
			   }
		    }
		
			public static string FieldMustBeNumeric 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("FieldMustBeNumeric", "Errors");
			   }
		    }
		
			public static string PropertyValueInvalid 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("PropertyValueInvalid", "Errors");
			   }
		    }
		
			public static string StaleObjectMessage 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("StaleObjectMessage", "Errors");
			   }
		    }
		}
	 
		public static class ImportUsers 
	    { 
			public static string Company 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Company", "ImportUsers");
			   }
		    }
		
			public static string Title 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Title", "ImportUsers");
			   }
		    }
		
			public static string Users 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Users", "ImportUsers");
			   }
		    }
		}
	 
		public static class Index 
	    { 
			public static string AdminTitle 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("AdminTitle", "Index");
			   }
		    }
		
			public static string Title 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Title", "Index");
			   }
		    }
		}
	 
		public static class Buttons 
	    { 
			public static string About 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("About", "Buttons");
			   }
		    }
		
			public static string Back 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Back", "Buttons");
			   }
		    }
		
			public static string Cancel 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Cancel", "Buttons");
			   }
		    }
		
			public static string ContactUs 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ContactUs", "Buttons");
			   }
		    }
		
			public static string Continue 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Continue", "Buttons");
			   }
		    }
		
			public static string Delete 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Delete", "Buttons");
			   }
		    }
		
			public static string Download 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Download", "Buttons");
			   }
		    }
		
			public static string DownloadFactSheet 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("DownloadFactSheet", "Buttons");
			   }
		    }
		
			public static string LearnMore 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("LearnMore", "Buttons");
			   }
		    }
		
			public static string Next 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Next", "Buttons");
			   }
		    }
		
			public static string OK 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("OK", "Buttons");
			   }
		    }
		
			public static string Registration 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Registration", "Buttons");
			   }
		    }
		
			public static string RequestTrial 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("RequestTrial", "Buttons");
			   }
		    }
		
			public static string Save 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Save", "Buttons");
			   }
		    }
		
			public static string Search 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Search", "Buttons");
			   }
		    }
		
			public static string See 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("See", "Buttons");
			   }
		    }
		
			public static string Send 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Send", "Buttons");
			   }
		    }
		
			public static string Sort 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Sort", "Buttons");
			   }
		    }
		
			public static string Submit 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Submit", "Buttons");
			   }
		    }
		}
	 
		public static class Error 
	    { 
			public static string ErrorAccessDeniedText 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ErrorAccessDeniedText", "Error");
			   }
		    }
		
			public static string ErrorBody 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ErrorBody", "Error");
			   }
		    }
		
			public static string ErrorText 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ErrorText", "Error");
			   }
		    }
		
			public static string ErrorTitle 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("ErrorTitle", "Error");
			   }
		    }
		}
	 
		public static class LogIn 
	    { 
			public static string Heading 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Heading", "LogIn");
			   }
		    }
		
			public static string LoginFailed 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("LoginFailed", "LogIn");
			   }
		    }
		
			public static string Logon 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Logon", "LogIn");
			   }
		    }
		
			public static string Logout 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Logout", "LogIn");
			   }
		    }
		
			public static string Password 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Password", "LogIn");
			   }
		    }
		
			public static string PasswordRequired 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("PasswordRequired", "LogIn");
			   }
		    }
		
			public static string RememberMe 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("RememberMe", "LogIn");
			   }
		    }
		
			public static string UserName 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("UserName", "LogIn");
			   }
		    }
		
			public static string UserNameRequired 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("UserNameRequired", "LogIn");
			   }
		    }
		
			public static string Welcome 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Welcome", "LogIn");
			   }
		    }
		}
	 
		public static class Shared 
	    { 
			public static string Language 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Language", "Shared");
			   }
		    }
		
			public static string Optional 
		    {
			   get 
			   {
			       return ResourceProvider.GetResourceString("Optional", "Shared");
			   }
		    }
		}
	 
	   private static class LazyNested
       {
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static LazyNested()
			{
			}

			internal static readonly IResourceProvider Instance = IoC.Resolve<IResourceProvider>();
       }
	   
	}

}


#endregion
#pragma warning restore 1591

