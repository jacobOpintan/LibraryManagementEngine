
using LibraryManagement.Domain.Entities;
using LibraryManagement.Repositories.Interfaces;

namespace LibraryManagement.Repositories.Implementations;

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _payments = new();

    public void Add(Payment payment)
    {
        if (payment is null)
            throw new ArgumentNullException(nameof(payment));

        if (_payments.Any(existingPayment => existingPayment.Id == payment.Id))
            throw new InvalidOperationException("This payment already exists.");

        _payments.Add(payment);
    }

    public Payment? GetById(Guid id)
    {
        return _payments.FirstOrDefault(payment => payment.Id == id);
    }

    public IEnumerable<Payment> GetByFineId(Guid fineId)
    {
        return _payments
            .Where(payment => payment.FineId == fineId)
            .ToList()
            .AsReadOnly();
    }

    public IEnumerable<Payment> GetAll()
    {
        return _payments.AsReadOnly();
    }
}
