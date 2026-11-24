
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books = new();

    public void Add(Book book)
    {
        if (book is null)
            throw new ArgumentNullException(nameof(book));

        if (ExistsByISBN(book.ISBN))
            throw new InvalidOperationException(
                "A book with this ISBN already exists.");

        _books.Add(book);
    }

    public Book? GetById(Guid id)
    {
        return _books.FirstOrDefault(book => book.Id == id);
    }

    public Book? GetByISBN(string isbn)
    {
        return _books.FirstOrDefault(
            book => string.Equals(
                book.ISBN,
                isbn,
                StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Book> GetAll()
    {
        return _books.AsReadOnly();
    }

    public bool ExistsByISBN(string isbn)
    {
        return _books.Any(
            book => string.Equals(
                book.ISBN,
                isbn,
                StringComparison.OrdinalIgnoreCase));
    }
}
