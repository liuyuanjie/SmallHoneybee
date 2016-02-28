using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow()
        {
            InitializeComponent();
        }

        private readonly SaleOrder _saleOrder;
        private BindingSource _partBindingSource;
        private IContainer mform_components;

        public PrintWindow(SaleOrder saleOrder)
        {
            _saleOrder = saleOrder;
            InitializeComponent();
            //InitializeReport();  
        }

        //private void InitializeReport()
        //{
        //    mform_components = new Container();  
        //    ReportDataSource reportDataSource = new ReportDataSource();

        //    _partBindingSource = new BindingSource(mform_components);
        //    ((ISupportInitialize)(_partBindingSource)).BeginInit();

        //    reportDataSource.Name = "PartDataSet";
        //    reportDataSource.Value = _partBindingSource;

        //    _reportViewer.LocalReport.DataSources.Add(reportDataSource);
        //    _reportViewer.LocalReport.ReportEmbeddedResource = "PartExpress.Reports.Part.rdlc";

        //    ((ISupportInitialize)(_partBindingSource)).EndInit();  
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //_partBindingSource.DataSource = _parts;
        //    //_reportViewer.RefreshReport();
        //}
    //}
}
