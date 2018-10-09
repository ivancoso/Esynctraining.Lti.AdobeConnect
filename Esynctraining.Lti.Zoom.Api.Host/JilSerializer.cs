using System;
using Esynctraining.Core.Json;
using Jil;

namespace Esynctraining.Lti.Zoom.Api.Host
{
    public class JilSerializer : IJsonSerializer, IJsonDeserializer
    {
        public static readonly Options JilOptions = new Options(false, true, false, DateTimeFormat.MillisecondsSinceUnixEpoch, true,
            UnspecifiedDateTimeKindBehavior.IsUTC,
            SerializationNameFormat.CamelCase);


        public string JsonSerialize<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return JSON.Serialize(obj, JilOptions);
        }

        public T JsonDeserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("Non-empty value expected", nameof(json));

            return JSON.Deserialize<T>(json, JilOptions);
        }

    }
}