namespace CarDealer
{
    using System.ComponentModel.DataAnnotations;
    using CarDealer.Data;
    using CarDealer.DTOs.Export;
    using CarDealer.DTOs.Import;
    using CarDealer.Models;
    using CarDealer.Utilities;
    using Microsoft.EntityFrameworkCore;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            context.Database.Migrate();

            //string suppliers = "../../../Datasets/suppliers.xml";
            //string inputXmlSuppliers = File.ReadAllText(suppliers);
            //Console.WriteLine(ImportSuppliers(context, inputXmlSuppliers));

            //string parts = "../../../Datasets/parts.xml";
            //string inputXmlParts=File.ReadAllText(parts);
            //Console.WriteLine(ImportParts(context, inputXmlParts));

            //string cars = "../../../Datasets/cars.xml";
            //string inputXmlCars = File.ReadAllText(cars);
            //Console.WriteLine(ImportCars(context, inputXmlCars));

            //string customers = "../../../Datasets/customers.xml";
            //string inputXmlCustomers = File.ReadAllText(customers);
            //Console.WriteLine(ImportCustomers(context, inputXmlCustomers));

            //string sales = "../../../Datasets/sales.xml";
            //string inputXmlSales = File.ReadAllText(sales);
            //Console.WriteLine(ImportSales(context, inputXmlSales));

            //Console.WriteLine(GetCarsWithDistance(context));
            //Console.WriteLine(GetCarsFromMakeBmw(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //Console.WriteLine(GetTotalSalesByCustomer(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportSupplierDto[]? supplierDtos = XmlHelper
                .Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            if (supplierDtos != null)
            {
                ICollection<Supplier> suppliers = new List<Supplier>();

                foreach (var supplierDto in supplierDtos)
                {
                    if (!IsValid(supplierDto))
                    {
                        continue;
                    }

                    bool isImporterValid = bool.TryParse(supplierDto.IsImporter, out bool isImporter);

                    if (!isImporterValid)
                    {
                        continue;
                    }

                    Supplier supplier = new Supplier()
                    {
                        Name = supplierDto.Name,
                        IsImporter = isImporter
                    };

                    suppliers.Add(supplier);
                }
                context.AddRange(suppliers);
                context.SaveChanges();

                result = $"Successfully imported {suppliers.Count}";
            }
            return result;
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportPartsDto[]? partDtos = XmlHelper
                .Deserialize<ImportPartsDto[]>(inputXml, "Parts");

            if (partDtos != null)
            {
                ICollection<int> supplierIds = context.Suppliers.Select(s => s.Id).ToList();

                ICollection<Part> parts = new List<Part>();

                foreach (var partDto in partDtos)
                {
                    if (!IsValid(partDto))
                    {
                        continue;
                    }

                    bool isPriceValid = decimal.TryParse(partDto.Price, out decimal price);
                    bool isQuantityValid = int.TryParse(partDto.Quantity, out int quantity);
                    bool isSupplierIdValid = int.TryParse(partDto.SupplierId, out int supplierId);

                    if ((!isPriceValid) || (!isQuantityValid) || (!isSupplierIdValid))
                    {
                        continue;
                    }

                    if (!supplierIds.Contains(supplierId))
                    {
                        continue;
                    }

                    Part part = new Part()
                    {
                        Name = partDto.Name,
                        Price = price,
                        Quantity = quantity,
                        SupplierId = supplierId
                    };
                    parts.Add(part);
                }
                context.AddRange(parts);
                context.SaveChanges();

                result = $"Successfully imported {parts.Count}";
            }
            return result;
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCarsDto[]? carsDtos = XmlHelper
                .Deserialize<ImportCarsDto[]>(inputXml, "Cars");

