
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Services;

public class MemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public Member RegisterMember(
        string membershipNumber,
        string fullName,
        MemberCategory category)
    {
        if (_memberRepository.ExistsByMembershipNumber(membershipNumber))
        {
            throw new InvalidOperationException(
                "A member with this membership number already exists.");
        }

        var member = new Member(
            membershipNumber,
            fullName,
            category);

        _memberRepository.Add(member);

        return member;
    }

    public IEnumerable<Member> GetAllMembers()
    {
        return _memberRepository.GetAll();
    }

    public Member? GetMemberById(Guid id)
    {
        return _memberRepository.GetById(id);
    }
}
