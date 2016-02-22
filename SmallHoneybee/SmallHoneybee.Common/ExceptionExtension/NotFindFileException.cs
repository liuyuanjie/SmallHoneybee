using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.Common.ExceptionExtension
{
    public class NotFindFileException : Exception
    {
        public NotFindFileException(string message="Cannot find the file.")
            :base(message)
        {
        }
    }
}
