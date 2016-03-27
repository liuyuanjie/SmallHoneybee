using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmallHoneybee.Common;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;

namespace SmallHoneybee.Wpf.Views.Shared
{
    /// <summary>
    /// Interaction logic for InputPassword.xaml
    /// </summary>
    public partial class InputPassword : Window
    {
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;
        private IMemberCardRepository _memberCardRepository;

        private int _memberCardId;

        public bool IsCheckOK = false;
        public InputPassword(int memberCardId)
        {
            _memberCardId = memberCardId;

            _unitOfWork = UnityInit.UnitOfWork;
            _userRepository = _unitOfWork.GetRepository<UserRepository>();
            _memberCardRepository = _unitOfWork.GetRepository<MemberCardRepository>();

            InitializeComponent();
            txtPassword.Focus();
        }

        private void TxtPassword_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var memberCard = _memberCardRepository.Query()
                    .FirstOrDefault(x => x.MemberCardId == _memberCardId);
                if (memberCard != null &&
                    SaltedHash.Create(memberCard.PasswordSalt, memberCard.PasswordHash).Verify(txtPassword.Password))
                {
                    IsCheckOK = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("密码不正确!", Properties.Resources.SystemName, MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPassword.Clear();
                }
            }
        }
    }
}
