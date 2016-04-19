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
    public partial class UserLog : Window
    {
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;

        public UserLog(int userId)
        {
            InitializeComponent();

            _unitOfWork = UnityInit.UnitOfWork;
            _userRepository = _unitOfWork.GetRepository<UserRepository>();

            DataContext = new
            {
                UserLogs = _userRepository.GetByUserId(userId).Userlogs.OrderByDescending(x => x.DateChanged)
            };
        }
    }
}
