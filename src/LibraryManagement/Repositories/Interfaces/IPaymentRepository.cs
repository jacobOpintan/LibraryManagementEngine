
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Repositories.Interfaces;

public interface IPaymentRepository
{
    void Add(Payment payment);

    Payment? GetById(Guid id);

    IEnumerable<Payment> GetByFineId(Guid fineId);

    IEnumerable<Payment> GetAll();
}
