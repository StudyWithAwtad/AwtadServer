namespace AwtadStudy.Course;

public interface ICourseServiceFactory
{
    ICourseService GetForUniversity(University uni);
}
