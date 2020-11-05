using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Type { get; set; }
        public int Verified { get; set; }
        public string Pmi { get; set; }
        public string Timezone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginTime { get; set; }
        public UserStatus Status { get; set; }
        public string Code { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public UserTypes Type { get; set; }

        [JsonProperty("pmi")]
        public string PersonalMeetingId { get; set; }

        public string Timezone { get; set; }

        [JsonProperty("dept")]
        public string Department { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedTime { get; set; }

        public DateTimeOffset? LastLoginTime { get; set; }

        public string LastClientVersion { get; set; }
    }

    public class CreateUser : ICreatable
    {
        public string Email { get; set; }

        public UserTypes Type { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public List<string> Validate()
        {
            List<string> stringList = new List<string>();
            if (string.IsNullOrWhiteSpace(this.Email))
                stringList.Add(string.Format("{0} property is required for creating user", (object)"Email"));
            if (!string.IsNullOrWhiteSpace(this.FirstName) && this.FirstName.Length > 64)
                stringList.Add(string.Format("{0} property max length is {1} characters", (object)"FirstName", (object)64));
            if (!string.IsNullOrWhiteSpace(this.LastName) && this.LastName.Length > 64)
                stringList.Add(string.Format("{0} property max length is {1} characters", (object)"LastName", (object)64));
            return stringList;
        }
    }
    public class ListUsers : PageList
    {
        public List<User> Users { get; set; }
    }
    public interface ICreatable
    {
        List<string> Validate();
    }
}