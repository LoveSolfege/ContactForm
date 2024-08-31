using ContactForm.Models;
using ContactForm.Utilities;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace ContactForm.Services {
    internal static class ContactManager
    {
        const string _path = "contacts.json";
        public static void ViewContacts()
        {
            List<Contact> contacts = LoadContactsFromJson(_path);
            DisplayContactsWithPaging(contacts, "Viewing contacts");
        }

        public static void SearchContact() {
            List<Contact> contacts = LoadContactsFromJson(_path);
            var contactFields = new Dictionary<string, string> {
                { "number", "Id" },
                { "name", "Name" },
                { "email", "Email" },
                { "mobile", "Phone" }
            };

            while (true) {
                Console.Clear();
                Console.WriteLine("Contact searching\nYou can search by [number], [name], [email], [mobile]\n[exit] to exit\n");

                string searchOption = Utils.GetInput("Search by? ", ConsoleColor.DarkCyan).ToLower();

                if (searchOption == "exit" || string.IsNullOrEmpty(searchOption)) {
                    return;
                }

                string searchTerm = Utils.GetInput("search for? ", ConsoleColor.DarkCyan);

                if (contactFields.TryGetValue(searchOption, out var attribute)) {
                    try {
                        List<Contact> filteredContacts = GetContactByAtribute(contacts, attribute, searchTerm);

                        if (filteredContacts.Any()) {
                            DisplayContactsWithPaging(filteredContacts, $"Results for '{searchTerm}' in [{searchOption}]s");
                        }
                        else {
                            Utils.PrintColoredText($"\nnothing found by {searchTerm} in any {attribute}s", ConsoleColor.DarkMagenta);
                            Console.ReadKey();
                        }
                    }
                    catch (Exception e) {
                        Utils.PrintColoredText($"{e.Message}", ConsoleColor.Red);
                        Console.ReadKey();
                    }
                }
                else {
                    Utils.PrintColoredText($"{searchOption} is not a valid search option\ntry again or press Enter to return", ConsoleColor.Red);
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

                        Id = JsonNextId(_path),
                        Name = name,
                        Email = email,
                        Phone = phone
                    };
                    SaveContacts(_path, contact);
                    Console.ReadKey();
                    creatingProcess = false;
                }
                catch (ArgumentException e) {
                    Utils.PrintColoredText($"Error: {e.Message}\nPress any key to continue.", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        private static void DisplayContactsWithPaging(List<Contact> contacts, string header) {
            const int contactsPerPage = 5;
            int currentPage = 1;
            (int totalContacts, int totalPages) = GetContactPageSizes(contacts, contactsPerPage);

            if (totalContacts == 0) {
                Console.Clear();
                Utils.PrintColoredText("No contacts available.\nPress any key to return.", ConsoleColor.DarkMagenta);
                Console.ReadKey();
                return;
            }

            while (true) {
                Console.Clear();
                Console.WriteLine($"{header} - Page {currentPage}/{totalPages}\n");
                PrintContactsFromList(contacts, contactsPerPage, currentPage);

                string input = Utils.GetInput("Enter page number or 0 to return to menu: ");
                if (input == "0") {
                    break;
                }
                else {
                    int selectedPage;
                    if (int.TryParse(input, out selectedPage) && selectedPage >= 1 && selectedPage <= totalPages) {
                        currentPage = selectedPage;
                    }
                    else {
                        Utils.PrintColoredText($"Invalid page number. Please enter a number between 1 and {totalPages}.", ConsoleColor.Red);
                        Console.ReadKey();
                    }
                }
            }
        }

        private static List<Contact> GetContactByAtribute(List<Contact> contacts, string option, string prompt) {
            PropertyInfo? property = typeof(Contact).GetProperty(option, BindingFlags.Public | BindingFlags.Instance);

            if (property == null) {
                throw new ArgumentException("Invalid property name", nameof(option));
            }
            return contacts.Where(c => {
                var value = property.GetValue(c);
                if(value is string stringValue) {
                    return value != null && stringValue.IndexOf(prompt, StringComparison.OrdinalIgnoreCase) >= 0;
                }
                else if (value is int intValue && int.TryParse(prompt, out var searchInt)) {
                    return intValue == searchInt;
                }

                return false;
            }).ToList();
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
                Console.WriteLine($"Contact #{contacts[i].Id}");
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
    }

}
