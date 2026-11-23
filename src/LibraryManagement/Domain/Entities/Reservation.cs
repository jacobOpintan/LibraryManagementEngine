
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Reservation
{
    public Guid Id { get; private set; }

    public Guid MemberId { get; private set; }

    public Guid BookId { get; private set; }

    public DateOnly ReservationDate { get; private set; }

    public ReservationStatus Status { get; private set; }

    public Reservation(Guid memberId, Guid bookId, DateOnly reservationDate)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException(
                "Member ID cannot be empty.",
                nameof(memberId));

        if (bookId == Guid.Empty)
            throw new ArgumentException(
                "Book ID cannot be empty.",
                nameof(bookId));

        Id = Guid.NewGuid();
        MemberId = memberId;
        BookId = bookId;
        ReservationDate = reservationDate;
        Status = ReservationStatus.Active;
    }

    public void Fulfill()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException(
                "Only active reservations can be fulfilled.");

        Status = ReservationStatus.Fulfilled;
    }

    public void Cancel()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException(
                "Only active reservations can be cancelled.");

        Status = ReservationStatus.Cancelled;
    }

    public void Expire()
    {
        if (Status != ReservationStatus.Active)
            throw new InvalidOperationException(
                "Only active reservations can expire.");

        Status = ReservationStatus.Expired;
    }
}
