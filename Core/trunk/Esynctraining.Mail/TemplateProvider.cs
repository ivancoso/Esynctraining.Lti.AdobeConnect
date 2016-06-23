using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using Esynctraining.Core.Extensions;
using Esynctraining.Mail.Configuration;

namespace Esynctraining.Mail
{
    public class TemplateProvider : ITemplateProvider
    {
        private readonly string templatesDirectory;
        private readonly string imagesDirectory;
        private readonly string asciiTemplatesDirectory;


        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProvider" /> class. Gets templates folder path from configuration file
        /// </summary>
        public TemplateProvider()
        {
            this.templatesDirectory = MailerConfigurationSection.Current.With(x => x.TemplatesFolderPath);
            this.imagesDirectory = MailerConfigurationSection.Current.With(x => x.ImagesFolderPath);
            this.asciiTemplatesDirectory = MailerConfigurationSection.Current.With(x => x.AsciiTemplatesFolderPath);

            if (string.IsNullOrWhiteSpace(templatesDirectory))
                throw new ArgumentException("templatesDirectory cannot be empty", "templatesDirectory");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the template transformer with template that is got by model type.
        /// </summary>
        /// <typeparam name="TTemplateModel">The type of the template model.</typeparam>
        /// <returns>
        /// Instance of the <see cref="TemplateTransformer" /> with loaded template to transform
        /// </returns>
        public ITemplateTransformer GetTemplate<TTemplateModel>()
        {
            var emailTemplate = new TemplateTransformer(this.LoadTemplate(typeof(TTemplateModel)), string.IsNullOrWhiteSpace(this.asciiTemplatesDirectory) ? null : this.LoadAsciiTemplate(typeof(TTemplateModel)));

            return emailTemplate;
        }

        /// <summary>
        /// Gets the template transformer with template that is got by name.
        /// </summary>
        /// <param name="templateName">
        /// Name of the template.
        /// </param>
        /// <returns>
        /// instance of the <see cref="TemplateTransformer"/> with loaded template to transform
        /// </returns>
        public ITemplateTransformer GetTemplate(string templateName)
        {
            return new TemplateTransformer(this.LoadTemplate(templateName), string.IsNullOrWhiteSpace(this.asciiTemplatesDirectory) ? null : this.LoadAsciiTemplate(templateName));
        }

        #endregion

        #region Methods
        
        private static string GetDefaultTemplateFileName(Type templateType)
        {
            return GetDefaultTemplateFileName(templateType.Name.Replace("Model", string.Empty));
        }
        
        private static string GetDefaultTemplateFileName(string templateName)
        {
            return string.Format("{0}.html", templateName);
        }
        
        public string GetFullPathToImage(string imageFileName)
        {
            if (HttpContext.Current != null)
            {
                return
                    HttpContext.Current.Server.MapPath(VirtualPathUtility.Combine(this.imagesDirectory, imageFileName));
            }

            if (!Directory.Exists(this.imagesDirectory))
            {
                var callingAssembly = Assembly.GetCallingAssembly();

                return
                    Path.Combine(
                        callingAssembly.Location.TrimEndStrings(
                            callingAssembly.GetName().Name + ".exe",
                            callingAssembly.GetName().Name + ".dll") + this.imagesDirectory,
                        imageFileName);
            }
            else
            {
                return Path.Combine(this.imagesDirectory, imageFileName);
            }
        }

        private string GetFullPathToTemplate(string templateFileName)
        {
            if (HttpContext.Current != null)
            {
                return
                    HttpContext.Current.Server.MapPath(VirtualPathUtility.Combine(this.templatesDirectory, templateFileName));
            }

            if (!Directory.Exists(this.templatesDirectory))
            {
                var callingAssembly = Assembly.GetCallingAssembly();

                return
                    Path.Combine(
                        callingAssembly.Location.TrimEndStrings(
                            callingAssembly.GetName().Name + ".exe",
                            callingAssembly.GetName().Name + ".dll") + this.templatesDirectory,
                        templateFileName);
            }
            else
            {
                return Path.Combine(this.templatesDirectory, templateFileName);
            }
        }

        /// <summary>
        /// The get ascii full path to template.
        /// </summary>
        /// <param name="templateFileName">
        /// The template file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetAsciiFullPathToTemplate(string templateFileName)
        {
            if (HttpContext.Current != null)
            {
                return
                    HttpContext.Current.Server.MapPath(
                        VirtualPathUtility.Combine(this.asciiTemplatesDirectory, templateFileName));
            }

            if (!Directory.Exists(this.asciiTemplatesDirectory))
            {
                var callingAssembly = Assembly.GetCallingAssembly();

                return
                    Path.Combine(
                        callingAssembly.Location.TrimEndStrings(
                            callingAssembly.GetName().Name + ".exe",
                            callingAssembly.GetName().Name + ".dll") + this.asciiTemplatesDirectory,
                        templateFileName);
            }
            else
            {
                return Path.Combine(this.asciiTemplatesDirectory, templateFileName);
            }
        }
        
        private static string GetTemplateFileName(Type templateType)
        {
            return GetTemplateFileName(templateType.Name.Replace("Model", string.Empty));
        }
        
        private static string GetTemplateFileName(string templateName)
        {
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;

            return string.Format("{0}.{1}.html", templateName, cultureName);
        }

        /// <summary>
        /// The load template.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Can't find template at the path
        /// </exception>
        private string LoadTemplate(string templateName)
        {
            string templatePath = this.GetFullPathToTemplate(GetTemplateFileName(templateName));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }

            templatePath = this.GetFullPathToTemplate(GetDefaultTemplateFileName(templateName));
            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }

            throw new Exception(string.Format("can't find template at the path: {0}", templatePath));
        }

        /// <summary>
        /// The load template.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Can't find template at the path
        /// </exception>
        private string LoadAsciiTemplate(string templateName)
        {
            string templatePath = this.GetAsciiFullPathToTemplate(GetTemplateFileName(templateName));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }

            templatePath = this.GetAsciiFullPathToTemplate(GetDefaultTemplateFileName(templateName));
            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }

            throw new Exception(string.Format("can't find template at the path: {0}", templatePath));
        }

        /// <summary>
        /// The load template.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Can't find template at the path
        /// </exception>
        private string LoadTemplate(Type templateType)
        {
            string templatePath = this.GetFullPathToTemplate(GetTemplateFileName(templateType));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            templatePath = this.GetFullPathToTemplate(GetDefaultTemplateFileName(templateType));
            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            throw new Exception(string.Format("can't find template at the path: {0}", templatePath));
        }

        /// <summary>
        /// The load template.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Can't find template at the path
        /// </exception>
        private string LoadAsciiTemplate(Type templateType)
        {
            string templatePath = this.GetAsciiFullPathToTemplate(GetTemplateFileName(templateType));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            templatePath = this.GetAsciiFullPathToTemplate(GetDefaultTemplateFileName(templateType));
            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            throw new Exception(string.Format("can't find template at the path: {0}", templatePath));
        }

        #endregion

    }

}
