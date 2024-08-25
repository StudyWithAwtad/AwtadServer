using System.Diagnostics;
using AwtadStudy.Database;
using AwtadStudy.Universities.Courses;
using AwtadStudy.Users;

namespace AwtadStudy.Universities.TAU;

public sealed class TauService(HttpClient http, AppDbContext db) : IUniversityService
{
    private const string ApiUrlTemplate = "https://bid-it.appspot.com/ajax/chosen-courses-info/?university=TAU&semester={0}&courses_list={1}";
    
    private readonly HttpClient _http = http;
    private readonly AppDbContext _db = db;

    /// <exception cref="InvalidOperationException">When no course is found with the specified id or semester.</exception>
    public async Task<CourseInfo> GetCourse(string courseId, Semester semester)
    {
        CourseDto courseDto = await QueryApi(courseId, semester);

        IEnumerable<GroupInfo> groups = ParseGroups(courseDto.kvutzaData);

        return new(courseDto.cNum,
                   courseDto.cName,
                   courseDto.hoursNum,
                   courseDto.depart,
                   int.Parse(courseDto.cYear!),
                   semester,
                   groups);
    }

    /// <exception cref="InvalidOperationException">When either no user is found with the specified id, or no course is found with the specified id or semester.</exception>
    public async IAsyncEnumerable<CourseInfo> GetCoursesForUser(long userId)
    {
        User user = await _db.Users.FindAsync(userId) ?? throw new InvalidOperationException($"No user found with primary key {userId}.");
        
        var courseQueries = user.Courses.Select(async c => (c, await QueryApi(c.Identifier, c.Semester)));
        var courseTuples = await Task.WhenAll(courseQueries);

        foreach (var (dbCourseDto, apiCourseDto) in courseTuples)
        {
            var apiGroups = apiCourseDto.kvutzaData.Where(g => g is not null && (g.gNum == dbCourseDto.LectureGroupId || g.gNum == dbCourseDto.PracticeGroupId));

            if (!apiGroups.Any()) throw new UnreachableException($"User is not registered to any groups from course {dbCourseDto.Identifier}.");

            var courseGroups = ParseGroups(apiGroups);

            yield return new(apiCourseDto.cNum,
                             apiCourseDto.cName,
                             apiCourseDto.hoursNum,
                             apiCourseDto.depart,
                             int.Parse(apiCourseDto.cYear!),
                             dbCourseDto.Semester,
                             courseGroups);
        }
    }

    /// <exception cref="InvalidOperationException">When no user is found with the specified id.</exception>
    public async Task RegisterCourseForUser(long userId, Course course)
    {
        User user = await _db.Users.FindAsync(userId) ?? throw new InvalidOperationException($"No user found with primary key {userId}.");
        user.Courses.Add(course);
        await _db.SaveChangesAsync();
    }

    private async Task<CourseDto> QueryApi(string courseId, Semester semester)
    {
        string semesterParam = semester switch
        {
            Semester.Winter => "1",
            Semester.Spring => "2",
            _ => throw new NotImplementedException()
        };

        string url = string.Format(ApiUrlTemplate, semesterParam, courseId);
        var rootDto = await _http.GetFromJsonAsync<RootDto>(url);

        CourseDto courseDto = rootDto.coursesinfo[0]
            ?? throw new InvalidOperationException("No course found with the specified ID and semester.");

        if (courseDto.cNum != courseId) throw new UnreachableException("courseID != returned course's ID.");
        if (courseDto.semester != semesterParam) throw new UnreachableException("semester != returned course's semester.");

        if (courseDto.kvutzaData is null) throw new UnreachableException("Course groups is null.");

        if (courseDto.cName is null) throw new UnreachableException("Course name is null.");

        return courseDto;
    }

    private static DayOfWeek DayFromHebrewDay(string? hebrewDay) => hebrewDay switch
    {
        "א" => DayOfWeek.Sunday,
        "ב" => DayOfWeek.Monday,
        "ג" => DayOfWeek.Tuesday,
        "ד" => DayOfWeek.Wednesday,
        "ה" => DayOfWeek.Thursday,
        "ו" => DayOfWeek.Friday,
        "ז" => DayOfWeek.Saturday,
        _ => throw new NotImplementedException()
    };

    private static IEnumerable<GroupInfo> ParseGroups(IEnumerable<GroupDto?> groupDtos)
    {
        foreach (GroupDto? groupDto in groupDtos)
        {
            if (groupDto is null) throw new UnreachableException("Group is null.");

            string id = groupDto.gNum ?? throw new UnreachableException("Group ID is null.");
            string lecturer = string.Join(" + ", groupDto.lecturer?.Distinct() ?? []);
            IEnumerable<LectureInfo> lectures = ParseLectureInfo(groupDto);
            IEnumerable<ExamInfo> exams = ParseExamInfo(groupDto);

            yield return new(id, lecturer, lectures, exams);
        }
    }

    private static IEnumerable<LectureInfo> ParseLectureInfo(GroupDto groupDto)
    {
        if (groupDto.days is null or { Length: 0 }) throw new UnreachableException("Groups has no lectures.");

        for (int i = 0; i < groupDto.days.Length; i++)
        {
            var day = DayFromHebrewDay(groupDto.days[i]);
            var startTime = TimeOnly.ParseExact(groupDto.beginHours![i].Replace(" ", ""), "H:mm");
            var endTime = TimeOnly.ParseExact(groupDto.endHours![i].Replace(" ", ""), "H:mm");
            var place = groupDto.place?[i] ?? "";

            yield return new(day, startTime, endTime, place);
        }
    }

    private static IEnumerable<ExamInfo> ParseExamInfo(GroupDto groupDto)
    {
        if (groupDto.dates is null or { Length: 0 }) yield break;

        for (int i = 0; i < groupDto.dates.Length; i++)
        {
            var date = DateOnly.ParseExact(groupDto.dates[i]!, "d/M/yyyy");
            var examType = groupDto.moedType![i] switch
            {
                "בחינה סופית" => ExamType.Final,
                _ => throw new NotImplementedException()
            };
            var moed = groupDto.moed![i]! switch
            {
                "א" => Moed.A,
                "ב" => Moed.B,
                _ => throw new NotImplementedException()
            };

            yield return new(date, moed, examType);
        }
    }
}
