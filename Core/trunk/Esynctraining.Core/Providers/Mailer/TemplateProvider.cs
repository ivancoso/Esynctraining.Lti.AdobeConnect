namespace Esynctraining.Core.Providers.Mailer
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Web;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers.Mailer.Configuration;

    /// <summary>
    ///     Defines contracts for getting templates
    /// </summary>
    public class TemplateProvider : ITemplateProvider
    {
        #region Fields

        /// <summary>
        /// The templates directory.
        /// </summary>
        private readonly string templatesDirectory;

        /// <summary>
        /// The templates directory.
        /// </summary>
        private readonly string asciiTemplatesDirectory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProvider" /> class. Gets templates folder path from configuration file
        /// </summary>
        public TemplateProvider()
        {
            this.templatesDirectory = MailerConfigurationSection.Current.With(x => x.TemplatesFolderPath);
            this.asciiTemplatesDirectory = MailerConfigurationSection.Current.With(x => x.AsciiTemplatesFolderPath);
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

        /// <summary>
        /// The get default template file name.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDefaultTemplateFileName(Type templateType)
        {
            return string.Format("{0}.html", templateType.Name.Replace("Model", string.Empty));
        }

        /// <summary>
        /// The get default template file name.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDefaultTemplateFileName(string templateName)
        {
            return string.Format("{0}.html", templateName);
        }

        /// <summary>
        /// The get full path to template.
        /// </summary>
        /// <param name="templateFileName">
        /// The template file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFullPathToTemplate(string templateFileName)
        {
            if (HttpContext.Current != null)
            {
                return
                    HttpContext.Current.Server.MapPath(
                        VirtualPathUtility.Combine(this.templatesDirectory, templateFileName));
            }
            return Path.Combine(this.templatesDirectory, templateFileName);
        }

        private string GetAsciiFullPathToTemplate(string templateFileName)
        {
            if (HttpContext.Current != null)
            {
                return
                    HttpContext.Current.Server.MapPath(
                        VirtualPathUtility.Combine(this.asciiTemplatesDirectory, templateFileName));
            }
            return Path.Combine(this.asciiTemplatesDirectory, templateFileName);
        }

        /// <summary>
        /// The get template file name.
        /// </summary>
        /// <param name="templateType">
        /// The template type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTemplateFileName(Type templateType)
        {
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;

            return string.Format("{0}.{1}.html", templateType.Name.Replace("Model", string.Empty), cultureName);
        }

        /// <summary>
        /// The get template file name.
        /// </summary>
        /// <param name="templateName">
        /// The template name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTemplateFileName(string templateName)
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
            string templatePath = this.GetFullPathToTemplate(this.GetTemplateFileName(templateName));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }

            templatePath = this.GetFullPathToTemplate(this.GetDefaultTemplateFileName(templateName));
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
            string templatePath = this.GetAsciiFullPathToTemplate(this.GetTemplateFileName(templateName));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }

            templatePath = this.GetAsciiFullPathToTemplate(this.GetDefaultTemplateFileName(templateName));
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
            string templatePath = this.GetFullPathToTemplate(this.GetTemplateFileName(templateType));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            templatePath = this.GetFullPathToTemplate(this.GetDefaultTemplateFileName(templateType));
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
            string templatePath = this.GetAsciiFullPathToTemplate(this.GetTemplateFileName(templateType));

            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            templatePath = this.GetAsciiFullPathToTemplate(this.GetDefaultTemplateFileName(templateType));
            if (File.Exists(templatePath))
            {
                return File.OpenText(templatePath).ReadToEnd();
            }
            throw new Exception(string.Format("can't find template at the path: {0}", templatePath));
        }

        #endregion
    }
}