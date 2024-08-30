using Newtonsoft.Json;

namespace ContactForm {
    internal static class ContactManager {
        const string _path = "contacts.json";

        public static void ViewContacts() {
            List<Contact> contacts = LoadContacts(_path);
            int contactsPerPage = 5;
            int totalContacts = contacts.Count;
            int totalPages = (int)Math.Ceiling(totalContacts / (double)contactsPerPage);
            int pageNumber = 1;

            if (totalContacts == 0) {
                Console.Clear();
                Console.WriteLine("You don't have any contacts.\nPress any key to return.");
                Console.ReadKey();
                return;
            }

            while (true) {
                Console.Clear();
                Console.WriteLine($"Viewing contacts - Page {pageNumber}/{totalPages}\n");

                int start = (pageNumber - 1) * contactsPerPage;
                int end = Math.Min(start + contactsPerPage, totalContacts);

                for (int i = start; i < end; i++) {
                    Console.WriteLine($"Contact #{i + 1}");
                    Console.WriteLine($"Name: {contacts[i].Name}");
                    Console.WriteLine($"Email: {contacts[i].Email}");
                    Console.WriteLine($"Phone: {contacts[i].Phone}\n");
                }

                string input = Utils.GetInput("Enter page number or 0 to return to menu: ");

                if (int.TryParse(input, out pageNumber)) {
                    if (pageNumber == 0) {
                        return;
                    }

                    if (pageNumber < 1 || pageNumber > totalPages) {
                        Utils.PrintColoredText($"Invalid page number. Please enter a number between 1 and {totalPages}.", ConsoleColor.Red);
                        pageNumber = 1;
                        Console.ReadKey();
                        continue;
                    }
                }
                else {
                    Utils.PrintColoredText("Invalid input. Please enter a valid number.", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        public static void CreateContact() {
            bool creatingProcess = true;
            while (creatingProcess) {
                Console.Clear();
                Console.WriteLine("Contact creation");
                string name = Utils.GetInput("contact name: ");
                string email = Utils.GetInput("contact email: ");
                string phone = Utils.GetInput("contact phone: ");
                try {
                    Contact contact = new Contact {

                        Id = NextId(_path),
                        Name = name,
                        Email = email,
                        Phone = phone
                    };
                    SaveContacts(_path, contact);
                    Console.ReadKey();
                    creatingProcess = false;
                }
                catch (ArgumentException e) {
                    Console.WriteLine($"error: {e.Message}\nPress any key to continue.");
                    Console.ReadKey();
                }
            }
        }

        private static  void SaveContacts(string path, Contact contact) {
            List<Contact> contacts = LoadContacts(path);
            contacts.Add(contact);

            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(path, json);

            Console.WriteLine("Contact added successfully.");
        }

        private static  List<Contact> LoadContacts(string path) {
            if(!File.Exists(path)) {
                return [];
            }
            string jsonData = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Contact>>(jsonData) ?? [];
        }

        private static int NextId(string path) {
            List<Contact> contacts = LoadContacts(path);
            return contacts.Count == 0 ? 1 : contacts[^1].Id + 1;
        }
        
    }
}
