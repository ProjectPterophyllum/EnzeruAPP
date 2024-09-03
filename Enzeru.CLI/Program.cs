using System;
using EnzeruAPP.Enzeru.DBManager;

namespace Enzeru.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            DBManager.InitializeDatabase();
        }
    }
}