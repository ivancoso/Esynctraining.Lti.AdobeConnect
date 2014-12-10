namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The company LMS DTO.
    /// </summary>
    [DataContract]
    public class CompanyLmsDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsDTO"/> class.
        /// </summary>
        public CompanyLmsDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLmsDTO"/> class.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        public CompanyLmsDTO(CompanyLms instance)
        {
            if (instance != null)
            {
                this.id = instance.Id;
//                this.acPassword = instance.AcPassword;
                this.acServer = instance.AcServer;
                this.acUsername = instance.AcUsername;
                this.companyId = instance.Company.Return(x => x.Id, 0);
                this.consumerKey = instance.ConsumerKey;
                this.createdBy = instance.CreatedBy.Return(x => x.Id, 0);
                this.modifiedBy = instance.ModifiedBy.Return(x => x.Id, 0);
                this.dateCreated = instance.DateCreated;
                this.dateModified = instance.DateModified;
                this.lmsProvider = instance.LmsProvider.Return(x => x.ShortName, string.Empty);
                this.sharedSecret = instance.SharedSecret;
                this.lmsAdmin = instance.AdminUser.With(x => x.Username);
                this.lmsAdminToken = instance.AdminUser.With(x => x.Token);
                this.lmsDomain = instance.LmsDomain.AddHttpProtocol(instance.UseSSL.GetValueOrDefault());
                this.primaryColor = instance.PrimaryColor;
                this.title = instance.Title;
                this.useUserFolder = instance.UseUserFolder.GetValueOrDefault();
                this.canRemoveMeeting = instance.CanRemoveMeeting.GetValueOrDefault();
                this.userFolderName = instance.UserFolderName;
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int id { get; set; }

        /// <summary>
        /// Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        /// Gets or sets the LMS admin.
        /// </summary>
        [DataMember]
        public string lmsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the LMS domain.
        /// </summary>
        [DataMember]
        public string lmsDomain { get; set; }

        /// <summary>
        /// Gets or sets the LMS admin password.
        /// </summary>
        [DataMember]
        public string lmsAdminPassword { get; set; }

        /// <summary>
        /// Gets or sets the LMS admin token.
        /// </summary>
        [DataMember]
        public string lmsAdminToken { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider.
        /// </summary>
        [DataMember]
        public string lmsProvider { get; set; }

        /// <summary>
        /// Gets or sets the AC server.
        /// </summary>
        [DataMember]
        public string acServer { get; set; }

        /// <summary>
        /// Gets or sets the AC username.
        /// </summary>
        [DataMember]
        public string acUsername { get; set; }

        /// <summary>
        /// Gets or sets the AC password.
        /// </summary>
        [DataMember]
        public string acPassword { get; set; }

        /// <summary>
        /// Gets or sets the consumer key.
        /// </summary>
        [DataMember]
        public string consumerKey { get; set; }

        /// <summary>
        /// Gets or sets the shared secret.
        /// </summary>
        [DataMember]
        public string sharedSecret { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public DateTime dateModified { get; set; }

        /// <summary>
        /// Gets or sets the primary color.
        /// </summary>
        [DataMember]
        public string primaryColor { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DataMember]
        public string title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use user folder.
        /// </summary>
        [DataMember]
        public bool useUserFolder { get; set; }

        /// <summary>
        /// Gets or sets the user folder name.
        /// </summary>
        [DataMember]
        public string userFolderName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether can remove meeting.
        /// </summary>
        [DataMember]
        public bool canRemoveMeeting { get; set; }
    }
}
