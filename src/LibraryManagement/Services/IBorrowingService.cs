
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Services;

public interface IBorrowingService
{
    Loan BorrowBook(
        Guid memberId,
        Guid bookId,
        DateOnly borrowingDate);

    Fine? ReturnBook(
        Guid loanId,
        DateOnly returnDate);
}
