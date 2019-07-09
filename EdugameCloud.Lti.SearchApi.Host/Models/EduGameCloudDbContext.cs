using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EdugameClaud.Lti.SearchApi.Host.Models
{
    public partial class EduGameCloudDbContext : DbContext
    {
        public EduGameCloudDbContext()
        {
        }

        public EduGameCloudDbContext(DbContextOptions<EduGameCloudDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AcCachePrincipal> AcCachePrincipal { get; set; }
        public virtual DbSet<Acsession> Acsession { get; set; }
        public virtual DbSet<AcuserMode> AcuserMode { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<AppletItem> AppletItem { get; set; }
        public virtual DbSet<AppletResult> AppletResult { get; set; }
        public virtual DbSet<ApplicationVersion> ApplicationVersion { get; set; }
        public virtual DbSet<BuildVersion> BuildVersion { get; set; }
        public virtual DbSet<BuildVersionType> BuildVersionType { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyAcDomains> CompanyAcDomains { get; set; }
        public virtual DbSet<CompanyEventQuizMapping> CompanyEventQuizMapping { get; set; }
        public virtual DbSet<CompanyLicense> CompanyLicense { get; set; }
        public virtual DbSet<CompanyLicenseHistory> CompanyLicenseHistory { get; set; }
        public virtual DbSet<CompanyLms> CompanyLms { get; set; }
        public virtual DbSet<CompanyTheme> CompanyTheme { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Distractor> Distractor { get; set; }
        public virtual DbSet<DistractorHistory> DistractorHistory { get; set; }
        public virtual DbSet<ErrorReport> ErrorReport { get; set; }
        public virtual DbSet<File> File { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<LmsCompanyRoleMapping> LmsCompanyRoleMapping { get; set; }
        public virtual DbSet<LmsCompanySetting> LmsCompanySetting { get; set; }
        public virtual DbSet<LmsCourseMeeting> LmsCourseMeeting { get; set; }
        public virtual DbSet<LmsCourseMeetingGuest> LmsCourseMeetingGuest { get; set; }
        public virtual DbSet<LmsCourseMeetingRecording> LmsCourseMeetingRecording { get; set; }
        public virtual DbSet<LmsCourseSection> LmsCourseSection { get; set; }
        public virtual DbSet<LmsMeetingSession> LmsMeetingSession { get; set; }
        public virtual DbSet<LmsMeetingType> LmsMeetingType { get; set; }
        public virtual DbSet<LmsProvider> LmsProvider { get; set; }
        public virtual DbSet<LmsQuestionType> LmsQuestionType { get; set; }
        public virtual DbSet<LmsUser> LmsUser { get; set; }
        public virtual DbSet<LmsUserMeetingRole> LmsUserMeetingRole { get; set; }
        public virtual DbSet<LmsUserParameters> LmsUserParameters { get; set; }
        public virtual DbSet<LmsUserSession> LmsUserSession { get; set; }
        public virtual DbSet<Module> Module { get; set; }
        public virtual DbSet<OfficeHours> OfficeHours { get; set; }
        public virtual DbSet<OfficeHoursSlot> OfficeHoursSlot { get; set; }
        public virtual DbSet<OfficeHoursTeacherAvailability> OfficeHoursTeacherAvailability { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<QuestionForLikert> QuestionForLikert { get; set; }
        public virtual DbSet<QuestionForOpenAnswer> QuestionForOpenAnswer { get; set; }
        public virtual DbSet<QuestionForRate> QuestionForRate { get; set; }
        public virtual DbSet<QuestionForSingleMultipleChoice> QuestionForSingleMultipleChoice { get; set; }
        public virtual DbSet<QuestionForTrueFalse> QuestionForTrueFalse { get; set; }
        public virtual DbSet<QuestionForWeightBucket> QuestionForWeightBucket { get; set; }
        public virtual DbSet<QuestionHistory> QuestionHistory { get; set; }
        public virtual DbSet<QuestionType> QuestionType { get; set; }
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<QuizFormat> QuizFormat { get; set; }
        public virtual DbSet<QuizQuestionResult> QuizQuestionResult { get; set; }
        public virtual DbSet<QuizQuestionResultAnswer> QuizQuestionResultAnswer { get; set; }
        public virtual DbSet<QuizResult> QuizResult { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<ScoreType> ScoreType { get; set; }
        public virtual DbSet<SngroupDiscussion> SngroupDiscussion { get; set; }
        public virtual DbSet<Snlink> Snlink { get; set; }
        public virtual DbSet<SnmapProvider> SnmapProvider { get; set; }
        public virtual DbSet<SnmapSettings> SnmapSettings { get; set; }
        public virtual DbSet<Snmember> Snmember { get; set; }
        public virtual DbSet<Snprofile> Snprofile { get; set; }
        public virtual DbSet<SnprofileSnservice> SnprofileSnservice { get; set; }
        public virtual DbSet<Snservice> Snservice { get; set; }
        public virtual DbSet<SocialUserTokens> SocialUserTokens { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<SubModule> SubModule { get; set; }
        public virtual DbSet<SubModuleCategory> SubModuleCategory { get; set; }
        public virtual DbSet<SubModuleItem> SubModuleItem { get; set; }
        public virtual DbSet<SubModuleItemTheme> SubModuleItemTheme { get; set; }
        public virtual DbSet<SubscriptionHistoryLog> SubscriptionHistoryLog { get; set; }
        public virtual DbSet<SubscriptionUpdate> SubscriptionUpdate { get; set; }
        public virtual DbSet<Survey> Survey { get; set; }
        public virtual DbSet<SurveyGroupingType> SurveyGroupingType { get; set; }
        public virtual DbSet<SurveyQuestionResult> SurveyQuestionResult { get; set; }
        public virtual DbSet<SurveyQuestionResultAnswer> SurveyQuestionResultAnswer { get; set; }
        public virtual DbSet<SurveyResult> SurveyResult { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestQuestionResult> TestQuestionResult { get; set; }
        public virtual DbSet<TestResult> TestResult { get; set; }
        public virtual DbSet<Theme> Theme { get; set; }
        public virtual DbSet<TimeZone> TimeZone { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserActivation> UserActivation { get; set; }
        public virtual DbSet<UserLoginHistory> UserLoginHistory { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Webinar> Webinar { get; set; }
        public virtual DbSet<WftSchool> WftSchool { get; set; }

        // Unable to generate entity type for table 'dbo.EmailHistory'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.NewsletterSubscription'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.ThemeAttribute'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.ServerStatus'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=192.168.10.113;Database=EduGameCloud.Dev;User ID=sa;Password=`12345tgB;Connection Timeout=180;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AcCachePrincipal>(entity =>
            {
                entity.Property(e => e.AcCachePrincipalId).HasColumnName("acCachePrincipalId");

                entity.Property(e => e.AccountId)
                    .HasColumnName("accountId")
                    .HasMaxLength(512);

                entity.Property(e => e.DisplayId)
                    .HasColumnName("displayId")
                    .HasMaxLength(512);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(512);

                entity.Property(e => e.FirstName)
                    .HasColumnName("firstName")
                    .HasMaxLength(512);

                entity.Property(e => e.HasChildren).HasColumnName("hasChildren");

                entity.Property(e => e.IsHidden).HasColumnName("isHidden");

                entity.Property(e => e.IsPrimary).HasColumnName("isPrimary");

                entity.Property(e => e.LastName)
                    .HasColumnName("lastName")
                    .HasMaxLength(512);

                entity.Property(e => e.LmsCompanyId).HasColumnName("lmsCompanyId");

                entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasMaxLength(512);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(512);

                entity.Property(e => e.PrincipalId)
                    .HasColumnName("principalId")
                    .HasMaxLength(512);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(512);

                entity.HasOne(d => d.LmsCompany)
                    .WithMany(p => p.AcCachePrincipal)
                    .HasForeignKey(d => d.LmsCompanyId)
                    .HasConstraintName("FK_AcCachePrincipal_CompanyLms");
            });

            modelBuilder.Entity<Acsession>(entity =>
            {
                entity.ToTable("ACSession");

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.AcUserModeId).HasColumnName("acUserModeId");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IncludeAcEmails).HasColumnName("includeAcEmails");

                entity.Property(e => e.LanguageId)
                    .HasColumnName("languageId")
                    .HasDefaultValueSql("((5))");

                entity.Property(e => e.MeetingUrl)
                    .IsRequired()
                    .HasColumnName("meetingURL")
                    .HasMaxLength(500);

                entity.Property(e => e.ScoId).HasColumnName("scoId");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((2))");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.AcUserMode)
                    .WithMany(p => p.Acsession)
                    .HasForeignKey(d => d.AcUserModeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ACSession_ACUserMode");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Acsession)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ACSession_Language");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.Acsession)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ACSession_SubModuleItem");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Acsession)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ACSession_User");
            });

            modelBuilder.Entity<AcuserMode>(entity =>
            {
                entity.ToTable("ACUserMode");

                entity.HasIndex(e => e.UserMode)
                    .HasName("UI_ACUserMode_userMode")
                    .IsUnique();

                entity.Property(e => e.AcUserModeId).HasColumnName("acUserModeId");

                entity.Property(e => e.ImageId).HasColumnName("imageId");

                entity.Property(e => e.UserMode)
                    .IsRequired()
                    .HasColumnName("userMode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.AcuserMode)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_ACUserMode_Image");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.AddressId).HasColumnName("addressId");

                entity.Property(e => e.Address1)
                    .HasColumnName("address1")
                    .HasMaxLength(255);

                entity.Property(e => e.Address2)
                    .HasColumnName("address2")
                    .HasMaxLength(255);

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasMaxLength(255);

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.StateId).HasColumnName("stateId");

                entity.Property(e => e.Zip)
                    .HasColumnName("zip")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK_Address_Country");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK_Address_State");
            });

            modelBuilder.Entity<AppletItem>(entity =>
            {
                entity.Property(e => e.AppletItemId).HasColumnName("appletItemId");

                entity.Property(e => e.AppletName)
                    .IsRequired()
                    .HasColumnName("appletName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DocumentXml)
                    .HasColumnName("documentXML")
                    .IsUnicode(false);

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.AppletItem)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .HasConstraintName("FK_AppletItem_SubModuleItem");
            });

            modelBuilder.Entity<AppletResult>(entity =>
            {
                entity.Property(e => e.AppletResultId).HasColumnName("appletResultId");

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.AppletItemId).HasColumnName("appletItemId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(500);

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsArchive).HasColumnName("isArchive");

                entity.Property(e => e.ParticipantName)
                    .IsRequired()
                    .HasColumnName("participantName")
                    .HasMaxLength(200);

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.AcSession)
                    .WithMany(p => p.AppletResult)
                    .HasForeignKey(d => d.AcSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppletResult_ACSession");

                entity.HasOne(d => d.AppletItem)
                    .WithMany(p => p.AppletResult)
                    .HasForeignKey(d => d.AppletItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AppletResult_AppletItem");
            });

            modelBuilder.Entity<ApplicationVersion>(entity =>
            {
                entity.HasKey(e => new { e.MajorVersion, e.MinorVersion });

                entity.Property(e => e.MajorVersion).HasColumnName("majorVersion");

                entity.Property(e => e.MinorVersion).HasColumnName("minorVersion");
            });

            modelBuilder.Entity<BuildVersion>(entity =>
            {
                entity.Property(e => e.BuildVersionId).HasColumnName("buildVersionId");

                entity.Property(e => e.BuildNumber)
                    .IsRequired()
                    .HasColumnName("buildNumber")
                    .HasMaxLength(20);

                entity.Property(e => e.BuildVersionTypeId).HasColumnName("buildVersionTypeId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.DescriptionHtml).HasColumnName("descriptionHTML");

                entity.Property(e => e.DescriptionSmall)
                    .HasColumnName("descriptionSmall")
                    .HasMaxLength(255);

                entity.Property(e => e.FileId).HasColumnName("fileId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.HasOne(d => d.BuildVersionType)
                    .WithMany(p => p.BuildVersion)
                    .HasForeignKey(d => d.BuildVersionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BuildVersion_BuildVersionType");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.BuildVersion)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_BuildVersion_File");
            });

            modelBuilder.Entity<BuildVersionType>(entity =>
            {
                entity.Property(e => e.BuildVersionTypeId).HasColumnName("buildVersionTypeId");

                entity.Property(e => e.BuildVersionType1)
                    .IsRequired()
                    .HasColumnName("buildVersionType")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.AddressId).HasColumnName("addressId");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnName("companyName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyThemeId).HasColumnName("companyThemeId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PrimaryContactId).HasColumnName("primaryContactId");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UseEventMapping)
                    .HasColumnName("useEventMapping")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Company)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_Company_Address");

                entity.HasOne(d => d.CompanyTheme)
                    .WithMany(p => p.Company)
                    .HasForeignKey(d => d.CompanyThemeId)
                    .HasConstraintName("FK_Company_CompanyTheme");

                entity.HasOne(d => d.PrimaryContact)
                    .WithMany(p => p.Company)
                    .HasForeignKey(d => d.PrimaryContactId)
                    .HasConstraintName("FK_Company_PrimaryContact");
            });

            modelBuilder.Entity<CompanyAcDomains>(entity =>
            {
                entity.HasKey(e => e.CompanyAcServerId);

                entity.Property(e => e.AcServer)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyAcDomains)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyAcDomains_Company");
            });

            modelBuilder.Entity<CompanyEventQuizMapping>(entity =>
            {
                entity.Property(e => e.AcEventScoId).HasMaxLength(50);

                entity.Property(e => e.Guid).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.CompanyAcDomain)
                    .WithMany(p => p.CompanyEventQuizMapping)
                    .HasForeignKey(d => d.CompanyAcDomainId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyEventQuizMapping_CompanyAcDomains");

                entity.HasOne(d => d.PostQuiz)
                    .WithMany(p => p.CompanyEventQuizMappingPostQuiz)
                    .HasForeignKey(d => d.PostQuizId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CompanyEventQuizMapping_Quiz_Cascade");

                entity.HasOne(d => d.PreQuiz)
                    .WithMany(p => p.CompanyEventQuizMappingPreQuiz)
                    .HasForeignKey(d => d.PreQuizId)
                    .HasConstraintName("FK_CompanyEventQuizMapping_Quiz");
            });

            modelBuilder.Entity<CompanyLicense>(entity =>
            {
                entity.Property(e => e.CompanyLicenseId).HasColumnName("companyLicenseId");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateStart)
                    .HasColumnName("dateStart")
                    .HasColumnType("datetime");

                entity.Property(e => e.Domain)
                    .HasColumnName("domain")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiryDate")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.HasApi).HasColumnName("hasApi");

                entity.Property(e => e.LicenseNumber)
                    .IsRequired()
                    .HasColumnName("licenseNumber")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LicenseStatus).HasColumnName("licenseStatus");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.TotalLicensesCount)
                    .HasColumnName("totalLicensesCount")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TotalParticipantsCount)
                    .HasColumnName("totalParticipantsCount")
                    .HasDefaultValueSql("((100))");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyLicense)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLicense_Company");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CompanyLicenseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLicense_CreatedBy");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.CompanyLicenseModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLicense_ModifiedBy");
            });

            modelBuilder.Entity<CompanyLicenseHistory>(entity =>
            {
                entity.Property(e => e.CompanyLicenseHistoryId).HasColumnName("companyLicenseHistoryId");

                entity.Property(e => e.CompanyLicenseId).HasColumnName("companyLicenseId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.CompanyLicenseHistory)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_CompanyLicenseHistory_CompanyLicense");
            });

            modelBuilder.Entity<CompanyLms>(entity =>
            {
                entity.Property(e => e.CompanyLmsId).HasColumnName("companyLmsId");

                entity.Property(e => e.AcPassword)
                    .IsRequired()
                    .HasColumnName("acPassword")
                    .HasMaxLength(100);

                entity.Property(e => e.AcScoId)
                    .HasColumnName("acScoId")
                    .HasMaxLength(50);

                entity.Property(e => e.AcServer)
                    .IsRequired()
                    .HasColumnName("acServer")
                    .HasMaxLength(100);

                entity.Property(e => e.AcTemplateScoId)
                    .HasColumnName("acTemplateScoId")
                    .HasMaxLength(50);

                entity.Property(e => e.AcUsername)
                    .IsRequired()
                    .HasColumnName("acUsername")
                    .HasMaxLength(100);

                entity.Property(e => e.AcUsesEmailAsLogin).HasColumnName("acUsesEmailAsLogin");

                entity.Property(e => e.AddPrefixToMeetingName).HasColumnName("addPrefixToMeetingName");

                entity.Property(e => e.AdminUserId).HasColumnName("adminUserId");

                entity.Property(e => e.CanEditMeeting).HasColumnName("canEditMeeting");

                entity.Property(e => e.CanRemoveMeeting).HasColumnName("canRemoveMeeting");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.ConsumerKey)
                    .IsRequired()
                    .HasColumnName("consumerKey")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("date");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("date");

                entity.Property(e => e.EnableCourseMeetings).HasColumnName("enableCourseMeetings");

                entity.Property(e => e.EnableOfficeHours).HasColumnName("enableOfficeHours");

                entity.Property(e => e.EnableProxyToolMode).HasColumnName("enableProxyToolMode");

                entity.Property(e => e.EnableStudyGroups).HasColumnName("enableStudyGroups");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsSettingsVisible).HasColumnName("isSettingsVisible");

                entity.Property(e => e.LastSignalId).HasColumnName("lastSignalId");

                entity.Property(e => e.LmsDomain)
                    .HasColumnName("lmsDomain")
                    .HasMaxLength(100);

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.LoginUsingCookie).HasColumnName("loginUsingCookie");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.PrimaryColor)
                    .HasColumnName("primaryColor")
                    .HasMaxLength(50);

                entity.Property(e => e.ProxyToolSharedPassword)
                    .HasColumnName("proxyToolSharedPassword")
                    .HasMaxLength(255);

                entity.Property(e => e.SharedSecret)
                    .IsRequired()
                    .HasColumnName("sharedSecret")
                    .HasMaxLength(100);

                entity.Property(e => e.ShowAnnouncements).HasColumnName("showAnnouncements");

                entity.Property(e => e.ShowEgchelp).HasColumnName("showEGCHelp");

                entity.Property(e => e.ShowLmsHelp).HasColumnName("showLmsHelp");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100);

                entity.Property(e => e.UseSsl).HasColumnName("useSSL");

                entity.Property(e => e.UseUserFolder).HasColumnName("useUserFolder");

                entity.Property(e => e.UserFolderName)
                    .HasColumnName("userFolderName")
                    .HasMaxLength(50);

                entity.HasOne(d => d.AdminUser)
                    .WithMany(p => p.CompanyLms)
                    .HasForeignKey(d => d.AdminUserId)
                    .HasConstraintName("FK_CompanyLms_LmsUser");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyLms)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_CompanyLms_Company");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.CompanyLmsCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLms_User");

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.CompanyLms)
                    .HasForeignKey(d => d.LmsProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompanyLms_LmsProvider");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.CompanyLmsModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_CompanyLms_User2");
            });

            modelBuilder.Entity<CompanyTheme>(entity =>
            {
                entity.Property(e => e.CompanyThemeId)
                    .HasColumnName("companyThemeId")
                    .ValueGeneratedNever();

                entity.Property(e => e.ButtonColor)
                    .HasColumnName("buttonColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ButtonTextColor)
                    .HasColumnName("buttonTextColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GridHeaderBackgroundColor)
                    .HasColumnName("gridHeaderBackgroundColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GridHeaderTextColor)
                    .HasColumnName("gridHeaderTextColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GridRolloverColor)
                    .HasColumnName("gridRolloverColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HeaderBackgroundColor)
                    .HasColumnName("headerBackgroundColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LoginHeaderTextColor)
                    .HasColumnName("loginHeaderTextColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LogoId).HasColumnName("logoId");

                entity.Property(e => e.PopupHeaderBackgroundColor)
                    .HasColumnName("popupHeaderBackgroundColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PopupHeaderTextColor)
                    .HasColumnName("popupHeaderTextColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.QuestionColor)
                    .HasColumnName("questionColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.QuestionHeaderColor)
                    .HasColumnName("questionHeaderColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.WelcomeTextColor)
                    .HasColumnName("welcomeTextColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.Logo)
                    .WithMany(p => p.CompanyTheme)
                    .HasForeignKey(d => d.LogoId)
                    .HasConstraintName("FK_CompanyTheme_File");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.Country1)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasMaxLength(255);

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnName("countryCode")
                    .HasMaxLength(3)
                    .IsUnicode(false);

                entity.Property(e => e.CountryCode3)
                    .IsRequired()
                    .HasColumnName("countryCode3")
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(18, 7)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(18, 7)");

                entity.Property(e => e.ZoomLevel).HasColumnName("zoomLevel");
            });

            modelBuilder.Entity<Distractor>(entity =>
            {
                entity.Property(e => e.DistractorId).HasColumnName("distractorId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Distractor1)
                    .IsRequired()
                    .HasColumnName("distractor");

                entity.Property(e => e.DistractorOrder).HasColumnName("distractorOrder");

                entity.Property(e => e.DistractorType).HasColumnName("distractorType");

                entity.Property(e => e.ImageId).HasColumnName("imageId");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsCorrect)
                    .HasColumnName("isCorrect")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LeftImageId).HasColumnName("leftImageId");

                entity.Property(e => e.LmsAnswer)
                    .HasColumnName("lmsAnswer")
                    .HasMaxLength(100);

                entity.Property(e => e.LmsAnswerId).HasColumnName("lmsAnswerId");

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.RightImageId).HasColumnName("rightImageId");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.DistractorCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Distractor_UserCreated");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.DistractorImage)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Distractor_Image");

                entity.HasOne(d => d.LeftImage)
                    .WithMany(p => p.DistractorLeftImage)
                    .HasForeignKey(d => d.LeftImageId)
                    .HasConstraintName("FK_Distractor_LeftImage");

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.Distractor)
                    .HasForeignKey(d => d.LmsProviderId)
                    .HasConstraintName("FK_Distractor_lmsProvider");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.DistractorModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Distractor_UserModified");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Distractor)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK_Distractor_Question");

                entity.HasOne(d => d.RightImage)
                    .WithMany(p => p.DistractorRightImage)
                    .HasForeignKey(d => d.RightImageId)
                    .HasConstraintName("FK_Distractor_RightImage");
            });

            modelBuilder.Entity<DistractorHistory>(entity =>
            {
                entity.HasKey(e => e.DistractoryHistoryId);

                entity.Property(e => e.DistractoryHistoryId).HasColumnName("distractoryHistoryId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Distractor)
                    .IsRequired()
                    .HasColumnName("distractor")
                    .IsUnicode(false);

                entity.Property(e => e.DistractorId).HasColumnName("distractorID");

                entity.Property(e => e.DistractorOrder).HasColumnName("distractorOrder");

                entity.Property(e => e.ImageId).HasColumnName("imageID");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsCorrect)
                    .HasColumnName("isCorrect")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.QuestionId).HasColumnName("questionID");

                entity.Property(e => e.Score)
                    .HasColumnName("score")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.DistractorNavigation)
                    .WithMany(p => p.DistractorHistory)
                    .HasForeignKey(d => d.DistractorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DistractorHistory_Distractor");
            });

            modelBuilder.Entity<ErrorReport>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.ApplicationVersion)
                    .IsRequired()
                    .HasColumnName("applicationVersion")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FlashVersion)
                    .HasColumnName("flashVersion")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .IsUnicode(false);

                entity.Property(e => e.Os)
                    .HasColumnName("os")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.Property(e => e.FileId)
                    .HasColumnName("fileId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("fileName")
                    .HasMaxLength(255);

                entity.Property(e => e.Height).HasColumnName("height");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Width).HasColumnName("width");

                entity.Property(e => e.X).HasColumnName("x");

                entity.Property(e => e.Y).HasColumnName("y");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.File)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Image_User");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasIndex(e => e.Language1)
                    .HasName("UI_Language_language")
                    .IsUnique();

                entity.Property(e => e.LanguageId).HasColumnName("languageId");

                entity.Property(e => e.Language1)
                    .IsRequired()
                    .HasColumnName("language")
                    .HasMaxLength(100);

                entity.Property(e => e.TwoLetterCode)
                    .IsRequired()
                    .HasColumnName("twoLetterCode")
                    .HasMaxLength(2)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<LmsCompanyRoleMapping>(entity =>
            {
                entity.HasIndex(e => new { e.LmsCompanyId, e.LmsRoleName })
                    .HasName("UI_LmsCompanyRoleMapping_lmsCompanyId_lmsRoleName")
                    .IsUnique();

                entity.Property(e => e.LmsCompanyRoleMappingId).HasColumnName("lmsCompanyRoleMappingId");

                entity.Property(e => e.AcRole).HasColumnName("acRole");

                entity.Property(e => e.IsDefaultLmsRole).HasColumnName("isDefaultLmsRole");

                entity.Property(e => e.IsTeacherRole).HasColumnName("isTeacherRole");

                entity.Property(e => e.LmsCompanyId).HasColumnName("lmsCompanyId");

                entity.Property(e => e.LmsRoleName)
                    .IsRequired()
                    .HasColumnName("lmsRoleName")
                    .HasMaxLength(100);

                entity.HasOne(d => d.LmsCompany)
                    .WithMany(p => p.LmsCompanyRoleMapping)
                    .HasForeignKey(d => d.LmsCompanyId)
                    .HasConstraintName("FK_LmsCompanyRoleMapping_LmsCompany");
            });

            modelBuilder.Entity<LmsCompanySetting>(entity =>
            {
                entity.HasIndex(e => new { e.LmsCompanyId, e.Name })
                    .HasName("UI_LmsCompanySetting_lmsCompanyId_name")
                    .IsUnique();

                entity.Property(e => e.LmsCompanySettingId).HasColumnName("lmsCompanySettingId");

                entity.Property(e => e.LmsCompanyId).HasColumnName("lmsCompanyId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");

                entity.HasOne(d => d.LmsCompany)
                    .WithMany(p => p.LmsCompanySetting)
                    .HasForeignKey(d => d.LmsCompanyId)
                    .HasConstraintName("FK_LmsCompanySetting_LmsCompany");
            });

            modelBuilder.Entity<LmsCourseMeeting>(entity =>
            {
                entity.Property(e => e.LmsCourseMeetingId).HasColumnName("lmsCourseMeetingId");

                entity.Property(e => e.AudioProfileId)
                    .HasColumnName("audioProfileId")
                    .HasMaxLength(50);

                entity.Property(e => e.AudioProfileProvider)
                    .HasColumnName("audioProfileProvider")
                    .HasMaxLength(50);

                entity.Property(e => e.CompanyLmsId).HasColumnName("companyLmsId");

                entity.Property(e => e.CourseId).HasColumnName("courseId");

                entity.Property(e => e.EnableDynamicProvisioning).HasColumnName("enableDynamicProvisioning");

                entity.Property(e => e.LmsCalendarEventId)
                    .HasColumnName("lmsCalendarEventId")
                    .HasMaxLength(50);

                entity.Property(e => e.LmsMeetingTypeId).HasColumnName("lmsMeetingTypeId");

                entity.Property(e => e.MeetingNameJson)
                    .HasColumnName("meetingNameJson")
                    .HasMaxLength(4000);

                entity.Property(e => e.OfficeHoursId).HasColumnName("officeHoursId");

                entity.Property(e => e.OwnerId).HasColumnName("ownerId");

                entity.Property(e => e.Reused).HasColumnName("reused");

                entity.Property(e => e.ScoId)
                    .HasColumnName("scoId")
                    .HasMaxLength(50);

                entity.Property(e => e.SourceCourseMeetingId).HasColumnName("sourceCourseMeetingId");

                entity.HasOne(d => d.CompanyLms)
                    .WithMany(p => p.LmsCourseMeeting)
                    .HasForeignKey(d => d.CompanyLmsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsCourseMeeting_CompanyLms");

                entity.HasOne(d => d.LmsMeetingType)
                    .WithMany(p => p.LmsCourseMeeting)
                    .HasForeignKey(d => d.LmsMeetingTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsCourseMeeting_LmsMeetingType");

                entity.HasOne(d => d.OfficeHours)
                    .WithMany(p => p.LmsCourseMeeting)
                    .HasForeignKey(d => d.OfficeHoursId)
                    .HasConstraintName("FK_LmsCourseMeeting_OfficeHours");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.LmsCourseMeeting)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_LmsCourseMeeting_LmsUser");
            });

            modelBuilder.Entity<LmsCourseMeetingGuest>(entity =>
            {
                entity.HasIndex(e => new { e.LmsCourseMeetingId, e.PrincipalId })
                    .HasName("UI_LmsCourseMeetingGuest_lmsCourseMeetingId_principalId")
                    .IsUnique();

                entity.Property(e => e.LmsCourseMeetingGuestId).HasColumnName("lmsCourseMeetingGuestId");

                entity.Property(e => e.LmsCourseMeetingId).HasColumnName("lmsCourseMeetingId");

                entity.Property(e => e.PrincipalId)
                    .IsRequired()
                    .HasColumnName("principalId")
                    .HasMaxLength(30);

                entity.HasOne(d => d.LmsCourseMeeting)
                    .WithMany(p => p.LmsCourseMeetingGuest)
                    .HasForeignKey(d => d.LmsCourseMeetingId)
                    .HasConstraintName("FK_LmsCourseMeetingGuest_LmsCourseMeeting");
            });

            modelBuilder.Entity<LmsCourseMeetingRecording>(entity =>
            {
                entity.HasIndex(e => new { e.LmsCourseMeetingId, e.ScoId })
                    .HasName("UI_LmsCourseMeetingRecording_lmsCourseMeetingId_[scoId")
                    .IsUnique();

                entity.Property(e => e.LmsCourseMeetingRecordingId).HasColumnName("lmsCourseMeetingRecordingId");

                entity.Property(e => e.LmsCourseMeetingId).HasColumnName("lmsCourseMeetingId");

                entity.Property(e => e.ScoId)
                    .IsRequired()
                    .HasColumnName("scoId")
                    .HasMaxLength(50);

                entity.HasOne(d => d.LmsCourseMeeting)
                    .WithMany(p => p.LmsCourseMeetingRecording)
                    .HasForeignKey(d => d.LmsCourseMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsCourseMeetingRecording_LmsCourseMeeting");
            });

            modelBuilder.Entity<LmsCourseSection>(entity =>
            {
                entity.Property(e => e.LmsCourseSectionId).HasColumnName("lmsCourseSectionId");

                entity.Property(e => e.LmsCourseMeetingId).HasColumnName("lmsCourseMeetingId");

                entity.Property(e => e.LmsId)
                    .IsRequired()
                    .HasColumnName("lmsId")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(256);

                entity.HasOne(d => d.LmsCourseMeeting)
                    .WithMany(p => p.LmsCourseSection)
                    .HasForeignKey(d => d.LmsCourseMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsCourseSection_LmsCourseMeeting");
            });

            modelBuilder.Entity<LmsMeetingSession>(entity =>
            {
                entity.Property(e => e.LmsMeetingSessionId).HasColumnName("lmsMeetingSessionId");

                entity.Property(e => e.EndDate).HasColumnName("endDate");

                entity.Property(e => e.LmsCalendarEventId)
                    .HasColumnName("lmsCalendarEventId")
                    .HasMaxLength(50);

                entity.Property(e => e.LmsCourseMeetingId).HasColumnName("lmsCourseMeetingId");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(200);

                entity.Property(e => e.StartDate).HasColumnName("startDate");

                entity.Property(e => e.Summary)
                    .HasColumnName("summary")
                    .HasMaxLength(2000);

                entity.HasOne(d => d.LmsCourseMeeting)
                    .WithMany(p => p.LmsMeetingSession)
                    .HasForeignKey(d => d.LmsCourseMeetingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsMeetingSession_LmsCourseMeeting");
            });

            modelBuilder.Entity<LmsMeetingType>(entity =>
            {
                entity.HasIndex(e => e.LmsMeetingTypeName)
                    .HasName("UI_LmsMeetingType_lmsMeetingTypeName")
                    .IsUnique();

                entity.Property(e => e.LmsMeetingTypeId).HasColumnName("lmsMeetingTypeId");

                entity.Property(e => e.LmsMeetingTypeName)
                    .IsRequired()
                    .HasColumnName("lmsMeetingTypeName")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<LmsProvider>(entity =>
            {
                entity.HasIndex(e => e.LmsProvider1)
                    .HasName("UI_LmsProvider_lmsProvider")
                    .IsUnique();

                entity.HasIndex(e => e.ShortName)
                    .HasName("UI_LmsProvider_shortName")
                    .IsUnique();

                entity.Property(e => e.LmsProviderId)
                    .HasColumnName("lmsProviderId")
                    .ValueGeneratedNever();

                entity.Property(e => e.ConfigurationUrl)
                    .HasColumnName("configurationUrl")
                    .HasMaxLength(100);

                entity.Property(e => e.LmsProvider1)
                    .IsRequired()
                    .HasColumnName("lmsProvider")
                    .HasMaxLength(50);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasColumnName("shortName")
                    .HasMaxLength(50);

                entity.Property(e => e.UserGuideFileUrl)
                    .HasColumnName("userGuideFileUrl")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<LmsQuestionType>(entity =>
            {
                entity.Property(e => e.LmsQuestionTypeId).HasColumnName("lmsQuestionTypeId");

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.LmsQuestionType1)
                    .IsRequired()
                    .HasColumnName("lmsQuestionType")
                    .HasMaxLength(50);

                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.SubModuleId).HasColumnName("subModuleId");

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.LmsQuestionType)
                    .HasForeignKey(d => d.LmsProviderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsQuestionType_LmsProvider");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.LmsQuestionType)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LmsQuestionType_QuestionType");
            });

            modelBuilder.Entity<LmsUser>(entity =>
            {
                entity.HasIndex(e => new { e.CompanyLmsId, e.UserId })
                    .IsUnique();

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.Property(e => e.AcConnectionMode)
                    .HasColumnName("acConnectionMode")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.AcPasswordData).HasColumnName("acPasswordData");

                entity.Property(e => e.CompanyLmsId).HasColumnName("companyLmsId");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(254);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(50);

                entity.Property(e => e.PrimaryColor)
                    .HasColumnName("primaryColor")
                    .HasMaxLength(50);

                entity.Property(e => e.PrincipalId)
                    .HasColumnName("principalId")
                    .HasMaxLength(30);

                entity.Property(e => e.SharedKey).HasColumnName("sharedKey");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId")
                    .HasMaxLength(64);

                entity.Property(e => e.UserIdExtended)
                    .HasColumnName("userIdExtended")
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(128);

                entity.HasOne(d => d.CompanyLmsNavigation)
                    .WithMany(p => p.LmsUser)
                    .HasForeignKey(d => d.CompanyLmsId)
                    .HasConstraintName("FK_LmsUser_CompanyLms");
            });

            modelBuilder.Entity<LmsUserMeetingRole>(entity =>
            {
                entity.Property(e => e.LmsUserMeetingRoleId).HasColumnName("lmsUserMeetingRoleId");

                entity.Property(e => e.LmsCourseMeetingId).HasColumnName("lmsCourseMeetingId");

                entity.Property(e => e.LmsRole)
                    .HasColumnName("lmsRole")
                    .HasMaxLength(100);

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.HasOne(d => d.LmsCourseMeeting)
                    .WithMany(p => p.LmsUserMeetingRole)
                    .HasForeignKey(d => d.LmsCourseMeetingId)
                    .HasConstraintName("FK_LmsUserMeetingRole_LmsCourseMeeting");

                entity.HasOne(d => d.LmsUser)
                    .WithMany(p => p.LmsUserMeetingRole)
                    .HasForeignKey(d => d.LmsUserId)
                    .HasConstraintName("FK_LmsUserMeetingRole_LmsUser");
            });

            modelBuilder.Entity<LmsUserParameters>(entity =>
            {
                entity.Property(e => e.LmsUserParametersId).HasColumnName("lmsUserParametersId");

                entity.Property(e => e.AcId)
                    .IsRequired()
                    .HasColumnName("acId")
                    .HasMaxLength(10);

                entity.Property(e => e.CompanyLmsId).HasColumnName("companyLmsId");

                entity.Property(e => e.Course).HasColumnName("course");

                entity.Property(e => e.CourseName)
                    .HasColumnName("courseName")
                    .HasMaxLength(4000);

                entity.Property(e => e.LastLoggedIn)
                    .HasColumnName("lastLoggedIn")
                    .HasColumnType("datetime");

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.Property(e => e.UserEmail)
                    .HasColumnName("userEmail")
                    .HasMaxLength(254);

                entity.Property(e => e.Wstoken)
                    .HasColumnName("wstoken")
                    .HasMaxLength(128);

                entity.HasOne(d => d.CompanyLms)
                    .WithMany(p => p.LmsUserParameters)
                    .HasForeignKey(d => d.CompanyLmsId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_LmsUserParameters_CompanyLms");

                entity.HasOne(d => d.LmsUser)
                    .WithMany(p => p.LmsUserParameters)
                    .HasForeignKey(d => d.LmsUserId)
                    .HasConstraintName("FK_LmsUserParameters_LmsUser");
            });

            modelBuilder.Entity<LmsUserSession>(entity =>
            {
                entity.Property(e => e.LmsUserSessionId)
                    .HasColumnName("lmsUserSessionId")
                    .ValueGeneratedNever();

                entity.Property(e => e.CompanyLmsId).HasColumnName("companyLmsId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.LmsCourseId).HasColumnName("lmsCourseId");

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.Property(e => e.SessionData)
                    .HasColumnName("sessionData")
                    .HasColumnType("ntext");

                entity.Property(e => e.ZoomAccessToken)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.Property(e => e.ZoomRefreshToken)
                    .HasMaxLength(512)
                    .IsUnicode(false);

                entity.HasOne(d => d.CompanyLms)
                    .WithMany(p => p.LmsUserSession)
                    .HasForeignKey(d => d.CompanyLmsId)
                    .HasConstraintName("FK_LmsUserSession_CompanyLms");

                entity.HasOne(d => d.LmsUser)
                    .WithMany(p => p.LmsUserSession)
                    .HasForeignKey(d => d.LmsUserId)
                    .HasConstraintName("FK_LmsUserSession_LmsUser");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.ModuleId).HasColumnName("moduleId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnName("moduleName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OfficeHours>(entity =>
            {
                entity.HasIndex(e => e.LmsUserId)
                    .HasName("UI_OfficeHours_lmsUserId")
                    .IsUnique();

                entity.Property(e => e.OfficeHoursId).HasColumnName("officeHoursId");

                entity.Property(e => e.Hours)
                    .HasColumnName("hours")
                    .HasMaxLength(100);

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.Property(e => e.ScoId)
                    .IsRequired()
                    .HasColumnName("scoId")
                    .HasMaxLength(50);

                entity.HasOne(d => d.LmsUser)
                    .WithOne(p => p.OfficeHours)
                    .HasForeignKey<OfficeHours>(d => d.LmsUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OfficeHours_LmsUser");
            });

            modelBuilder.Entity<OfficeHoursSlot>(entity =>
            {
                entity.Property(e => e.OfficeHoursSlotId).HasColumnName("officeHoursSlotId");

                entity.Property(e => e.AvailabilityId).HasColumnName("availabilityId");

                entity.Property(e => e.End).HasColumnName("end");

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.Property(e => e.Questions)
                    .HasColumnName("questions")
                    .HasMaxLength(2000);

                entity.Property(e => e.Start).HasColumnName("start");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasMaxLength(200);

                entity.HasOne(d => d.Availability)
                    .WithMany(p => p.OfficeHoursSlot)
                    .HasForeignKey(d => d.AvailabilityId);

                entity.HasOne(d => d.LmsUser)
                    .WithMany(p => p.OfficeHoursSlot)
                    .HasForeignKey(d => d.LmsUserId);
            });

            modelBuilder.Entity<OfficeHoursTeacherAvailability>(entity =>
            {
                entity.Property(e => e.OfficeHoursTeacherAvailabilityId).HasColumnName("officeHoursTeacherAvailabilityId");

                entity.Property(e => e.DaysOfWeek)
                    .IsRequired()
                    .HasColumnName("daysOfWeek")
                    .HasMaxLength(20);

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.Intervals)
                    .IsRequired()
                    .HasColumnName("intervals")
                    .HasMaxLength(1000);

                entity.Property(e => e.LmsUserId).HasColumnName("lmsUserId");

                entity.Property(e => e.OfficeHoursId).HasColumnName("officeHoursId");

                entity.Property(e => e.PeriodEnd).HasColumnName("periodEnd");

                entity.Property(e => e.PeriodStart).HasColumnName("periodStart");

                entity.HasOne(d => d.LmsUser)
                    .WithMany(p => p.OfficeHoursTeacherAvailability)
                    .HasForeignKey(d => d.LmsUserId);

                entity.HasOne(d => d.OfficeHours)
                    .WithMany(p => p.OfficeHoursTeacherAvailability)
                    .HasForeignKey(d => d.OfficeHoursId);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.CorrectMessage).HasColumnName("correctMessage");

                entity.Property(e => e.CorrectReference)
                    .HasColumnName("correctReference")
                    .HasMaxLength(2000);

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Hint).HasColumnName("hint");

                entity.Property(e => e.HtmlText)
                    .HasColumnName("htmlText")
                    .HasMaxLength(2000);

                entity.Property(e => e.ImageId).HasColumnName("imageId");

                entity.Property(e => e.IncorrectMessage).HasColumnName("incorrectMessage");

                entity.Property(e => e.Instruction).HasColumnName("instruction");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsMoodleSingle).HasColumnName("isMoodleSingle");

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.LmsQuestionId).HasColumnName("lmsQuestionId");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.Question1)
                    .IsRequired()
                    .HasColumnName("question")
                    .HasMaxLength(2000);

                entity.Property(e => e.QuestionOrder).HasColumnName("questionOrder");

                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.RandomizeAnswers).HasColumnName("randomizeAnswers");

                entity.Property(e => e.Rows).HasColumnName("rows");

                entity.Property(e => e.ScoreValue).HasColumnName("scoreValue");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_Question_Image");

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.LmsProviderId)
                    .HasConstraintName("FK_Question_LmsProvider");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_QuestionType");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.Question)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .HasConstraintName("FK_Question_SubModuleItem");
            });

            modelBuilder.Entity<QuestionForLikert>(entity =>
            {
                entity.Property(e => e.QuestionForLikertId).HasColumnName("questionForLikertId");

                entity.Property(e => e.AllowOther).HasColumnName("allowOther");

                entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionForLikert)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionForLikert_Question");
            });

            modelBuilder.Entity<QuestionForOpenAnswer>(entity =>
            {
                entity.Property(e => e.QuestionForOpenAnswerId).HasColumnName("questionForOpenAnswerId");

                entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.Restrictions)
                    .HasColumnName("restrictions")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionForOpenAnswer)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionForOpenAnswer_Question");
            });

            modelBuilder.Entity<QuestionForRate>(entity =>
            {
                entity.Property(e => e.QuestionForRateId).HasColumnName("questionForRateId");

                entity.Property(e => e.AllowOther).HasColumnName("allowOther");

                entity.Property(e => e.IsAlwaysRateDropdown).HasColumnName("isAlwaysRateDropdown");

                entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.Restrictions)
                    .HasColumnName("restrictions")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionForRate)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionForRate_Question");
            });

            modelBuilder.Entity<QuestionForSingleMultipleChoice>(entity =>
            {
                entity.Property(e => e.QuestionForSingleMultipleChoiceId).HasColumnName("questionForSingleMultipleChoiceId");

                entity.Property(e => e.AllowOther).HasColumnName("allowOther");

                entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.Restrictions)
                    .HasColumnName("restrictions")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionForSingleMultipleChoice)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionForSingleMultipleChoice_Question");
            });

            modelBuilder.Entity<QuestionForTrueFalse>(entity =>
            {
                entity.Property(e => e.QuestionForTrueFalseId).HasColumnName("questionForTrueFalseId");

                entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionForTrueFalse)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionForTrueFalse_Question");
            });

            modelBuilder.Entity<QuestionForWeightBucket>(entity =>
            {
                entity.Property(e => e.QuestionForWeightBucketId).HasColumnName("questionForWeightBucketId");

                entity.Property(e => e.AllowOther).HasColumnName("allowOther");

                entity.Property(e => e.IsMandatory).HasColumnName("isMandatory");

                entity.Property(e => e.PageNumber).HasColumnName("pageNumber");

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.TotalWeightBucket)
                    .HasColumnName("totalWeightBucket")
                    .HasColumnType("decimal(18, 9)");

                entity.Property(e => e.WeightBucketType).HasColumnName("weightBucketType");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionForWeightBucket)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionForWeightBucket_Question");
            });

            modelBuilder.Entity<QuestionHistory>(entity =>
            {
                entity.Property(e => e.QuestionHistoryId).HasColumnName("questionHistoryId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Hint)
                    .HasColumnName("hint")
                    .IsUnicode(false);

                entity.Property(e => e.ImageId).HasColumnName("imageId");

                entity.Property(e => e.IncorrectMessage)
                    .HasColumnName("incorrectMessage")
                    .IsUnicode(false);

                entity.Property(e => e.Instruction)
                    .HasColumnName("instruction")
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasColumnName("question")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.QuestionOrder).HasColumnName("questionOrder");

                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.HasOne(d => d.QuestionNavigation)
                    .WithMany(p => p.QuestionHistory)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionHistory_Question");
            });

            modelBuilder.Entity<QuestionType>(entity =>
            {
                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.CorrectText)
                    .HasColumnName("correctText")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IconSource)
                    .HasColumnName("iconSource")
                    .HasMaxLength(500);

                entity.Property(e => e.IncorrectMessage)
                    .HasColumnName("incorrectMessage")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Instruction)
                    .HasColumnName("instruction")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.QuestionTypeDescription)
                    .HasColumnName("questionTypeDescription")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.QuestionTypeOrder).HasColumnName("questionTypeOrder");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.QuizId).HasColumnName("quizId");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsPostQuiz)
                    .HasColumnName("isPostQuiz")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.LmsQuizId).HasColumnName("lmsQuizId");

                entity.Property(e => e.PassingScore).HasColumnName("passingScore");

                entity.Property(e => e.QuizFormatId).HasColumnName("quizFormatId");

                entity.Property(e => e.QuizName)
                    .IsRequired()
                    .HasColumnName("quizName")
                    .HasMaxLength(100);

                entity.Property(e => e.ScoreTypeId).HasColumnName("scoreTypeId");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.Quiz)
                    .HasForeignKey(d => d.LmsProviderId)
                    .HasConstraintName("FK_Quiz_LmsProvider");

                entity.HasOne(d => d.QuizFormat)
                    .WithMany(p => p.Quiz)
                    .HasForeignKey(d => d.QuizFormatId)
                    .HasConstraintName("FK_Quiz_QuizFormat");

                entity.HasOne(d => d.ScoreType)
                    .WithMany(p => p.Quiz)
                    .HasForeignKey(d => d.ScoreTypeId)
                    .HasConstraintName("FK_Quiz_ScoreType");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.Quiz)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .HasConstraintName("FK_Quiz_SubModuleItem");
            });

            modelBuilder.Entity<QuizFormat>(entity =>
            {
                entity.Property(e => e.QuizFormatId).HasColumnName("quizFormatId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.QuizFormatName)
                    .HasColumnName("quizFormatName")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<QuizQuestionResult>(entity =>
            {
                entity.Property(e => e.QuizQuestionResultId).HasColumnName("quizQuestionResultId");

                entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasColumnName("question")
                    .HasMaxLength(1500);

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.QuizResultId).HasColumnName("quizResultId");

                entity.HasOne(d => d.QuestionNavigation)
                    .WithMany(p => p.QuizQuestionResult)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizQuestionResult_Question");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.QuizQuestionResult)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizQuestionResult_QuestionType");

                entity.HasOne(d => d.QuizResult)
                    .WithMany(p => p.QuizQuestionResult)
                    .HasForeignKey(d => d.QuizResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizQuestionResult_QuizResult");
            });

            modelBuilder.Entity<QuizQuestionResultAnswer>(entity =>
            {
                entity.Property(e => e.QuizQuestionResultAnswerId).HasColumnName("quizQuestionResultAnswerId");

                entity.Property(e => e.QuizDistractorAnswerId).HasColumnName("quizDistractorAnswerId");

                entity.Property(e => e.QuizQuestionResultId).HasColumnName("quizQuestionResultId");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(500);

                entity.HasOne(d => d.QuizDistractorAnswer)
                    .WithMany(p => p.QuizQuestionResultAnswer)
                    .HasForeignKey(d => d.QuizDistractorAnswerId)
                    .HasConstraintName("FK_QuizQuestionResultAnswer_DistractorAnswer");

                entity.HasOne(d => d.QuizQuestionResult)
                    .WithMany(p => p.QuizQuestionResultAnswer)
                    .HasForeignKey(d => d.QuizQuestionResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizQuestionResultAnswer_QuizQuestionResult");
            });

            modelBuilder.Entity<QuizResult>(entity =>
            {
                entity.Property(e => e.QuizResultId).HasColumnName("quizResultId");

                entity.Property(e => e.AcEmail)
                    .HasColumnName("acEmail")
                    .HasMaxLength(500);

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.AppInFocusTime).HasColumnName("appInFocusTime");

                entity.Property(e => e.AppMaximizedTime).HasColumnName("appMaximizedTime");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(500);

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Guid)
                    .HasColumnName("guid")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsArchive).HasColumnName("isArchive");

                entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");

                entity.Property(e => e.LmsId).HasColumnName("lmsId");

                entity.Property(e => e.LmsUserParametersId).HasColumnName("lmsUserParametersId");

                entity.Property(e => e.ParticipantName)
                    .IsRequired()
                    .HasColumnName("participantName")
                    .HasMaxLength(200);

                entity.Property(e => e.QuizId).HasColumnName("quizId");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.AcSession)
                    .WithMany(p => p.QuizResult)
                    .HasForeignKey(d => d.AcSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizResult_ACSession");

                entity.HasOne(d => d.EventQuizMapping)
                    .WithMany(p => p.QuizResult)
                    .HasForeignKey(d => d.EventQuizMappingId)
                    .HasConstraintName("FK_QuizResult_EventQuizMapping");

                entity.HasOne(d => d.Quiz)
                    .WithMany(p => p.QuizResult)
                    .HasForeignKey(d => d.QuizId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuizResult_Quiz");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.ScheduleId).HasColumnName("scheduleId");

                entity.Property(e => e.Interval).HasColumnName("interval");

                entity.Property(e => e.IsEnabled).HasColumnName("isEnabled");

                entity.Property(e => e.NextRun)
                    .HasColumnName("nextRun")
                    .HasColumnType("datetime");

                entity.Property(e => e.ScheduleDescriptor).HasColumnName("scheduleDescriptor");

                entity.Property(e => e.ScheduleType).HasColumnName("scheduleType");
            });

            modelBuilder.Entity<ScoreType>(entity =>
            {
                entity.Property(e => e.ScoreTypeId).HasColumnName("scoreTypeId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DefaultValue)
                    .HasColumnName("defaultValue")
                    .HasDefaultValueSql("((10))");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Prefix)
                    .HasColumnName("prefix")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScoreType1)
                    .HasColumnName("scoreType")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SngroupDiscussion>(entity =>
            {
                entity.ToTable("SNGroupDiscussion");

                entity.Property(e => e.SnGroupDiscussionId).HasColumnName("snGroupDiscussionId");

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.GroupDiscussionData)
                    .IsRequired()
                    .HasColumnName("groupDiscussionData")
                    .HasColumnType("ntext");

                entity.Property(e => e.GroupDiscussionTitle)
                    .HasColumnName("groupDiscussionTitle")
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.HasOne(d => d.AcSession)
                    .WithMany(p => p.SngroupDiscussion)
                    .HasForeignKey(d => d.AcSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SNGroupDiscussion_ACSession");
            });

            modelBuilder.Entity<Snlink>(entity =>
            {
                entity.ToTable("SNLink");

                entity.Property(e => e.SnLinkId).HasColumnName("snLinkId");

                entity.Property(e => e.LinkName)
                    .IsRequired()
                    .HasColumnName("linkName")
                    .HasMaxLength(255);

                entity.Property(e => e.LinkValue)
                    .HasColumnName("linkValue")
                    .HasMaxLength(2000);

                entity.Property(e => e.SnProfileId).HasColumnName("snProfileId");

                entity.HasOne(d => d.SnProfile)
                    .WithMany(p => p.Snlink)
                    .HasForeignKey(d => d.SnProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SNLink_SNProfile");
            });

            modelBuilder.Entity<SnmapProvider>(entity =>
            {
                entity.ToTable("SNMapProvider");

                entity.Property(e => e.SnMapProviderId).HasColumnName("snMapProviderId");

                entity.Property(e => e.MapProvider)
                    .IsRequired()
                    .HasColumnName("mapProvider")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SnmapSettings>(entity =>
            {
                entity.ToTable("SNMapSettings");

                entity.Property(e => e.SnMapSettingsId).HasColumnName("snMapSettingsId");

                entity.Property(e => e.CountryId).HasColumnName("countryId");

                entity.Property(e => e.SnMapProviderId).HasColumnName("snMapProviderId");

                entity.Property(e => e.ZoomLevel).HasColumnName("zoomLevel");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.SnmapSettings)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("fk_SNMapSettings_Country");

                entity.HasOne(d => d.SnMapProvider)
                    .WithMany(p => p.SnmapSettings)
                    .HasForeignKey(d => d.SnMapProviderId)
                    .HasConstraintName("FK_SNProfileMapSettings_SNMapProvider");
            });

            modelBuilder.Entity<Snmember>(entity =>
            {
                entity.ToTable("SNMember");

                entity.Property(e => e.SnMemberId).HasColumnName("snMemberId");

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsBlocked).HasColumnName("isBlocked");

                entity.Property(e => e.Participant)
                    .IsRequired()
                    .HasColumnName("participant")
                    .HasMaxLength(255);

                entity.Property(e => e.ParticipantProfile)
                    .HasColumnName("participantProfile")
                    .HasColumnType("ntext");

                entity.HasOne(d => d.AcSession)
                    .WithMany(p => p.Snmember)
                    .HasForeignKey(d => d.AcSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SNMember_ACSession");
            });

            modelBuilder.Entity<Snprofile>(entity =>
            {
                entity.ToTable("SNProfile");

                entity.Property(e => e.SnProfileId).HasColumnName("snProfileId");

                entity.Property(e => e.About)
                    .HasColumnName("about")
                    .HasColumnType("ntext");

                entity.Property(e => e.AddressId).HasColumnName("addressId");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.JobTitle)
                    .HasColumnName("jobTitle")
                    .HasMaxLength(500);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(255);

                entity.Property(e => e.ProfileName)
                    .IsRequired()
                    .HasColumnName("profileName")
                    .HasMaxLength(255);

                entity.Property(e => e.SnMapSettingsId).HasColumnName("snMapSettingsId");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("userName")
                    .HasMaxLength(255);

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Snprofile)
                    .HasForeignKey(d => d.AddressId)
                    .HasConstraintName("FK_SNProfile_Address");

                entity.HasOne(d => d.SnMapSettings)
                    .WithMany(p => p.Snprofile)
                    .HasForeignKey(d => d.SnMapSettingsId)
                    .HasConstraintName("FK_SNProfile_SNMapSettings");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.Snprofile)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SNProfile_SubModuleItem");
            });

            modelBuilder.Entity<SnprofileSnservice>(entity =>
            {
                entity.ToTable("SNProfileSNService");

                entity.Property(e => e.SnProfileSnserviceId).HasColumnName("snProfileSNServiceId");

                entity.Property(e => e.IsEnabled).HasColumnName("isEnabled");

                entity.Property(e => e.ServiceUrl)
                    .HasColumnName("serviceUrl")
                    .HasMaxLength(2000);

                entity.Property(e => e.SnProfileId).HasColumnName("snProfileId");

                entity.Property(e => e.SnServiceId).HasColumnName("snServiceId");

                entity.HasOne(d => d.SnProfile)
                    .WithMany(p => p.SnprofileSnservice)
                    .HasForeignKey(d => d.SnProfileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SNProfileSNService_SNProfile");

                entity.HasOne(d => d.SnService)
                    .WithMany(p => p.SnprofileSnservice)
                    .HasForeignKey(d => d.SnServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SNProfileSNService_SNService");
            });

            modelBuilder.Entity<Snservice>(entity =>
            {
                entity.ToTable("SNService");

                entity.Property(e => e.SnServiceId).HasColumnName("snServiceId");

                entity.Property(e => e.SocialService)
                    .IsRequired()
                    .HasColumnName("socialService")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SocialUserTokens>(entity =>
            {
                entity.Property(e => e.SocialUserTokensId).HasColumnName("socialUserTokensId");

                entity.Property(e => e.Key)
                    .HasColumnName("key")
                    .HasMaxLength(255);

                entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasMaxLength(500);

                entity.Property(e => e.Secret)
                    .HasColumnName("secret")
                    .HasMaxLength(1000);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(1000);

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SocialUserTokens)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_SocialUserTokens_User");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(e => e.StateId).HasColumnName("stateId");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(18, 7)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(18, 7)");

                entity.Property(e => e.StateCode)
                    .IsRequired()
                    .HasColumnName("stateCode")
                    .HasMaxLength(10);

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .HasColumnName("stateName")
                    .HasMaxLength(50);

                entity.Property(e => e.ZoomLevel).HasColumnName("zoomLevel");
            });

            modelBuilder.Entity<SubModule>(entity =>
            {
                entity.Property(e => e.SubModuleId).HasColumnName("subModuleId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModuleId).HasColumnName("moduleID");

                entity.Property(e => e.SubModuleName)
                    .IsRequired()
                    .HasColumnName("subModuleName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.SubModule)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubModule_Module");
            });

            modelBuilder.Entity<SubModuleCategory>(entity =>
            {
                entity.Property(e => e.SubModuleCategoryId).HasColumnName("subModuleCategoryId");

                entity.Property(e => e.CategoryName)
                    .HasColumnName("categoryName")
                    .HasMaxLength(255);

                entity.Property(e => e.CompanyLmsId).HasColumnName("companyLmsId");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.LmsCourseId).HasColumnName("lmsCourseId");

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.SubModuleId).HasColumnName("subModuleId");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.CompanyLms)
                    .WithMany(p => p.SubModuleCategory)
                    .HasForeignKey(d => d.CompanyLmsId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_SubModuleCategory_CompanyLms");

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.SubModuleCategory)
                    .HasForeignKey(d => d.LmsProviderId)
                    .HasConstraintName("FK_SubModuleCategory_LmsProvider");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.SubModuleCategoryModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_SubModuleCategory_User");

                entity.HasOne(d => d.SubModule)
                    .WithMany(p => p.SubModuleCategory)
                    .HasForeignKey(d => d.SubModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSubModuleCategory_SubModule");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SubModuleCategoryUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSubModuleCategory_User");
            });

            modelBuilder.Entity<SubModuleItem>(entity =>
            {
                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsShared)
                    .HasColumnName("isShared")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.SubModuleCategoryId).HasColumnName("subModuleCategoryId");

                entity.HasOne(d => d.SubModuleCategory)
                    .WithMany(p => p.SubModuleItem)
                    .HasForeignKey(d => d.SubModuleCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubModuleItem_SubModuleCategory");
            });

            modelBuilder.Entity<SubModuleItemTheme>(entity =>
            {
                entity.Property(e => e.SubModuleItemThemeId).HasColumnName("subModuleItemThemeId");

                entity.Property(e => e.BgColor)
                    .HasColumnName("bgColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.BgImageId).HasColumnName("bgImageId");

                entity.Property(e => e.CorrectColor)
                    .HasColumnName("correctColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.HintColor)
                    .HasColumnName("hintColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.IncorrectColor)
                    .HasColumnName("incorrectColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.InstructionColor)
                    .HasColumnName("instructionColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.QuestionColor)
                    .HasColumnName("questionColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SelectionColor)
                    .HasColumnName("selectionColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.Property(e => e.TitleColor)
                    .HasColumnName("titleColor")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.BgImage)
                    .WithMany(p => p.SubModuleItemTheme)
                    .HasForeignKey(d => d.BgImageId)
                    .HasConstraintName("FK_SubModuleItemTheme_File");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.SubModuleItemTheme)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SubModuleItemTheme_SubModuleItem");
            });

            modelBuilder.Entity<SubscriptionHistoryLog>(entity =>
            {
                entity.Property(e => e.SubscriptionHistoryLogId).HasColumnName("subscriptionHistoryLogId");

                entity.Property(e => e.LastQueryTime)
                    .HasColumnName("lastQueryTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.SubscriptionId).HasColumnName("subscriptionId");

                entity.Property(e => e.SubscriptionTag)
                    .IsRequired()
                    .HasColumnName("subscriptionTag")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<SubscriptionUpdate>(entity =>
            {
                entity.Property(e => e.SubscriptionUpdateId).HasColumnName("subscriptionUpdateId");

                entity.Property(e => e.ChangedAspect)
                    .IsRequired()
                    .HasColumnName("changed_aspect")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("createdDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Object)
                    .IsRequired()
                    .HasColumnName("object")
                    .HasMaxLength(20);

                entity.Property(e => e.ObjectId)
                    .IsRequired()
                    .HasColumnName("object_id")
                    .HasMaxLength(1000);

                entity.Property(e => e.SubscriptionId).HasColumnName("subscription_id");

                entity.Property(e => e.Time)
                    .IsRequired()
                    .HasColumnName("time")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.Property(e => e.SurveyId).HasColumnName("surveyId");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.LmsProviderId).HasColumnName("lmsProviderId");

                entity.Property(e => e.LmsSurveyId).HasColumnName("lmsSurveyId");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.Property(e => e.SurveyGroupingTypeId).HasColumnName("surveyGroupingTypeId");

                entity.Property(e => e.SurveyName)
                    .IsRequired()
                    .HasColumnName("surveyName")
                    .HasMaxLength(255);

                entity.HasOne(d => d.LmsProvider)
                    .WithMany(p => p.Survey)
                    .HasForeignKey(d => d.LmsProviderId)
                    .HasConstraintName("FK_Survey_LmsProvider");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.Survey)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .HasConstraintName("FK_Survey_SubModuleItem");

                entity.HasOne(d => d.SurveyGroupingType)
                    .WithMany(p => p.Survey)
                    .HasForeignKey(d => d.SurveyGroupingTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Survey_SurveyGroupingType");
            });

            modelBuilder.Entity<SurveyGroupingType>(entity =>
            {
                entity.Property(e => e.SurveyGroupingTypeId).HasColumnName("surveyGroupingTypeId");

                entity.Property(e => e.SurveyGroupingType1)
                    .IsRequired()
                    .HasColumnName("surveyGroupingType")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<SurveyQuestionResult>(entity =>
            {
                entity.Property(e => e.SurveyQuestionResultId).HasColumnName("surveyQuestionResultId");

                entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasColumnName("question")
                    .HasMaxLength(500);

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.SurveyResultId).HasColumnName("surveyResultId");

                entity.HasOne(d => d.QuestionNavigation)
                    .WithMany(p => p.SurveyQuestionResult)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyQuestionResult_Question");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.SurveyQuestionResult)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyQuestionResult_QuestionType");

                entity.HasOne(d => d.SurveyResult)
                    .WithMany(p => p.SurveyQuestionResult)
                    .HasForeignKey(d => d.SurveyResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyQuestionResult_SurveyResult");
            });

            modelBuilder.Entity<SurveyQuestionResultAnswer>(entity =>
            {
                entity.Property(e => e.SurveyQuestionResultAnswerId).HasColumnName("surveyQuestionResultAnswerId");

                entity.Property(e => e.SurveyDistractorAnswerId).HasColumnName("surveyDistractorAnswerId");

                entity.Property(e => e.SurveyDistractorId).HasColumnName("surveyDistractorId");

                entity.Property(e => e.SurveyQuestionResultId).HasColumnName("surveyQuestionResultId");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(500);

                entity.HasOne(d => d.SurveyDistractorAnswer)
                    .WithMany(p => p.SurveyQuestionResultAnswerSurveyDistractorAnswer)
                    .HasForeignKey(d => d.SurveyDistractorAnswerId)
                    .HasConstraintName("FK_SurveyQuestionResultAnswer_SurveyQuestionResultAnswer");

                entity.HasOne(d => d.SurveyDistractor)
                    .WithMany(p => p.SurveyQuestionResultAnswerSurveyDistractor)
                    .HasForeignKey(d => d.SurveyDistractorId)
                    .HasConstraintName("FK_SurveyQuestionResultAnswer_Distractor");

                entity.HasOne(d => d.SurveyQuestionResult)
                    .WithMany(p => p.SurveyQuestionResultAnswer)
                    .HasForeignKey(d => d.SurveyQuestionResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyQuestionResultAnswer_SurveyQuestionResult");
            });

            modelBuilder.Entity<SurveyResult>(entity =>
            {
                entity.Property(e => e.SurveyResultId).HasColumnName("surveyResultId");

                entity.Property(e => e.AcEmail)
                    .HasColumnName("acEmail")
                    .HasMaxLength(500);

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(500);

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsArchive).HasColumnName("isArchive");

                entity.Property(e => e.LmsUserParametersId).HasColumnName("lmsUserParametersId");

                entity.Property(e => e.ParticipantName)
                    .IsRequired()
                    .HasColumnName("participantName")
                    .HasMaxLength(50);

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SurveyId).HasColumnName("surveyId");

                entity.HasOne(d => d.AcSession)
                    .WithMany(p => p.SurveyResult)
                    .HasForeignKey(d => d.AcSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SurveyResult_Survey");

                entity.HasOne(d => d.LmsUserParameters)
                    .WithMany(p => p.SurveyResult)
                    .HasForeignKey(d => d.LmsUserParametersId)
                    .HasConstraintName("FK_SurveyResult_LmsUserParameters");
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.Property(e => e.TestId).HasColumnName("testId");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.InstructionDescription).HasColumnName("instructionDescription");

                entity.Property(e => e.InstructionTitle).HasColumnName("instructionTitle");

                entity.Property(e => e.PassingScore)
                    .HasColumnName("passingScore")
                    .HasColumnType("decimal(18, 9)");

                entity.Property(e => e.ScoreFormat)
                    .HasColumnName("scoreFormat")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ScoreTypeId).HasColumnName("scoreTypeId");

                entity.Property(e => e.SubModuleItemId).HasColumnName("subModuleItemId");

                entity.Property(e => e.TestName)
                    .IsRequired()
                    .HasColumnName("testName")
                    .HasMaxLength(50);

                entity.Property(e => e.TimeLimit).HasColumnName("timeLimit");

                entity.HasOne(d => d.ScoreType)
                    .WithMany(p => p.Test)
                    .HasForeignKey(d => d.ScoreTypeId)
                    .HasConstraintName("FK_Test_ScoreType");

                entity.HasOne(d => d.SubModuleItem)
                    .WithMany(p => p.Test)
                    .HasForeignKey(d => d.SubModuleItemId)
                    .HasConstraintName("FK_Test_SubModuleItem");
            });

            modelBuilder.Entity<TestQuestionResult>(entity =>
            {
                entity.Property(e => e.TestQuestionResultId).HasColumnName("testQuestionResultId");

                entity.Property(e => e.IsCorrect).HasColumnName("isCorrect");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasColumnName("question")
                    .HasMaxLength(500);

                entity.Property(e => e.QuestionId).HasColumnName("questionId");

                entity.Property(e => e.QuestionTypeId).HasColumnName("questionTypeId");

                entity.Property(e => e.TestResultId).HasColumnName("testResultId");

                entity.HasOne(d => d.QuestionNavigation)
                    .WithMany(p => p.TestQuestionResult)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestQuestionResult_Question");

                entity.HasOne(d => d.QuestionType)
                    .WithMany(p => p.TestQuestionResult)
                    .HasForeignKey(d => d.QuestionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestQuestionResult_QuestionType");

                entity.HasOne(d => d.TestResult)
                    .WithMany(p => p.TestQuestionResult)
                    .HasForeignKey(d => d.TestResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestQuestionResult_TestResult");
            });

            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.Property(e => e.TestResultId).HasColumnName("testResultId");

                entity.Property(e => e.AcEmail)
                    .HasColumnName("acEmail")
                    .HasMaxLength(500);

                entity.Property(e => e.AcSessionId).HasColumnName("acSessionId");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(500);

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsArchive).HasColumnName("isArchive");

                entity.Property(e => e.IsCompleted).HasColumnName("isCompleted");

                entity.Property(e => e.ParticipantName)
                    .IsRequired()
                    .HasColumnName("participantName")
                    .HasMaxLength(200);

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.TestId).HasColumnName("testId");

                entity.HasOne(d => d.AcSession)
                    .WithMany(p => p.TestResult)
                    .HasForeignKey(d => d.AcSessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestResult_ACSession");

                entity.HasOne(d => d.Test)
                    .WithMany(p => p.TestResult)
                    .HasForeignKey(d => d.TestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TestResult_Test");
            });

            modelBuilder.Entity<Theme>(entity =>
            {
                entity.Property(e => e.ThemeId).HasColumnName("themeId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsActive)
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.ThemeName)
                    .IsRequired()
                    .HasColumnName("themeName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ThemeCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Theme_UserCreated");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ThemeModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_Theme_UserModified");
            });

            modelBuilder.Entity<TimeZone>(entity =>
            {
                entity.HasIndex(e => e.TimeZone1)
                    .HasName("UI_TimeZone_timeZone")
                    .IsUnique();

                entity.Property(e => e.TimeZoneId).HasColumnName("timeZoneId");

                entity.Property(e => e.TimeZone1)
                    .IsRequired()
                    .HasColumnName("timeZone")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZoneGmtdiff).HasColumnName("timeZoneGMTDiff");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.CompanyId).HasColumnName("companyId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateModified)
                    .HasColumnName("dateModified")
                    .HasColumnType("smalldatetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(450)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("firstName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsUnsubscribed).HasColumnName("isUnsubscribed");

                entity.Property(e => e.LanguageId).HasColumnName("languageId");

                entity.Property(e => e.LastName)
                    .HasColumnName("lastName")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LogoId).HasColumnName("logoId");

                entity.Property(e => e.ModifiedBy).HasColumnName("modifiedBy");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SessionToken)
                    .HasColumnName("sessionToken")
                    .HasMaxLength(64);

                entity.Property(e => e.SessionTokenExpirationDate)
                    .HasColumnName("sessionTokenExpirationDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TimeZoneId).HasColumnName("timeZoneId");

                entity.Property(e => e.UserRoleId).HasColumnName("userRoleId");

                entity.HasOne(d => d.CompanyNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Company");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.InverseCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_User_CreatedBy");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_Language");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.InverseModifiedByNavigation)
                    .HasForeignKey(d => d.ModifiedBy)
                    .HasConstraintName("FK_User_Modified");

                entity.HasOne(d => d.TimeZone)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.TimeZoneId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_TimeZone");

                entity.HasOne(d => d.UserRole)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserRole");
            });

            modelBuilder.Entity<UserActivation>(entity =>
            {
                entity.Property(e => e.UserActivationId).HasColumnName("userActivationId");

                entity.Property(e => e.ActivationCode)
                    .IsRequired()
                    .HasColumnName("activationCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateExpires)
                    .HasColumnName("dateExpires")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserActivation)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserActivation_User");
            });

            modelBuilder.Entity<UserLoginHistory>(entity =>
            {
                entity.Property(e => e.UserLoginHistoryId).HasColumnName("userLoginHistoryId");

                entity.Property(e => e.Application)
                    .HasColumnName("application")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreated)
                    .HasColumnName("dateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.FromIp)
                    .HasColumnName("fromIP")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLoginHistory)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserLoginHistory_User");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.UserRoleId).HasColumnName("userRoleId");

                entity.Property(e => e.UserRole1)
                    .IsRequired()
                    .HasColumnName("userRole")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Webinar>(entity =>
            {
                entity.Property(e => e.WebinarId).HasColumnName("webinar_id");

                entity.Property(e => e.WebinarDate)
                    .HasColumnName("webinar_date")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.WebinarDescription)
                    .HasColumnName("webinar_description")
                    .HasMaxLength(255);

                entity.Property(e => e.WebinarHost)
                    .HasColumnName("webinar_host")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<WftSchool>(entity =>
            {
                entity.HasKey(e => e.SchoolId)
                    .HasName("PK_School");

                entity.ToTable("_wft_School");

                entity.Property(e => e.AccountName).HasMaxLength(100);

                entity.Property(e => e.AdvRepresentative).HasMaxLength(200);

                entity.Property(e => e.CorporateName).HasMaxLength(100);

                entity.Property(e => e.Essrepresentative)
                    .HasColumnName("ESSRepresentative")
                    .HasMaxLength(200);

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.Fbcrepresentative)
                    .HasColumnName("FBCRepresentative")
                    .HasMaxLength(200);

                entity.Property(e => e.FirstDirector).HasMaxLength(200);

                entity.Property(e => e.MainPhone).HasMaxLength(20);

                entity.Property(e => e.MktgRepresentative).HasMaxLength(200);

                entity.Property(e => e.OnsiteOperator).HasMaxLength(500);

                entity.Property(e => e.SchoolEmail).HasMaxLength(100);

                entity.Property(e => e.SchoolNumber).HasMaxLength(20);

                entity.Property(e => e.SpeedDialNumber).HasMaxLength(20);

                entity.Property(e => e.StandardsRepresentative).HasMaxLength(200);

                entity.HasOne(d => d.State)
                    .WithMany(p => p.WftSchool)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__School__StateId__674A37FD");
            });
        }
    }
}
