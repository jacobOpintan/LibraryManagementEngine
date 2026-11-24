
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public Book AddBook(
        string isbn,
        string title,
        string author,
        string publisher,
        int publicationYear,
        decimal replacementCost)
    {
        if (_bookRepository.ExistsByISBN(isbn))
        {
            throw new InvalidOperationException(
                "A book with this ISBN already exists.");
        }

        var book = new Book(
            isbn,
            title,
            author,
            publisher,
            publicationYear,
            replacementCost,true);

        _bookRepository.Add(book);

        return book;
    }

    public IEnumerable<Book> GetAllBooks()
    {
        return _bookRepository.GetAll();
    }

    public Book? GetBookById(Guid id)
    {
        return _bookRepository.GetById(id);
    }
}
