
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface ILoanRepository
{
    void Add(Loan loan);

    Loan? GetById(Guid id);

    IEnumerable<Loan> GetByMemberId(Guid memberId);

    IEnumerable<Loan> GetByBookCopyId(Guid bookCopyId);

    IEnumerable<Loan> GetAll();

    IEnumerable<Loan> GetActiveLoansByMemberId(Guid memberId);

    bool HasActiveLoanForBook(Guid memberId, Guid bookId);
}
