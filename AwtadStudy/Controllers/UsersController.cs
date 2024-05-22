using Microsoft.AspNetCore.Mvc;
using AwtadStudy.FirebaseAdmin;
using FirebaseAdmin.Auth;
using AwtadStudy.Interfaces;

namespace AwtadStudy.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public ILogger<UsersController> Logger { get; }
        private readonly FirebaseService _firebaseService;
        private readonly IFirebaseAuth _firebaseAuthService;

        public UsersController(
            ILogger<UsersController> logger,
            FirebaseService firebaseService,
            IFirebaseAuth firebaseAuthService) {
            Logger = logger;
            // Inject the FirebaseService Dependency
            _firebaseService = firebaseService;
            _firebaseAuthService = firebaseAuthService;
        }

        // GET: api/values
        [HttpGet]
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

        // GET api/values/5
        [HttpGet("{uid}")]
        public async Task<ActionResult<Task<UserRecord>>> Get(string uid)
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
        
        // POST api/values
        [HttpPost]
        public async Task<ActionResult<Task<UserRecord>>> Post([FromBody] UserRecordArgs userArgs)
        {
            try
            {
                UserRecord user = await _firebaseAuthService.CreateUserAsync(userArgs);
                return Ok(user);
            } catch (FirebaseAuthException error)
            {
                Console.WriteLine($"Exception error: {error}");
                return BadRequest(error.Message); ;
            }
        }

        // PUT api/values/5
        [HttpPut("{uid}")]
        public async Task<ActionResult<Task<UserRecord>>> Put(string uid, [FromBody] UserRecordArgs newUserArgs)
        {
            try
            {
                newUserArgs.Uid = uid;
                UserRecord user = await _firebaseAuthService.UpdateUserAsync(newUserArgs);
                return Ok(user);
            }
            catch (FirebaseAuthException error)
            {
                Console.WriteLine($"Exception error: {error}");
                return BadRequest(error.Message); ;
            }
        }

        // DELETE api/values/5
        [HttpDelete("{uid}")]
        public async Task<ActionResult<Task<string>>> Delete(string uid)
        {
            try
            {
                string deletedUserId = await _firebaseAuthService.DeleteUserAsync(uid);
                return Ok(deletedUserId);
            }
            catch (FirebaseAuthException error)
            {
                Console.WriteLine($"Exception error: {error}");
                return BadRequest(error.Message); ;
            }
        }


    }
}

