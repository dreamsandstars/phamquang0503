namespace MiniCustomerManagement.Api.Models;

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Segment { get; set; } = "";
    public int YearJoined { get; set; }
    public int LoyaltyPoints { get; set; }
}
