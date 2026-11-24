
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Policies;
using LibraryManagement.Repositories.Implementations;
using LibraryManagement.Services;
using Xunit;

namespace LibraryManagement.Tests.Services;

public class BorrowingServiceTests
{
    [Fact]
    public void BorrowBook_ShouldCreateLoanAndMarkCopyAsBorrowed()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var book = new Book(
            "9780132350884",
            "Clean Code",
            "Robert C. Martin",
            "Prentice Hall",
            2008,
            100.00m,
            true);

        bookRepository.Add(book);

        var bookCopy = new BookCopy(book.Id);
        bookCopyRepository.Add(bookCopy);

        var member = new Member(
            "MEM001",
            "John Doe",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var borrowingService = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        var borrowingDate = new DateOnly(2026, 11, 24);

        // Act
        var loan = borrowingService.BorrowBook(
            member.Id,
            book.Id,
            borrowingDate);

        // Assert
        Assert.NotNull(loan);
        Assert.Equal(member.Id, loan.MemberId);
        Assert.Equal(bookCopy.Id, loan.BookCopyId);
        Assert.Equal(
            borrowingDate.AddDays(14),
            loan.DueDate);

        Assert.Equal(
            BookCopyStatus.Borrowed,
            bookCopy.Status);

        Assert.Single(
            loanRepository.GetActiveLoansByMemberId(member.Id));
    }


    [Fact]
    public void BorrowBook_ShouldRejectSuspendedMember()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var book = new Book(
            "9780132350884",
            "Clean Code",
            "Robert C. Martin",
            "Prentice Hall",
            2008,
            100.00m,
            true);

        bookRepository.Add(book);

        var bookCopy = new BookCopy(book.Id);
        bookCopyRepository.Add(bookCopy);

        var member = new Member(
            "MEM002",
            "Jane Doe",
            MemberCategory.Standard);

