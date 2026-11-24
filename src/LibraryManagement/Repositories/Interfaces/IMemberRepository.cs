
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IMemberRepository
{
    void Add(Member member);

    Member? GetById(Guid id);

    Member? GetByMembershipNumber(string membershipNumber);

    IEnumerable<Member> GetAll();

    bool ExistsByMembershipNumber(string membershipNumber);
}
