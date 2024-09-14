using ContactForm.Models;
using ContactForm.Utilities;
using Newtonsoft.Json;
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

            while (true) {
                Utils.ClearConsolePlaceHeader("Contact searching\nYou can search by [number], [name], [email], [mobile]\n[exit] to exit", addLineBreak: true);

                string searchOption = Utils.GetInput("Search by? ", ConsoleColor.DarkCyan).ToLower();

                if (searchOption == "exit" || string.IsNullOrEmpty(searchOption)) {
                    return;
                }

                string searchTerm = Utils.GetInput("search for? ", ConsoleColor.DarkCyan);

                HandleSearch(contacts, searchOption, searchTerm);
            }
        }

        public static void CreateContact() {
            bool creatingProcess = true;
            while (creatingProcess) {
                Utils.ClearConsolePlaceHeader("Contact creation");
                string name = Utils.GetInput("contact name: ");
                string email = Utils.GetInput("contact email: ");
                string phone = Utils.GetInput("contact phone: ");
                try {
                    Contact contact = new Contact(
                        JsonNextId(_path), name, email, phone
                    );

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

        public static void EditContact() {
            List<Contact> contacts = LoadContactsFromJson(_path);
            while (true) {
                Utils.ClearConsolePlaceHeader("Contact Editor", addLineBreak: true);
                string selectedId = Utils.GetInput("insert contact # to eddit: ", ConsoleColor.DarkCyan);
                try {
                    Contact searchResult = SearchById(contacts, selectedId);
                    DisplaySingleContact(searchResult);
                    string choice = Utils.GetInput("Proceed with edition of this contact? yes/no: ", ConsoleColor.DarkRed).ToLower();
                    if (choice == "yes" || choice == "y") {
                        contacts = HandleEdition(searchResult, contacts, selectedId);
                        ModifyConctacsJson(_path, contacts, 0);
                        Utils.ClearConsolePlaceHeader("Contact modified successfuly", ConsoleColor.Green);
                        Console.ReadKey();
                        return;
                    }
                    else {
                        Utils.ClearConsolePlaceHeader("edition canceled", ConsoleColor.Red);
                        Console.ReadKey();
                        return;
                    }
                }
                catch (Exception) {
                    Utils.PrintColoredText($"No contact fond under number {selectedId}", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        public static void DeleteContact() {
            List<Contact> contacts = LoadContactsFromJson(_path);
            while (true) {
                Utils.ClearConsolePlaceHeader("Contact Deletion", addLineBreak: true);
                string selectedId = Utils.GetInput("insert contact # to delete: ", ConsoleColor.DarkCyan);
                try {
                    Contact searchResult = SearchById(contacts, selectedId);
                    DisplaySingleContact(searchResult);
                    string choice = Utils.GetInput("Proceed with deletion of this contact? yes/no: ", ConsoleColor.DarkRed).ToLower();
                    if(choice == "yes" || choice == "y") {
                        int deletedId = searchResult.Id;
                        contacts.Remove(searchResult);
                        ModifyConctacsJson(_path, contacts, deletedId);
                        Utils.ClearConsolePlaceHeader("Contact deleted successfuly", ConsoleColor.Green);
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        Utils.ClearConsolePlaceHeader("Deletion canceled", ConsoleColor.Red);
                        Console.ReadKey();
                        return;
                    }
                }
                catch(Exception){
                    Utils.PrintColoredText($"No contact fond under number {selectedId}", ConsoleColor.Red);
                    Console.ReadKey();
                }
            }
        }

        //Helper methods below

        private static List<Contact> HandleEdition(Contact contact, List<Contact> contacts, string selectedId) {
            int Id = int.Parse(selectedId);

            (int contactId, string name, string email, string phone) oldData = (contact.Id,  contact.Name, contact.Email, contact.Phone);

            Utils.ClearConsolePlaceHeader($"Editing contact #{contact.Id}\nLeave blanks to keep old values", ConsoleColor.DarkCyan, true);

            string? newName = GetUpdatedValue(oldData.name, "Name");
            string? newEmail = GetUpdatedValue(oldData.email, "Email");
            string? newPhone = GetUpdatedValue(oldData.phone, "Phone");

            contacts[Id - 1] = new Contact(oldData.contactId, newName, newEmail, newPhone);
            return contacts;
        }

        private static string GetUpdatedValue(string oldValue, string fieldName) {
            string? newValue = Utils.GetInput($"Old {fieldName} '{oldValue}' provide new {fieldName}: ", ConsoleColor.DarkYellow);
            return string.IsNullOrWhiteSpace(newValue) ? oldValue : newValue;
        }

        private static Contact SearchById(List<Contact> contacts, string id) {
            if(int.TryParse(id, out int index)) {
                return contacts[index-1];
            }
            else {
                throw new IndexOutOfRangeException();
            }
        }

        private static void DisplaySingleContact(Contact contact) {
            Console.WriteLine($"\nContact #{contact.Id}\nName: {contact.Name}\nEmail: {contact.Email}\nPhone: {contact.Phone}\n");
        }

        private static void HandleSearch(List<Contact> contacts, string searchOption, string searchTerm) {
            var contactFields = new Dictionary<string, string> {
                { "number", "Id" },
                { "name", "Name" },
                { "email", "Email" },
                { "mobile", "Phone" }
            };


            if (contactFields.TryGetValue(searchOption, out var attribute)) {
                try {
                    List<Contact> filteredContacts = GetContactByAtribute(contacts, attribute, searchTerm);

                    if (filteredContacts.Any()) {
                        DisplayContactsWithPaging(filteredContacts, $"Results for '{searchTerm}' in {searchOption} fields");
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

        private static void ModifyConctacsJson(string path, List<Contact> modifiedContacts, int deletedId) {

            if (deletedId > 0) {
                for (int i = deletedId - 1; i < modifiedContacts.Count; i++) {
                    modifiedContacts[i].Id = i + 1;
                }
            }

            string json = JsonConvert.SerializeObject(modifiedContacts, Formatting.Indented);
            File.WriteAllText(path, json);

            Utils.PrintColoredText("Contacts modified successfully", ConsoleColor.Yellow);
        }

        private static void SaveContacts(string path, Contact contact) {
            List<Contact> contacts = LoadContactsFromJson(path);
            contacts.Add(contact);

            string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            File.WriteAllText(path, json);

            Utils.PrintColoredText("Contact added successfully.", ConsoleColor.Green);
        }

        private static void DisplayContactsWithPaging(List<Contact> contacts, string header) {
            const int contactsPerPage = 5;
            int currentPage = 1;
            (int totalContacts, int totalPages) = GetContactPageSizes(contacts, contactsPerPage);

            if (totalContacts == 0) {
                Utils.ClearConsolePlaceHeader("No contacts available.\nPress any key to return.", ConsoleColor.DarkMagenta);
                Console.ReadKey();
                return;
            }

            while (true) {
                Utils.ClearConsolePlaceHeader($"{header} - Page {currentPage}/{totalPages}");
                PrintContactsFromList(contacts, contactsPerPage, currentPage);

                string input = Utils.GetInput("Enter page number or 0 to return to menu: ");
                if (input == "0" || input == "exit" || input == "confirm") {
                    break;
                }
                else {
                    if (int.TryParse(input, out int selectedPage) && selectedPage >= 1 && selectedPage <= totalPages) {
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
            return contacts.Count == 0 ? 1 : contacts[^1].Id;
        }

        private static void PrintContactsFromList(List<Contact> contacts, int contactsPerPage, int pageNumber) {
            int totalContacts = contacts.Count;
            int start = (pageNumber - 1) * contactsPerPage;
            int end = Math.Min(start + contactsPerPage, totalContacts);

            for (int i = start; i < end; i++) {
                DisplaySingleContact(contacts[i]);
            }

        }

        private static (int, int) GetContactPageSizes(List<Contact> contacts, int contactsPerPage) {
            int totalContacts = contacts.Count;
            int totalPages = (int)Math.Ceiling(totalContacts / (double)contactsPerPage);

            return (totalContacts, totalPages);
        }
        
    }
}
