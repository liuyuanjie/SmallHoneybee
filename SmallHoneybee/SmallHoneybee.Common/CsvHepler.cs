using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using SmallHoneybee.Common.ExceptionExtension;

namespace SmallHoneybee.Common
{
    public class CsvHepler
    {
        public static string Delimiter = ",";
        public static IEnumerable<IEnumerable<KeyValuePair<string, string>>> GetCsvData(string filePath)
        {
            List<List<KeyValuePair<string, string>>> pageContents = new List<List<KeyValuePair<string, string>>>();
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new NotFindFileException();
                }

                CsvConfiguration csvConfig = new CsvConfiguration
                {
                    Delimiter = Delimiter,
                    Encoding = Encoding.UTF8
                };

                List<string> lineHeaders = new List<string>();
                using (StreamReader reader = new StreamReader(File.OpenRead(filePath), Encoding.UTF8))
                using (CsvReader csv = new CsvReader(reader, csvConfig))
                {
                    csv.Read();
                    lineHeaders = csv.FieldHeaders.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    do
                    {
                        List<KeyValuePair<string, string>> content = new List<KeyValuePair<string, string>>();
                        lineHeaders.ForEach(x =>
                        {
                            content.Add(new KeyValuePair<string, string>(x, csv[x].Trim()));
                        });

                        pageContents.Add(content);
                    } while (csv.Read());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pageContents;
        }
    }
}
