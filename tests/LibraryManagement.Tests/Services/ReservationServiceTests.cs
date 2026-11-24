
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Repositories.Implementations;
using LibraryManagement.Services;

namespace LibraryManagement.Tests.Services;

public class ReservationServiceTests
{
    [Fact]
    public void ReserveBook_ShouldCreateReservationWhenNoCopiesAvailable()
    {
        var books = new InMemoryBookRepository();
        var members = new InMemoryMemberRepository();
        var copies = new InMemoryBookCopyRepository();
        var reservations = new InMemoryReservationRepository();
        var fines = new InMemoryFineRepository();

        var member = new Member("MEM100", "Test Member", MemberCategory.Standard);
        members.Add(member);

        var book = new Book("ISBN100", "C# Basics", "Author",
            "Publisher", 2023, 100m, true);
        books.Add(book);

        var copy = new BookCopy(book.Id);
        copies.Add(copy);
        copy.MarkAsBorrowed();

        var service = new ReservationService(
            books, members, copies, reservations, fines);

        var reservation = service.ReserveBook(
            member.Id, book.Id, new DateOnly(2026, 11, 20));

        Assert.Equal(ReservationStatus.Active, reservation.Status);
        Assert.Equal(member.Id, reservation.MemberId);
        Assert.Single(reservations.GetActiveReservationsByBookId(book.Id));
    }

    [Fact]
    public void ReserveBook_ShouldRejectDuplicateActiveReservation()
    {
        var books = new InMemoryBookRepository();
        var members = new InMemoryMemberRepository();
        var copies = new InMemoryBookCopyRepository();
        var reservations = new InMemoryReservationRepository();
        var fines = new InMemoryFineRepository();

        var member = new Member("MEM101", "Test Member", MemberCategory.Standard);
        members.Add(member);

        var book = new Book("ISBN101", "C# Advanced", "Author",
            "Publisher", 2023, 100m, true);
        books.Add(book);

        var copy = new BookCopy(book.Id);
        copies.Add(copy);
        copy.MarkAsBorrowed();

        var service = new ReservationService(
            books, members, copies, reservations, fines);

        service.ReserveBook(member.Id, book.Id, new DateOnly(2026, 11, 20));

        Assert.Throws<InvalidOperationException>(() =>
            service.ReserveBook(member.Id, book.Id, new DateOnly(2026, 11, 21)));
    }

    [Fact]
    public void ReserveBook_ShouldRejectSuspendedMember()
    {
        var books = new InMemoryBookRepository();
        var members = new InMemoryMemberRepository();
        var copies = new InMemoryBookCopyRepository();
        var reservations = new InMemoryReservationRepository();
        var fines = new InMemoryFineRepository();

        var member = new Member("MEM102", "Suspended Member", MemberCategory.Standard);
        member.Suspend();
        members.Add(member);

        var book = new Book("ISBN102", "Programming", "Author",
            "Publisher", 2023, 100m, true);
        books.Add(book);

        var copy = new BookCopy(book.Id);
        copies.Add(copy);
        copy.MarkAsBorrowed();

        var service = new ReservationService(
            books, members, copies, reservations, fines);

        Assert.Throws<InvalidOperationException>(() =>
            service.ReserveBook(member.Id, book.Id, new DateOnly(2026, 11, 20)));
    }

    [Fact]
    public void CancelReservation_ShouldUpdateReservationStatus()
    {
        var books = new InMemoryBookRepository();
        var members = new InMemoryMemberRepository();
        var copies = new InMemoryBookCopyRepository();
        var reservations = new InMemoryReservationRepository();
        var fines = new InMemoryFineRepository();

        var member = new Member("MEM103", "Test Member", MemberCategory.Standard);
        members.Add(member);

        var book = new Book("ISBN103", "Software Engineering", "Author",
            "Publisher", 2023, 100m, true);
        books.Add(book);

        var copy = new BookCopy(book.Id);
        copies.Add(copy);
        copy.MarkAsBorrowed();

        var service = new ReservationService(
            books, members, copies, reservations, fines);

        var reservation = service.ReserveBook(
            member.Id, book.Id, new DateOnly(2026, 11, 20));

        service.CancelReservation(reservation.Id);

        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }
}
