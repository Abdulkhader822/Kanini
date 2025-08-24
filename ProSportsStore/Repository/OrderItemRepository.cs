// Repository/OrderItemRepository.cs
using Microsoft.EntityFrameworkCore;
using ProSportsStore.Interface;
using ProSportsStore.Models;

namespace ProSportsStore.Repository
{
    public class OrderItemRepository : IOrderItem
    {
        private readonly ProSportsStoreContext _context;
        public OrderItemRepository(ProSportsStoreContext context) => _context = context;

        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync() =>
            await _context.OrderItems
                .Include(oi => oi.Product)
                .AsNoTracking()
                .ToListAsync();

        public async Task<OrderItem?> GetOrderItemByIdAsync(int id) =>
            await _context.OrderItems
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == id);

        public async Task<OrderItem> AddOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }

        public async Task<OrderItem?> UpdateOrderItemAsync(OrderItem orderItem)
        {
            var existing = await _context.OrderItems.FindAsync(orderItem.OrderItemId);
            if (existing == null) return null;

            existing.Quantity = orderItem.Quantity;
            existing.Price = orderItem.Price;
            existing.ProductId = orderItem.ProductId;
            existing.OrderId = orderItem.OrderId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteOrderItemAsync(int id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item == null) return false;
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
