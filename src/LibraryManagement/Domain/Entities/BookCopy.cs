
using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Entities;

public class BookCopy
{
    public Guid Id { get; private set; }

    public Guid BookId { get; private set; }

    public BookCopyStatus Status { get; private set; }

    public DateOnly DateAdded { get; private set; }

    public BookCopy(Guid bookId)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentException(
                "Book ID cannot be empty.",
                nameof(bookId));

        Id = Guid.NewGuid();
        BookId = bookId;
        Status = BookCopyStatus.Available;
        DateAdded = DateOnly.FromDateTime(DateTime.Today);
    }


    public void MarkAsBorrowed()
    {
        if (Status != BookCopyStatus.Available)
            throw new InvalidOperationException(
                "Only available copies can be borrowed.");

        Status = BookCopyStatus.Borrowed;
    }

    public void MarkAsAvailable()
    {
        if (Status != BookCopyStatus.Borrowed)
            throw new InvalidOperationException(
                "Only borrowed copies can be returned to available status.");

        Status = BookCopyStatus.Available;
    }

    public void MarkAsLost()
    {
        if (Status == BookCopyStatus.Lost)
            throw new InvalidOperationException(
                "This copy is already marked as lost.");

        Status = BookCopyStatus.Lost;
    }

    public void MarkAsDamaged()
    {
        if (Status != BookCopyStatus.Available)
            throw new InvalidOperationException(
                "Only available copies can be marked as damaged.");

        Status = BookCopyStatus.Damaged;
    }

}
