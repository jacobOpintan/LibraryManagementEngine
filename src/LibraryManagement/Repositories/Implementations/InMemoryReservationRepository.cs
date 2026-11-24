
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryReservationRepository : IReservationRepository
{
    private readonly List<Reservation> _reservations = new();

    public void Add(Reservation reservation)
    {
        if (reservation is null)
            throw new ArgumentNullException(nameof(reservation));

        if (_reservations.Any(
            existingReservation => existingReservation.Id == reservation.Id))
        {
            throw new InvalidOperationException(
                "This reservation already exists.");
        }

        _reservations.Add(reservation);
    }

    public Reservation? GetById(Guid id)
    {
        return _reservations.FirstOrDefault(
            reservation => reservation.Id == id);
    }

    public IEnumerable<Reservation> GetByMemberId(Guid memberId)
    {
        return _reservations
            .Where(reservation => reservation.MemberId == memberId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Reservation> GetByBookId(Guid bookId)
    {
        return _reservations
            .Where(reservation => reservation.BookId == bookId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Reservation> GetActiveReservationsByBookId(Guid bookId)
    {
        return _reservations
            .Where(reservation =>
                reservation.BookId == bookId &&
                reservation.Status == ReservationStatus.Active)
            .OrderBy(reservation => reservation.ReservationDate)
            .ThenBy(reservation => reservation.Id)
            .ToList()
            .AsReadOnly();
    }

    public bool HasActiveReservation(Guid memberId, Guid bookId)
    {
        return _reservations.Any(reservation =>
            reservation.MemberId == memberId &&
            reservation.BookId == bookId &&
            reservation.Status == ReservationStatus.Active);
    }

    public IEnumerable<Reservation> GetAll()
    {
        return _reservations.AsReadOnly();
    }
}
