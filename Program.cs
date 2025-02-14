using System.Drawing;
namespace Capture
{
    public static class Program
    {
        public static Form1 form;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            form = new Form1();
            Application.Run(form);
        }
    }
}
