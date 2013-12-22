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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.IO;
using System.Data.SqlServerCe;
using System.Security.Cryptography;

namespace Q3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SqlCeConnection cn = null;

        public MainWindow()
        {
            InitializeComponent();
            //testc();
        }

        private void testc()
        {
            try
            {
                string connectionSting = ConfigurationManager.ConnectionStrings["Q3.Properties.Settings.QuestionsDbConnectionString"].ConnectionString;
                //MessageBox.Show(connectionSting);
                cn = new SqlCeConnection(connectionSting);
                //cn.Open();
            }
            catch (SqlCeException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SelectUserComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectUserCombobox.SelectedIndex == 0)
            {
                try
                {
                    AttemptToQuisGrid.Visibility = Visibility.Visible;
                    AdminLogInGrid.Visibility = Visibility.Hidden;
                }catch(Exception ex)
                {
                }
            }
            else
            {
                try
                {
                    AttemptToQuisGrid.Visibility = Visibility.Hidden;
                    AdminLogInGrid.Visibility = Visibility.Visible;
                }
                catch (Exception ex) 
                {
                }
            }
        }

        private void AttemptTOQuisButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!ValidateIDNumber(NICNumberTB.Text)) return;
            if (NameTB.Text == null || NameTB.Text == String.Empty) return;

            CRUDManager CRUDM = new CRUDManager();
            if (CRUDM.AddNewUser(NameTB.Text, NICNumberTB.Text))
            {
                UserWindow user = new UserWindow(NameTB.Text, NICNumberTB.Text);
                user.Show();
                NICNumberTB.Text = "";
                NameTB.Text = "";
                //this.Close();
            }
            else
            {
                //MessageBox.Show("");
            }
        }

        private void LoginButtonClicked(object sender, RoutedEventArgs e)
        {

            if (PasswordAdmin.Password == null || PasswordAdmin.Password == String.Empty) return;
            if (UserNameAdminTB.Text == null || UserNameAdminTB.Text == String.Empty) return;
            CRUDManager CRUDM = new CRUDManager();

            #region add new admin code
            //PasswordHash hash1 = new PasswordHash(PasswordAdmin.Password);
            //byte[] hashBytes1 = hash1.ToArray();
            //var str = System.Text.Encoding.Default.GetString(hashBytes1);

            //bool IsAdminAdded = CRUDM.AddNewAdmin(UserNameAdminTB.Text.Trim(), str);
            #endregion
            
            User AdminInfo = CRUDM.GetAdminLoginDetails(UserNameAdminTB.Text);//read from store.

            if (AdminInfo == null || AdminInfo.Password == null) { MessageBox.Show("පරිපාලකයාගේ නම නිවැරදි කරන්න​"); return; }// if user name does not match login failed

            byte[] hashBytes = System.Text.Encoding.Default.GetBytes(AdminInfo.Password);
            PasswordHash hash = new PasswordHash(hashBytes);
            if (hash.Verify(PasswordAdmin.Password))
            {
                AdminWindow Admin = new AdminWindow();
                Admin.Show();
                UserNameAdminTB.Text = "";
                PasswordAdmin.Password = "";
                //this.Close();
            }
            else 
            {
                MessageBox.Show("රහස් පදය නිවැරදි කරන්න​");
            }
        }

        #region Internal Methods

        private bool ValidateIDNumber(string str)
        {
            if ((str.Count(char.IsDigit) == 9) &&
                    (str.EndsWith("X", StringComparison.OrdinalIgnoreCase)
                    || str.EndsWith("V", StringComparison.OrdinalIgnoreCase)) &&
                    (str[2] != '4' && str[2] != '9'))
            {
                return true;
            }
            else
            {
                MessageBox.Show("ජා. හැ. අ. නිවැරදි කරන්න​");
                return false;
            }
        }

        #endregion
    }

    #region Inner Class for Password Encription

    public sealed class PasswordHash
    {
        const int SaltSize = 16, HashSize = 20, HashIter = 10000;
        readonly byte[] _salt, _hash;

        public PasswordHash(string password)
        {
            new RNGCryptoServiceProvider().GetBytes(_salt = new byte[SaltSize]);
            _hash = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
        }

        public PasswordHash(byte[] hashBytes)
        {
            Array.Copy(hashBytes, 0, _salt = new byte[SaltSize], 0, SaltSize);
            Array.Copy(hashBytes, SaltSize, _hash = new byte[HashSize], 0, HashSize);
        }

        public PasswordHash(byte[] salt, byte[] hash)
        {
            Array.Copy(salt, 0, _salt = new byte[SaltSize], 0, SaltSize);
            Array.Copy(hash, 0, _hash = new byte[HashSize], 0, HashSize);
        }

        public byte[] ToArray()
        {
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(_salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(_hash, 0, hashBytes, SaltSize, HashSize);
            return hashBytes;
        }

        public byte[] Salt { get { return (byte[])_salt.Clone(); } }

        public byte[] Hash { get { return (byte[])_hash.Clone(); } }

        public bool Verify(string password)
        {
            byte[] test = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
            for (int i = 0; i < HashSize; i++)
                if (test[i] != _hash[i])
                    return false;
            return true;
        }
    }

    #endregion
    
}
