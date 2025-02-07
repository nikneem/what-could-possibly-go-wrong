using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Votr.Core.CosmosDb.Resolvers;



/// <summary>
/// The default Cosmos JSON.NET serializer.
/// </summary>
/// <remarks>
/// This is copied from the SDK implementation of CosmosJsonSerializerWrapper
/// </remarks>
public sealed class CustomCosmosSerializer : CosmosSerializer
{
    private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
    private readonly JsonSerializerSettings? _serializerSettings;

    /// <summary>
    /// Create a serializer that uses the JSON.net serializer
    /// </summary>
    /// <remarks>
    /// This is internal to reduce exposure of JSON.net types so
    /// it is easier to convert to System.Text.Json
    /// </remarks>
    public CustomCosmosSerializer(CosmosSerializationOptions? cosmosSerializerOptions = null)
    {
        if (cosmosSerializerOptions == null)
        {
            _serializerSettings = null;
            return;
        }

        var jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = cosmosSerializerOptions.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
            Formatting = cosmosSerializerOptions.Indented ? Formatting.Indented : Formatting.None,
            ContractResolver = cosmosSerializerOptions.PropertyNamingPolicy == CosmosPropertyNamingPolicy.CamelCase
                ? new CamelCasePropertyNamesContractResolver()
                : null,
            MaxDepth = 64, // https://github.com/advisories/GHSA-5crp-9r3c-p9vr
        };

        _serializerSettings = jsonSerializerSettings;
    }

    /// <summary>
    /// Create a serializer that uses the JSON.net serializer
    /// </summary>
    /// <remarks>
    /// This is internal to reduce exposure of JSON.net types so
    /// it is easier to convert to System.Text.Json
    /// </remarks>
    public CustomCosmosSerializer(JsonSerializerSettings jsonSerializerSettings)
    {
        _serializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
    }

    /// <summary>
    /// Convert a Stream to the passed in type.
    /// </summary>
    /// <typeparam name="T">The type of object that should be deserialized</typeparam>
    /// <param name="stream">An open stream that is readable that contains JSON</param>
    /// <returns>The object representing the deserialized stream</returns>
    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            using (StreamReader sr = new StreamReader(stream))
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(sr))
                {
                    var deserializedObject =  GetSerializer().Deserialize<T>(jsonTextReader);
                    if (deserializedObject != null)
                    {
                        return deserializedObject;
                    }
                }
            }
        }

        throw new ArgumentException("Failed to deserialize object from stream");
    }

    /// <summary>
    /// Converts an object to a open readable stream
    /// </summary>
    /// <typeparam name="T">The type of object being serialized</typeparam>
    /// <param name="input">The object to be serialized</param>
    /// <returns>An open readable stream containing the JSON of the serialized object</returns>
    public override Stream ToStream<T>(T input)
    {
        var streamPayload = new MemoryStream();
        using (var streamWriter = new StreamWriter(streamPayload, encoding: DefaultEncoding, bufferSize: 1024, leaveOpen: true))
        {
            using (JsonWriter writer = new JsonTextWriter(streamWriter))
            {
                writer.Formatting = Formatting.None;
                var jsonSerializer = GetSerializer();
                jsonSerializer.Serialize(writer, input);
                writer.Flush();
                streamWriter.Flush();
            }
        }

        streamPayload.Position = 0;
        return streamPayload;
    }

    /// <summary>
    /// JsonSerializer has hit a race conditions with custom settings that cause null reference exception.
    /// To avoid the race condition a new JsonSerializer is created for each call
    /// </summary>
    private JsonSerializer GetSerializer()
    {
        return JsonSerializer.Create(_serializerSettings);
    }
}
