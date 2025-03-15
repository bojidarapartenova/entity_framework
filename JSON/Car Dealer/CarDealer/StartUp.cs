namespace CarDealer
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();
            context.Database.Migrate();

            Console.WriteLine();

            //string suppliers = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, suppliers));

            //string parts = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, parts));

            //string cars = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, cars));

            //string customers = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, customers));

            //string sales = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, sales));

            //Console.WriteLine(GetOrderedCustomers(context));
            //Console.WriteLine(GetCarsFromMakeToyota(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //Console.WriteLine(GetTotalSalesByCustomer(context));
            //Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportSuppliersDto[] suppliersDtos = JsonConvert
                .DeserializeObject<ImportSuppliersDto[]>(inputJson);

            if (suppliersDtos != null)
            {
                ICollection<Supplier> suppliers = new List<Supplier>();
                foreach (var supplierDto in suppliersDtos)
                {
                    if (!IsValid(supplierDto))
                    {
                        continue;
                    }

                    bool isIsImporterValid = bool.TryParse(supplierDto.IsImporter, out bool validIsImporter);
                    if (!isIsImporterValid)
                    {
                        continue;
                    }

                    Supplier supplier = new Supplier()
                    {
                        Name = supplierDto.Name,
                        IsImporter = validIsImporter
                    };

                    suppliers.Add(supplier);
                }
                context.Suppliers.AddRange(suppliers);
                context.SaveChanges();

                result = $"Successfully imported {suppliers.Count}.";
            }
            return result;
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson);

            var validSupplierIds = context.Suppliers.Select(s => s.Id).ToHashSet();

            parts.RemoveAll(p =>
                !p.SupplierId.HasValue ||
                !validSupplierIds.Contains(p.SupplierId.Value));
            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCarsDto[] carsDtos = JsonConvert
                .DeserializeObject<ImportCarsDto[]>(inputJson);

            if (carsDtos != null)
            {
                ICollection<Car> cars = new List<Car>();
                ICollection<PartCar> partsCars = new List<PartCar>();
                foreach (var carsDto in carsDtos)
                {
                    if (!IsValid(carsDto))
                    {
                        continue;
                    }

                    bool isTraveledDistanceValid = int.TryParse(carsDto.TraveledDistance, out int traveledDistance);

                    if ((!isTraveledDistanceValid))
                    {
                        continue;
                    }

                    Car car = new Car()
                    {
                        Make = carsDto.Make,
                        Model = carsDto.Model,
                        TraveledDistance = traveledDistance
                    };
                    cars.Add(car);

                    foreach (var partId in carsDto.PartsId.Distinct())
                    {
                        PartCar partCar = new PartCar()
                        {
                            Car = car,
                            PartId = partId
                        };
                    }
                }

                context.Cars.AddRange(cars);
                context.PartsCars.AddRange(partsCars);
                context.SaveChanges();

                result = $"Successfully imported {cars.Count}.";
            }
            return result;
        }
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            string result = string.Empty;

            ImportCustomersDto[] customersDtos = JsonConvert
                .DeserializeObject<ImportCustomersDto[]>(inputJson);

            if (customersDtos != null)
            {
                ICollection<Customer> customers = new List<Customer>();
                foreach (var customerDto in customersDtos)
                {
                    if (!IsValid(customerDto))
                    {
                        continue;
                    }

                    bool isBirthDateValid = DateTime.TryParse(customerDto.BirthDate, out DateTime birthDate);
                    bool isIsYoungDriverValid = bool.TryParse(customerDto.IsYoungDriver, out bool isYoungDriver);

                    if ((!isBirthDateValid) || (!isIsYoungDriverValid))
                    {
                        continue;
                    }

                    Customer customer = new Customer()
                    {
                        Name = customerDto.Name,
                        BirthDate = birthDate,
                        IsYoungDriver = isYoungDriver
                    };

                    customers.Add(customer);
                }
                context.Customers.AddRange(customers);
                context.SaveChanges();

                result = $"Successfully imported {customers.Count}.";
            }
            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    c.IsYoungDriver
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(customers, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(cars, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter.Equals(false))
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(suppliers, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Make,
                        Model = s.Model,
                        TraveledDistance = s.TraveledDistance
                    },
                    parts = s.PartsCars.Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("f2")
                    })
                        .ToArray()
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(cars, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales
                        .SelectMany(s => s.Car.PartsCars)
                        .Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(customers, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                });

            return jsonResult;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    priceWithDiscount = s.Car.PartsCars.Sum(pc => pc.Part.Price * (1 - s.Discount * 0.01m))
                        .ToString("f2")
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new DefaultNamingStrategy()
            };

            string jsonResult = JsonConvert
                .SerializeObject(sales, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
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