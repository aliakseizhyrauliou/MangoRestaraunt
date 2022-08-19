namespace Mango.Services.ShoppingCart.API.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public CartHeader CartHeader { get; set; }
        public IEnumerable<CartDetails> CartDetails { get; set; }
    }
}
