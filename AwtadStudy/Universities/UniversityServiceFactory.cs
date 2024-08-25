using AwtadStudy.Database;
using AwtadStudy.Universities.TAU;

namespace AwtadStudy.Universities;

public sealed class UniversityServiceFactory(IHttpClientFactory httpFactory, AppDbContext db) : IUniversityServiceFactory
{
    private readonly IHttpClientFactory _httpFactory = httpFactory;
    private readonly AppDbContext _db = db;

    public IUniversityService GetOrCreate(University university) => university switch
    {
        University.TelAviv => new TauService(_httpFactory.CreateClient(), _db),
        _ => throw new NotImplementedException()
    };
}
