using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerAuthentication
{
    public class Output
    {
        public static void Message(OutputType type, dynamic message) =>
            Console.WriteLine(string.Format("[LoginAuthentication][{0}] - {1}",
                Enum.GetName(type.GetType(), type), message));

    }

    public enum OutputType
    {
        Info = 1,
        Error = 2,
        Debug = 3
    }
}
