using ASF_OneBot.API.Data.Requests;
using ASF_OneBot.Data.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ASF_OneBot.Host
{
    internal class RequestParser
    {
        public abstract class JsonCreationConverter<T> : JsonConverter
        {
            protected abstract T Create(Type objectType, JObject jsonObject);
            public override bool CanConvert(Type objectType)
            {
                return typeof(T).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                T target = Create(objectType, jsonObject);
                serializer.Populate(jsonObject.CreateReader(), target);
                return target;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public class JsonRequestConverter : JsonCreationConverter<BaseRequest>
        {
            protected override BaseRequest Create(Type objectType, JObject jsonObject)
            {
                string action = jsonObject["action"].ToString();
                switch (action)
                {
                    case "send_private_msg":
                        return new SendPrivateMsgRequest();
                    case "send_group_msg":
                        return new SendGroupMsgRequest();
                    case "send_msg":
                        return new SendMsgRequest();
                    default: return null;
                }
            }
        }
    }
}
