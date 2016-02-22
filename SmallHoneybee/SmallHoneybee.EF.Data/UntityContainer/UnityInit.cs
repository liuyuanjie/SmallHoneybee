using Microsoft.Practices.Unity;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data.Impl;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;

namespace SmallHoneybee.EF.Data.UntityContainer
{
    public class UnityInit
    {
        public static readonly IUnityContainer UnityContainer = new UnityContainer();

        static UnityInit()
        {
            UnityContainer.RegisterType<SmallHoneybeeEntities, SmallHoneybeeEntities>();
            UnityContainer.RegisterType<IUnitOfWork, UnitOfWork>();
            UnityContainer.RegisterType<IDatabaseContext, DatabaseContext>();
            UnityContainer.RegisterType<IProduceRepository, ProduceRepository>();
            UnityContainer.RegisterType<ICategoryRepository, CategoryRepository>();
            UnityContainer.RegisterType<IPurchaseOrderRepository, PurchaseOrderRepository>();
            UnityContainer.RegisterType<IPOItemRepository, POItemRepository>();
            UnityContainer.RegisterType<ISaleOrderRepository, SaleOrderRepository>();
            UnityContainer.RegisterType<IUserRepository, UserRepository>();
        }

        public static IUnitOfWork UnitOfWork
        {
            get { return UnityContainer.Resolve<UnitOfWork>(); }
        }
    }
}
