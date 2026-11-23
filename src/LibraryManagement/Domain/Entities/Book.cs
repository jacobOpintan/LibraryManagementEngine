
namespace LibraryManagement.Domain.Entities;

public class Book
{
    public Guid Id { get; private set; }

    public string ISBN { get; private set; }

    public string Title { get; private set; }

    public string Author { get; private set; }

    public string Publisher { get; private set; }

    public int PublicationYear { get; private set; }

    public decimal ReplacementCost { get; private set; }

    public bool IsAvailableForLoan { get; private set; }

    public Book(
        string isbn,
        string title,
        string author,
        string publisher,
        int publicationYear,
        decimal replacementCost,
        bool isAvailableForLoan)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty.", nameof(isbn));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty.", nameof(author));

        if (string.IsNullOrWhiteSpace(publisher))
            throw new ArgumentException("Publisher cannot be empty.", nameof(publisher));

        if (publicationYear <= 0 || publicationYear > DateTime.UtcNow.Year)
            throw new ArgumentOutOfRangeException(
                nameof(publicationYear),
                "Publication year must be valid and cannot be in the future.");

        if (replacementCost < 0)
            throw new ArgumentOutOfRangeException(
                nameof(replacementCost),
                "Replacement cost cannot be negative.");

        Id = Guid.NewGuid();
        ISBN = isbn.Trim();
        Title = title.Trim();
        Author = author.Trim();
        Publisher = publisher.Trim();
        PublicationYear = publicationYear;
        ReplacementCost = replacementCost;
        IsAvailableForLoan = isAvailableForLoan;
    }
}
