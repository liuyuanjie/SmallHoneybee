using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for UserWithMemberCard.xaml
    /// </summary>
    public partial class ProduceLog : Window
    {
        private IUnitOfWork _unitOfWork;
        private IProduceRepository _produceRepository;

        public ProduceLog(int produceId)
        {
            InitializeComponent();

            _unitOfWork = UnityInit.UnitOfWork;
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();

            DataContext = new
            {
                ProduceLogs = _produceRepository.GetByProduceId(produceId).Producelogs.OrderByDescending(x => x.DateChanged)
            };
        }
    }
}
