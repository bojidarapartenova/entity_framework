using P01_StudentSystem.Data;

namespace P01_StudentSystem
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            try
            {
                StudentSystemContext context = new StudentSystemContext();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Console.WriteLine("Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail!");
                Console.WriteLine(ex.ToString());
            }

        }
    }
}