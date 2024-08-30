namespace ContactForm
{
    internal class Program
    {
        static void Main(string[] args)
        {

            while(true) {
                Console.Clear();
                DisplayMenu();
                string choice = Utils.GetInput("Enter option number: ");

                switch (choice) {
                    case "1":
                        ContactManager.ViewContacts();
                        break;
                    case "2":
                        break;
                    case "3":
                        ContactManager.CreateContact();
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine($"\"{choice}\" is invalid option\nPress any key to continue.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void DisplayMenu() {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. View all");
            Console.WriteLine("2. Search");
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Edit");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Exit ");
        }
    }
}
