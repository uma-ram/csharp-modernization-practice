namespace LibraryAPI.Exceptions;

public class MemberNotFoundException: Exception
{
    public MemberNotFoundException(int id): base($"Member with ID {{id}} not found")
    {
        
    }
}
