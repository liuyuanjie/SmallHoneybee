using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.UpdateDataModelEdmx
{
    class Program
    {
        static void Main(string[] args)
        {
            string content = string.Empty;


            using (StreamReader tr = new StreamReader(new FileStream(
                Path.Combine(ConfigurationManager.AppSettings["edmxPath"], "Model.edmx.diagram"), FileMode.Open, FileAccess.Read)))
            {
                content = tr.ReadToEnd();
            }

            using (StreamWriter tw = new StreamWriter(new FileStream(
                Path.Combine(ConfigurationManager.AppSettings["edmxPath"], "Model.edmx.diagram"), FileMode.Open, FileAccess.Write)))
            {
                tw.Write(ReplaceContent(content));
            }

            using (StreamReader tr = new StreamReader(new FileStream(
                Path.Combine(ConfigurationManager.AppSettings["edmxPath"], "Model.edmx"), FileMode.Open, FileAccess.Read)))
            {
                content = tr.ReadToEnd();
            }

            using (StreamWriter tw = new StreamWriter(new FileStream(
                Path.Combine(ConfigurationManager.AppSettings["edmxPath"], "Model.edmx"), FileMode.Open, FileAccess.Write)))
            {
                tw.Write(ReplaceContent(content));
            }

        }

        static string ReplaceContent(string fileContent)
        {
            return fileContent
                .Replace("<NavigationProperty Name=\"user1\" Relationship=\"Self.FK_Produce_SaleOrder_PurchaseOrderUserId_UserId\" FromRole=\"saleorder\" ToRole=\"user\" />",
                    "<NavigationProperty Name=\"PrucahseOrderUserUser\" Relationship=\"Self.FK_Produce_SaleOrder_PurchaseOrderUserId_UserId\" FromRole=\"saleorder\" ToRole=\"user\" />")
                .Replace("<NavigationProperty Name=\"user\" Relationship=\"Self.FK_Produce_SaleOrder_OriginUserId_UserId\" FromRole=\"saleorder\" ToRole=\"user\" />",
                    "<NavigationProperty Name=\"OriginUserUser\" Relationship=\"Self.FK_Produce_SaleOrder_OriginUserId_UserId\" FromRole=\"saleorder\" ToRole=\"user\" />")
                .Replace("<NavigationProperty Name=\"saleorders\" Relationship=\"Self.FK_Produce_SaleOrder_OriginUserId_UserId\" FromRole=\"user\" ToRole=\"saleorder\" />",
                    "<NavigationProperty Name=\"SaleOrdersOriginUser\" Relationship=\"Self.FK_Produce_SaleOrder_OriginUserId_UserId\" FromRole=\"user\" ToRole=\"saleorder\" />")
                .Replace("<NavigationProperty Name=\"saleorders1\" Relationship=\"Self.FK_Produce_SaleOrder_PurchaseOrderUserId_UserId\" FromRole=\"user\" ToRole=\"saleorder\" />",
                   "<NavigationProperty Name=\"SaleOrdersPurchaseOrderUser\" Relationship=\"Self.FK_Produce_SaleOrder_PurchaseOrderUserId_UserId\" FromRole=\"user\" ToRole=\"saleorder\" />")
                                .Replace("soproduce", "SOProduce")

                .Replace("<NavigationProperty Name=\"user\" Relationship=\"Self.FK_MemberCard_User_DispatchUserId_UserId\" FromRole=\"membercard\" ToRole=\"user\" />",
                    "<NavigationProperty Name=\"DispatchUserUser\" Relationship=\"Self.FK_MemberCard_User_DispatchUserId_UserId\" FromRole=\"membercard\" ToRole=\"user\" />")
                .Replace("<NavigationProperty Name=\"user1\" Relationship=\"Self.FK_MemberCard_User_RelateUserId_UserId\" FromRole=\"membercard\" ToRole=\"user\" />",
                    "<NavigationProperty Name=\"RelateUserUser\" Relationship=\"Self.FK_MemberCard_User_RelateUserId_UserId\" FromRole=\"membercard\" ToRole=\"user\" />")
                .Replace("<NavigationProperty Name=\"membercards\" Relationship=\"Self.FK_MemberCard_User_DispatchUserId_UserId\" FromRole=\"user\" ToRole=\"membercard\" />",
                    "<NavigationProperty Name=\"membercardsDispatchUser\" Relationship=\"Self.FK_MemberCard_User_DispatchUserId_UserId\" FromRole=\"user\" ToRole=\"membercard\" />")
                .Replace("<NavigationProperty Name=\"membercards1\" Relationship=\"Self.FK_MemberCard_User_RelateUserId_UserId\" FromRole=\"user\" ToRole=\"membercard\" />",
                   "<NavigationProperty Name=\"membercardsRelateUser\" Relationship=\"Self.FK_MemberCard_User_RelateUserId_UserId\" FromRole=\"user\" ToRole=\"membercard\" />")
                
                   .Replace("soproduce", "SOProduce")
                .Replace("categor", "Categor")
                .Replace("daybook", "DayBook")
                .Replace("membercardlog", "MemberCardLog")
                .Replace("membercard", "MemberCard")

                .Replace("poitem", "POItem")
                .Replace("produce", "Produce")
                .Replace("producelog", "ProduceLog")
                .Replace("purchaseorder", "PurchaseOrder")
                .Replace("saleorder", "SaleOrder")
                .Replace("solog", "SOLog")

                .Replace("systemsetting", "SystemSetting")
                .Replace("user", "User")
                .Replace("userlog", "UserLog")
                .Replace("usermembercard", "UserMemberCard")
                .Replace("<Property Name=\"RowVersion\" Type=\"timestamp\" Precision=\"0\" Nullable=\"false\" />",
                    "<Property Name=\"RowVersion\" Type=\"timestamp\" Precision=\"0\" Nullable=\"false\"  StoreGeneratedPattern=\"Computed\" />")
                .Replace("<Property Name=\"RowVersion\" Type=\"DateTime\" Nullable=\"false\" />",
                    "<Property Name=\"RowVersion\" Type=\"DateTime\" Nullable=\"false\" annotation:StoreGeneratedPattern=\"Computed\" />")
                                ;
        }
    }
}
