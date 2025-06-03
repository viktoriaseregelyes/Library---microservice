using EasyNetQ;
using Library.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.Book.IntegrationEventHandlers;
using Service.User.IntegrationEvents;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Service.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger _logger;

        public UserController(UserService userService, IPublishEndpoint publishEndpoint, ILogger<UserController> logger)
        {
            _userService = userService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [HttpGet("ListUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            var deleteUserRequestedEvent = new
            {
                UserId = id
            };

            await _publishEndpoint.Publish<IReturnDeletedUsersBooks>(deleteUserRequestedEvent);

            return Accepted();
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid user data.");
            }

            _userService.AddUser(newUser);

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id.ToString() }, newUser);
        }

        [HttpPost("BorrowABook")]
        public async Task<IActionResult> BorrowABook([FromBody] BorrowBook book)
        {
            if(_userService.GetUserById(book.UserID) == null)
            {
                return NotFound("User not found");
            }

            var message = new BorrowMessage
            {
                BookID = book.BookID,
                DateTime = DateTime.Now,
                BorrowType = true,
                UserID = book.UserID
            };

            await _publishEndpoint.Publish<IBorrowingEvents>(message);

            _logger.LogInformation("BookID: {BookID}, BorrowType: borrow", message.BookID);

            return Ok("Succesfull");
        }

        [HttpPost("ReturnABook")]
        public async Task<IActionResult> ReturnABook([FromBody] BorrowBook book)
        {
            if (_userService.GetUserById(book.UserID) == null)
            {
                return NotFound("User not found");
            }

            var message = new BorrowMessage
            {
                BookID = book.BookID,
                DateTime = DateTime.Now,
                BorrowType = false,
                UserID = book.UserID
            };

            await _publishEndpoint.Publish<IBorrowingEvents>(message);

            _logger.LogInformation("BookID: {BookID}, BorrowType: return", message.BookID);

            return Ok("Succesfull");
        }
    }
}
