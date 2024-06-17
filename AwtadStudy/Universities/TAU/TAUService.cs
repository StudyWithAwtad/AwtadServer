using System.Diagnostics;
using AwtadStudy.Universities.Courses;
using DBCourseDTO = AwtadStudy.Database.CourseDTO;

namespace AwtadStudy.Universities.TAU;

public sealed class TAUService(HttpClient http) : IUniversityService
{
    private const string ApiUrlTemplate = "https://bid-it.appspot.com/ajax/chosen-courses-info/?university=TAU&semester={0}&courses_list={1}";

    public async Task<Course> GetCourse(string courseID, Semester semester)
    {
        CourseDTO courseDTO = await QueryApi(courseID, semester);

        IEnumerable<CourseGroup> groups = ParseGroups(courseDTO.kvutzaData);

        return new(courseDTO.cNum,
                   courseDTO.cName,
                   courseDTO.hoursNum,
                   courseDTO.depart,
                   int.Parse(courseDTO.cYear!),
                   semester,
                   groups);
    }

    public async IAsyncEnumerable<Course> GetCoursesForUser(string userID)
    {
        IEnumerable<DBCourseDTO> courseDTOs = await GetUserCoursesFromDB(userID);

        var courseQueries = courseDTOs.Select(async c => (c, await QueryApi(c.CourseID, c.Semester)));
        var courseTuples = await Task.WhenAll(courseQueries);

        foreach (var (dbCourseDTO, apiCourseDTO) in courseTuples)
        {
            var apiGroups = apiCourseDTO.kvutzaData.Where(g => g is not null && dbCourseDTO.GroupIDs.Contains(g.gNum));

            if (!apiGroups.Any()) throw new UnreachableException($"User is not registered to any groups from course {dbCourseDTO.CourseID}.");

            var courseGroups = ParseGroups(apiGroups);

            yield return new(apiCourseDTO.cNum,
                             apiCourseDTO.cName,
                             apiCourseDTO.hoursNum,
                             apiCourseDTO.depart,
                             int.Parse(apiCourseDTO.cYear!),
                             dbCourseDTO.Semester,
                             courseGroups);
        }

        Task<IEnumerable<DBCourseDTO>> GetUserCoursesFromDB(string userID) => throw new NotImplementedException();
    }

    public Task RegisterCourseForUser(string userID, string courseID, string groupID)
    {
        throw new NotImplementedException();
    }

    private async Task<CourseDTO> QueryApi(string courseID, Semester semester)
    {
        string semesterParam = semester switch
        {
            Semester.Winter => "1",
            Semester.Spring => "2",
            _ => throw new NotImplementedException()
        };

        string url = string.Format(ApiUrlTemplate, semesterParam, courseID);
        var rootDTO = await http.GetFromJsonAsync<RootDTO>(url);

        CourseDTO courseDTO = rootDTO.coursesinfo[0]
            ?? throw new InvalidOperationException("No course found with the specified ID and semester.");

        if (courseDTO.cNum != courseID) throw new UnreachableException("courseID != returned course's ID.");
        if (courseDTO.semester != semesterParam) throw new UnreachableException("semester != returned course's semester.");

        if (courseDTO.kvutzaData is null) throw new UnreachableException("Course groups is null.");

        if (courseDTO.cName is null) throw new UnreachableException("Course name is null.");

        return courseDTO;
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

    private static IEnumerable<CourseGroup> ParseGroups(IEnumerable<GroupDTO?> groupDTOs)
    {
        foreach (GroupDTO? groupDTO in groupDTOs)
        {
            if (groupDTO is null) throw new UnreachableException("Group is null.");

            string id = groupDTO.gNum ?? throw new UnreachableException("Group ID is null.");
            string lecturer = string.Join(" + ", groupDTO.lecturer?.Distinct() ?? []);
            IEnumerable<LectureInfo> lectures = ParseLectureInfo(groupDTO);
            IEnumerable<ExamInfo> exams = ParseExamInfo(groupDTO);

            yield return new(id, lecturer, lectures, exams);
        }
    }

    private static IEnumerable<LectureInfo> ParseLectureInfo(GroupDTO groupDTO)
    {
        if (groupDTO.days is null or { Length: 0 }) throw new UnreachableException("Groups has no lectures.");

        for (int i = 0; i < groupDTO.days.Length; i++)
        {
            var day = DayFromHebrewDay(groupDTO.days[i]);
            var startTime = TimeOnly.ParseExact(groupDTO.beginHours![i].Replace(" ", ""), "H:mm");
            var endTime = TimeOnly.ParseExact(groupDTO.endHours![i].Replace(" ", ""), "H:mm");
            var place = groupDTO.place?[i] ?? "";

            yield return new(day, startTime, endTime, place);
        }
    }

    private static IEnumerable<ExamInfo> ParseExamInfo(GroupDTO groupDTO)
    {
        if (groupDTO.dates is null or { Length: 0 }) yield break;

        for (int i = 0; i < groupDTO.dates.Length; i++)
        {
            var date = DateOnly.ParseExact(groupDTO.dates[i]!, "d/M/yyyy");
            var examType = groupDTO.moedType![i] switch
            {
                "בחינה סופית" => ExamType.Final,
                _ => throw new NotImplementedException()
            };
            var moed = groupDTO.moed![i]! switch
            {
                "א" => Moed.A,
                "ב" => Moed.B,
                _ => throw new NotImplementedException()
            };

            yield return new(date, moed, examType);
        }
    }
}
