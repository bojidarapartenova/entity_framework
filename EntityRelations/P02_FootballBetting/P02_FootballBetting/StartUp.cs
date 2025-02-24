using P02_FootballBetting.Data;

namespace P02_FootballBetting
{
    public class StartUp
    {
       public static void Main(string[] args)
        {
            try
            {
                using FootballBettingContext context = new FootballBettingContext();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Console.WriteLine("Successful creation!");
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Failed creation!");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}