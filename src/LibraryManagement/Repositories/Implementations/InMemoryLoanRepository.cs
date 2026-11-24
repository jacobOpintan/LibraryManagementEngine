
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryLoanRepository : ILoanRepository
{
    private readonly List<Loan> _loans = new();
    private readonly IBookCopyRepository _bookCopyRepository;

    public InMemoryLoanRepository(
        IBookCopyRepository bookCopyRepository)
    {
        _bookCopyRepository = bookCopyRepository
            ?? throw new ArgumentNullException(
                nameof(bookCopyRepository));
    }

    public void Add(Loan loan)
    {
        if (loan is null)
            throw new ArgumentNullException(nameof(loan));

        if (_loans.Any(existingLoan => existingLoan.Id == loan.Id))
            throw new InvalidOperationException(
                "This loan already exists.");

        _loans.Add(loan);
    }

    public Loan? GetById(Guid id)
    {
        return _loans.FirstOrDefault(loan => loan.Id == id);
    }

    public IEnumerable<Loan> GetByMemberId(Guid memberId)
    {
        return _loans
            .Where(loan => loan.MemberId == memberId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Loan> GetByBookCopyId(Guid bookCopyId)
    {
        return _loans
            .Where(loan => loan.BookCopyId == bookCopyId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Loan> GetAll()
    {
        return _loans.AsReadOnly();
    }

    public IEnumerable<Loan> GetActiveLoansByMemberId(Guid memberId)
    {
        return _loans
            .Where(loan =>
                loan.MemberId == memberId &&
                !loan.IsReturned)
            .ToList()
            .AsReadOnly();
    }

    public bool HasActiveLoanForBook(Guid memberId, Guid bookId)
    {
        var activeLoans = GetActiveLoansByMemberId(memberId);

        return activeLoans.Any(loan =>
        {
            var bookCopy = _bookCopyRepository.GetById(
                loan.BookCopyId);

            return bookCopy is not null &&
                   bookCopy.BookId == bookId;
        });
    }
}
