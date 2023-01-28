namespace Edstem.Services.ShoppingCartAPI.Models.Dto;

public class CartDetailsDto
{
    public int CartDetailsId { get; set; }
    public int CartHeaderId { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }
}