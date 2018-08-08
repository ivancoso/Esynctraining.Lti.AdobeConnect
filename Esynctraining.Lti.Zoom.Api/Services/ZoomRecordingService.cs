﻿using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Zoom.ApiWrapper;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomRecordingService
    {
        private readonly ZoomApiWrapper _zoomApi;


        public ZoomRecordingService(ZoomApiWrapper zoomApi)
        {
            _zoomApi = zoomApi ?? throw new ArgumentNullException(nameof(zoomApi));
        }


        public IEnumerable<ZoomRecordingFileDto> GetRecordings(string meetingId, bool trash = false)
        {
            var meetings = _zoomApi.GetRecordings(meetingId, trash);

            return meetings.RecordingFiles.Where(x => x.FileType.ToUpper() == "MP4").Select(x => new ZoomRecordingFileDto
            {
                Id = x.Id,
                StartTime = x.RecordingStart.DateTime,
                EndTime = x.RecordingEnd?.DateTime,
                //Duration = x.,
                //Topic = meetings.Topic,
                Status = x.Status,
                DownloadUrl = x.DownloadUrl,
                ViewUrl = x.PlayUrl,
                FileType = x.FileType,
                FileSize = x.FileSize,
            });
        }

        public IEnumerable<ZoomRecordingSessionDto> GetRecordings(string userId, string meetingId)
        {
            var result = new List<ZoomRecordingSessionDto>();
            var apiRecordings = _zoomApi.GetUserRecordings(userId, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1));
            var recordings = apiRecordings.Meetings.Where(x => x.Id == meetingId);
            foreach (var rec in recordings)
            {
                var dto = new ZoomRecordingSessionDto
                {
                    Id = rec.Uuid,
                    Duration = rec.Duration,
                    RecordingCount = rec.RecordingCount,
                    StartTime = rec.StartTime.DateTime,
                    Topic = rec.Topic,
                    TotalSize = rec.TotalSize,
                    Recordings = new List<ZoomRecordingDto>()
                };
                var recFiles = rec.RecordingFiles.Select(x => new ZoomRecordingFileDto
                {
                    Id = x.Id ?? x.MeetingId, //in case when deleted all recordings for meeting, there is one record for restore all items
                    ViewUrl = x.PlayUrl,
                    DownloadUrl = x.DownloadUrl,
                    Status = x.Status,
                    StartTime = x.RecordingStart.DateTime,
                    EndTime = x.RecordingEnd?.DateTime,
                    FileType = x.FileType,
                    FileSize = x.FileSize,
                });
                var grouped = recFiles.GroupBy(x => x.StartTime).Select(x => new ZoomRecordingDto
                {
                    StartTime = x.Key,
                    Files = x.ToList()
                }).ToList();
                dto.Recordings = grouped;
                result.Add(dto);
            }

            return result;
        }

        public IEnumerable<ZoomRecordingsTrashItemDto> GetTrashRecordings(string meetingId, string userId)
        {
            var result = new List<ZoomRecordingsTrashItemDto>();
            var apiRecordings = _zoomApi.GetUserRecordings(userId, trash: true);
            var recordings = apiRecordings.Meetings.Where(x => x.Id == meetingId);
            foreach (var rec in recordings)
            {
                ///We have two files VIDEO and AUDIO
                /// We delete video file to trash and after that delete permamently this video file
                /// We delete audio file to trash
                /// Go to trash and we will see permamently deleted VIDEO file. 
                /// ZOOM Api returns this file fith ID = null
                /// So we filter this null file records.
                var recordingFiles = rec.RecordingFiles.Where(f => f.Id != null);
                foreach (var file in recordingFiles)
                {
                    var dto = new ZoomRecordingsTrashItemDto
                    {
                        Topic = rec.Topic,
                        MeetingId = rec.Id,
                        RecordingCount = rec.RecordingCount,
                        StartTime = file.RecordingStart.DateTime,
                        RecordingFileId = file.Id,
                        RecordingId = file.MeetingId,
                        DeletedTime = file.DeletedTime?.DateTime,
                        FileSize = file.FileSize,
                        FileType = file.FileType,
                    };

                    result.Add(dto);
                }
            }

            return result;
        }

        public bool DeleteRecordings(string recordingId, string recordingFileId = null, bool trash = true)
        {
            //check permissions


            return _zoomApi.DeleteRecording(recordingId, recordingFileId, trash);
        }

        public bool RecoverRecordings(string recordingId, string recordingFileId = null)
        {
            //check permissions
            return _zoomApi.RecoverRecording(recordingId, recordingFileId);
        }

        /// <summary>
        /// In zoom API this field is called "MeetingId" for recording file or "Uuid" for meeting object(recording). There is no "session" term in zoom API
        /// </summary>
        public string GetMeetingUuId(string userId, string meetingId, string recordingFileId, bool trash = true)
        {
            // Zoom Api returns only date for last Month. Need too implement cycle for all period from Creating meeting.
            var apiRecordings = trash
                ? _zoomApi.GetUserRecordings(userId, trash: true)
                : _zoomApi.GetUserRecordings(userId, DateTime.Now.AddDays(-30), DateTime.Now.AddDays(1));

            var meetingSession = apiRecordings.Meetings.FirstOrDefault(x =>
                x.Id == meetingId && x.RecordingFiles.Any(rf =>
                    rf.Id != null &&  rf.Id.Equals(recordingFileId, StringComparison.InvariantCultureIgnoreCase)));

            return meetingSession?.Uuid;
        }

    }

}