
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Repositories.Interfaces;

public interface IBookCopyRepository
{
    void Add(BookCopy bookCopy);

    BookCopy? GetById(Guid id);

    IEnumerable<BookCopy> GetByBookId(Guid bookId);

    IEnumerable<BookCopy> GetAll();

    int CountByStatus(Guid bookId, BookCopyStatus status);
}
