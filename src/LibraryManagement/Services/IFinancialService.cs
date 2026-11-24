
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Services;

public interface IFinancialService
{
    Payment PayFine(
        Guid fineId,
        decimal amount,
        DateOnly paymentDate);
}
