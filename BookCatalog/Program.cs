using BookCatalog.Models;

var builder = WebApplication.CreateBuilder(args);

// Enable Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var authors = new List<Author>
{
    new Author { ID = 1, Name = "Dato Turashvili" },
    new Author { ID = 2, Name = "Shota Rustaveli" }
};

var books = new List<Book>
{
    new Book { ID = 1, Title = "Jeans Generation", AuthorID = 1, PublicationYear = 2008 },
    new Book { ID = 2, Title = "The Knight in the Panther's Skin", AuthorID = 2, PublicationYear = 1186 },
    new Book { ID = 3, Title = "The King of Forests", AuthorID = 1, PublicationYear = 2013 }
};

// Get all books by author name
app.MapGet("/api/books", () =>
{
    var result = books.Select(b =>
    {
        var author = authors.FirstOrDefault(a => a.ID == b.AuthorID);
        return new BookDto
        {
            ID = b.ID,
            Title = b.Title,
            AuthorName = author?.Name ?? "Unknown",
            PublicationYear = b.PublicationYear
        };
    });

    return Results.Ok(result);
});

// Get the book by author ID
app.MapGet("/api/authors/{id}/books", (int id) =>
{
    var author = authors.FirstOrDefault(a => a.ID == id);
    if (author == null)
        return Results.NotFound($"Author with ID {id} not found.");

    var authorBooks = books
        .Where(b => b.AuthorID == id)
        .Select(b => new BookDto
        {
            ID = b.ID,
            Title = b.Title,
            AuthorName = author.Name,
            PublicationYear = b.PublicationYear
        });

    return Results.Ok(authorBooks);
});

// Add a new book
app.MapPost("/api/books", (Book newBook) =>
{
    if (string.IsNullOrWhiteSpace(newBook.Title))
        return Results.BadRequest("Book title cannot be empty.");

    if (newBook.PublicationYear > DateTime.Now.Year)
        return Results.BadRequest("Publication year cannot be in the future.");

    var author = authors.FirstOrDefault(a => a.ID == newBook.AuthorID);
    if (author == null)
        return Results.BadRequest($"Author with ID {newBook.AuthorID} does not exist.");

    newBook.ID = books.Max(b => b.ID) + 1;
    books.Add(newBook);

    return Results.Created($"/api/books/{newBook.ID}", newBook);
});

// Filtering and sorting books
app.MapGet("/api/books/filter", (int? publicationYear, string? sortBy) =>
{
    var query = books.AsEnumerable();

    // If the publication year is given, we filter only books published in this year
    if (publicationYear.HasValue)
        query = query.Where(b => b.PublicationYear == publicationYear.Value);

    if (!string.IsNullOrWhiteSpace(sortBy) && sortBy.ToLower() == "title")
        query = query.OrderBy(b => b.Title);

    var result = query.Select(b =>
    {
        var author = authors.FirstOrDefault(a => a.ID == b.AuthorID);
        return new BookDto
        {
            ID = b.ID,
            Title = b.Title,
            AuthorName = author?.Name ?? "Unknown",
            PublicationYear = b.PublicationYear
        };
    });

    return Results.Ok(result);
});

// Update existing book
app.MapPut("/api/books/{id}", (int id, Book updatedBook) =>
{
    var existingBook = books.FirstOrDefault(b => b.ID == id);
    if (existingBook == null)
        return Results.NotFound($"Book with ID {id} not found.");

    if (string.IsNullOrWhiteSpace(updatedBook.Title))
        return Results.BadRequest("Title cannot be empty.");
    if (updatedBook.PublicationYear > DateTime.Now.Year)
        return Results.BadRequest("Publication year cannot be in the future.");

    var author = authors.FirstOrDefault(a => a.ID == updatedBook.AuthorID);
    if (author == null)
        return Results.BadRequest($"Author with ID {updatedBook.AuthorID} does not exist.");

    existingBook.Title = updatedBook.Title;
    existingBook.AuthorID = updatedBook.AuthorID;
    existingBook.PublicationYear = updatedBook.PublicationYear;

    return Results.NoContent();
});

// Get author's books
app.MapGet("/api/authors/{id}", (int id) =>
{
    var author = authors.FirstOrDefault(a => a.ID == id);
    if (author == null)
        return Results.NotFound($"Author with ID {id} not found.");

    var authorBooks = books.Where(b => b.AuthorID == id).ToList();

    var authorDetails = new AuthorDetailsDto
    {
        ID = author.ID,
        Name = author.Name,
        Books = authorBooks
    };

    return Results.Ok(authorDetails);
});
app.Run();