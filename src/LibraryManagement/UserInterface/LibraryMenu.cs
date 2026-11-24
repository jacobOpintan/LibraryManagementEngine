
using LibraryManagement.Domain.Enums;
using LibraryManagement.Services;

namespace LibraryManagement.UserInterface;

public class LibraryMenu
{
    private readonly MemberService _memberService;
    private readonly BookService _bookService;
    private readonly BookCopyService _bookCopyService;
    private readonly IBorrowingService _borrowingService;
    private readonly IFinancialService _financialService;
    private readonly IReservationService _reservationService;

    public LibraryMenu(MemberService memberService,BookService bookService,BookCopyService bookCopyService,IBorrowingService borrowingService,IFinancialService financialService,IReservationService reservationService)
    {
        _memberService = memberService;
        _bookService = bookService;
        _bookCopyService = bookCopyService;
        _borrowingService = borrowingService;
        _financialService = financialService;
        _reservationService = reservationService;


    }

    public void Run()
    {
        bool isRunning = true;

        Console.WriteLine("======================================");
        Console.WriteLine("       LIBRARY MANAGEMENT SYSTEM");
        Console.WriteLine("======================================");

        while (isRunning)
        {
            DisplayMenu();

            Console.Write("\nSelect an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllBooks();
                    break;

                case "2":
                    RegisterMember();
                    break;

                case "3":
                    ViewAllMembers();
                    break;

                case "4":
                    isRunning = false;
                    Console.WriteLine("\nThank you for using the Library Management System!");
                    break;
                case "5":
                    AddBook();
                    break;

                case "6":
                    AddBookCopy();
                    break;

                case "7":
                    ViewBookCopies();
                    break;
                case "8":
                    BorrowBook();
                    break;

                case "9":
                    ReturnBook();
                    break;
                case "10":
                    ReportLostBook();
                    break;

                case "11":
                    PayFine();
                    break;
                case "12":
                    ReserveBook();
                    break;

                case "13":
                    CancelReservation();
                    break;

                case "14":
                    ViewReservationQueue();
                    break;
                default:
                    Console.WriteLine("\nInvalid option. Please select 1–4.");
                    break;
            }
        }
    }

    
    private void DisplayMenu()
    {
        Console.WriteLine("\n============== MAIN MENU ==============");

        Console.WriteLine("\n--- BOOK MANAGEMENT ---");
        Console.WriteLine("1. View all books");
        Console.WriteLine("5. Add a new book");
        Console.WriteLine("6. Add physical book copies");
        Console.WriteLine("7. View book copies");

        Console.WriteLine("\n--- MEMBER MANAGEMENT ---");
        Console.WriteLine("2. Register a member");
        Console.WriteLine("3. View all members");

        Console.WriteLine("\n--- BORROWING MANAGEMENT ---");
        Console.WriteLine("8. Borrow a book");
        Console.WriteLine("9. Return a book");
        Console.WriteLine("10. Report a lost book");

        Console.WriteLine("\n--- FINANCIAL MANAGEMENT ---");
        Console.WriteLine("11. Pay a fine");

        Console.WriteLine("\n--- RESERVATION MANAGEMENT ---");
        Console.WriteLine("12. Reserve a book");
        Console.WriteLine("13. Cancel a reservation");
        Console.WriteLine("14. View reservation queue");

        Console.WriteLine("\n4. Exit");
        Console.WriteLine("=======================================");
    }


