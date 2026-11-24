
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Services;

public class FinancialService : IFinancialService
{
    private readonly IFineRepository _fineRepository;
    private readonly IPaymentRepository _paymentRepository;

    public FinancialService(
        IFineRepository fineRepository,
        IPaymentRepository paymentRepository)
    {
        _fineRepository = fineRepository
            ?? throw new ArgumentNullException(nameof(fineRepository));

        _paymentRepository = paymentRepository
            ?? throw new ArgumentNullException(nameof(paymentRepository));
    }

    public Payment PayFine(
        Guid fineId,
        decimal amount,
        DateOnly paymentDate)
    {
        // Validate the payment amount.
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Payment amount must be greater than zero.");

        // Find the fine.
        var fine = _fineRepository.GetById(fineId);

        if (fine is null)
            throw new InvalidOperationException("Fine was not found.");

        if (paymentDate < fine.DateIssued)
            throw new ArgumentException(
                "Payment date cannot be before the fine issue date.",
                nameof(paymentDate));

        // Check whether the fine has already been settled.
        if (fine.IsSettled)
            throw new InvalidOperationException(
                "This fine has already been fully paid.");

        // Ensure the payment does not exceed the balance.
        if (amount > fine.OutstandingBalance)
            throw new InvalidOperationException(
                "Payment cannot exceed the outstanding balance.");

        // Update the fine's payment balance.
        fine.RecordPayment(amount);

        // Create a payment transaction.
        var payment = new Payment(
            fineId,
            amount,
            paymentDate);

        // Save the payment transaction.
        _paymentRepository.Add(payment);

        return payment;
    }
}
