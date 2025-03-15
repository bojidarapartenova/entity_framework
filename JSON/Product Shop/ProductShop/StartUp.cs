
namespace ProductShop
{
    using System.ComponentModel.DataAnnotations;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ProductShop.DTOs.Import;
    using ProductShop.Models;

    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();
            context.Database.Migrate();

            //string users = File.ReadAllText("../../../Datasets/users.json");
            //Console.WriteLine(ImportUsers(context, users));

            //string products = File.ReadAllText("../../../Datasets/products.json");
            //Console.WriteLine(ImportProducts(context, products));

            //string categories = File.ReadAllText("../../../Datasets/categories.json");
            //Console.WriteLine(ImportCategories(context, categories));

            //string categoriesProducts = File.ReadAllText("../../../Datasets/categories-products.json");
            //Console.WriteLine(ImportCategoryProducts(context, categoriesProducts));

            //Console.WriteLine(GetProductsInRange(context));
            //Console.WriteLine(GetSoldProducts(context));
            //Console.WriteLine(GetCategoriesByProductsCount(context));
            //Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportUserDTO[]? userDtos = JsonConvert
                .DeserializeObject<ImportUserDTO[]>(inputJson);

            if (userDtos != null)
            {
                ICollection<User> usersToAdd = new List<User>();
                foreach (var userDto in userDtos)
                {
                    if (!IsValid(userDto))
                    {
                        continue;
                    }

                    User user = new User()
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Age = userDto.Age
                    };

                    usersToAdd.Add(user);
                }
                context.Users.AddRange(usersToAdd);
                context.SaveChanges();

                result = $"Successfully imported {usersToAdd.Count}";
            }
            return result;
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;

            ImportProductDTO?[] productDtos = JsonConvert
                .DeserializeObject<ImportProductDTO[]>(inputJson);

            if (productDtos != null)
            {
                //ICollection<int> usersDb = context.Users.Select(u => u.Id).ToArray();

                ICollection<Product> validProducts = new List<Product>();
                foreach (var productDto in productDtos)
                {
                    if (!IsValid(productDto))
                    {
                        continue;
                    }

                    bool isPriceValid = decimal.TryParse(productDto.Price, out decimal productPrice);
                    bool isSellerIdValid = int.TryParse(productDto.SellerId, out int productSellerId);

                    if ((!isPriceValid) || (!isSellerIdValid))
                    {
                        continue;
                    }

                    int? buyerId = null;
                    if (productDto.BuyerId != null)
                    {
                        bool isBuyerIdValid = int.TryParse(productDto.BuyerId, out int productBuyerId);
                        if (!isBuyerIdValid)
                        {
                            continue;
                        }

                        //if(!usersDb.Contains(productBuyerId))
                        //{
                        //    continue;
                        //}

                        buyerId = productBuyerId;
                    }

                    //if(!usersDb.Contains(productSellerId))
                    //{
                    //    continue;
                    //}

                    Product product = new Product()
                    {
                        Name = productDto.Name,
                        Price = productPrice,
                        SellerId = productSellerId,
                        BuyerId = buyerId
                    };

                    validProducts.Add(product);
                }
                context.Products.AddRange(validProducts);
                context.SaveChanges();

                result = $"Successfully imported {validProducts.Count}";
            }
            return result;
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;
            ImportCategoryDTO?[] categoryDtos = JsonConvert
                .DeserializeObject<ImportCategoryDTO?[]>(inputJson);

            if (categoryDtos != null)
            {
                ICollection<Category> validCategories = new List<Category>();
                foreach (var categoryDto in categoryDtos)
                {
                    if (!IsValid(categoryDto))
                    {
                        continue;
                    }

                    Category category = new Category()
                    {
                        Name = categoryDto.Name
                    };

                    validCategories.Add(category);
                }
                context.Categories.AddRange(validCategories);
                context.SaveChanges();

                result = $"Successfully imported {validCategories.Count}";
            }
            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            string result = string.Empty;
            ImportCategoryProductDTO[] categoryProductDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductDTO?[]>(inputJson);

            if (categoryProductDtos != null)
            {
                ICollection<CategoryProduct> validCategoriesProducts = new List<CategoryProduct>();

                foreach (var categoryProductDto in categoryProductDtos)
                {
                    if (!IsValid(categoryProductDto))
                    {
                        continue;
                    }

                    bool isCategoryIdValid = int.TryParse(categoryProductDto.CategoryId, out int categoryId);
                    bool isProductIdValid = int.TryParse(categoryProductDto.ProductId, out int productId);

                    if ((!isCategoryIdValid) || (!isProductIdValid))
                    {
                        continue;
                    }

                    CategoryProduct categoryProduct = new CategoryProduct()
                    {
                        CategoryId = categoryId,
                        ProductId = productId
                    };

                    validCategoriesProducts.Add(categoryProduct);
                }
                context.CategoriesProducts.AddRange(validCategoriesProducts);
                context.SaveChanges();

                result = $"Successfully imported {validCategoriesProducts.Count}";
            }
            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    Seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .OrderBy(p => p.Price)
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(products, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new
                    {
                        p.Name,
                        p.Price,
                        BuyerFirstName = p.Buyer!.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                     .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(users, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count())
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count(),
                    AveragePrice = c.CategoriesProducts.Average(cp => cp.Product.Price).ToString("F2"),
                    TotalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price).ToString("F2")
                }).ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(categories, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold.Count(p => p.BuyerId.HasValue),
                        Products = u.ProductsSold
                        .Where(p => p.BuyerId.HasValue)
                        .Select(p => new
                        {
                            p.Name,
                            p.Price
                        }).ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToArray();

            var serializedUsers = new
            {
                UsersCount = users.Length,
                Users = users
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(serializedUsers, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver,
                    NullValueHandling = NullValueHandling.Ignore
                });

            return jsonResult;
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