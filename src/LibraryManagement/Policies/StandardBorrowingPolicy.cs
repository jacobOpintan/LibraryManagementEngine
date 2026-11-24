
namespace LibraryManagement.Policies;

public class StandardBorrowingPolicy : IBorrowingPolicy
{
    public int MaximumActiveLoans { get; } = 3;

    public int LoanPeriodInDays { get; } = 14;

    public int MaximumRenewals { get; } = 1;

    public decimal DailyOverdueFine { get; } = 2.00m;
}
