
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Services;

public class BookCopyService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookCopyRepository _bookCopyRepository;

    public BookCopyService(
        IBookRepository bookRepository,
        IBookCopyRepository bookCopyRepository)
    {
        _bookRepository = bookRepository;
        _bookCopyRepository = bookCopyRepository;
    }

    public BookCopy AddBookCopy(Guid bookId)
    {
        var book = _bookRepository.GetById(bookId);

        if (book is null)
            throw new InvalidOperationException("Book was not found.");

        var copy = new BookCopy(bookId);

        _bookCopyRepository.Add(copy);

        return copy;
    }

    public IEnumerable<BookCopy> GetCopiesByBookId(Guid bookId)
    {
        var book = _bookRepository.GetById(bookId);

        if (book is null)
            throw new InvalidOperationException("Book was not found.");

        return _bookCopyRepository.GetByBookId(bookId);
    }
}
