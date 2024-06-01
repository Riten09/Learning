namespace API.DTOs;

public class CreateMessageDto
{
    // here we are just need to know the username of the person that's reciving the message, who are we sending the messageto?
    public string RecipientUsername{get;set;}
    public string Content{get;set;}
}
