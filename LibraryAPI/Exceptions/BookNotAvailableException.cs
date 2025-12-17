namespace LibraryAPI.Exceptions;

public class BookNotAvailableException : Exception
{
    public BookNotAvailableException(int bookId)
        : base($"Book with ID {bookId} is not available for loan") { }
}
