namespace TodoApi.Exceptions;

public class TodoNotFoundException : Exception
{
    public int TodoId { get; }

    public TodoNotFoundException(int id)
        : base($"Todo with ID {id} was not found")
    {
        TodoId = id;
    }

}
