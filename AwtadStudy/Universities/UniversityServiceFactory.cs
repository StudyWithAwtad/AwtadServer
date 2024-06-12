using System.Collections.Concurrent;
using AwtadStudy.Universities.TAU;

namespace AwtadStudy.Universities;

public sealed class UniversityServiceFactory(IHttpClientFactory httpClientFactory) : IUniversityServiceFactory
{
    private readonly ConcurrentDictionary<University, IUniversityService> _uniServices = new();

    public IUniversityService GetOrCreate(University university) => _uniServices.GetOrAdd(university,
    (uni, httpFactory) => uni switch
    {
        University.TelAviv => new TAUService(httpFactory.CreateClient()),
        _ => throw new NotImplementedException()
    }, httpClientFactory);
}

public enum University
{
    TelAviv,
    Technion,
    BarIlan,
    Haifa,
}
