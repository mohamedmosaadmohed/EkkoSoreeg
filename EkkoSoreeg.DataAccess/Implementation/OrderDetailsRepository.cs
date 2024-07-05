using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.DataAccess.Implementation
{
	public class OrderDetailsRepository : GenericRepository<OrderDetails>, IOrderDetailsRepository
	{
		private readonly ApplicationDbContext _context;
		public OrderDetailsRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderDetails orderDetails)
		{
			_context.TbOrderDetails.Update(orderDetails);
		}
	}
}
