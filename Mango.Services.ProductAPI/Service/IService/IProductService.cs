using Mango.Services.ProductAPI.Models.DTO;

namespace Mango.Services.ProductAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProducts();
        Task<ProductDTO>GetProductById(int productId);
        Task<int>CreateProduct(ProductDTO productDTO);
        Task<string>UpdateProduct(ProductDTO productDTO);
        Task<string> DeleteProduct(int productId);
    }
}
