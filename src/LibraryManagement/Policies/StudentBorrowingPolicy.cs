
namespace LibraryManagement.Policies;

public class StudentBorrowingPolicy : IBorrowingPolicy
{
    public int MaximumActiveLoans { get; } = 5;

    public int LoanPeriodInDays { get; } = 21;

    public int MaximumRenewals { get; } = 2;

    public decimal DailyOverdueFine { get; } = 1.00m;
}
