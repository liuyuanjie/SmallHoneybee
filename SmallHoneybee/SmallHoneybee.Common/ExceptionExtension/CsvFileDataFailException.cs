using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.Common.ExceptionExtension
{
    public class CsvFileDataFailException : Exception
    {
        public CsvFileDataFailException(string message = "Get the csv data fail.")
            :base(message)
        {
        }
    }
}
