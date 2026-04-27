using MiniCustomerManagement.Api.Models;

namespace MiniCustomerManagement.Api.Services;

public class CustomerService
{
    private readonly List<Customer> _customers =
    [
        new Customer { Id = 1, FullName = "Nguyen Minh Anh", Email = "minhanh@example.com", Segment = "Retail", YearJoined = 2021, LoyaltyPoints = 1200 },
        new Customer { Id = 2, FullName = "Tran Quoc Bao", Email = "quocbao@example.com", Segment = "SMB", YearJoined = 2023, LoyaltyPoints = 320 },
        new Customer { Id = 3, FullName = "Le Thu Ha", Email = "thuha@example.com", Segment = "Enterprise", YearJoined = 2020, LoyaltyPoints = 0 },
        new Customer { Id = 4, FullName = "Pham Gia Huy", Email = "giahuy@example.com", Segment = "Retail", YearJoined = 2024, LoyaltyPoints = 680 }
    ];

    public List<Customer> GetAll() => _customers;

    public IEnumerable<Customer> QueryCustomers(
        string? segment,
        string? status,
        string? sortBy,
        string? sortDir)
    {
        var query = _customers.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(segment))
        {
            query = query.Where(c =>
                string.Equals(c.Segment, segment, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c =>
                string.Equals(GetCustomerStatus(c.LoyaltyPoints), status, StringComparison.OrdinalIgnoreCase));
        }

        var isDescending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return (sortBy ?? "id").ToLowerInvariant() switch
        {
            "fullname" => isDescending
                ? query.OrderByDescending(c => c.FullName)
                : query.OrderBy(c => c.FullName),
            "segment" => isDescending
                ? query.OrderByDescending(c => c.Segment)
                : query.OrderBy(c => c.Segment),
            "yearjoined" => isDescending
                ? query.OrderByDescending(c => c.YearJoined)
                : query.OrderBy(c => c.YearJoined),
            "loyaltypoints" => isDescending
                ? query.OrderByDescending(c => c.LoyaltyPoints)
                : query.OrderBy(c => c.LoyaltyPoints),
            _ => isDescending
                ? query.OrderByDescending(c => c.Id)
                : query.OrderBy(c => c.Id)
        };
    }

    public object GetStats()
    {
        var totalCustomers = _customers.Count;
        var totalLoyaltyPoints = _customers.Sum(c => c.LoyaltyPoints);
        var activeCustomers = _customers.Count(c => c.LoyaltyPoints > 0);

        return new
        {
            TotalCustomers = totalCustomers,
            TotalLoyaltyPoints = totalLoyaltyPoints,
            ActiveCustomers = activeCustomers
        };
    }

    public string GetCustomerStatus(int loyaltyPoints)
    {
        if (loyaltyPoints <= 0) return "Inactive";
        if (loyaltyPoints < 500) return "Regular";
        return "VIP";
    }
}
