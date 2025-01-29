namespace Application.DTO;

public class CreateProductDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}