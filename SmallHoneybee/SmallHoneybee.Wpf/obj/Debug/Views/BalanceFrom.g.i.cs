﻿#pragma checksum "..\..\..\Views\BalanceFrom.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B5851712A4A30C31D02C6955D452EC4F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
    /// BalanceFrom
    /// </summary>
    public partial class BalanceFrom : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox UserNoOrPhone;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton RadBanlanceMode;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtDiscount;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtRealPrice;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtDeduct;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButSure;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\Views\BalanceFrom.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/SmallHoneybee.Wpf;component/views/balancefrom.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Views\BalanceFrom.xaml"
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
            
            #line 5 "..\..\..\Views\BalanceFrom.xaml"
            ((SmallHoneybee.Wpf.Views.BalanceFrom)(target)).Closed += new System.EventHandler(this.BalanceFrom_OnClosed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.UserNoOrPhone = ((System.Windows.Controls.TextBox)(target));
            
            #line 28 "..\..\..\Views\BalanceFrom.xaml"
            this.UserNoOrPhone.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.UserNoOrPhone_OnTextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.RadBanlanceMode = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 4:
            this.TxtDiscount = ((System.Windows.Controls.TextBox)(target));
            
            #line 61 "..\..\..\Views\BalanceFrom.xaml"
            this.TxtDiscount.KeyDown += new System.Windows.Input.KeyEventHandler(this.TxtDiscount_OnKeyDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TxtRealPrice = ((System.Windows.Controls.TextBox)(target));
            
            #line 74 "..\..\..\Views\BalanceFrom.xaml"
            this.TxtRealPrice.KeyDown += new System.Windows.Input.KeyEventHandler(this.TxtRealPrice_OnKeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.TxtDeduct = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.ButSure = ((System.Windows.Controls.Button)(target));
            
            #line 103 "..\..\..\Views\BalanceFrom.xaml"
            this.ButSure.Click += new System.Windows.RoutedEventHandler(this.ButSure_OnClick);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ButCancel = ((System.Windows.Controls.Button)(target));
            
            #line 104 "..\..\..\Views\BalanceFrom.xaml"
            this.ButCancel.Click += new System.Windows.RoutedEventHandler(this.ButCancel_OnClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

