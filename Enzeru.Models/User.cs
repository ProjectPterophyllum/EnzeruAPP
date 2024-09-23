using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnzeruAPP.Enzeru.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }  // Имя пользователя

        public User() { }

        public User(int id, string username)
        {
            ID = id;
            Username = username;
        }
    }
}