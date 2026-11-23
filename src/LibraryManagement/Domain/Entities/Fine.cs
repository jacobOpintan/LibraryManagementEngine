using LibraryManagement.Domain.Enums;
namespace LibraryManagement.Domain.Entities;

public class Fine
{
    public Guid Id { get; private set; }

    public Guid MemberId { get; private set; }

    public Guid? LoanId { get; private set; }

    public decimal Amount { get; private set; }

    public decimal AmountPaid { get; private set; }

    public FineType Type { get; private set; }

    public DateOnly DateIssued { get; private set; }

    public decimal OutstandingBalance => Amount - AmountPaid;

    public bool IsSettled => OutstandingBalance == 0;

    public Fine(
        Guid memberId,
        Guid? loanId,
        decimal amount,
        FineType type,
        DateOnly dateIssued)
    {
        if (memberId == Guid.Empty)
            throw new ArgumentException(
                "Member ID cannot be empty.",
                nameof(memberId));

        if (loanId == Guid.Empty)
            throw new ArgumentException(
                "Loan ID cannot be empty.",
                nameof(loanId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Fine amount must be greater than zero.");

        if (!Enum.IsDefined(typeof(FineType), type))
            throw new ArgumentException(
                "Invalid fine type.",
                nameof(type));

        Id = Guid.NewGuid();
        MemberId = memberId;
        LoanId = loanId;
        Amount = amount;
        AmountPaid = 0;
        Type = type;
        DateIssued = dateIssued;
    }

    public void RecordPayment(decimal paymentAmount)
    {
        if (paymentAmount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(paymentAmount),
                "Payment amount must be greater than zero.");

        if (IsSettled)
            throw new InvalidOperationException(
                "This fine has already been fully paid.");

        if (paymentAmount > OutstandingBalance)
            throw new InvalidOperationException(
                "Payment cannot exceed the outstanding balance.");

        AmountPaid += paymentAmount;
    }
}
