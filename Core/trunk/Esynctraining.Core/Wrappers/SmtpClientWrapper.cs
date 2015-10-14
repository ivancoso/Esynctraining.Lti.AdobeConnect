namespace Esynctraining.Core.Wrappers
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// The simple mail transfer protocol client wrapper.
    /// </summary>
    internal class SmtpClientWrapper : IDisposable
    {
        #region Fields

        /// <summary>
        /// The simple mail transfer protocol client.
        /// </summary>
        private readonly SmtpClient smtpClient;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpClientWrapper"/> class.
        /// </summary>
        /// <param name="smtpClient">
        /// The simple mail transfer protocol client.
        /// </param>
        public SmtpClientWrapper(SmtpClient smtpClient)
        {
            this.smtpClient = smtpClient;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The send completed.
        /// </summary>
        public event SendCompletedEventHandler SendCompleted
        {
            add
            {
                this.smtpClient.SendCompleted += value;
            }

            remove
            {
                this.smtpClient.SendCompleted -= value;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the client certificates.
        /// </summary>
        public virtual X509CertificateCollection ClientCertificates
        {
            get
            {
                return this.smtpClient.ClientCertificates;
            }
        }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        public virtual ICredentialsByHost Credentials
        {
            get
            {
                return this.smtpClient.Credentials;
            }

            set
            {
                this.smtpClient.Credentials = value;
            }
        }

        /// <summary>
        /// Gets or sets the delivery method.
        /// </summary>
        public virtual SmtpDeliveryMethod DeliveryMethod
        {
            get
            {
                return this.smtpClient.DeliveryMethod;
            }

            set
            {
                this.smtpClient.DeliveryMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable secure service layer.
        /// </summary>
        public virtual bool EnableSsl
        {
            get
            {
                return this.smtpClient.EnableSsl;
            }

            set
            {
                this.smtpClient.EnableSsl = value;
            }
        }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        public virtual string Host
        {
            get
            {
                return this.smtpClient.Host;
            }

            set
            {
                this.smtpClient.Host = value;
            }
        }

        /// <summary>
        /// Gets or sets the pickup directory location.
        /// </summary>
        public virtual string PickupDirectoryLocation
        {
            get
            {
                return this.smtpClient.PickupDirectoryLocation;
            }

            set
            {
                this.smtpClient.PickupDirectoryLocation = value;
            }
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        public virtual int Port
        {
            get
            {
                return this.smtpClient.Port;
            }

            set
            {
                this.smtpClient.Port = value;
            }
        }

        /// <summary>
        /// Gets the service point.
        /// </summary>
        public virtual ServicePoint ServicePoint
        {
            get
            {
                return this.smtpClient.ServicePoint;
            }
        }

        /// <summary>
        /// Gets or sets the target name.
        /// </summary>
        public virtual string TargetName
        {
            get
            {
                return this.smtpClient.TargetName;
            }

            set
            {
                this.smtpClient.TargetName = value;
            }
        }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public virtual int Timeout
        {
            get
            {
                return this.smtpClient.Timeout;
            }

            set
            {
                this.smtpClient.Timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether use default credentials.
        /// </summary>
        public virtual bool UseDefaultCredentials
        {
            get
            {
                return this.smtpClient.UseDefaultCredentials;
            }

            set
            {
                this.smtpClient.UseDefaultCredentials = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void Send(MailMessage message)
        {
            this.smtpClient.Send(message);
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public virtual void SendAsync(MailMessage message)
        {
            this.smtpClient.SendAsync(message, Guid.NewGuid());
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="recipients">
        /// The recipients.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        public virtual void Send(string from, string recipients, string subject, string body)
        {
            this.smtpClient.Send(from, recipients, subject, body);
        }

        /// <summary>
        /// The send async.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="userToken">
        /// The user token.
        /// </param>
        public virtual void SendAsync(MailMessage message, object userToken)
        {
            this.smtpClient.SendAsync(message, userToken);
        }

        /// <summary>
        /// The send async.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="recipients">
        /// The recipients.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="userToken">
        /// The user token.
        /// </param>
        public virtual void SendAsync(string from, string recipients, string subject, string body, object userToken)
        {
            this.smtpClient.SendAsync(from, recipients, subject, body, userToken);
        }

        /// <summary>
        /// The send async cancel.
        /// </summary>
        public virtual void SendAsyncCancel()
        {
            this.smtpClient.SendAsyncCancel();
        }

        #endregion
    }
}