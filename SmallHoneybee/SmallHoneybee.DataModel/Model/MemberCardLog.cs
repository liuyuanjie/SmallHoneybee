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
    
    public partial class MemberCardLog
    {
        public int MemberCardLogId { get; set; }
        public int MemberCardId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTime DateChanged { get; set; }
        public string ChangedBy { get; set; }
        public System.DateTime RowVersion { get; set; }
        public Nullable<sbyte> LogType { get; set; }
        public sbyte HowBalance { get; set; }
        public float FavorableMoney { get; set; }
        public float PrincipalMoney { get; set; }
    
        public virtual MemberCard MemberCard { get; set; }
    }
}
