using Newtonsoft.Json;

namespace Votr.Core.CosmosDb;

public abstract class VotrDataEntity<TId>
{
    [JsonProperty(PropertyName = "id")]
    public required TId Id { get; set; }

    public virtual string EntityType => GetType().Name;
}