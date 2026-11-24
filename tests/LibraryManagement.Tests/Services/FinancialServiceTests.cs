
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Implementations;
using LibraryManagement.Services;

namespace LibraryManagement.Tests.Services;

public class FinancialServiceTests
{
    [Fact]
    public void PayFine_ShouldRecordFullPayment()
    {
        var fineRepository = new InMemoryFineRepository();
        var paymentRepository = new InMemoryPaymentRepository();

        var fine = new Fine(
            Guid.NewGuid(),
            Guid.NewGuid(),
            20.00m,
            FineType.Overdue,
            new DateOnly(2026, 11, 1));

        fineRepository.Add(fine);

        var service = new FinancialService(
            fineRepository,
            paymentRepository);

        var payment = service.PayFine(
            fine.Id,
            20.00m,
            new DateOnly(2026, 11, 5));

        Assert.Equal(20.00m, payment.Amount);
        Assert.Equal(0.00m, fine.OutstandingBalance);
        Assert.True(fine.IsSettled);

        Assert.Single(paymentRepository.GetByFineId(fine.Id));
    }

    [Fact]
    public void PayFine_ShouldSupportPartialPayments()
    {
        var fineRepository = new InMemoryFineRepository();
        var paymentRepository = new InMemoryPaymentRepository();

        var fine = new Fine(
            Guid.NewGuid(),
            Guid.NewGuid(),
            20.00m,
            FineType.Overdue,
            new DateOnly(2026, 11, 1));

        fineRepository.Add(fine);

        var service = new FinancialService(
            fineRepository,
            paymentRepository);

        service.PayFine(
            fine.Id,
            8.00m,
            new DateOnly(2026, 11, 5));

        Assert.Equal(12.00m, fine.OutstandingBalance);
        Assert.False(fine.IsSettled);

        service.PayFine(
            fine.Id,
            12.00m,
            new DateOnly(2026, 11, 6));

        Assert.Equal(0.00m, fine.OutstandingBalance);
        Assert.True(fine.IsSettled);

        Assert.Equal(
            2,
            paymentRepository.GetByFineId(fine.Id).Count());
    }

    [Fact]
    public void PayFine_ShouldRejectPaymentGreaterThanBalance()
    {
        var fineRepository = new InMemoryFineRepository();
        var paymentRepository = new InMemoryPaymentRepository();

        var fine = new Fine(
            Guid.NewGuid(),
            Guid.NewGuid(),
            20.00m,
            FineType.Overdue,
            new DateOnly(2026, 11, 1));

        fineRepository.Add(fine);

        var service = new FinancialService(
            fineRepository,
            paymentRepository);

        Assert.Throws<InvalidOperationException>(
            () => service.PayFine(
                fine.Id,
                25.00m,
                new DateOnly(2026, 11, 5)));

        Assert.Equal(20.00m, fine.OutstandingBalance);
        Assert.Empty(paymentRepository.GetByFineId(fine.Id));
    }

    [Fact]
    public void PayFine_ShouldRejectZeroOrNegativePayment()
    {
        var fineRepository = new InMemoryFineRepository();
        var paymentRepository = new InMemoryPaymentRepository();

        var service = new FinancialService(
            fineRepository,
            paymentRepository);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.PayFine(
                Guid.NewGuid(),
                0m,
                new DateOnly(2026, 11, 5)));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.PayFine(
                Guid.NewGuid(),
                -5m,
                new DateOnly(2026, 11, 5)));
    }

    [Fact]
    public void PayFine_ShouldRejectPaymentBeforeFineIssueDate()
    {
        var fineRepository = new InMemoryFineRepository();
        var paymentRepository = new InMemoryPaymentRepository();

        var fine = new Fine(
            Guid.NewGuid(),
            Guid.NewGuid(),
            20.00m,
            FineType.Overdue,
            new DateOnly(2026, 11, 10));

        fineRepository.Add(fine);

        var service = new FinancialService(
            fineRepository,
            paymentRepository);

        Assert.Throws<ArgumentException>(
            () => service.PayFine(
                fine.Id,
                10.00m,
                new DateOnly(2026, 11, 5)));

        Assert.Equal(20.00m, fine.OutstandingBalance);
        Assert.Empty(paymentRepository.GetByFineId(fine.Id));
    }
}
