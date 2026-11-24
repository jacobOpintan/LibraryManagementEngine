
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Policies;

public class BorrowingPolicyFactory
{
    public IBorrowingPolicy GetPolicy(MemberCategory category)
    {
        return category switch
        {
            MemberCategory.Standard => new StandardBorrowingPolicy(),

            MemberCategory.Student => new StudentBorrowingPolicy(),

            _ => throw new ArgumentException(
                "Unsupported member category.",
                nameof(category))
        };
    }
}
