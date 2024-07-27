using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AwtadStudy.FirebaseAdmin;

[Route("api/[controller]")]
public class FirebaseController(ILogger<FirebaseController> logger,
                                FirebaseService firebaseService,
                                IFirebaseAuth firebaseAuthService) : Controller
{
    public ILogger<FirebaseController> Logger { get; } = logger;

    private readonly FirebaseService _firebaseService = firebaseService;
    private readonly IFirebaseAuth _firebaseAuthService = firebaseAuthService;

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserRecord>>> Get()
    {
        try
        {
            IEnumerable<UserRecord> users = await _firebaseAuthService.GetUsersAsync();
            return Ok(users);
        }
        catch (FirebaseAuthException error)
        {
            Console.WriteLine($"Exception error: {error}");
            return BadRequest(error.Message);
        }
    }

    [HttpGet("users/{uid}")]
    public async Task<ActionResult<UserRecord>> Get(string uid)
    {
        try
        {
            UserRecord user = await _firebaseAuthService.GetUserAsync(uid);
            string customToken = await _firebaseAuthService.CreateCustomToken(user.Uid);
            return Ok(user);
        }
        catch (FirebaseAuthException error)
        {
            Console.WriteLine($"Exception error: {error}");
            return BadRequest(error.Message);
        }
    }

    [HttpPost("users")]
    public async Task<ActionResult<UserRecord>> Post([FromBody] UserRecordArgs userArgs)
    {
        try
        {
            UserRecord user = await _firebaseAuthService.CreateUserAsync(userArgs);
            return Created($"/api/firebase/{user.Uid}", user);
        }
        catch (FirebaseAuthException error)
        {
            Console.WriteLine($"Exception error: {error}");
            return BadRequest(error.Message); ;
        }
    }

    [HttpPut("users/{uid}")]
    public async Task<ActionResult> Put(string uid, [FromBody] UserRecordArgs newUserArgs)
    {
        try
        {
            newUserArgs.Uid = uid;
            UserRecord user = await _firebaseAuthService.UpdateUserAsync(newUserArgs);
            return NoContent();
        }
        catch (FirebaseAuthException error)
        {
            Console.WriteLine($"Exception error: {error}");
            return BadRequest(error.Message); ;
        }
    }

    [HttpDelete("users/{uid}")]
    public async Task<ActionResult> Delete(string uid)
    {
        try
        {
            string deletedUserId = await _firebaseAuthService.DeleteUserAsync(uid);
            return NoContent();
        }
        catch (FirebaseAuthException error)
        {
            Console.WriteLine($"Exception error: {error}");
            return BadRequest(error.Message); ;
        }
    }
}