            if (carsDtos != null)
            {
                ICollection<int> dbPartIds = context.Parts.Select(p => p.Id).ToArray();
                ICollection<Car> cars = new List<Car>();

                foreach (var carDto in carsDtos)
                {
                    if (!IsValid(carDto))
                    {
                        continue;
                    }

                    bool isTraveledDistanceValid = long.TryParse(carDto.TraveledDistance, out long traveledDistance);
                    if (!isTraveledDistanceValid)
                    {
                        continue;
                    }

                    Car car = new Car()
                    {
                        Make = carDto.Make,
                        Model = carDto.Model,
                        TraveledDistance = traveledDistance
                    };

                    if (carDto.Parts != null)
                    {
                        int[] partIds = carDto
                            .Parts
                            .Where(p => IsValid(p) && int.TryParse(p.Id, out int id))
                            .Select(p => int.Parse(p.Id))
                            .Distinct()
                            .ToArray();

                        foreach (var partId in partIds)
                        {
                            if (!dbPartIds.Contains(partId))
                            {
                                continue;
                            }

                            PartCar partCar = new PartCar()
                            {
                                PartId = partId,
                                Car = car
                            };

                            car.PartsCars.Add(partCar);
                        }
                    }
                    cars.Add(car);
                }
                context.Cars.AddRange(cars);
                context.SaveChanges();

                result = $"Successfully imported {cars.Count}";
            }
            return result;
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportCustomerDto[]? customerDtos = XmlHelper
                .Deserialize<ImportCustomerDto[]>(inputXml, "Customers");

            if (customerDtos != null)
            {
                ICollection<Customer> customers = new List<Customer>();

                foreach (var customerDto in customerDtos)
                {
                    if (!IsValid(customerDto))
                    {
                        continue;
                    }

                    bool isBirthDateValid = DateTime.TryParse(customerDto.BirthDate, out DateTime birthDate);
                    bool isYoungDriverValid = bool.TryParse(customerDto.IsYoungDriver, out bool isYoungDriver);

                    if ((!isBirthDateValid) || (!isYoungDriverValid))
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
                context.AddRange(customers);
                context.SaveChanges();

                result = $"Successfully imported {customers.Count}";
            }
            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            string result = string.Empty;

            ImportSaleDto[]? saleDtos = XmlHelper
                .Deserialize<ImportSaleDto[]>(inputXml, "Sales");

            if (saleDtos != null)
            {
                ICollection<int> carIds = context.Cars.Select(c => c.Id).ToArray();
                ICollection<Sale> sales = new List<Sale>();

                foreach (var saleDto in saleDtos)
                {
                    if (!IsValid(saleDto))
                    {
                        continue;
                    }

                    bool isCarIdValid = int.TryParse(saleDto.CarId, out int carId);
                    bool isCustomerIdValid = int.TryParse(saleDto.CustomerId, out int customerId);
                    bool isDiscountValid = decimal.TryParse(saleDto.Discount, out decimal discount);

                    if ((!isCarIdValid) || (!isCustomerIdValid) || (!isDiscountValid))
                    {
                        continue;
                    }

                    if (!carIds.Contains(carId))
                    {
                        continue;
                    }

                    Sale sale = new Sale()
                    {
                        CarId = carId,
                        CustomerId = customerId,
                        Discount = discount
                    };

                    sales.Add(sale);
                }
                context.AddRange(sales);
                context.SaveChanges();

                result = $"Successfully imported {sales.Count}";
            }
            return result;
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            ExportCarsWithDistanceDto[] cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .Select(c => new ExportCarsWithDistanceDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance.ToString()
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            string result = XmlHelper.Serialize(cars, "cars");
            return result;
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            ExportCarsBMWDto[] cars = context.Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new ExportCarsBMWDto()
                {
                    Id = c.Id.ToString(),
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            string result = XmlHelper.Serialize(cars, "cars");
            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportNotImporterSuppliersDto[] suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new ExportNotImporterSuppliersDto()
                {
                    Id = s.Id.ToString(),
                    Name = s.Name,
                    PartsCount = s.Parts.Count.ToString()
                })
                .ToArray();

            string result = XmlHelper.Serialize(suppliers, "suppliers");
            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarsWithPartsDto[] cars = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Select(c => new ExportCarsWithPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars
                    .Select(pc => pc.Part)
                    .OrderByDescending(p => p.Price)
                    .Select(p => new ExportCarsWithPartsPartDto()
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .Take(5)
                .ToArray();

            string result = XmlHelper.Serialize(cars, "cars");
            return result;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportTotalSalesByCustomerDto[] customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new ExportTotalSalesByCustomerDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s => 
                    c.IsYoungDriver
                    ? s.Car.PartsCars.Sum(pc=>Math.Round(pc.Part.Price * 0.95m, 2))
                    : s.Car.PartsCars.Sum(pc=>pc.Part.Price)
                    )
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            string result = XmlHelper.Serialize(customers, "customers");
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