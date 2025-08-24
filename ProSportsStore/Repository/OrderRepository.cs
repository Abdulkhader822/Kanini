// Repository/OrderRepository.cs
using Microsoft.EntityFrameworkCore;
using ProSportsStore.Interface;
using ProSportsStore.Models;

namespace ProSportsStore.Repository
{
    public class OrderRepository : IOrder
    {
        private readonly ProSportsStoreContext _context;
        public OrderRepository(ProSportsStoreContext context) => _context = context;

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
            await _context.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Order?> GetOrderByIdAsync(int id) =>
            await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task<Order> AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> UpdateOrderAsync(Order order)
        {
            var existing = await _context.Orders.FindAsync(order.OrderId);
            if (existing == null) return null;
            existing.Status = order.Status;
            existing.TotalAmount = order.TotalAmount;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var o = await _context.Orders.FindAsync(id);
            if (o == null) return false;
            _context.Orders.Remove(o);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
