using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SuperPartner.Utils.Json
{
    /// <summary>
    /// Author: Robert
    /// The zero will keep when convert decimal to string default.
    /// For example: 12.0000 will be convert to "12.0000". 
    /// But We want remove "0" from end, that mean, we want convert to "12" not "12.0000"
    /// The class is used to resolve the issue
    /// </summary>
    public class JsonMinDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
            {
                return token.ToObject<decimal>();
            }
            if (token.Type == JTokenType.String)
            {
                return Decimal.Parse(token.ToString());
            }
            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
            {
                return null;
            }
            throw new JsonSerializationException("Unexpected token type: " + token.Type.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Decimal? d = default(Decimal?);
            if (value != null)
            {
                d = value as Decimal?;
                if (d.HasValue) // If value was a decimal?, then this is possible
                {
                    var str = d.ToString();
                    if (str.IndexOf('.') > 0)
                    {
                        d = decimal.Parse(str.TrimEnd('0').TrimEnd('.'));
                    }
                }
            }

            JToken.FromObject(d).WriteTo(writer);
        }
    }
}
