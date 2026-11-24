
using LibraryManagement.Repositories.Interfaces;
using LibraryManagement.Policies;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Services;

public class BorrowingService : IBorrowingService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookCopyRepository _bookCopyRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IFineRepository _fineRepository;
    private readonly BorrowingPolicyFactory _policyFactory;

    public BorrowingService(
        IBookRepository bookRepository,
        IBookCopyRepository bookCopyRepository,
        IMemberRepository memberRepository,
        ILoanRepository loanRepository,
        IFineRepository fineRepository,
        BorrowingPolicyFactory policyFactory)
    {
        _bookRepository = bookRepository
            ?? throw new ArgumentNullException(nameof(bookRepository));

        _bookCopyRepository = bookCopyRepository
            ?? throw new ArgumentNullException(nameof(bookCopyRepository));

        _memberRepository = memberRepository
            ?? throw new ArgumentNullException(nameof(memberRepository));

        _loanRepository = loanRepository
            ?? throw new ArgumentNullException(nameof(loanRepository));

        _fineRepository = fineRepository
            ?? throw new ArgumentNullException(nameof(fineRepository));

        _policyFactory = policyFactory
            ?? throw new ArgumentNullException(nameof(policyFactory));
    }

    
    public Loan BorrowBook(
        Guid memberId,
        Guid bookId,
        DateOnly borrowingDate)
    {
        // Find the member.
        var member = _memberRepository.GetById(memberId);

        if (member is null)
            throw new InvalidOperationException("Member was not found.");

        //  Check whether the member is suspended.
        if (member.IsSuspended)
            throw new InvalidOperationException(
                "Suspended members cannot borrow books.");

        // Find the book.
        var book = _bookRepository.GetById(bookId);

        if (book is null)
            throw new InvalidOperationException("Book was not found.");

        // Check whether the book can be borrowed.
        if (!book.IsAvailableForLoan)
            throw new InvalidOperationException(
                "This book is not available for borrowing.");

        // Retrieve the member's borrowing policy.
        var policy = _policyFactory.GetPolicy(member.Category);

        // Retrieve the member's active loans.
        var activeLoans =
            _loanRepository.GetActiveLoansByMemberId(memberId);

        // Check the maximum borrowing limit.
        if (activeLoans.Count() >= policy.MaximumActiveLoans)
            throw new InvalidOperationException(
                "Member has reached the maximum borrowing limit.");

        // Prevent borrowing the same title twice.
        if (_loanRepository.HasActiveLoanForBook(memberId, bookId))
            throw new InvalidOperationException(
                "Member already has an active loan for this book.");

        // Check for overdue loans.
        if (activeLoans.Any(loan => borrowingDate > loan.DueDate))
            throw new InvalidOperationException(
                "Members with overdue books cannot borrow additional books.");

        // Check outstanding fines.
        var outstandingFines =
            _fineRepository.GetOutstandingFinesByMemberId(memberId);

        if (outstandingFines.Any())
            throw new InvalidOperationException(
                "Outstanding library charges must be settled before borrowing.");

        // Find an available physical copy.
        var availableCopy = _bookCopyRepository
            .GetByBookId(bookId)
            .FirstOrDefault(copy =>
                copy.Status == Domain.Enums.BookCopyStatus.Available);

        if (availableCopy is null)
            throw new InvalidOperationException(
                "No available copies of this book.");

        // Create the loan.
        var loan = new Loan(
            memberId,
            availableCopy.Id,
            borrowingDate,
            policy.LoanPeriodInDays);

        // 13. Update the physical copy's status.
        availableCopy.MarkAsBorrowed();

        // 14. Save the loan in the repository.
        _loanRepository.Add(loan);

        return loan;
    }


        
    public Fine? ReturnBook(Guid loanId, DateOnly returnDate)
    {
        // Find the loan.
        var loan = _loanRepository.GetById(loanId);

        if (loan is null)
            throw new InvalidOperationException("Loan was not found.");
        
        if (returnDate < loan.BorrowedDate)
            throw new ArgumentException(
                "Return date cannot be before the borrowing date.",
                nameof(returnDate));

        // Ensure the loan has not already been returned.
        if (loan.IsReturned)
            throw new InvalidOperationException(
                "This book has already been returned.");

        // Find the physical book copy.
        var bookCopy = _bookCopyRepository.GetById(loan.BookCopyId);

        if (bookCopy is null)
            throw new InvalidOperationException(
                "The physical book copy was not found.");

        // Ensure the copy is currently borrowed.
        if (bookCopy.Status != BookCopyStatus.Borrowed)
            throw new InvalidOperationException(
                "This book copy is not currently marked as borrowed.");

        // Retrieve the member's borrowing policy.
        var member = _memberRepository.GetById(loan.MemberId);

        if (member is null)
            throw new InvalidOperationException("Member was not found.");

        var policy = _policyFactory.GetPolicy(member.Category);

        // Calculate overdue days.
        int overdueDays = Math.Max(
            0,
            returnDate.DayNumber - loan.DueDate.DayNumber);

        Fine? fine = null;

        // Calculate and create an overdue fine if necessary.
        if (overdueDays > 0)
        {
            decimal fineAmount = Math.Min(
                overdueDays * policy.DailyOverdueFine,
                50.00m);

            fine = new Fine(
                loan.MemberId,
                loan.Id,
                fineAmount,
                FineType.Overdue,
                returnDate);
        }

        // Record the return.
        loan.ReturnBook(returnDate);

        // Make the physical copy available again.
        bookCopy.MarkAsAvailable();

        // Save the fine if one was generated.
        if (fine is not null)
            _fineRepository.Add(fine);

        return fine;
    }


    public Fine ReportLostBook(Guid loanId, DateOnly lostDate)
    {
        // 1. Find the loan.
        var loan = _loanRepository.GetById(loanId);

        if (loan is null)
            throw new InvalidOperationException("Loan was not found.");

        // 2. Ensure the loan is still active.
        if (!loan.IsActive)
            throw new InvalidOperationException(
                "Only active loans can be reported as lost.");

        // 3. Validate the lost date.
        if (lostDate < loan.BorrowedDate)
            throw new ArgumentException(
                "Lost date cannot be before the borrowing date.",
                nameof(lostDate));

        // 4. Find the physical book copy.
        var bookCopy = _bookCopyRepository.GetById(loan.BookCopyId);

        if (bookCopy is null)
            throw new InvalidOperationException(
                "The physical book copy was not found.");

        // 5. Find the book's details.
        var book = _bookRepository.GetById(bookCopy.BookId);

        if (book is null)
            throw new InvalidOperationException("Book was not found.");

        // 6. Find the member.
        var member = _memberRepository.GetById(loan.MemberId);

        if (member is null)
            throw new InvalidOperationException("Member was not found.");

        // 7. Retrieve the member's borrowing policy.
        var policy = _policyFactory.GetPolicy(member.Category);

        // 8. Calculate overdue days up to the lost date.
        int overdueDays = Math.Max(
            0,
            lostDate.DayNumber - loan.DueDate.DayNumber);

        // 9. Calculate the overdue fine, respecting the GHS 50 cap.
        decimal overdueFineAmount = Math.Min(
            overdueDays * policy.DailyOverdueFine,
            50.00m);

        // 10. Create the overdue fine if applicable.
        if (overdueFineAmount > 0)
        {
            var overdueFine = new Fine(
                member.Id,
                loan.Id,
                overdueFineAmount,
                FineType.Overdue,
                lostDate);

            _fineRepository.Add(overdueFine);
        }

        // 11. Create the replacement charge.
        var replacementFine = new Fine(
            member.Id,
            loan.Id,
            book.ReplacementCost,
            FineType.Replacement,
            lostDate);

        // 12. Mark the loan and physical copy as lost.
        loan.MarkAsLost(lostDate);
        bookCopy.MarkAsLost();

        // 13. Save the replacement charge.
        _fineRepository.Add(replacementFine);

        return replacementFine;
    }


}
