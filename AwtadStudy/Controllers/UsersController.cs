using Microsoft.AspNetCore.Mvc;
using AwtadStudy.FirebaseAdmin;
using FirebaseAdmin.Auth;
using System.Collections.Generic;

namespace AwtadStudy.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        public ILogger<UsersController> Logger { get; }
        private readonly FirebaseService _firebaseService;

        public UsersController(ILogger<UsersController> logger, FirebaseService firebaseService) {
            Logger = logger;
            // Inject the FirebaseService Dependency
            _firebaseService = firebaseService;
        }

        // GET: api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRecord>>> Get()
        {
            try
            {
                IEnumerable<UserRecord> users = await FirebaseAuthService.GetUsersAsync();
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
        public async Task<IActionResult> Get(string uid)
        {
            try
            {
                UserRecord user = await FirebaseAuthService.GetUserAsync(uid);
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
        public async Task<IActionResult> Post([FromBody] UserRecordArgs userArgs)
        {
            try
            {
                UserRecord user = await FirebaseAuthService.CreateUserAsync(userArgs);
                return Ok(user);
            } catch (FirebaseAuthException error)
            {
                Console.WriteLine($"Exception error: {error}");
                return BadRequest(error.Message); ;
            }
        }

        // PUT api/values/5
        [HttpPut("{uid}")]
        public async Task<IActionResult> Put(string uid, [FromBody] UserRecordArgs newUserArgs)
        {
            try
            {
                newUserArgs.Uid = uid;
                UserRecord user = await FirebaseAuthService.UpdateUserAsync(newUserArgs);
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
        public async Task<IActionResult> Delete(string uid)
        {
            try
            {
                string deletedUserId = await FirebaseAuthService.DeleteUserAsync(uid);
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

