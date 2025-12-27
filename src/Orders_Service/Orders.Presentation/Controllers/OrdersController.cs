using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Orders.Presentation.Contracts;
using Orders.UseCases.Commands.CreateOrder;
using Orders.UseCases.Queries.GetOrders;
using Orders.UseCases.Queries.GetOrderById;
using SharedKernel.ValueObjects;

namespace Orders.Presentation.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _createOrder;
    private readonly GetOrdersHandler _getOrders;
    private readonly GetOrderByIdHandler _getOrderById;

    public OrdersController(
        CreateOrderHandler createOrder,
        GetOrdersHandler getOrders,
        GetOrderByIdHandler getOrderById)
    {
        _createOrder = createOrder;
        _getOrders = getOrders;
        _getOrderById = getOrderById;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.AccountNumber))
            return BadRequest("AccountNumber is required");

        if (request.Amount <= 0)
            return BadRequest("Amount must be positive");

        if (string.IsNullOrWhiteSpace(request.Description))
            return BadRequest("Description is required");

        var accountNumber = new AccountNumber(request.AccountNumber);

        var publicId = await _createOrder.Handle(
            new CreateOrderCommand(
                accountNumber,
                request.Amount,
                request.Description),
            ct);

        return Created(
            $"/orders/{publicId}",
            new CreateOrderResponse(publicId, "New"));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<OrderListItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string accountNumber,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return BadRequest("AccountNumber is required");

        var orders = await _getOrders.Handle(
            new GetOrdersQuery(new AccountNumber(accountNumber)),
            ct);

        var result = orders.Select(o =>
            new OrderListItemResponse(
                o.PublicId,
                o.Amount,
                o.Status.ToString()));

        return Ok(result);
    }

    [HttpGet("{publicId}")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        string publicId,
        CancellationToken ct)
    {
        var order = await _getOrderById.Handle(
            new GetOrderByIdQuery(publicId),
            ct);

        if (order == null)
            return NotFound();

        return Ok(new OrderDetailsResponse(
            order.PublicId,
            order.AccountNumber.Value,
            order.Amount,
            order.Description,
            order.Status.ToString()));
    }
}