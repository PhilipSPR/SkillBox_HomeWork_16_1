using SkillBox_HomeWork_16_1.Models;
using SkillBox_HomeWork_16_1.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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

namespace SkillBox_HomeWork_16_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MsSqlRepository _usersRepository = new MsSqlRepository("TestDB", "Users");
        private readonly MsSqlRepository _productRepository = new MsSqlRepository("SecondDB", "Product");
        DataRowView _rowUsers;
        DataRowView _rowProduct;

        public MainWindow()
        {
            InitializeComponent();

            Prepaire();
        }

        private void Prepaire()
        {

            ConnectToMSSQL.Content = "Подключиться к \n MSSQL";
            ConnectToMSAccess.Content = "Подключиться к \n MSSQL (2)";

            _usersRepository.StateChanged += MsSqlRepository_StateChanged;
            _productRepository.StateChanged += MsSqlRepository2_StateChanged;
            UsersSource.ItemsSource = _usersRepository.DataTable.DefaultView;
            ProductSource.ItemsSource = _productRepository.DataTable.DefaultView;

            //UsersSource.SelectionChanged += UsersSource_SelectionChanged;

            SetSqlUsersCommands();
            SetSqlProductCommands();
        }

        private void UsersSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_productRepository.State.Equals(State.Online))
            {

                var row = (UsersSource.SelectedItem as DataRowView);
                var email = row.Row["Email"].ToString();
            }
        }

        private void MsSqlRepository_StateChanged(State state)
        {
            MsSqlStatus.Text = state.Equals(State.Offline) ? "Офлайн" : "Онлайн";
        }

        private async void ConnectToMSSQL_Click(object sender, RoutedEventArgs e)
        {
            BaseResponse response;
            if (_usersRepository.State.Equals(State.Offline))
            {
                response = await _usersRepository.Start();
                if (response.Succes)
                {
                    ConnectToMSSQL.Content = "Отключиться от \n MSSQL";
                }
                else
                {
                    MessageBox.Show(response.Message,
                                    "Ошибка подключения к базе данных",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            else
            {
                _usersRepository.Dispose();
                ConnectToMSSQL.Content = "Подключиться к \n MSSQL";
            }
        }

        private void MsSqlRepository2_StateChanged(State state)
        {
            MsAccessStatus.Text = state.Equals(State.Offline) ? "Офлайн" : "Онлайн";
        }

        private async void ConnectToMSAccess_Click(object sender, RoutedEventArgs e)
        {
            BaseResponse response;
            if (_productRepository.State.Equals(State.Offline))
            {
                response = await _productRepository.Start();
                if (response.Succes)
                    ConnectToMSAccess.Content = "Отключиться от \n MSSQL (2)";
                else
                {
                    MessageBox.Show(response.Message,
                                    "Ошибка подключения к базе данных",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            else
            {
                _productRepository.Dispose();
                ConnectToMSAccess.Content = "Подключиться к \n MSSQL (2)";
            }

        }

        private void UsersSource_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _rowUsers = (DataRowView)UsersSource.SelectedItem;
            _rowUsers.BeginEdit();
        }

        private void SetSqlProductCommands()
        {
            SqlDataAdapter da = new SqlDataAdapter();

            da.SelectCommand = new SqlCommand(@"SELECT * FROM Product ORDER BY Product.Id");
            #region INSERT
            da.InsertCommand = new SqlCommand(@"INSERT INTO dbo.Product VALUES (
                                            @email, 
                                            @productCode,
                                            @name)");

            da.InsertCommand.Parameters.Add("@email", SqlDbType.NVarChar, 20, "Email");
            da.InsertCommand.Parameters.Add("@productCode", SqlDbType.Int, 0, "ProductCode");
            da.InsertCommand.Parameters.Add("@name", SqlDbType.NVarChar, 20, "Name");
            #endregion
            
            #region UPDATE
            da.UpdateCommand = new SqlCommand(@"UPDATE Product SET
                                              Email = @email,
                                              ProductCode = @productCode,
                                              Name = @name
                                              WHERE ID = @id");

            da.UpdateCommand.Parameters.Add("@id", SqlDbType.Int, 0, "Id").SourceVersion = DataRowVersion.Original;
            da.UpdateCommand.Parameters.Add("@email", SqlDbType.NVarChar, 30, "Email");
            da.UpdateCommand.Parameters.Add("@productCode", SqlDbType.Int, 0, "ProductCode");
            da.UpdateCommand.Parameters.Add("@name", SqlDbType.NVarChar, 15, "Name");
            #endregion

            #region DELETE
            da.DeleteCommand = new SqlCommand("DELETE FROM Product " +
                                                "WHERE ID = @id");

            da.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 0, "Id").SourceVersion = DataRowVersion.Original;
            #endregion

            _productRepository.DataAdapter = da;
        }
        private void SetSqlUsersCommands()
        {
            SqlDataAdapter da = new SqlDataAdapter();

            da.SelectCommand = new SqlCommand(@"SELECT * FROM Users ORDER BY Users.Id");
            #region Update
            da.UpdateCommand = new SqlCommand(@"UPDATE Users SET 
                           LastName = @lastName,
                           FirstName = @firstName, 
                           MiddleName = @middleName, 
                           PhoneNumber = @phoneNumber, 
                           Email = @email 
                    WHERE Id = @id");
            da.UpdateCommand.Parameters.Add("@id", SqlDbType.Int, 0, "Id").SourceVersion = DataRowVersion.Original;
            da.UpdateCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 20, "LastName");
            da.UpdateCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 20, "FirstName");
            da.UpdateCommand.Parameters.Add("@middleName", SqlDbType.NVarChar, 20, "MiddleName");
            da.UpdateCommand.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 20, "PhoneNumber");
            da.UpdateCommand.Parameters.Add("@email", SqlDbType.NVarChar, 20, "Email");
            #endregion

            #region INSERT
            da.InsertCommand = new SqlCommand("INSERT INTO Users VALUES" +
                                            "(@lastName, " +
                                            "@firstName, " +
                                            "@middleName, " +
                                            "@phoneNumber, " +
                                            "@email)");

            da.InsertCommand.Parameters.Add("@lastName", SqlDbType.NVarChar, 20, "LastName");
            da.InsertCommand.Parameters.Add("@firstName", SqlDbType.NVarChar, 20, "FirstName");
            da.InsertCommand.Parameters.Add("@middleName", SqlDbType.NVarChar, 20, "MiddleName");
            da.InsertCommand.Parameters.Add("@phoneNumber", SqlDbType.NVarChar, 20, "PhoneNumber");
            da.InsertCommand.Parameters.Add("@email", SqlDbType.NVarChar, 20, "Email");
            #endregion

            #region DELETE
            da.DeleteCommand = new SqlCommand("DELETE FROM Users " +
                "WHERE ID = @id");

            da.DeleteCommand.Parameters.Add("@id", SqlDbType.Int, 0, "Id").SourceVersion = DataRowVersion.Original;
            #endregion
            _usersRepository.DataAdapter = da;
        }

        private void UsersSource_CurrentCellChanged(object sender, EventArgs e)
        {
            if (_rowUsers is null)
                return;

            _rowUsers.EndEdit();
            var response = _usersRepository.Update();

            if (!response.Succes)
                MessageBox.Show($"При обновлении базы данных пользовтаелей возникла ошибка: {response.Message}",
                    "При изменении данных возникла ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        private void MenuUserAddClick(object sender, RoutedEventArgs e)
        {
            DataRow row = _usersRepository.DataTable.NewRow();
            AddUserWindows add = new AddUserWindows(row);
            add.ShowDialog();


            if (add.DialogResult.Value)
            {
                _usersRepository.DataTable.Rows.Add(row);
                _usersRepository.Update();
            }
        }

        private void MenuUserDeleteClick(object sender, RoutedEventArgs e)
        {
            _usersRepository.DataRowView = (DataRowView)UsersSource.SelectedItem;
            _usersRepository.DataRowView.Row.Delete();
            _usersRepository.Update();
        }

        private void MenuProductDeleteClick(object sender, RoutedEventArgs e)
        {
            _productRepository.DataRowView = (DataRowView)ProductSource.SelectedItem;
            _productRepository.DataRowView.Row.Delete();
            _productRepository.Update();
        }

        private void MenuProductAddClick(object sender, RoutedEventArgs e)
        {
            DataRow row = _productRepository.DataTable.NewRow();
            AddProductWindow add = new AddProductWindow(row);
            add.ShowDialog();


            if (add.DialogResult.Value)
            {
                _productRepository.DataTable.Rows.Add(row);
                _productRepository.Update();
            }
        }

        private void ProductSource_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _rowProduct = (DataRowView)ProductSource.SelectedItem;
            _rowProduct.BeginEdit();
        }

        private void ProductSource_CurrentCellChanged(object sender, EventArgs e)
        {
            if (_rowProduct is null)
                return;

            _rowProduct.EndEdit();
            var response = _productRepository.Update();

            if (!response.Succes)
                MessageBox.Show($"При обновлении базы данных товаров возникла ошибка: {response.Message}",
                    "При изменении данных возникла ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
        }

        private void UsersSource_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (_productRepository.State.Equals(State.Online))
            {
                var selectedRow = UsersSource.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    string selectedEmail = selectedRow["Email"].ToString();

                    _productRepository.DataAdapter.SelectCommand.CommandText = $@"SELECT * FROM Product WHERE Email = '{selectedEmail}' ORDER BY Product.Id";
                    _productRepository.DataAdapter.SelectCommand.Parameters.Clear(); 

                    try
                    {
                        _productRepository.ClearDataTable(); 
                        _productRepository.DataAdapter.Fill(_productRepository.DataTable); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                                        "Ошибка",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
        }
    }   
}
