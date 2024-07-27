using AwtadStudy.Universities.TAU;

namespace AwtadStudy.Universities;

public sealed class UniversityServiceFactory(IHttpClientFactory httpFactory) : IUniversityServiceFactory
{
    private readonly IHttpClientFactory _httpFactory = httpFactory;

    public IUniversityService GetOrCreate(University university) => university switch
    {
        University.TelAviv => new TauService(_httpFactory.CreateClient()),
        _ => throw new NotImplementedException()
    };
}
