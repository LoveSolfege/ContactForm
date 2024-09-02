using System.Text.RegularExpressions;

namespace ContactForm.Models
{

    internal class Contact
    {

        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        public int Id { get;  set; }


        public Contact(int id, string name, string email, string phone) {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be empty.");
                }

                if (value.Length < 2 || value.Length > 20)
                {
                    throw new ArgumentException("Name must be between 2 and 20 characters long.");
                }

                _name = value;
            }
        }


        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Email cannot be empty.");
                }

                if (!IsValidEmail(value))
                {
                    throw new ArgumentException("Email is invalid.");
                }

                _email = value;
            }
        }



        public string Phone
        {
            get => _phone;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Email cannot be empty.");
                }

                if (!IsValidPhone(value))
                {
                    throw new ArgumentException("Phone number is invalid.");
                }

                _phone = value;
            }
        }

        private bool IsValidEmail(string email)
        {
            string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, EmailPattern);
        }

        private bool IsValidPhone(string phone)
        {
            string NumberPattern = @"^\+?[1-9]\d{1,12}$";
            return Regex.IsMatch(phone, NumberPattern);
        }

    }

}
