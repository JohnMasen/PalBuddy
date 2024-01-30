using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalBuddy.Core
{
    public class InvalidConfigfileException:ApplicationException
    {
        public InvalidConfigfileException(string message):base(message)
        {
        }
    }
}
