using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Reporting.WinForms;

namespace SmallHoneybee.Wpf.Report
{
    public class ReportPrint : IDisposable
    {
        private int _currentPageIndex;
        private IList<Stream> _streams;

        readonly string _printerName = "SmallHoneybee";
        const string _deviceInfo = "<DeviceInfo>" +
                          "  <OutputFormat>EMF</OutputFormat>" +
                          "  <PageWidth>8.5in</PageWidth>" +
                          "  <PageHeight>11in</PageHeight>" +
                          "  <MarginTop>0.25in</MarginTop>" +
                          "  <MarginLeft>0.25in</MarginLeft>" +
                          "  <MarginRight>0.25in</MarginRight>" +
                          "  <MarginBottom>0.25in</MarginBottom>" +
                          "</DeviceInfo>";

        public ReportPrint(string printName)
        {
            _printerName = printName;
        }

        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new FileStream(String.Format(@"..\..\{0}.{1}", name, fileNameExtension), FileMode.Create);
            _streams.Add(stream);
            return stream;
        }

        private void Export(LocalReport report)
        {
            Warning[] warnings;
            _streams = new List<Stream>();
            report.Render("Image", _deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in _streams)
            {
                stream.Position = 0;
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new Metafile(_streams[_currentPageIndex]);

            ev.Graphics.DrawImage(pageImage, ev.PageBounds);
            _currentPageIndex++;
            ev.HasMorePages = (_currentPageIndex < _streams.Count);
        }

        private void Print()
        {
            if (_streams == null || _streams.Count == 0)
            {
                return;
            }

            PrintDocument printDoc = new PrintDocument
            {
                PrinterSettings = { PrinterName = _printerName }
            };

            if (!printDoc.PrinterSettings.IsValid)
            {
                MessageBox.Show(String.Format("打印失败，没有找到打印机 \"{0}\".", _printerName), Properties.Resources.SystemName);
                return;
            }
            printDoc.PrintPage += PrintPage;
            printDoc.Print();
        }

        public void Run(SaleOrderReportModel saleOrderReportModel)
        {
            using (LocalReport report = new LocalReport())
            {
                report.ReportPath = @"Report\SaleOrderDetail.rdlc";
                report.DataSources.Add(
                    new ReportDataSource("SOProuceDetailDataSet", saleOrderReportModel.SOProucedDetailReportModels));
                report.DataSources.Add(
                    new ReportDataSource("SOProduceSummaryDataSet", saleOrderReportModel.SOProuceSummaryReportModels));
                Export(report);
            }

            _currentPageIndex = 0;
            Print();
        }

        public void Dispose()
        {
            if (_streams != null)
            {
                foreach (Stream stream in _streams)
                {
                    stream.Close();
                }
                _streams = null;
            }
        }
    }
}
