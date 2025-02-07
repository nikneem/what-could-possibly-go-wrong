using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Votr.Core.CosmosDb.Resolvers;

public class PluggableContractResolver : DefaultContractResolver
{
    protected override JsonConverter? ResolveContractConverter(Type objectType)
    {
        if (_converters.TryGetValue(objectType, out var converter))
        {
            return converter;
        }
        return base.ResolveContractConverter(objectType);
    }

    public PluggableContractResolver AddConverter<TEntity>(JsonConverter converter)
    {
        _converters[typeof(TEntity)] = converter;
        return this;
    }

    private readonly Dictionary<Type, JsonConverter> _converters = new();
}