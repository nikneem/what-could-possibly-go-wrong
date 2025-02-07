using Newtonsoft.Json;

namespace Votr.Core.CosmosDb;

public abstract class SpreaViewDataEntity<TId>
{
    [JsonProperty(PropertyName = "id")]
    public required TId Id { get; set; }

    public abstract string EntityType { get; }
}