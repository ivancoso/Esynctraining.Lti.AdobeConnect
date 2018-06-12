using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Lti.Zoom.Api.Dto;
using Esynctraining.Lti.Zoom.Api.Dto.Enums;
using Esynctraining.Zoom.ApiWrapper;
using Esynctraining.Zoom.ApiWrapper.Model;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class ZoomUserService
    {
        private ZoomApiWrapper _zoomApi;
        public ZoomUserService(ZoomApiWrapper zoomApi)
        {
            _zoomApi = zoomApi;
        }

        public ListUsers GetUsers()
        {
            var allUsers = _zoomApi.GetUsers(UserStatuses.Active, 100, 1);
            return allUsers;
        }

        public UserInfoDto GetUser(string idOrEmail)
        {
            var user = _zoomApi.GetUser(idOrEmail);
            return user == null
                ? null
                : new UserInfoDto
                {
                    Id = user.Id,
                    Type = (int)user.Type,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Verified = user.Verified == 1,
                };

        }

        public UserDto CreateUser(CreateUserDto dto)
        {
            var user = _zoomApi.CreateUser(new CreateUser
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Password = dto.Password,
                Type = UserTypes.Basic
            }, "create");

            return new UserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }


        public List<ZoomMeetingRegistrantDto> GetMeetingRegistrants(string meetingId, string occurrenceId = null, ZoomMeetingRegistrantStatus? status = null)
        {
            var registrants = new List<ZoomMeetingRegistrantDto>();
            var statuses = new List<ZoomMeetingRegistrantStatus>();
            if (status.HasValue)
                statuses.Add(status.Value);
            else
            {
                statuses.AddRange(new[] { ZoomMeetingRegistrantStatus.Approved, ZoomMeetingRegistrantStatus.Denied, ZoomMeetingRegistrantStatus.Pending });
            }
            foreach (var st in statuses)
            {
                var users = _zoomApi.GetMeetingRegistrants(meetingId, occurrenceId, st.ToString().ToLower());
                registrants.AddRange(users.Registrants.Select(x => ConvertFromApiObjectToDto(x)));
            }

            return registrants;
        }

        public bool RegisterUsersToMeetingAndApprove(string meetingId, IEnumerable<RegistrantDto> registrants, bool checkRegistrants)
        {
            var registrantsToApprove = new List<ZoomMeetingRegistrantDto>();
            var zoomUsers = GetUsers().Users;
            foreach (var registrant in registrants)
            {
                if (zoomUsers.Any(x => x.Email.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase)))
                {
                    bool addRegistrant = !checkRegistrants;
                    if (checkRegistrants)
                    {
                        var regs = GetMeetingRegistrants(meetingId);
                        var reg = regs.FirstOrDefault(x =>
                            x.Email.Equals(registrant.Email, StringComparison.InvariantCultureIgnoreCase));
                        if (reg == null)
                        {
                            addRegistrant = true;
                        }
                        else
                        {
                            if (reg.Status == ZoomMeetingRegistrantStatus.Pending)
                            {
                                registrantsToApprove.Add(reg);
                            }
                        }
                    }

                    if (addRegistrant)
                    {
                        var addResult = _zoomApi.AddRegistrant(meetingId,
                            new ZoomAddRegistrantRequest
                            {
                                Email = registrant.Email,
                                FirstName = registrant.FirstName,
                                LastName = registrant.LastName
                            });
                        registrantsToApprove.Add(new ZoomMeetingRegistrantDto{Id = addResult.RegistrantId, Email = registrant.Email });

                    }
                }
                else
                {
                    //create user
                    try
                    {
                        var userInfo = CreateUser(new CreateUserDto
                        {
                            Email = registrant.Email,
                            FirstName = registrant.FirstName,
                            LastName = registrant.LastName
                        });
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
            }

            if (registrantsToApprove.Any())
            {
                _zoomApi.UpdateRegistrantsStatus(meetingId, new ZoomUpdateRegistrantStatusRequest
                {
                    Action = "approve",
                    Registrants = registrantsToApprove.Select(x => new ZoomRegistrantForStatusRequest
                    {
                        Email = x.Email,
                        Id = x.Id
                    }).ToList()
                }, null);
            }

            return true;
        }

        private ZoomMeetingRegistrantDto ConvertFromApiObjectToDto(ZoomMeetingRegistrant apiObject)
        {
            return new ZoomMeetingRegistrantDto
            {
                Id = apiObject.Id,
                Email = apiObject.Email,
                FirstName = apiObject.FirstName,
                LastName = apiObject.LastName,
                JoinUrl = apiObject.JoinUrl,
                Status = (ZoomMeetingRegistrantStatus) Enum.Parse(typeof(ZoomMeetingRegistrantStatus), apiObject.Status,
                    true),
                CreateTime = apiObject.CreateTime.DateTime
            };
        }
    }
}