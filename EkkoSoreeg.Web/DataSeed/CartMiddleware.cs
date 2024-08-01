namespace EkkoSoreeg.Web.DataSeed
{
	public class CartMiddleware
	{
		private readonly RequestDelegate _next;

		public CartMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, CartService cartService)
		{
			if (context.User.Identity.IsAuthenticated)
			{
				await cartService.TransferCartDataAsync();
			}

			await _next(context);
		}
	}
}
