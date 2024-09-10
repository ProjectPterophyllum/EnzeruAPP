using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enzeru.Models
{
    public class User
    {
        public int ID { get; set; }
        private string _username;
        private string _password;
        public string Username
        {
            get { return _username; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Username cannot be empty.");
                }
                _username = value;
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Password cannot be empty.");
                }
                _password = value;
            }
        }
    }
}