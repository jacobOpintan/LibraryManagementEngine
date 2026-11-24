
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryFineRepository : IFineRepository
{
    private readonly List<Fine> _fines = new();

    public void Add(Fine fine)
    {
        if (fine is null)
            throw new ArgumentNullException(nameof(fine));

        if (_fines.Any(existingFine => existingFine.Id == fine.Id))
            throw new InvalidOperationException(
                "This fine already exists.");

        _fines.Add(fine);
    }

    public Fine? GetById(Guid id)
    {
        return _fines.FirstOrDefault(fine => fine.Id == id);
    }

    public IEnumerable<Fine> GetByMemberId(Guid memberId)
    {
        return _fines
            .Where(fine => fine.MemberId == memberId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Fine> GetOutstandingFinesByMemberId(Guid memberId)
    {
        return _fines
            .Where(fine =>
                fine.MemberId == memberId &&
                !fine.IsSettled)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Fine> GetAll()
    {
        return _fines.AsReadOnly();
    }
}
