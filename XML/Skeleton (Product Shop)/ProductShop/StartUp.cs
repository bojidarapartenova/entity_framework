using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            context.Database.Migrate();

            //string users = "../../../Datasets/users.xml";
            //string inputUsers=File.ReadAllText(users);
            //Console.WriteLine(ImportUsers(context, inputUsers));

            //string products = "../../../Datasets/products.xml";
            //string inputProducts=File.ReadAllText(products);
            //Console.WriteLine(ImportProducts(context, inputProducts));

            //string categories = "../../../Datasets/categories.xml";
            //string inputCategories = File.ReadAllText(categories);
            //Console.WriteLine(ImportCategories(context, inputCategories));

            //string categoryProducts = "../../../Datasets/categories-products.xml";
            //string inputCategoryProducts = File.ReadAllText(categoryProducts);
            //Console.WriteLine(ImportCategoryProducts(context, inputCategoryProducts));

            //Console.WriteLine(GetProductsInRange(context));
            //Console.WriteLine(GetSoldProducts(context));
            //Console.WriteLine(GetCategoriesByProductsCount(context));
            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;

            ImportUsersDto[]? userDtos = XmlHelper
                .Deserialize<ImportUsersDto[]>(inputXml, "Users");

            if (userDtos != null)
            {
                ICollection<User> users = new List<User>();
                foreach (var userDto in userDtos)
                {
                    if (!IsValid(userDto))
                    {
                        continue;
                    }

                    bool isAgeValid = int.TryParse(userDto.Age, out int age);
                    if (!isAgeValid)
                    {
                        continue;
                    }

                    User user = new User()
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Age = age
                    };
                    users.Add(user);
                }
                context.AddRange(users);
                context.SaveChanges();

                result = $"Successfully imported {users.Count}";
            }
            return result;
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;

            ImportProductsDto[]? productDtos = XmlHelper
                .Deserialize<ImportProductsDto[]>(inputXml, "Products");

            if (productDtos != null)
            {
                ICollection<Product> products = new List<Product>();
                foreach (var productDto in productDtos)
                {
                    if (!IsValid(productDto))
                    {
                        continue;
                    }

                    Product product = new Product()
                    {
                        Name = productDto.Name,
                        Price = productDto.Price,
                        SellerId = productDto.SellerId,
                        BuyerId = productDto.BuyerId
                    };
                    products.Add(product);
                }
                context.AddRange(products);
                context.SaveChanges();

                result = $"Successfully imported {products.Count}";
            }
            return result;
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCategoriesDto[]? categoryDtos = XmlHelper
                .Deserialize<ImportCategoriesDto[]>(inputXml, "Categories");

            if (categoryDtos != null)
            {
                ICollection<Category> categories = new List<Category>();
                foreach (var categoryDto in categoryDtos)
                {
                    if (!IsValid(categoryDto))
                    {
                        continue;
                    }

                    if (categoryDto.Name == null)
                    {
                        continue;
                    }

                    Category category = new Category()
                    {
                        Name = categoryDto.Name
                    };
                    categories.Add(category);
                }
                context.AddRange(categories);
                context.SaveChanges();

                result = $"Successfully imported {categories.Count}";
            }
            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCategoryProductsDto[]? categoryProductsDtos = XmlHelper
                .Deserialize<ImportCategoryProductsDto[]>(inputXml, "CategoryProducts");

            if(categoryProductsDtos != null)
            {
                List<int> categoryIds = context.Categories.Select(c => c.Id).ToList();
                List<int> productIds = context.Products.Select(p => p.Id).ToList();

                ICollection<CategoryProduct> categoryProducts = new List<CategoryProduct>();

                foreach(var categoryProductDto in categoryProductsDtos)
                {
                    if(!IsValid(categoryProductDto))
                    {
                        continue;
                    }

                    if ((!categoryIds.Contains(categoryProductDto.CategoryId))
                        || (!productIds.Contains(categoryProductDto.ProductId)))
                    {
                        continue;
                    }

                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = categoryProductDto.CategoryId,
                        ProductId = categoryProductDto.ProductId
                    };
                    categoryProducts.Add(categoryProduct);
                }
                context.CategoryProducts.AddRange(categoryProducts);
                context.SaveChanges();

                result = $"Successfully imported {categoryProducts.Count}";
            }
            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductsInRangeDto[] products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductsInRangeDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .Take(10)
                .ToArray();

            string result = XmlHelper.Serialize(products, "Products");
            return result;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportSoldProductsUserDto[] users = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new ExportSoldProductsUserDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ExportSoldProductsProductDto()
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .Take(5)
                .ToArray();

            string result = XmlHelper.Serialize(users, "Users");
            return result;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoriesByProductsCountDto[] categories = context.Categories
                .Select(c => new ExportCategoriesByProductsCountDto()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            string result = XmlHelper.Serialize(categories, "Categories");
            return result;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUsersAndProductsDto users = new ExportUsersAndProductsDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),

                Users = context.Users
                    .Take(10)
                    .OrderByDescending(u => u.ProductsSold.Count())
                    .Where(u => u.ProductsSold.Any())
                    .Select(u => new ExportUsersAndProductsUserDto()
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Age = u.Age,
                        SoldProducts = new ExportSoldProductsDto()
                        {
                            Count = u.ProductsSold.Count(),
                            Products = u.ProductsSold
                            .Select(p => new ExportUsersAndProductsProductDto()
                            {
                                Name = p.Name,
                                Price = p.Price
                            })
                            .OrderByDescending(p => p.Price)
                            .ToArray()
                        }
                    })
                    .ToArray()
            };

            string result = XmlHelper.Serialize(users, "Users");
            return result;
        }
        public static bool IsValid(object dto)
        {
            var validateContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator
                .TryValidateObject(dto, validateContext, validationResults);

            return isValid;
        }
    }
}