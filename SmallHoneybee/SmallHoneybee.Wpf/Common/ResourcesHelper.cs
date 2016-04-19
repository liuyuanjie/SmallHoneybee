using System.Collections.Generic;

using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.Wpf.Properties;

namespace SmallHoneybee.Wpf.Common
{
    public class ResourcesHelper
    {
        public static string MFTMemberCard = "MFT.";
        public static string SystemName = Resources.SystemName;
        public const string ReRunSystemError = "系统已经在运行,请先停止当前运行系统!";
        public static User CurrentUser;
        public static Dictionary<short, string> SystemSettings = new Dictionary<short, string>();
        public static string CoustomUserNoStart = "900000";

        public static string MemberCardLogFormat = "充值方式: {0}, 会员: {1}, 充值金额: {2}, 赠送金额: {3}, 当前总金额: {4}";
        public static string MemberCardLogUserFormat = "会员: {0}与购物卡: {1}绑定成功, 初始金额: {2}";
        public static string UserLogMemberCardFormat = "充值方式: {0}, 充值金额: {1}, 赠送金额: {2}, 当前总金额: {3}";

        public static string ProduceLogSaleOrderFormat = "消费单据: {0}, 本次扣除数量: {1}, 本次消费价格: {2}, 当前剩余数量: {3}";
        public static string UserLogSaleOrderFormat = "消费单据: {0}, 结算方式: {1}, 本次消费: {2}, 本次累计积分: {3}, 当前剩余金额: {4}, 当前累计积分: {5}";
        public static string MemberCardLogSaleOrderFormat = "消费单据: {0}, 结算方式: {1}, 本次消费: {2}, 本次累计积分: {3}, 当前剩余金额: {4}, 当前累计积分: {5}";
        public static string PurchaseOrderImporterFormat = "进货单据: {0}, 本次进货数量: {1}, 本次进货价格: {2}, 总计数量: {3}";

        public static RolePermission CurrentUserRolePermission;

    }
}
