namespace AwtadStudy.Universities.Courses;

public sealed record LectureInfo(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime, string Place);
