
using LibraryManagement.Policies;
using LibraryManagement.Repositories.Implementations;
using LibraryManagement.Services;
using LibraryManagement.UserInterface;

var memberRepository = new InMemoryMemberRepository();
var bookRepository = new InMemoryBookRepository();
var bookCopyRepository = new InMemoryBookCopyRepository();
var fineRepository = new InMemoryFineRepository();

var loanRepository = new InMemoryLoanRepository(bookCopyRepository);
var reservationRepository = new InMemoryReservationRepository();
var paymentRepository = new InMemoryPaymentRepository();

var memberService = new MemberService(memberRepository);
var bookService = new BookService(bookRepository);

var bookCopyService = new BookCopyService(
    bookRepository,
    bookCopyRepository);

IBorrowingService borrowingService = new BorrowingService(
    bookRepository,
    bookCopyRepository,
    memberRepository,
    loanRepository,
    fineRepository,
    reservationRepository,
    new BorrowingPolicyFactory());

IFinancialService financialService = new FinancialService(
    fineRepository,
    paymentRepository);
IReservationService reservationService = new ReservationService(
                                            bookRepository,
                                            memberRepository,
                                            bookCopyRepository,
                                            reservationRepository,
                                            fineRepository);

var menu = new LibraryMenu(
    memberService,
    bookService,
    bookCopyService,
    borrowingService,
    financialService,reservationService);

menu.Run();
