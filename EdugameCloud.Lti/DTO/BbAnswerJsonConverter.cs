using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.BlackBoard
{
    // AA: currently not used as it changes the way answers are handled for all types of questions
    // standart deserializer skips answers with the same name like answersList: {""<p>Correct</p>"":true, ""<p>Wrong</p>"":false, ""<p>Correct</p>"":true}
    public class BbAnswerJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = new object();
            if (reader.TokenType == JsonToken.StartObject)
            {
                var isKeyValueFormat = true;
                var result = new JObject();
                reader.Read();
                while (reader.TokenType != JsonToken.EndObject)
                {
                    var key = reader.Value;
                    reader.Read();
                    var value = reader.Value;
                    if (value == null)
                    {
                        isKeyValueFormat = false;
                        break;
                    }
                    if (result[key] != null)
                    {
                        // AA: hack, adding space to the key in order to keep it both in JObject
                        while (result[key] != null)
                        {
                            key = key + " ";
                        }
                    }
                    result.Add(key.ToString(), value.ToString());
                    reader.Read();
                }
                if (!isKeyValueFormat)
                {
                    // reopen the reader, but how?
                    retVal = serializer.Deserialize(reader, objectType);
                    return retVal;
                }
                retVal = result;
            }
            else if (reader.TokenType == JsonToken.StartArray)
            {
                retVal = serializer.Deserialize(reader, objectType);
            }
            return retVal;
        }

        public override bool CanConvert(Type objectType)
        {
            return false;
        }
    }
}