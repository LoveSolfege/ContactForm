using ContactForm.Models;
using ContactForm.Utilities;
using Newtonsoft.Json;

namespace ContactForm.Services {
    internal static class ContactManager
    {
        const string _path = "contacts.json";
        const int contactsPerPage = 5;
        public static void ViewContacts()
        {
            List<Contact> contacts = LoadContactsFromJson(_path);
            (int totalContacts, int totalPages) = GetContactPageSizes(contacts, contactsPerPage);
            int pageNumber = 1;
            if (totalContacts == 0)
            {
                Console.Clear();
                Console.WriteLine("You don't have any contacts.\nPress any key to return.");
                Console.ReadKey();
                return;
            }
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Viewing contacts - Page {pageNumber}/{totalPages}\n");
                PrintContactsFromList(contacts, contactsPerPage, pageNumber);

                string choice = Utils.GetInput("Enter page number or 0 to return to menu: ");
                if(choice == "0") {
                    return;
                }
                else {
                    pageNumber = SelectPage(totalPages, choice);
                }
            }
        }

        public static void SearchContact() {
            List<Contact> contacts = LoadContactsFromJson(_path);

            while (true) {
                Console.Clear();
                Console.WriteLine("Contact searching\n You can search by [number], [name], [email], [mobile]\n");
                string searchOption = Utils.GetInput("Select search type: ").ToLower();
                switch (searchOption) {
                    case "number":

                        break;
                    case "name":

                        break;
                    case "email":

                        break;
                    case "mobile":

                        break;
                    case "":
                        return;
                    default:
                        Utils.PrintColoredText($"{searchOption} is not a valide search option\n try again or press Enter to return", ConsoleColor.Red);
                        break;
                }
            }
        }

        public static void CreateContact()
        {
            bool creatingProcess = true;
            while (creatingProcess)
            {
                Console.Clear();
                Console.WriteLine("Contact creation");
                string name = Utils.GetInput("contact name: ");
                string email = Utils.GetInput("contact email: ");
                string phone = Utils.GetInput("contact phone: ");
                try
                {
                    Contact contact = new Contact
                    {

                        Id = JsonNextId(_path),
                        Name = name,
                        Email = email,
                        Phone = phone
                    };
                    SaveContacts(_path, contact);
                    Console.ReadKey();
                    creatingProcess = false;
                }
                catch (ArgumentException e)
                {
                    Utils.PrintColoredText($"Error: {e.Message}\nPress any key to continue.", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        private static void SaveContacts(string path, Contact contact)
        {
            List<Contact> contacts = LoadContactsFromJson(path);
            contacts.Add(contact);

            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(path, json);

            Console.WriteLine("Contact added successfully.");
        }

        private static List<Contact> LoadContactsFromJson(string path)
        {
            if (!File.Exists(path))
            {
                return [];
            }
            string jsonData = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Contact>>(jsonData) ?? [];
        }

        private static void GetContactByAtribute(List<Contact> contacts, string option, string prompt) {

        }

        private static int JsonNextId(string jsonPath)
        {
            List<Contact> contacts = LoadContactsFromJson(jsonPath);
            return contacts.Count == 0 ? 1 : contacts[^1].Id + 1;
        }

        private static void PrintContactsFromList(List<Contact> contacts, int contactsPerPage, int pageNumber) {
            int totalContacts = contacts.Count;
            int start = (pageNumber - 1) * contactsPerPage;
            int end = Math.Min(start + contactsPerPage, totalContacts);

            for (int i = start; i < end; i++) {
                Console.WriteLine($"Contact #{i + 1}");
                Console.WriteLine($"Name: {contacts[i].Name}");
                Console.WriteLine($"Email: {contacts[i].Email}");
                Console.WriteLine($"Phone: {contacts[i].Phone}\n");
            }

        }

        private static (int, int) GetContactPageSizes(List<Contact> contacts, int contactsPerPage) {
            int totalContacts = contacts.Count;
            int totalPages = (int)Math.Ceiling(totalContacts / (double)contactsPerPage);

            return (totalContacts, totalPages);
        }

        private static int SelectPage(int totalPages, string input) {
            if (int.TryParse(input, out int pageNumber)) {
                if (pageNumber == 0) {
                    throw new ArgumentOutOfRangeException();
                }
                if (pageNumber < 1 || pageNumber > totalPages) {
                    Utils.PrintColoredText($"Invalid page number. Please enter a number between 1 and {totalPages}.", ConsoleColor.Red);
                    pageNumber = 1;
                    Console.ReadKey();
                    return 1;
                }
                return pageNumber;
            }
            else {
                Utils.PrintColoredText("Invalid input. Please enter a valid number.", ConsoleColor.Red);
                Console.ReadKey();
                return 1;
            }
        }

    }
}
