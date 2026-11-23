
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class Member
{
    public Guid Id { get; private set; }

    public string MembershipNumber { get; private set; }

    public string FullName { get; private set; }

    public MemberCategory Category { get; private set; }

    public bool IsSuspended { get; private set; }

    public DateOnly RegistrationDate { get; private set; }

    public Member(
        string membershipNumber,
        string fullName,
        MemberCategory category)
    {
        if (string.IsNullOrWhiteSpace(membershipNumber))
            throw new ArgumentException(
                "Membership number cannot be empty.",
                nameof(membershipNumber));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException(
                "Member name cannot be empty.",
                nameof(fullName));

        if (!Enum.IsDefined(typeof(MemberCategory), category))
            throw new ArgumentException(
                "Invalid member category.",
                nameof(category));

        Id = Guid.NewGuid();
        MembershipNumber = membershipNumber.Trim();
        FullName = fullName.Trim();
        Category = category;
        IsSuspended = false;
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today);
    }

    //checks whether the member is already suspended
    public void Suspend()
    {
        if (IsSuspended)
            throw new InvalidOperationException(
                "This member is already suspended.");

        IsSuspended = true;
    }

    //checks whether the member is currently suspended before allowing reinstatement.
    public void Reinstate()
    {
        if (!IsSuspended)
            throw new InvalidOperationException(
                "This member is not currently suspended.");

        IsSuspended = false;
    }

}
