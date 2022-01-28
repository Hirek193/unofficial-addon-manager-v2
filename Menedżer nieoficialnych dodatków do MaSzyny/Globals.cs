using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace Menedżer_nieoficialnych_dodatków_do_MaSzyny
{
    public static class Globals
    {
        private static Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static string version = String.Format("{0}.{1}.{2}", assemblyVersion.Major,
            assemblyVersion.Minor, assemblyVersion.Revision);
        public static string api = "127.0.0.1:20706";
        public static string simDir;
    }
}
