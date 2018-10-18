﻿// <auto-generated />
using System;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    [DbContext(typeof(ZoomDbContext))]
    partial class ZoomDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.ExternalFileInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ProviderFileRecordId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("ProviderId");

                    b.Property<int?>("lmsCourseMeetingId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("lmsCourseMeetingId");

                    b.ToTable("ExternalFileInfo");
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.LmsCourseMeeting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Details")
                        .IsRequired();

                    b.Property<Guid>("LicenseKey");

                    b.Property<string>("ProviderHostId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("ProviderMeetingId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<bool>("Reused");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("LmsCourseMeeting");
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.LmsMeetingSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("Summary")
                        .HasMaxLength(2000);

                    b.Property<int?>("lmsCourseMeetingId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("lmsCourseMeetingId");

                    b.ToTable("LmsMeetingSession");
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.LmsUserSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CourseId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<Guid>("LicenseKey");

                    b.Property<string>("LmsUserId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(200);

                    b.Property<string>("SessionData")
                        .IsRequired();

                    b.Property<string>("Token")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("LmsUserSession");
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.OfficeHoursSlot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("End");

                    b.Property<string>("LmsUserId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Questions")
                        .HasMaxLength(2000);

                    b.Property<string>("RequesterName")
                        .HasMaxLength(200);

                    b.Property<DateTime>("Start");

                    b.Property<int>("Status");

                    b.Property<string>("Subject")
                        .HasMaxLength(200);

                    b.Property<int?>("availabilityId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("availabilityId");

                    b.ToTable("OfficeHoursSlot");
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.OfficeHoursTeacherAvailability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DaysOfWeek")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("Duration");

                    b.Property<string>("Intervals")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<string>("LmsUserId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("PeriodEnd");

                    b.Property<DateTime>("PeriodStart");

                    b.Property<int?>("lmsCourseMeetingId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("lmsCourseMeetingId");

                    b.ToTable("OfficeHoursTeacherAvailability");
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.ExternalFileInfo", b =>
                {
                    b.HasOne("Esynctraining.Lti.Zoom.Domain.LmsCourseMeeting", "Meeting")
                        .WithMany()
                        .HasForeignKey("lmsCourseMeetingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.LmsMeetingSession", b =>
                {
                    b.HasOne("Esynctraining.Lti.Zoom.Domain.LmsCourseMeeting", "Meeting")
                        .WithMany("MeetingSessions")
                        .HasForeignKey("lmsCourseMeetingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.OfficeHoursSlot", b =>
                {
                    b.HasOne("Esynctraining.Lti.Zoom.Domain.OfficeHoursTeacherAvailability", "Availability")
                        .WithMany("Slots")
                        .HasForeignKey("availabilityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Esynctraining.Lti.Zoom.Domain.OfficeHoursTeacherAvailability", b =>
                {
                    b.HasOne("Esynctraining.Lti.Zoom.Domain.LmsCourseMeeting", "Meeting")
                        .WithMany()
                        .HasForeignKey("lmsCourseMeetingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
