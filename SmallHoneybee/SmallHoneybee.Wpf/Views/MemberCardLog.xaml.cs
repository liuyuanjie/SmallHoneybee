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
    public partial class MemberCardLog : Window
    {
        private IUnitOfWork _unitOfWork;
        private IMemberCardRepository _memberCardRepository;

        public MemberCardLog(int memberCardId)
        {
            InitializeComponent();

            _unitOfWork = UnityInit.UnitOfWork;
            _memberCardRepository = _unitOfWork.GetRepository<MemberCardRepository>();

            DataContext = new
            {
                MemberCardLogs = _memberCardRepository.GetByMemberCardId(memberCardId).MemberCardlogs.OrderByDescending(x => x.DateChanged)
            };
        }
    }
}