        private void ViewAllBooks()
    {
        Console.WriteLine("\n--- ALL BOOKS ---");

        var books = _bookService.GetAllBooks().ToList();

        if (!books.Any())
        {
            Console.WriteLine("No books have been registered yet.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"ISBN: {book.ISBN}");
            Console.WriteLine($"Title: {book.Title}");
            Console.WriteLine($"Author: {book.Author}");
            Console.WriteLine($"Publisher: {book.Publisher}");
            Console.WriteLine($"Publication Year: {book.PublicationYear}");
            Console.WriteLine($"Replacement Cost: GHS {book.ReplacementCost}");
            Console.WriteLine($"Available for Loan: {book.IsAvailableForLoan}");
        }
    }

    private void AddBook()
    {
        Console.WriteLine("\n--- ADD NEW BOOK ---");

        try
        {
            Console.Write("ISBN: ");
            string isbn = Console.ReadLine() ?? "";

            Console.Write("Title: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Author: ");
            string author = Console.ReadLine() ?? "";

            Console.Write("Publisher: ");
            string publisher = Console.ReadLine() ?? "";

            Console.Write("Publication year: ");
            if (!int.TryParse(Console.ReadLine(), out int publicationYear))
            {
                Console.WriteLine("Invalid publication year.");
                return;
            }

            Console.Write("Replacement cost (GHS): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal replacementCost))
            {
                Console.WriteLine("Invalid replacement cost.");
                return;
            }

            var book = _bookService.AddBook(
                isbn,
                title,
                author,
                publisher,
                publicationYear,
                replacementCost);

            Console.WriteLine("\nBook added successfully!");
            Console.WriteLine($"Title: {book.Title}");
            Console.WriteLine($"ISBN: {book.ISBN}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nUnable to add book: {ex.Message}");
        }
    }

    private void RegisterMember()
    {
        Console.WriteLine("\n--- REGISTER MEMBER ---");

        try
        {
            Console.Write("Membership number: ");
            string membershipNumber = Console.ReadLine() ?? "";

            Console.Write("Full name: ");
            string fullName = Console.ReadLine() ?? "";

            Console.WriteLine("\nMember category:");
            Console.WriteLine("1. Standard");
            Console.WriteLine("2. Student");

            Console.Write("Select category: ");
            string? categoryChoice = Console.ReadLine();

            MemberCategory category = categoryChoice switch
            {
                "1" => MemberCategory.Standard,
                "2" => MemberCategory.Student,
                _ => throw new ArgumentException("Invalid member category.")
            };

            var member = _memberService.RegisterMember(
                membershipNumber,
                fullName,
                category);

            Console.WriteLine("\nMember registered successfully!");
            Console.WriteLine($"Membership Number: {member.MembershipNumber}");
            Console.WriteLine($"Name: {member.FullName}");
            Console.WriteLine($"Category: {member.Category}");
            Console.WriteLine($"Member ID: {member.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nRegistration failed: {ex.Message}");
        }
    }

    private void ViewAllMembers()
    {
        Console.WriteLine("\n--- REGISTERED MEMBERS ---");

        var members = _memberService.GetAllMembers().ToList();

        if (!members.Any())
        {
            Console.WriteLine("No members have been registered yet.");
            return;
        }

        foreach (var member in members)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Membership Number: {member.MembershipNumber}");
            Console.WriteLine($"Name: {member.FullName}");
            Console.WriteLine($"Category: {member.Category}");
            Console.WriteLine($"Suspended: {(member.IsSuspended ? "Yes" : "No")}");
        }
    }

    private void AddBookCopy()
    {
        Console.WriteLine("\n--- ADD BOOK COPIES ---");

        try
        {
            Console.Write("Enter the Book ID: ");
            string? input = Console.ReadLine();

            if (!Guid.TryParse(input, out Guid bookId))
            {
                Console.WriteLine("Invalid Book ID.");
                return;
            }

            Console.Write("How many copies do you want to add? ");

            if (!int.TryParse(Console.ReadLine(), out int quantity) ||
                quantity <= 0)
            {
                Console.WriteLine("Please enter a valid positive quantity.");
                return;
            }

            for (int i = 0; i < quantity; i++)
            {
                var copy = _bookCopyService.AddBookCopy(bookId);
                Console.WriteLine($"Copy added. Copy ID: {copy.Id}");
            }

            Console.WriteLine("\nBook copies added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to add copies: {ex.Message}");
        }
    }

    private void ViewBookCopies()
    {
        Console.WriteLine("\n--- VIEW BOOK COPIES ---");

        try
        {
            Console.Write("Enter the Book ID: ");
            string? input = Console.ReadLine();

            if (!Guid.TryParse(input, out Guid bookId))
            {
                Console.WriteLine("Invalid Book ID.");
                return;
            }

            var copies = _bookCopyService.GetCopiesByBookId(bookId).ToList();

            if (!copies.Any())
            {
                Console.WriteLine("No copies found for this book.");
                return;
            }

            foreach (var copy in copies)
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"Copy ID: {copy.Id}");
                Console.WriteLine($"Book ID: {copy.BookId}");
                Console.WriteLine($"Status: {copy.Status}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unable to retrieve copies: {ex.Message}");
        }
    }

    private void BorrowBook()
    {
        Console.WriteLine("\n--- BORROW A BOOK ---");

        try
        {
            Console.Write("Enter Member ID: ");

            if (!Guid.TryParse(Console.ReadLine(), out Guid memberId))
            {
                Console.WriteLine("Invalid Member ID.");
                return;
            }

            Console.Write("Enter Book ID: ");

            if (!Guid.TryParse(Console.ReadLine(), out Guid bookId))
            {
                Console.WriteLine("Invalid Book ID.");
                return;
            }

            var loan = _borrowingService.BorrowBook(
                memberId,
                bookId,
                DateOnly.FromDateTime(DateTime.Today));

            Console.WriteLine("\nBook borrowed successfully!");
            Console.WriteLine($"Loan ID: {loan.Id}");
            Console.WriteLine($"Borrowed Date: {loan.BorrowedDate}");
            Console.WriteLine($"Due Date: {loan.DueDate}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nBorrowing failed: {ex.Message}");
        }
    }

