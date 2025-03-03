namespace BookShop
{
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            //string command = Console.ReadLine();

            //Console.WriteLine(GetBooksByAgeRestriction(db, command));

            //Console.WriteLine(GetGoldenBooks(db));

            Console.WriteLine(GetBooksByPrice(db));

            //Console.WriteLine(GetBooksNotReleasedIn(db, 2000));

            //string input = "horror mystery drama";
            //Console.WriteLine(GetBooksByCategory(db, input));

            //Console.WriteLine(GetBooksReleasedBefore(db, "12-04-1992"));

            //Console.WriteLine(GetAuthorNamesEndingIn(db, "dy"));

            //Console.WriteLine(GetBookTitlesContaining(db, "sK"));

            //Console.WriteLine(GetBooksByAuthor(db, "R"));

            //Console.WriteLine(CountBooks(db, 12));

            //Console.WriteLine(CountCopiesByAuthor(db));

            //Console.WriteLine(GetTotalProfitByCategory(db);

            //Console.WriteLine(GetMostRecentBooks(db));

            //IncreasePrices(db);

            //Console.WriteLine(RemoveBooks(db));

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            var bookTitles = context
                .Books
                .Select(b => new
                {
                    BookTitle = b.Title,
                    Restriction = b.AgeRestriction.ToString().ToLower()
                })
                .OrderBy(b => b.BookTitle)
                .ToArray();

            foreach (var book in bookTitles)
            {
                if (book.Restriction == command.ToLower())
                {
                    sb.AppendLine(book.BookTitle);
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var bookTitles = context
                .Books
                .Where(b => b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    Edition = b.EditionType.ToString()
                })
                .ToArray();

            foreach (var book in bookTitles)
            {
                if (book.Edition == "Gold")
                {
                    sb.AppendLine(book.BookTitle);
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    BookPrice = b.Price
                })
                .OrderByDescending(b=>b.BookPrice)
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.BookTitle} - ${book.BookPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => new 
                { 
                    BookTitle = b.Title 
                })
                .ToArray();

            foreach (var book in books)
            {
                sb.AppendLine(book.BookTitle);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] searchCategories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLowerInvariant()).ToArray();

            var books = context
                .Books
                .Where(b => b.BookCategories.Any(bc => searchCategories.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime dateInput = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context
                .Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value < dateInput)
                .OrderByDescending(b=>b.ReleaseDate)
                .Select(b => new
                {
                    BookTitle = b.Title,
                    Edition = b.EditionType.ToString(),
                    BookPrice = b.Price
                })
                .ToArray();

            foreach(var book in books)
            {
                sb.AppendLine($"{book.BookTitle} - {book.Edition} - ${book.BookPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context
                .Authors
                .Where(a=>a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    AuthorFirstName = a.FirstName,
                    AuthorLastName = a.LastName
                })
                .OrderBy(a=>a.AuthorFirstName)
                .ThenBy(a => a.AuthorLastName)
                .ToArray();

            foreach(var author in authors)
            {
                sb.AppendLine($"{author.AuthorFirstName} {author.AuthorLastName}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b=>b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new
                {
                    BookTitle = b.Title
                })
                .OrderBy(b => b.BookTitle)
                .ToArray();

            foreach(var book in books)
            {
                sb.AppendLine(book.BookTitle);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .OrderBy(b=>b.BookId)
                .Where(b=>b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    BookTitle = b.Title,
                    AuthorFirstName = b.Author.FirstName,
                    AuthorLastName = b.Author.LastName
                })
                .ToArray();

            foreach(var book in books)
            {
                sb.AppendLine($"{book.BookTitle} ({book.AuthorFirstName} {book.AuthorLastName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books=context
                .Books
                .Where(b=>b.Title.Length>lengthCheck) .ToArray();

            return books.Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authorstCopies = context
                .Authors
                .Select(a => new
                {
                    AuthorFirstName = a.FirstName,
                    AuthorLastName = a.LastName,
                    TotalBookCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a=>a.TotalBookCopies)
                .ToArray();

            foreach(var copy in authorstCopies)
            {
                sb.AppendLine($"{copy.AuthorFirstName} {copy.AuthorLastName} - {copy.TotalBookCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var totalProfit = context
               .Categories
               .Include(c => c.CategoryBooks)
               .ThenInclude(cb => cb.Book)
               .Select(c => new
               {
                   CategoryName = c.Name,
                   Profit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
               })
               .OrderByDescending(c=>c.Profit)
               .ThenBy(c=>c.CategoryName)
               .ToArray();

            foreach(var profit in totalProfit)
            {
                sb.AppendLine($"{profit.CategoryName} ${profit.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context
                .Categories
                .Include(c => c.CategoryBooks)
                .ThenInclude(cb => cb.Book)
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books=c.CategoryBooks.Select(c=>c.Book).OrderByDescending(b=>b.ReleaseDate).Take(3).ToArray()
                })
                .OrderBy(c=>c.CategoryName)
                .ToArray();

            foreach(var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");
                foreach(var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var booksToIncreasePrice = context
                .Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach(var book in booksToIncreasePrice)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            foreach(var book in booksToRemove)
            {
                context.Remove(book);
            }

            context.SaveChanges();

            return booksToRemove.Length;
        }
    }
}


