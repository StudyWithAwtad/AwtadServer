namespace AwtadStudy.Course;

public sealed class CourseServiceFactory(IHttpClientFactory httpClientFactory) : ICourseServiceFactory
{
    private readonly TAUCourseService _tauService = new(httpClientFactory.CreateClient());

    public ICourseService GetForUniversity(University uni) => uni switch
    {
        University.TelAviv => _tauService,
        _ => throw new NotImplementedException()
    };
}

public enum University
{
    TelAviv,
    Technion,
    BarIlan,
    Haifa,
}