    private void ReturnBook()
    {
        Console.WriteLine("\n--- RETURN A BOOK ---");

        try
        {
            Console.Write("Enter Loan ID: ");

            if (!Guid.TryParse(Console.ReadLine(), out Guid loanId))
            {
                Console.WriteLine("Invalid Loan ID.");
                return;
            }

            var fine = _borrowingService.ReturnBook(
                loanId,
                DateOnly.FromDateTime(DateTime.Today));

            Console.WriteLine("\nBook returned successfully!");

            if (fine is not null)
            {
                Console.WriteLine($"Fine issued: GHS {fine.Amount}");
            }
            else
            {
                Console.WriteLine("No overdue fine was incurred.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nReturn failed: {ex.Message}");
        }
    }


    private void ReportLostBook()
    {
        Console.WriteLine("\n--- REPORT LOST BOOK ---");

        try
        {
            Console.Write("Enter Loan ID: ");

            if (!Guid.TryParse(Console.ReadLine(), out Guid loanId))
            {
                Console.WriteLine("Invalid Loan ID.");
                return;
            }

            var fine = _borrowingService.ReportLostBook(
                loanId,
                DateOnly.FromDateTime(DateTime.Today));

            Console.WriteLine("\nLost book reported successfully!");
            Console.WriteLine($"Replacement charge: GHS {fine.Amount}");
            Console.WriteLine($"Fine ID: {fine.Id}");
            Console.WriteLine($"Outstanding balance: GHS {fine.OutstandingBalance}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nUnable to report lost book: {ex.Message}");
        }
    }


    private void PayFine()
    {
        Console.WriteLine("\n--- PAY FINE ---");

        try
        {
            Console.Write("Enter Fine ID: ");

            if (!Guid.TryParse(Console.ReadLine(), out Guid fineId))
            {
                Console.WriteLine("Invalid Fine ID.");
                return;
            }

            Console.Write("Enter payment amount (GHS): ");

            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Invalid payment amount.");
                return;
            }

            var payment = _financialService.PayFine(
                fineId,
                amount,
                DateOnly.FromDateTime(DateTime.Today));

            Console.WriteLine("\nPayment recorded successfully!");
            Console.WriteLine($"Payment ID: {payment.Id}");
            Console.WriteLine($"Amount paid: GHS {payment.Amount}");
            Console.WriteLine($"Payment date: {payment.PaymentDate}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nPayment failed: {ex.Message}");
        }
    }

private void ReserveBook()
{
    Console.WriteLine("\n--- RESERVE A BOOK ---");

    try
    {
        Console.Write("Enter Member ID: ");

        if (!Guid.TryParse(Console.ReadLine(), out Guid memberId))
        {
            Console.WriteLine("Invalid Member ID.");
            return;
        }

        Console.Write("Enter Book ID: ");

        if (!Guid.TryParse(Console.ReadLine(), out Guid bookId))
        {
            Console.WriteLine("Invalid Book ID.");
            return;
        }

        var reservation = _reservationService.ReserveBook(
            memberId,
            bookId,
            DateOnly.FromDateTime(DateTime.Today));

        Console.WriteLine("\nReservation created successfully!");
        Console.WriteLine($"Reservation ID: {reservation.Id}");
        Console.WriteLine($"Reservation Date: {reservation.ReservationDate}");
        Console.WriteLine($"Status: {reservation.Status}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nReservation failed: {ex.Message}");
    }
}

private void CancelReservation()
{
    Console.WriteLine("\n--- CANCEL RESERVATION ---");

    try
    {
        Console.Write("Enter Reservation ID: ");

        if (!Guid.TryParse(Console.ReadLine(), out Guid reservationId))
        {
            Console.WriteLine("Invalid Reservation ID.");
            return;
        }

        _reservationService.CancelReservation(reservationId);

        Console.WriteLine("\nReservation cancelled successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nCancellation failed: {ex.Message}");
    }
}

private void ViewReservationQueue()
{
    Console.WriteLine("\n--- RESERVATION QUEUE ---");

    try
    {
        Console.Write("Enter Book ID: ");

        if (!Guid.TryParse(Console.ReadLine(), out Guid bookId))
        {
            Console.WriteLine("Invalid Book ID.");
            return;
        }

        var reservations = _reservationService
            .GetReservationQueue(bookId)
            .ToList();

        if (!reservations.Any())
        {
            Console.WriteLine("No active reservations found.");
            return;
        }

        int position = 1;

        foreach (var reservation in reservations)
        {
            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Position: {position++}");
            Console.WriteLine($"Reservation ID: {reservation.Id}");
            Console.WriteLine($"Member ID: {reservation.MemberId}");
            Console.WriteLine($"Date: {reservation.ReservationDate}");
            Console.WriteLine($"Status: {reservation.Status}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nUnable to retrieve queue: {ex.Message}");
    }
}


}
