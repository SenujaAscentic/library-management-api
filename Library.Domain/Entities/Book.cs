namespace Library.Domain.Entities;

using Library.Domain.Exceptions;

public sealed class Book : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Isbn { get; private set; } = string.Empty;
    public int PublishedYear { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }

    private Book() { } 

    public static Book Create(string title, string author, string isbn, int publishedYear, int totalCopies)
    {
        if (totalCopies <= 0)
            throw new BusinessRuleException("invalid_total_copies","Total copies must be greater than zero.");

        return new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            Isbn = isbn,
            PublishedYear = publishedYear,
            TotalCopies = totalCopies,
            AvailableCopies = totalCopies
        };
    }

    public void UpdateDetails(string title, string author, string isbn, int publishedYear)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
        PublishedYear = publishedYear;
    }

    public void UpdateTotalCopies(int newTotalCopies)
    {
        var borrowedCount = TotalCopies - AvailableCopies;
        if (newTotalCopies < borrowedCount)
            throw new BusinessRuleException("total_copies_below_borrowed",$"Cannot reduce total copies below the number currently borrowed ({borrowedCount}).");

        TotalCopies = newTotalCopies;
        AvailableCopies = newTotalCopies - borrowedCount;
    }

    public void DecrementAvailableCopies()
    {
        if (AvailableCopies <= 0)
            throw new BusinessRuleException("invalid_available_copies","No available copies to borrow.");
        AvailableCopies--;
    }

    public void IncrementAvailableCopies() => AvailableCopies++;
}