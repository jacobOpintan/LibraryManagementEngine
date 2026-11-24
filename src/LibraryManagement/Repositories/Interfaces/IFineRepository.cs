
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IFineRepository
{
    void Add(Fine fine);

    Fine? GetById(Guid id);

    IEnumerable<Fine> GetByMemberId(Guid memberId);

    IEnumerable<Fine> GetOutstandingFinesByMemberId(Guid memberId);

    IEnumerable<Fine> GetAll();
}
