using LiteDB;
using QuoteApi.Quotes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

string dbFile = "quotes.db";

// GET random quote
app.MapGet("/quote", () =>
{
    using var db = new LiteDatabase(dbFile);
    var col = db.GetCollection<Quote>("quotes");
    var count = col.Count();
    if (count == 0)
        return Results.Problem("No quotes available.");
    var random = new Random();
    var skip = random.Next(count);
    var quote = col.Query().Skip(skip).Limit(1).FirstOrDefault();
    return Results.Ok(new { quote = quote?.Text });
});

// POST new quote
app.MapPost("/quote", (QuoteRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Quote))
        return Results.BadRequest("Quote cannot be empty.");

    using var db = new LiteDatabase(dbFile);
    var col = db.GetCollection<Quote>("quotes");
    var newQuote = new Quote { Text = request.Quote };
    col.Insert(newQuote);
    return Results.Ok(new { message = "Quote added.", quote = request.Quote });
});

app.MapGet("/", () => Results.Text("Application is running", "text/html"));

app.Run();
