using Votr.Surveys.DomainModels;

namespace Votr.Surveys.Abstractions;

public interface ISurveysRepository
{
    Task<List<Survey>> List(CancellationToken cancellationToken);
    Task<Survey> Get(Guid id, CancellationToken cancellationToken);
    Task<bool> Save(Survey domainModel, CancellationToken cancellationToken);
}