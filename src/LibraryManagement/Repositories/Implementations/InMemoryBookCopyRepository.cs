
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryBookCopyRepository : IBookCopyRepository
{
    private readonly List<BookCopy> _bookCopies = new();

    public void Add(BookCopy bookCopy)
    {
        if (bookCopy is null)
            throw new ArgumentNullException(nameof(bookCopy));

        if (_bookCopies.Any(copy => copy.Id == bookCopy.Id))
            throw new InvalidOperationException(
                "This book copy already exists.");

        _bookCopies.Add(bookCopy);
    }

    public BookCopy? GetById(Guid id)
    {
        return _bookCopies.FirstOrDefault(copy => copy.Id == id);
    }

    public IEnumerable<BookCopy> GetByBookId(Guid bookId)
    {
        return _bookCopies
            .Where(copy => copy.BookId == bookId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<BookCopy> GetAll()
    {
        return _bookCopies.AsReadOnly();
    }

    public int CountByStatus(Guid bookId, BookCopyStatus status)
    {
        return _bookCopies.Count(
            copy => copy.BookId == bookId && copy.Status == status);
    }
}
