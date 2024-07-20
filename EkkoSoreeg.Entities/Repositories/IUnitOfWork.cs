using EkkoSoreeg.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkoSoreeg.Entities.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICatagoryRepository Catagory { get; }
        IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
		IApplicationUserRepository ApplicationUser { get; }
        IColorRepository Color { get; }
        ISizeRepository Size { get; }
		int Complete();
    }
}
