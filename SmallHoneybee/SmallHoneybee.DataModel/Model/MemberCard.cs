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
    
    public partial class MemberCard
    {
        public MemberCard()
        {
            this.MemberCardlogs = new HashSet<MemberCardLog>();
        }
    
        public int MemberCardId { get; set; }
        public string MemberCardNo { get; set; }
        public string Name { get; set; }
        public sbyte MemberType { get; set; }
        public float MemberMoney { get; set; }
        public System.DateTime DispatchDate { get; set; }
        public Nullable<int> DispatchUserId { get; set; }
        public float PrincipalSurplusMoney { get; set; }
        public bool IsEnable { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordHash { get; set; }
        public sbyte MemberCardStatus { get; set; }
        public string Descript { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public System.DateTime RowVersion { get; set; }
        public float TotalSurplusMoney { get; set; }
        public float FavorableSurplusMoney { get; set; }
        public Nullable<int> RelateUserId { get; set; }
    
        public virtual User DispatchUserUser { get; set; }
        public virtual User RelateUserUser { get; set; }
        public virtual ICollection<MemberCardLog> MemberCardlogs { get; set; }
    }
}
