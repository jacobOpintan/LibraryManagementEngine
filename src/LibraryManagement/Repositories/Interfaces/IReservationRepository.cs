
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IReservationRepository
{
    void Add(Reservation reservation);

    Reservation? GetById(Guid id);

    IEnumerable<Reservation> GetByMemberId(Guid memberId);

    IEnumerable<Reservation> GetByBookId(Guid bookId);

    IEnumerable<Reservation> GetActiveReservationsByBookId(Guid bookId);

    bool HasActiveReservation(Guid memberId, Guid bookId);

    IEnumerable<Reservation> GetAll();
}
