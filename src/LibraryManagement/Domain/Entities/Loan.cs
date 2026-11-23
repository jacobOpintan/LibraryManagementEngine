
namespace LibraryManagement.Domain.Entities;

public class Loan
{
    public Guid Id { get; private set; }

    public Guid MemberId { get; private set; }

    public Guid BookCopyId { get; private set; }

    public DateOnly BorrowedDate { get; private set; }

    public DateOnly DueDate { get; private set; }

    public DateOnly? ReturnDate { get; private set; }

    public int RenewalCount { get; private set; }

    public DateOnly? LastRenewalDate { get; private set; }

    public bool IsReturned => ReturnDate.HasValue;

    public Loan(
        Guid memberId,
        Guid bookCopyId,
        DateOnly borrowedDate,
        int loanPeriodInDays)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException(
                "Member ID cannot be empty.",
                nameof(memberId));

        if (bookCopyId == Guid.Empty)
            throw new ArgumentException(
                "Book copy ID cannot be empty.",
                nameof(bookCopyId));

        if (loanPeriodInDays <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(loanPeriodInDays),
                "Loan period must be greater than zero.");

        Id = Guid.NewGuid();
        MemberId = memberId;
        BookCopyId = bookCopyId;
        BorrowedDate = borrowedDate;
        DueDate = borrowedDate.AddDays(loanPeriodInDays);
        RenewalCount = 0;
    }

    //implementing method for returning a book
    public void ReturnBook(DateOnly returnDate)
    {
        if (IsReturned)
            throw new InvalidOperationException(
                "This book has already been returned.");

        if (returnDate < BorrowedDate)
            throw new ArgumentException(
                "Return date cannot be before the borrowing date.",
                nameof(returnDate));

        ReturnDate = returnDate;
    }


    //implementing method  for renewal of loaned book 
    public void Renew(DateOnly renewalDate, int loanPeriodInDays, int maximumRenewals)
    {
        if (IsReturned)
            throw new InvalidOperationException(
                "A returned loan cannot be renewed.");

        if (renewalDate < BorrowedDate)
            throw new ArgumentException(
                "Renewal date cannot be before the borrowing date.",
                nameof(renewalDate));

        if (renewalDate > DueDate)
            throw new InvalidOperationException(
                "An overdue loan cannot be renewed.");

        if (loanPeriodInDays <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(loanPeriodInDays),
                "Loan period must be greater than zero.");

        if (maximumRenewals < 0)
            throw new ArgumentOutOfRangeException(
                nameof(maximumRenewals),
                "Maximum renewals cannot be negative.");

        if (RenewalCount >= maximumRenewals)
            throw new InvalidOperationException(
                "Maximum renewal limit has been reached.");

        DueDate = DueDate.AddDays(loanPeriodInDays);
        RenewalCount++;
        LastRenewalDate = renewalDate;
    }


}
