
using cs_apiEcommerce.Models; // Add this line or update with the correct namespace for Product

namespace cs_apiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();
    ICollection<Product> GetProductsPaginated(int pageNumber, int pageSize);
    int GetTotalProducts();
    ICollection<Product> GetProductsByCategory(int categoryId);
    ICollection<Product> SearchProducts(string searchTerm);

    Product? GetProduct(int id);

    bool BuyProduct(string name, int quantity);
    bool ProductExists(int id);
    bool ProductExists(string name);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();
}
