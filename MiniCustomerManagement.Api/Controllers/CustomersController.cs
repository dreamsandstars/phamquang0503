using Microsoft.AspNetCore.Mvc;
using MiniCustomerManagement.Api.Services;

namespace MiniCustomerManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;
    private const int MaxPageSize = 100;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? segment,
        [FromQuery] string? status,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var filteredCustomers = _customerService.QueryCustomers(segment, status, sortBy, sortDir);
        var totalItems = filteredCustomers.Count();
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);

        var customers = filteredCustomers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.FullName,
                c.Email,
                c.Segment,
                c.YearJoined,
                c.LoyaltyPoints,
                Status = _customerService.GetCustomerStatus(c.LoyaltyPoints)
            });

        return Ok(new
        {
            Filters = new
            {
                Segment = segment,
                Status = status
            },
            Sort = new
            {
                SortBy = string.IsNullOrWhiteSpace(sortBy) ? "id" : sortBy,
                SortDirection = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc"
            },
            Pagination = new
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            },
            Items = customers
        });
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(_customerService.GetStats());
    }
}
