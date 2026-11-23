
namespace LibraryManagement.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }

    public Guid FineId { get; private set; }

    public decimal Amount { get; private set; }

    public DateOnly PaymentDate { get; private set; }

    public Payment(Guid fineId, decimal amount, DateOnly paymentDate)
    {
        if (fineId == Guid.Empty)
            throw new ArgumentException(
                "Fine ID cannot be empty.",
                nameof(fineId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Payment amount must be greater than zero.");

        Id = Guid.NewGuid();
        FineId = fineId;
        Amount = amount;
        PaymentDate = paymentDate;
    }
}
