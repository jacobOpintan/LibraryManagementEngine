
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Services;

public class ReservationService : IReservationService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IBookCopyRepository _bookCopyRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IFineRepository _fineRepository;

    public ReservationService(
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IBookCopyRepository bookCopyRepository,
        IReservationRepository reservationRepository,
        IFineRepository fineRepository)
    {
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _bookCopyRepository = bookCopyRepository;
        _reservationRepository = reservationRepository;
        _fineRepository = fineRepository;
    }

    public Reservation ReserveBook(
        Guid memberId,
        Guid bookId,
        DateOnly reservationDate)
    {
        var member = _memberRepository.GetById(memberId);

        if (member is null)
            throw new InvalidOperationException("Member was not found.");

        if (member.IsSuspended)
            throw new InvalidOperationException(
                "Suspended members cannot make reservations.");

        var book = _bookRepository.GetById(bookId);

        if (book is null)
            throw new InvalidOperationException("Book was not found.");

        if (!book.IsAvailableForLoan)
            throw new InvalidOperationException(
                "This book is not available for borrowing.");

        var outstandingFines =
            _fineRepository.GetOutstandingFinesByMemberId(memberId);

        if (outstandingFines.Any())
            throw new InvalidOperationException(
                "Outstanding charges must be settled before making reservations.");

        var availableCopies = _bookCopyRepository
            .GetByBookId(bookId)
            .Any(copy => copy.Status == BookCopyStatus.Available);

        if (availableCopies)
            throw new InvalidOperationException(
                "This book currently has available copies.");

        var activeReservations =
            _reservationRepository.GetActiveReservationsByBookId(bookId);

        if (activeReservations.Any(r => r.MemberId == memberId))
            throw new InvalidOperationException(
                "Member already has an active reservation for this book.");

        var reservation = new Reservation(
            memberId,
            bookId,
            reservationDate);

        _reservationRepository.Add(reservation);

        return reservation;
    }

    public void CancelReservation(Guid reservationId)
    {
        var reservation =
            _reservationRepository.GetById(reservationId);

        if (reservation is null)
            throw new InvalidOperationException(
                "Reservation was not found.");

        reservation.Cancel();
    }

    public IEnumerable<Reservation> GetReservationQueue(Guid bookId)
    {
        var book = _bookRepository.GetById(bookId);

        if (book is null)
            throw new InvalidOperationException("Book was not found.");

        return _reservationRepository
            .GetActiveReservationsByBookId(bookId);
    }
}
