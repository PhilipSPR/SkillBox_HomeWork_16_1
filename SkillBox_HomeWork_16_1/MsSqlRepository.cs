using SkillBox_HomeWork_16_1.Models;
using SkillBox_HomeWork_16_1.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace SkillBox_HomeWork_16_1
{
    internal class MsSqlRepository : IDisposable
    {
        private SqlConnection _connection;
        private SqlConnectionStringBuilder _sqlConnectionString;
        private State _state;
        private DataTable _dataTable;
        private DataRowView _dataRowView;
        private SqlDataAdapter _dataAdapter;
        private string _tableName;

        public SqlDataAdapter DataAdapter
        {
            get { return _dataAdapter; }
            set { _dataAdapter = value; }
        }

        public DataTable DataTable
        {
            get { return _dataTable; }
            set { _dataTable = value; }
        }

        public DataRowView DataRowView
        {
            get { return _dataRowView; }
            set { _dataRowView = value; }
        }

        public State State
        {
            get { return _state; }
            set
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }

        public MsSqlRepository(string catalog, string tableName)
        {
            _sqlConnectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = catalog,
                IntegratedSecurity = true,
                Pooling = false,
            };
            _dataTable = new DataTable();
            _dataAdapter = new SqlDataAdapter();
            _tableName = tableName;
        }

        public BaseResponse Update()
        {
            try
            {
                _dataAdapter.Update(_dataTable);

                return new BaseResponse(true, "База успешно обновлена");
            }
            catch (Exception ex)
            {
                return new BaseResponse(false, ex.Message);
            }
        }

        public void ClearDataTable()
        {
            _dataTable?.Clear();
        }

        private void Preparing()
        {
            try
            {
                DataAdapter.SelectCommand.Connection = _connection;
                DataAdapter.UpdateCommand.Connection = _connection;
                DataAdapter.InsertCommand.Connection = _connection;
                DataAdapter.DeleteCommand.Connection = _connection;
                DataAdapter.Fill(DataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При установки конфигурации возникла ошибка: {ex.Message}");
            }
        }

        private void _connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            var state = (sender as SqlConnection).State;

            if (state.Equals(ConnectionState.Open))
                State = State.Online;
            else
                State = State.Offline;
        }

        public event Action<State> StateChanged;

        internal Task<BaseResponse> Start()
        {
            try
            {
                _connection = new SqlConnection(_sqlConnectionString.ConnectionString);
                _connection.StateChange += _connection_StateChange;

                Preparing();

                _connection.Open();

                MessageBox.Show(_connection.ConnectionString,
                                "MsSql успешно подключен",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                return Task.FromResult(new BaseResponse(true, "MsSql успешно подключен"));

            }
            catch (Exception ex)
            {
                string errMessage = $"При подключении к базе данных 'MSSQL' возникла ошибка: {ex.Message}";

                return Task.FromResult(new BaseResponse(false, errMessage));
            }

        }

        public void Dispose()
        {
            DataTable.Clear();
            _connection?.Dispose();
            _connection.StateChange -= _connection_StateChange;
        }
    }
}
