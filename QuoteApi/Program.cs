var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var quotes = new[]
{
    "Knowledge is power.",
    "Simplicity is the soul of efficiency.",
    "Code is like humor. When you have to explain it, it’s bad.",
    "Fix the cause, not the symptom."
};

app.MapGet("/quote", () =>
{
    var quote = quotes[Random.Shared.Next(quotes.Length)];
    return Results.Ok(new { quote });
});

app.Run();
