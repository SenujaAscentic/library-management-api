namespace Library.Domain.Entities;
public class Member
{
    public Guid Id {get;set;}

    public String FullName {get; set;}= string.Empty;

    public String Email {get;set;}= string.Empty;

    public string PhoneNumber { get; set;}= string.Empty;

    public DateTime RegisteredDate {get; set;}

    public bool IsActive {get; set;} = true;


}