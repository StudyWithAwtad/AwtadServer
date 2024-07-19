namespace AwtadStudy.Universities;

public interface IUniversityServiceFactory
{
    IUniversityService GetOrCreate(University uni);
}
