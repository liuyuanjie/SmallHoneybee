﻿#pragma checksum "..\..\..\Views\SaleOrder.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2FF59453350A1C94B11EB56B966FED09"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SmallHoneybee.Wpf.Properties;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SmallHoneybee.Wpf.Views {
    
    
    /// <summary>
    /// SaleOrder
    /// </summary>
    public partial class SaleOrder : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 46 "..\..\..\Views\SaleOrder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtSearchBox;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\Views\SaleOrder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid GridSaleOrders;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\..\Views\SaleOrder.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid GridSOProduces;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SmallHoneybee.Wpf;component/views/saleorder.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\SaleOrder.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TxtSearchBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            
            #line 47 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnSearch_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 48 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnClear_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 49 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.GridSaleOrders = ((System.Windows.Controls.DataGrid)(target));
            
            #line 54 "..\..\..\Views\SaleOrder.xaml"
            this.GridSaleOrders.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.GridSaleOrders_OnSelectionChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.GridSOProduces = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 9:
            
            #line 186 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CommandBinding_ClearSearchText_CanExecute);
            
            #line default
            #line hidden
            
            #line 187 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandBinding_ClearSearchText_Executed);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 189 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CommandBinding_ExecuteSearchText_CanExecute);
            
            #line default
            #line hidden
            
            #line 190 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandBinding_ExecuteSearchText_Executed);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 6:
            
            #line 59 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Controls.Label)(target)).MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ButEditSaleOrder_Click);
            
            #line default
            #line hidden
            break;
            case 7:
            
            #line 70 "..\..\..\Views\SaleOrder.xaml"
            ((System.Windows.Controls.Label)(target)).MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.ButDeleteSaleOrder_Click);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

