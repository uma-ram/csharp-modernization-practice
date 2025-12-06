


// Create a blog post
var originalPost = new BlogPost(
    Id: 1,
    Title: "My First Post",
    Content: "Hello World!",
    AuthorId: 100,
    CreatedDate: DateTime.Now
);

//Console.WriteLine(originalPost.Title); // Prints: My First Post

// Create a new post with updated title (everything else stays the same)
var updatedPost = originalPost with { Title = "My Updated Post" };

Console.WriteLine(originalPost.Title);  // Still "My First Post"
Console.WriteLine(updatedPost.Title);   // Now "My Updated Post"
Console.WriteLine(updatedPost.Content); // Still "Hello World!"
Console.WriteLine(updatedPost.Id);      // Still 1

// Record with named properties
public record BlogPost(
    int Id,
    string Title,
    string Content,
    int AuthorId,
    DateTime CreatedDate
);

public record Author(
    int Id,
    string Name,
    string Email
);

public record Comment(
    int Id,
    int PostId,
    int AuthorId,
    string Text,
    DateTime CreatedDate
);