        member.Suspend();
        memberRepository.Add(member);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                book.Id,
                new DateOnly(2026, 11, 24)));

        Assert.Equal(BookCopyStatus.Available, bookCopy.Status);
    }

    [Fact]
    public void BorrowBook_ShouldRejectNonCirculatingBook()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var book = new Book(
            "9780132350885",
            "Library Reference Book",
            "Jane Author",
            "Academic Press",
            2020,
            150.00m,
            false);

        bookRepository.Add(book);
        bookCopyRepository.Add(new BookCopy(book.Id));

        var member = new Member(
            "MEM003",
            "Peter Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                book.Id,
                new DateOnly(2026, 11, 24)));
    }

    [Fact]
    public void BorrowBook_ShouldRejectWhenNoCopyIsAvailable()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var book = new Book(
            "9780132350886",
            "C# Programming",
            "John Author",
            "Tech Press",
            2022,
            120.00m,
            true);

        bookRepository.Add(book);

        var bookCopy = new BookCopy(book.Id);
        bookCopyRepository.Add(bookCopy);
        bookCopy.MarkAsBorrowed();

        var member = new Member(
            "MEM004",
            "Mary Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                book.Id,
                new DateOnly(2026, 11, 24)));
    }


    [Fact]
    public void BorrowBook_ShouldRejectMemberWhoReachedLoanLimit()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM005",
            "David Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        // Create three different books and active loans.
        for (int i = 0; i < 3; i++)
        {
            var book = new Book(
                $"ISBN-TEST-{i}",
                $"Book {i}",
                "Author",
                "Publisher",
                2020,
                50.00m,
                true);

            bookRepository.Add(book);

            var copy = new BookCopy(book.Id);
            bookCopyRepository.Add(copy);
            copy.MarkAsBorrowed();

            var loan = new Loan(
                member.Id,
                copy.Id,
                new DateOnly(2026, 11, 1),
                14);

            loanRepository.Add(loan);
        }

        var additionalBook = new Book(
            "ISBN-ADDITIONAL",
            "Additional Book",
            "Author",
            "Publisher",
            2020,
            50.00m,
            true);

        bookRepository.Add(additionalBook);
        bookCopyRepository.Add(new BookCopy(additionalBook.Id));

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                additionalBook.Id,
                new DateOnly(2026, 11, 24)));
    }

    [Fact]
    public void BorrowBook_ShouldRejectDuplicateActiveLoanForSameBook()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM006",
            "Alice Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-DUPLICATE",
            "C# Fundamentals",
            "Author",
            "Publisher",
            2022,
            80.00m,
            true);

        bookRepository.Add(book);

        var firstCopy = new BookCopy(book.Id);
        var secondCopy = new BookCopy(book.Id);

        bookCopyRepository.Add(firstCopy);
        bookCopyRepository.Add(secondCopy);

        firstCopy.MarkAsBorrowed();

        var existingLoan = new Loan(
            member.Id,
            firstCopy.Id,
            new DateOnly(2026, 11, 20),
            14);

        loanRepository.Add(existingLoan);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                book.Id,
                new DateOnly(2026, 11, 24)));

        Assert.Equal(BookCopyStatus.Available, secondCopy.Status);
    }

    [Fact]
    public void BorrowBook_ShouldRejectMemberWithOverdueLoan()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM007",
            "Robert Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var existingBook = new Book(
            "ISBN-OVERDUE",
            "Overdue Book",
            "Author",
            "Publisher",
            2020,
            50.00m,
            true);

        bookRepository.Add(existingBook);

        var existingCopy = new BookCopy(existingBook.Id);
        bookCopyRepository.Add(existingCopy);
        existingCopy.MarkAsBorrowed();

        // Borrowed on 1 November, due on 15 November.
        var existingLoan = new Loan(
            member.Id,
            existingCopy.Id,
            new DateOnly(2026, 11, 1),
            14);

        loanRepository.Add(existingLoan);

        var newBook = new Book(
            "ISBN-NEW",
            "New Book",
            "Author",
            "Publisher",
            2020,
            60.00m,
            true);

        bookRepository.Add(newBook);
        var newCopy = new BookCopy(newBook.Id);
        bookCopyRepository.Add(newCopy);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                newBook.Id,
                new DateOnly(2026, 11, 24)));
    }

    [Fact]
    public void BorrowBook_ShouldRejectMemberWithOutstandingFines()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM008",
            "Michael Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        // Create an outstanding fine.
        var fine = new Fine(
            member.Id,
            null,
            20.00m,
            FineType.Overdue,
            new DateOnly(2026, 11, 20));

        fineRepository.Add(fine);

        var book = new Book(
            "ISBN-FINE-TEST",
            "Software Engineering",
            "Author",
            "Publisher",
            2022,
            100.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.BorrowBook(
                member.Id,
                book.Id,
                new DateOnly(2026, 11, 24)));

        Assert.Equal(BookCopyStatus.Available, copy.Status);
    }
    [Fact]
    public void ReturnBook_ShouldReturnCopyWithoutFineWhenReturnedOnTime()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM009",
            "Sarah Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-RETURN-TEST",
            "Object Oriented Programming",
            "Author",
            "Publisher",
            2021,
            90.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var borrowingDate = new DateOnly(2026, 11, 1);

        var loan = new Loan(
            member.Id,
            copy.Id,
            borrowingDate,
            14);

        loanRepository.Add(loan);
        copy.MarkAsBorrowed();

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        var returnDate = new DateOnly(2026, 11, 10);

        // Act
        var fine = service.ReturnBook(loan.Id, returnDate);

        // Assert
        Assert.Null(fine);

        Assert.True(loan.IsReturned);
        Assert.Equal(returnDate, loan.ReturnDate);

        Assert.Equal(BookCopyStatus.Available, copy.Status);

        Assert.Empty(fineRepository.GetByMemberId(member.Id));
    }

    [Fact]
    public void ReturnBook_ShouldGenerateCorrectOverdueFine()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM010",
            "Daniel Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-OVERDUE-RETURN",
            "Advanced C#",
            "Author",
            "Publisher",
            2022,
            120.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var borrowingDate = new DateOnly(2026, 11, 1);

        var loan = new Loan(
            member.Id,
            copy.Id,
            borrowingDate,
            14);

        loanRepository.Add(loan);
        copy.MarkAsBorrowed();

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Due date: 15 November 2026.
        // Return date: 18 November 2026.
        var returnDate = new DateOnly(2026, 11, 18);

        // Act
        var fine = service.ReturnBook(loan.Id, returnDate);

        // Assert
        Assert.NotNull(fine);

        Assert.Equal(6.00m, fine.Amount);
        Assert.Equal(FineType.Overdue, fine.Type);
        Assert.Equal(member.Id, fine.MemberId);
        Assert.Equal(loan.Id, fine.LoanId);

        Assert.True(loan.IsReturned);
        Assert.Equal(BookCopyStatus.Available, copy.Status);

        Assert.Single(fineRepository.GetByMemberId(member.Id));
    }


    [Fact]
    public void ReturnBook_ShouldNotExceedMaximumFineCap()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM011",
            "James Smith",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-FINE-CAP",
            "Enterprise C#",
            "Author",
            "Publisher",
            2022,
            150.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var borrowingDate = new DateOnly(2026, 11, 1);

        var loan = new Loan(
            member.Id,
            copy.Id,
            borrowingDate,
            14);

        loanRepository.Add(loan);
        copy.MarkAsBorrowed();

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Due date: 15 November 2026.
        // Return date: 20 February 2027.
        var returnDate = new DateOnly(2027, 2, 20);

        // Act
        var fine = service.ReturnBook(loan.Id, returnDate);

        // Assert
        Assert.NotNull(fine);
        Assert.Equal(50.00m, fine.Amount);
        Assert.Equal(50.00m, fine.OutstandingBalance);
    }

    [Fact]
    public void ReturnBook_ShouldRejectAlreadyReturnedLoan()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM012",
            "Peter Brown",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-ALREADY-RETURNED",
            "Clean Architecture",
            "Author",
            "Publisher",
            2020,
            100.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var loan = new Loan(
            member.Id,
            copy.Id,
            new DateOnly(2026, 11, 1),
            14);

        loanRepository.Add(loan);

        copy.MarkAsBorrowed();

        // Return the book once.
        loan.ReturnBook(new DateOnly(2026, 11, 10));
        copy.MarkAsAvailable();

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() =>
            service.ReturnBook(
                loan.Id,
                new DateOnly(2026, 11, 12)));
    }


    [Fact]
    public void ReturnBook_ShouldRejectDateBeforeBorrowingDate()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM013",
            "Mary Brown",
            MemberCategory.Standard);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-INVALID-RETURN",
            "Programming Fundamentals",
            "Author",
            "Publisher",
            2021,
            80.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);
        copy.MarkAsBorrowed();

        var borrowingDate = new DateOnly(2026, 11, 10);

        var loan = new Loan(
            member.Id,
            copy.Id,
            borrowingDate,
            14);

        loanRepository.Add(loan);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Act and Assert
        Assert.Throws<ArgumentException>(() =>
            service.ReturnBook(
                loan.Id,
                new DateOnly(2026, 11, 5)));

        // The failed operation must not modify the loan or copy.
        Assert.False(loan.IsReturned);
        Assert.Equal(BookCopyStatus.Borrowed, copy.Status);
    }


    [Fact]
    public void BorrowBook_ShouldApplyStudentBorrowingPolicy()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM014",
            "Student Member",
            MemberCategory.Student);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-STUDENT-TEST",
            "Introduction to Programming",
            "Author",
            "Publisher",
            2023,
            75.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        var borrowingDate = new DateOnly(2026, 11, 24);

        // Act
        var loan = service.BorrowBook(
            member.Id,
            book.Id,
            borrowingDate);

        // Assert
        Assert.Equal(
            borrowingDate.AddDays(21),
            loan.DueDate);

        Assert.Equal(member.Id, loan.MemberId);
        Assert.Equal(copy.Id, loan.BookCopyId);
        Assert.Equal(BookCopyStatus.Borrowed, copy.Status);
    }


    [Fact]
    public void ReturnBook_ShouldApplyStudentOverdueFineRate()
    {
        // Arrange
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM015",
            "Student Member",
            MemberCategory.Student);

        memberRepository.Add(member);

        var book = new Book(
            "ISBN-STUDENT-FINE",
            "Data Structures",
            "Author",
            "Publisher",
            2023,
            90.00m,
            true);

        bookRepository.Add(book);

        var copy = new BookCopy(book.Id);
        bookCopyRepository.Add(copy);

        var loan = new Loan(
            member.Id,
            copy.Id,
            new DateOnly(2026, 11, 1),
            21);

        loanRepository.Add(loan);
        copy.MarkAsBorrowed();

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Due date: 22 November 2026.
        // Return date: 25 November 2026.
        var returnDate = new DateOnly(2026, 11, 25);

        // Act
        var fine = service.ReturnBook(loan.Id, returnDate);

        // Assert
        Assert.NotNull(fine);
        Assert.Equal(3.00m, fine.Amount);
        Assert.Equal(FineType.Overdue, fine.Type);
    }

    [Fact]
    public void Renew_ShouldRejectRenewalOnDueDate()
    {
        var loan = new Loan(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 11, 1),
            14);

        var dueDate = new DateOnly(2026, 11, 15);

        Assert.Throws<InvalidOperationException>(() =>
            loan.Renew(dueDate, 14, 1));
    }


    [Fact]
    public void BorrowBook_ShouldRejectStudentWhoReachedLoanLimit()
    {
        var bookRepository = new InMemoryBookRepository();
        var bookCopyRepository = new InMemoryBookCopyRepository();
        var memberRepository = new InMemoryMemberRepository();
        var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
        var fineRepository = new InMemoryFineRepository();
        var policyFactory = new BorrowingPolicyFactory();

        var member = new Member(
            "MEM016",
            "Student Member",
            MemberCategory.Student);

        memberRepository.Add(member);

        // Create six different books and copies.
        var books = new List<Book>();

        for (int i = 1; i <= 6; i++)
        {
            var book = new Book(
                $"ISBN-STUDENT-{i}",
                $"Student Book {i}",
                "Author",
                "Publisher",
                2023,
                50.00m,
                true);

            bookRepository.Add(book);
            books.Add(book);

            var copy = new BookCopy(book.Id);
            bookCopyRepository.Add(copy);

            // The first five books are already borrowed.
            if (i <= 5)
            {
                copy.MarkAsBorrowed();

                var loan = new Loan(
                    member.Id,
                    copy.Id,
                    new DateOnly(2026, 11, 1),
                    21);

                loanRepository.Add(loan);
            }
        }

        var service = new BorrowingService(
            bookRepository,
            bookCopyRepository,
            memberRepository,
            loanRepository,
            fineRepository,
            policyFactory);

        // Attempt to borrow the sixth book.
        var exception = Assert.Throws<InvalidOperationException>(
            () => service.BorrowBook(
                member.Id,
                books[5].Id,
                new DateOnly(2026, 11, 2)));

        Assert.Equal(
            "Member has reached the maximum borrowing limit.",
            exception.Message);
    }


}