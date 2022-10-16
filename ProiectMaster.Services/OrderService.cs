using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using ProiectMaster.DataAccess.Interfaces;
using ProiectMaster.Models.DTOs;
using ProiectMaster.Models.DTOs.VM;
using ProiectMaster.Models.Entites;
using ProiectMaster.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProiectMaster.Services
{
    public class OrderService : IOrderService
    {

        private readonly IRepository<Order, int> orderRep;
        private readonly IMapper mapper;
        private readonly IHostingEnvironment hostingEnvironment;

        public OrderService(IRepository<Order, int> orderRep, IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            this.orderRep = orderRep;
            this.mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;
        }

        public void AddOrder(OrderVM dto)
        {
            var entity = mapper.Map<Order>(dto);
            orderRep.Add(entity);
        }

        public void DeleteOrder(int id)
        {
            var entity = orderRep.GetInstance(id);

            orderRep.Delete(entity);
        }

        public IEnumerable<OrderVM> GetAllOrders()
        {
            var list = orderRep.GetAll();
            return mapper.Map<List<OrderVM>>(list);
        }

        public OrderVM GetOrder(int id)
        {
            var entity = orderRep.GetInstance(id);
            return mapper.Map<OrderVM>(entity);
        }

        public void UpdateOrder(int id, OrderVM dto)
        {
            var entity = orderRep.GetInstance(id);

            mapper.Map(dto, entity);
            orderRep.Update(entity);
        }
    }
}
