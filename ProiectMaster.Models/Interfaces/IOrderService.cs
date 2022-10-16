using ProiectMaster.Models.DTOs;
using ProiectMaster.Models.DTOs.VM;
using System.Collections.Generic;

namespace ProiectMaster.Models.Interfaces
{
    public interface IOrderService
    {
        IEnumerable<OrderVM> GetAllOrders();
        OrderVM GetOrder(int id);
        void AddOrder(OrderVM dto);
        void UpdateOrder(int id, OrderVM dto);
        void DeleteOrder(int id);
    }
}
