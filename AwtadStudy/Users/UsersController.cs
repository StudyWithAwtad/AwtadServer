using AwtadStudy.Database;
using AwtadStudy.Universities;
using AwtadStudy.Universities.Courses;
using Microsoft.AspNetCore.Mvc;

namespace AwtadStudy.Users;

// NOTE: these endpoint are currently temporary until auth is fully implemented.

[Route("api/[controller]")]
public class UsersController(IUniversityServiceFactory uniServiceFactory,
                             AppDbContext db) : Controller
{
    private readonly IUniversityServiceFactory _uniServiceFactory = uniServiceFactory;
    private readonly AppDbContext _db = db;

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(long id)
    {
        User? user = await _db.Users.FindAsync(id);

        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpGet("{id}/courses")]
    public ActionResult<IAsyncEnumerable<CourseInfo>> GetCourses(long id)
    {
        try
        {
            University uni = GetUniversityFromJwt();
            var courses = _uniServiceFactory.GetOrCreate(uni).GetCoursesForUser(id);
            return Ok(courses);

            // TODO
            University GetUniversityFromJwt() => throw new NotImplementedException();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(long id, University? uni, [FromBody] List<Course>? courses)
    {
        User? user = await _db.Users.FindAsync(id);

        if (user is null) return NotFound();

        if (uni is not null) user.UniversityId = (University)uni;
        if (courses is not null) user.Courses = courses;

        int changes = await _db.SaveChangesAsync();

        if (changes == 0) return StatusCode(500);
        
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<User>> Post(University uni, string? courseId, string? group)
    {
        var user = await _db.Users.AddAsync(new()
        {
            FirebaseUid = "0",
            UniversityId = uni,
            Courses = courseId is null || group is null ? [] : [new() { Identifier = courseId, LectureGroupId = group, PracticeGroupId = group, Semester = Semester.Winter }]
        });

        int changes = await _db.SaveChangesAsync();

        if (changes == 0) return StatusCode(500);
        
        return Created($"api/users/{user.Entity.Id}", user.Entity);
    }
    
}

