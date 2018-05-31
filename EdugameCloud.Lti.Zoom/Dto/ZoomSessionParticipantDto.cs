using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Edugamecloud.Lti.Zoom.Dto
{
    public class ZoomSessionParticipantDto
    {
        public string ParticipantName { get; set; }
        public string ParticipantEmail { get; set; }
        public int Duration { get; set; }
        public string Score { get; set; }

        //[Required]
        [DataMember(Name = "enteredAt")]
        public DateTime EnteredAt { get; set; }

        [DataMember(Name = "leftAt")]
        public DateTime LeftAt { get; set; }

        public ZoomSessionParticipantDetailsDto Details { get; set; }
        /*
{
  "data": [
    {
      "participants": [
        {
          "participantName": "string",
          "participantEmail": "string",
          "duration": 0,
          "score": "string",
          "enteredAt": 1527262617327,
          "leftAt": 1527262617327,
          "details":{
            "id": "9St6ertHQ_m5d9rIl319aQ",
            "name": "Mike Kollen", //should we remove if we have it for parent?
            "device": "Mac",
            "ipAddress": "68.101.110.42",
            "location": "Laguna Beach (US)",
            "networkType": "Wifi",
            "startedAt": 1527262617327, //should we remove if we have it for parent?
            "endedAt": long, //should we remove if we have for parent?

            "shareApplication": false,
            "shareDesktop": false,
            "shareWhiteboard": false,
            "recording": false,
            "pcName": "",
            "domain": "",
            "macAddr": "",
            "harddiskId": "",
            "version": "4.1.23501.0416"
          }

        }
      ],
      "startedAt": 1527262617327,
      "endedAt": 1527262617327,
      "sessionId": "string"
    }
  ],
  "isSuccess": true,
  "message": "string"
}
        {
            "id": "9St6ertHQ_m5d9rIl319aQ",
            "name": "Mike Kollen", //should we remove if we have it for parent?
            "device": "Mac",
            "ipAddress": "68.101.110.42",
            "location": "Laguna Beach (US)",
            "networkType": "Wifi",
            "startTime": long, //should we remove if we have it for parent?
            "endTime": long, //should we remove if we have for parent?
            "shareApplication": false,
            "shareDesktop": false,
            "shareWhiteboard": false,
            "recording": false,
            "pcName": "",
            "domain": "",
            "macAddr": "",
            "harddiskId": "",
            "version": "4.1.23501.0416"
        }
        */
    }
}