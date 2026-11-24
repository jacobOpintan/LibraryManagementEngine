
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Services;

public interface IReservationService
{
    Reservation ReserveBook(
        Guid memberId,
        Guid bookId,
        DateOnly reservationDate);

    void CancelReservation(Guid reservationId);

    IEnumerable<Reservation> GetReservationQueue(Guid bookId);
}
