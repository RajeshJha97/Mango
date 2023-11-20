using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Mango.Services.ProductAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Service
{
    public class ProductService : IProductService
    {
        #region PrivateMember
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private ILogger<ProductService> _logger;
        #endregion

        #region Constructor
        public ProductService(ApplicationDbContext db, IMapper mapper, ILogger<ProductService> logger )
        {
            
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region GetAllProducts
        public async Task<IEnumerable<ProductDTO>> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products...");
            var data = await _db.Products.ToListAsync();
            if (data != null)
            {
                _logger.LogInformation($"{data.Count} number of product fetched.");
                var products = _mapper.Map<IEnumerable<ProductDTO>>(data);
                return products;
            }
            return null;
        }
        #endregion

        #region GetProductByID
        public async Task<ProductDTO> GetProductById(int productId)
        {
            _logger.LogInformation($"Fetching product by id : {productId} ...");
            var data = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == productId);
            var product = _mapper.Map<ProductDTO>(data);
            if (product != null)
            {
                _logger.LogInformation($"Product {data.Name} fetched.");
                return product;
            }
            return null;
        }
        #endregion

        #region Create
        public async Task<int> CreateProduct(ProductDTO productDTO)
        {
            _logger.LogInformation("Checking for product for duplicate");
            if (_db.Products.FirstOrDefaultAsync(u => u.Name == productDTO.Name).GetAwaiter().GetResult() == null)
            {
                _logger.LogInformation("Creating product");
                var product=_mapper.Map<Product>(productDTO);
                await _db.Products.AddAsync(product);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"Created product {productDTO.Name} successfully");

                return product.ProductId;
            }
            _logger.LogError($"Product {productDTO.Name} already exist.");
            return 0;
        }
        #endregion

        #region Update
        public async Task<string> UpdateProduct(ProductDTO productDTO)
        {
            _logger.LogInformation("Checking for product for existence.");
            if (_db.Products.AsNoTracking().FirstOrDefaultAsync(u=>u.ProductId==productDTO.ProductId).GetAwaiter().GetResult()!=null)
            {
                var product = _mapper.Map<Product>(productDTO);
                _db.Products.Update(product);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"{product.Name} updated successfully.");
                return $"{product.Name} updated successfully";
            }
            _logger.LogError($"Product {productDTO.Name} dosen't exist.");
            return null;
        }
        #endregion

        #region Delete
        public async Task<string> DeleteProduct(int productId)
        {
            _logger.LogInformation("Checking for product for existence.");
            if (_db.Products.FirstOrDefaultAsync(u => u.ProductId == productId).GetAwaiter().GetResult() != null)
            {
                var product = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == productId);
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"{product.Name} removed successfully.");
                return $"Product {product.Name} removed successfully.";
            }
            _logger.LogError($"Product {productId} dosen't exist.");
            return null;
        }
        #endregion

    }
}
