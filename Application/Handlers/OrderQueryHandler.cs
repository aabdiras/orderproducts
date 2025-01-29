using Domain.Entities;
using Domain.Interfaces;

public class OrderQueryHandler
{
    private readonly IOrderRepository _orderRepository;

    public OrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
}