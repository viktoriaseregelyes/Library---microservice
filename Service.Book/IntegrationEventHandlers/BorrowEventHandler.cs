using Library.Events;
using MassTransit;

namespace Service.Book.IntegrationEventHandlers
{
    public class BorrowEventHandler : IConsumer<IBorrowingEvents>
    {
        private readonly ILogger<BorrowEventHandler> _logger;
        private readonly BookContext _context;

        public BorrowEventHandler(ILogger<BorrowEventHandler> logger, BookContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<IBorrowingEvents> borrowedBook)
        {
            if(borrowedBook.Message.BorrowType)
            {
                _logger.LogInformation("Processing loan event for BookID: {BookID}, UserID: {UserID}", borrowedBook.Message.BookID, borrowedBook.Message.UserID);

                var book = await _context.Books.FindAsync(borrowedBook.Message.BookID);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookID} not found", borrowedBook.Message.BookID);
                    return;
                }
                else if(!book.isAvailable)
                {
                    _logger.LogWarning("Book with ID {BookID} is already borrowed", borrowedBook.Message.BookID);
                    return;
                }

                book.isAvailable = false;
                book.UserId = borrowedBook.Message.UserID;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Book with ID {BookID} is now borrowed by User {UserID}", borrowedBook.Message.BookID, borrowedBook.Message.UserID);
            }
            else
            {
                _logger.LogInformation("Processing return event for BookID: {BookID}, UserID: {UserID}", borrowedBook.Message.BookID, borrowedBook.Message.UserID);

                var book = await _context.Books.FindAsync(borrowedBook.Message.BookID);
                if (book == null)
                {
                    _logger.LogWarning("Book with ID {BookID} not found", borrowedBook.Message.BookID);
                    return;
                }
                else if (book.isAvailable)
                {
                    _logger.LogWarning("Book with ID {BookID} is in the library", borrowedBook.Message.BookID);
                    return;
                }

                book.isAvailable = true;
                book.UserId = null;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Book with ID {BookID} is now returned by User {UserID}", borrowedBook.Message.BookID, borrowedBook.Message.UserID);
            }
        }
    }
}
