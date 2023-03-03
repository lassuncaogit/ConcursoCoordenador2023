using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Interfaces;

namespace ClienteWinForms
{

    
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
           
        }
    }
}
