using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Services;
using LibraryAPI.Middleware;
using LibraryAPI.Models.DTO;
using LibraryAPI.Exceptions;
using LibraryAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Add database
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ILoanService, LoanService>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

////  ADD THIS - Configure JSON to handle circular references
//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//    options.SerializerOptions.ReferenceHandler =
//        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//});

var app = builder.Build();

//Add global exception handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ==================== BOOK ENDPOINTS ====================
app.MapGet("/api/books", async (IBookService service, CancellationToken ct) =>
{
    var books = await service.GetAllAsync(ct);
    return Results.Ok(books);
});

app.MapGet("/api/books/{id}", async (int id, IBookService service, CancellationToken ct) =>
{
    var book = await service.GetByIdAsync(id, ct);
    if(book == null)
    {
        throw new BookNotFoundException(id);
    }
    return Results.Ok(book);
});

app.MapPost("/api/books",async (CreateBookRequest createBookRequest, IBookService service, CancellationToken ct) =>
{
    var (isValid, errors) = createBookRequest.Validate();
    if(!isValid)
        throw new ValidationException(errors);

    var book = await service.CreateAsync(createBookRequest, ct);
    return Results.Created($"/api/books/{book.Id}", book);
});

app.MapPut("/api/books/{id}", async (int id, CreateBookRequest updateBookRequest, IBookService service, CancellationToken ct) =>
{
    var (isValid, errors) = updateBookRequest.Validate();

    if(!isValid)
        throw new ValidationException(errors);

    var book = await service.UpdateAsync(id, updateBookRequest, ct);

    if(book == null)
    {
        throw new BookNotFoundException(id);
    }
    return Results.Ok(book);
});

app.MapDelete("/api/books/{id}", async (int id, IBookService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    if(!deleted)
    {
        throw new BookNotFoundException(id);
    }
    return Results.NoContent();
});

app.MapGet("/api/books/search", async (string query, IBookService service, CancellationToken ct) =>
{
    if(string.IsNullOrWhiteSpace(query))
        return Results.BadRequest("Query parameter cannot be empty.");

    var books = await service.SearchAsync(query, ct);
    return Results.Ok(books);
});

app.MapGet("/api/books/available", async (IBookService service, CancellationToken ct) =>
{
    var books = await service.GetAvailableBooksAsync(ct);
    return Results.Ok(books);
});

// ==================== MEMBER ENDPOINTS ====================

app.MapGet("/api/members", async (IMemberService service, CancellationToken ct) =>
{
    var members = await service.GetAllAsync(ct);
    return Results.Ok(members);
});

app.MapGet("/api/members/{id}", async (int id, IMemberService service, CancellationToken ct) =>
{
    var member = await service.GetByIdAsync(id, ct);
    if (member == null)
        throw new MemberNotFoundException(id);
    return Results.Ok(member);
});

app.MapPost("/api/members", async (CreateMemberRequest request, IMemberService service, CancellationToken ct) =>
{
    var (isValid, errors) = request.Validate();
    if (!isValid)
        throw new ValidationException(errors);

    var member = await service.CreateAsync(request, ct);
    return Results.Created($"/api/members/{member.Id}", member);
});

app.MapPut("/api/members/{id}", async (int id, CreateMemberRequest request, IMemberService service, CancellationToken ct) =>
{
    var (isValid, errors) = request.Validate();
    if (!isValid)
        throw new ValidationException(errors);

    var member = await service.UpdateAsync(id, request, ct);
    if (member == null)
        throw new MemberNotFoundException(id);
    return Results.Ok(member);
});

app.MapDelete("/api/members/{id}", async (int id, IMemberService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    if (!deleted)
        throw new MemberNotFoundException(id);
    return Results.NoContent();
});

// ==================== LOAN ENDPOINTS ====================

app.MapPost("/api/loans", async (LoanBookRequest request, ILoanService service, CancellationToken ct) =>
{
    var (isValid, errors) = request.Validate();
    if (!isValid)
        throw new ValidationException(errors);

    var loan = await service.LoanBookAsync(request, ct);
    return Results.Created($"/api/loans/{loan.Id}", loan);
});

app.MapPost("/api/loans/{id}/return", async (int id, ILoanService service, CancellationToken ct) =>
{
    var loan = await service.ReturnBookAsync(id, ct);
    return Results.Ok(loan);
});

app.MapGet("/api/loans/active", async (ILoanService service, CancellationToken ct) =>
{
    var loans = await service.GetActiveLoansAsync(ct);
    return Results.Ok(loans);
});

app.MapGet("/api/loans/overdue", async (ILoanService service, CancellationToken ct) =>
{
    var loans = await service.GetOverdueLoansAsync(ct);
    return Results.Ok(loans);
});

app.MapGet("/api/members/{memberId}/loans", async (int memberId, ILoanService service, CancellationToken ct) =>
{
    var loans = await service.GetMemberLoanHistoryAsync(memberId, ct);
    return Results.Ok(loans);
});

app.Run();
