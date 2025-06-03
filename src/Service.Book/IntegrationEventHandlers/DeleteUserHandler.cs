using Library.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Service.Book.IntegrationEventHandlers
{
    public class DeleteUserHandler : IConsumer<IReturnDeletedUsersBooks>
    {
        private readonly BookContext _context;
        private readonly ILogger<DeleteUserHandler> _logger;
        List<(int BookId, bool IsAvailable, string? UserId)> previousBookStates = new();

        public DeleteUserHandler(ILogger<DeleteUserHandler> logger, BookContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IReturnDeletedUsersBooks> context)
        {
            var userId = context.Message.UserId;

            try
            {
                previousBookStates = await ReturnBooksToLibrary(userId);

                var deleteUserRequestedEvent = new
                {
                    UserId = userId
                };

                _logger.LogInformation("Before publish");

                await context.Publish<IDeleteUserEvents>(deleteUserRequestedEvent);

                _logger.LogInformation("Published");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning books");

                await RollbackBookChanges(previousBookStates);
            }  
        }

        private async Task<List<(int BookId, bool IsAvailable, string? UserId)>> ReturnBooksToLibrary(string userId)
        {
            var books = await _context.Books.Where(b => b.UserId == userId.ToString()).ToListAsync();
            var previousStates = books.Select(b => (b.Id, b.isAvailable, b.UserId)).ToList();

            foreach (var book in books)
            {
                book.isAvailable = true;
                book.UserId = null;
            }

            _logger.LogInformation("Save is in progress......");

            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved");

            return previousStates;
        }

        private async Task RollbackBookChanges(List<(int BookId, bool IsAvailable, string? UserId)> previousBookStates)
        {
            foreach (var (bookId, wasAvailable, userId) in previousBookStates)
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book != null)
                {
                    book.isAvailable = wasAvailable;
                    book.UserId = userId;
                }
            }

            _logger.LogInformation("Rollback is in progress...");
            await _context.SaveChangesAsync();
            _logger.LogInformation("Rollback completed.");
        }
    }
}

