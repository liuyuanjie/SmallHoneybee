//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SmallHoneybee.DataModel.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Producelog
    {
        public long ProduceLogId { get; set; }
        public int ProduceId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTime DateChanged { get; set; }
        public string ChangedBy { get; set; }
        public System.DateTime RowVersion { get; set; }
    
        public virtual Produce Produce { get; set; }
    }
}
