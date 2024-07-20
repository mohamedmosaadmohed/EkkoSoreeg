using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Repositories;

namespace EkkoSoreeg.DataAccess.Implementation
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		public ICatagoryRepository Catagory { get; private set; }
		public IProductRepository Product { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }

		public IOrderDetailsRepository OrderDetails { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IColorRepository Color { get; private set; }
		public ISizeRepository Size { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Catagory = new CatagoryRepository(context);
			Product = new ProductRepository(context);
			ShoppingCart = new ShoppingCartRepository(context);
			OrderHeader = new OrderHeaderRepository(context);
			OrderDetails = new OrderDetailsRepository(context);
			ApplicationUser = new ApplicationUserRepository(context);
			Color = new ColorRepository(context);
			Size = new SizeRepository(context);
		}
		public int Complete()
		{
			return _context.SaveChanges();
		}
		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
