using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext db, IMapper mapper, ILogger<ProductService> logger )
        {
            
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<string> CreateProduct(ProductDTO productDTO)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteProduct(int ProductId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products...");
            var data=await _db.Products.ToListAsync();
            if (data != null)
            {
                _logger.LogInformation($"{data.Count} number of product fetched.");
                var products=_mapper.Map<IEnumerable<ProductDTO>>(data);
                return products;
            }
            return null;
        }

        public async Task<ProductDTO> GetProductById(int productId)
        {
            _logger.LogInformation($"Fetching product by id : {productId} ...");
            var data = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == productId);          
            var product =_mapper.Map<ProductDTO>(data);
            if (product != null)
            {
                _logger.LogInformation($"Product {data.Name} fetched.");
                return product;
            }
            return null;
        }

        public Task<string> UpdateProduct(ProductDTO productDTO)
        {
            throw new NotImplementedException();
        }
    }
}
