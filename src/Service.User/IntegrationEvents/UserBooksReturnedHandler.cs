using Library.Events;
using MassTransit;
using Service.User;

namespace Service.User.IntegrationEvents
{
    public class UserBooksReturnedHandler : IConsumer<IDeleteUserEvents>
    {
        private readonly UserService _userService;
        private readonly ILogger<UserBooksReturnedHandler> _logger;

        public UserBooksReturnedHandler(ILogger<UserBooksReturnedHandler> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<IDeleteUserEvents> context)
        {
            _logger.LogInformation("In the consume");

            var userId = context.Message.UserId;

            _userService.DeleteUserById(userId);

            _logger.LogInformation($"Deleted user {userId} after books returned.");
        }
    }
}
