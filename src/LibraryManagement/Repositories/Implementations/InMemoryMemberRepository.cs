
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryMemberRepository : IMemberRepository
{
    private readonly List<Member> _members = new();

    public void Add(Member member)
    {
        if (member is null)
            throw new ArgumentNullException(nameof(member));

        if (ExistsByMembershipNumber(member.MembershipNumber))
            throw new InvalidOperationException(
                "A member with this membership number already exists.");

        _members.Add(member);
    }

    public Member? GetById(Guid id)
    {
        return _members.FirstOrDefault(member => member.Id == id);
    }

    public Member? GetByMembershipNumber(string membershipNumber)
    {
        return _members.FirstOrDefault(
            member => string.Equals(
                member.MembershipNumber,
                membershipNumber,
                StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Member> GetAll()
    {
        return _members.AsReadOnly();
    }

    public bool ExistsByMembershipNumber(string membershipNumber)
    {
        return _members.Any(
            member => string.Equals(
                member.MembershipNumber,
                membershipNumber,
                StringComparison.OrdinalIgnoreCase));
    }
}
