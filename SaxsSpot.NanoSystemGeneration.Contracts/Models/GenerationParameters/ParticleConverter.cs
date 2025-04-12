using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

public class ParticleConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);

        if (obj["Epsilon"] != null)
        {
            return obj.ToObject<ParallelepipedGenerationParameters>(serializer);
        }
        else
        {
            return obj.ToObject<SphereGenerationParameters>(serializer);
        }    
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ParticleGenerationParameters);
    }
}