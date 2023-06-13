using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QuanLyCafe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();

        BindingSource accountList = new BindingSource();

        BindingSource categoryList = new BindingSource();

        BindingSource tableList = new BindingSource();

        public Account loginAccount;

        public fAdmin()
        {
            InitializeComponent();

            Loader();
        }

        #region methods
        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void Loader()
        {
            dataGridViewFood.DataSource = foodList;

            dataGridViewAccount.DataSource = accountList;

            dataGridViewCategory.DataSource = categoryList;

            dataGridViewTable.DataSource = tableList;


            LoadListBillByDate(dateTimePickerFromDate.Value, dateTimePickerToDate.Value);

            LoadListFood();

            LoadAccount();

            LoadListCategory();

            LoadListTable();

            LoadCategoryIntoCombobox(comboBoxFoodCategory);

            AddFoodBinding();

            AddAccountBinding();

            AddCategoryBinding();

            AddTableBinding();
        }

        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.GetListTable();
        }

        void AddTableBinding()
        {
            txbTableID.DataBindings.Add(new Binding("Text", dataGridViewTable.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbTableName.DataBindings.Add(new Binding("Text", dataGridViewTable.DataSource, "name", true, DataSourceUpdateMode.Never));
            txbTableStatus.DataBindings.Add(new Binding("Text", dataGridViewTable.DataSource, "status", true, DataSourceUpdateMode.Never));
        }

        void AddCategoryBinding()
        {
            txbCategoryID.DataBindings.Add(new Binding("Text", dataGridViewCategory.DataSource, "id", true, DataSourceUpdateMode.Never));
            txbCategoryName.DataBindings.Add(new Binding("Text", dataGridViewCategory.DataSource, "name", true, DataSourceUpdateMode.Never));
        }

        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dataGridViewAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dataGridViewAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            numericUpDownAccountType.DataBindings.Add(new Binding("Value", dataGridViewAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dataGridViewFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dataGridViewFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            numericFoodPrice.DataBindings.Add(new Binding("Value", dataGridViewFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dataGridViewBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Bạn không thể xoá tài khoản đang sử dụng");
                return;
            }

            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }

            LoadAccount();
        }

        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }

        void AddTable(string name)
        {
            if (TableDAO.Instance.InsertTable(name))
            {
                MessageBox.Show("Thêm bàn thành công");
            }
            else
            {
                MessageBox.Show("Thêm bàn thất bại");
            }

            LoadListTable();
        }

        void DeleteTable(int id)
        {
            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Xoá bàn thành công");
            }
            else
            {
                MessageBox.Show("Xoá bàn thất bại");
            }

            LoadListTable();
        }

        void UpdateTable(string name, int id)
        {
            if (TableDAO.Instance.UpdateTable(name, id))
            {
                MessageBox.Show("Sửa thông tin bàn thành công");
            }
            else
            {
                MessageBox.Show("Sửa thông tin bàn thất bại");
            }

            LoadListTable();
        }

        void AddCategory(string name)
        {
            if (CategoryDAO.Instance.InsertCategory(name))
            {
                MessageBox.Show("Thêm danh mục thành công");
            }
            else
            {
                MessageBox.Show("Thêm danh mục thất bại");
            }

            LoadListCategory();
        }

        void UpdateCategory(string name, int id)
        {
            if (CategoryDAO.Instance.UpdateCategory(name, id))
            {
                MessageBox.Show("Sửa thông tin danh mục thành công");
            }
            else
            {
                MessageBox.Show("Sửa thông tin danh mục thất bại");
            }

            LoadListCategory();
        }

        void DeleteCategory(int id)
        {
            if (CategoryDAO.Instance.DeleteCategory(id))
            {
                MessageBox.Show("Xoá danh mục thành công");
            }
            else
            {
                MessageBox.Show("Xoá danh mục thất bại");
            }

            LoadListCategory();
        }
        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void fAdmin_Load(object sender, EventArgs e)
        {

        }

        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dateTimePickerFromDate.Value, dateTimePickerToDate.Value);
        }

        
        private void btnViewFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        
        private void dataGridViewFood_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewFood.SelectedCells.Count > 0)
                {
                    int id = (int)dataGridViewFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                    Category cateogory = CategoryDAO.Instance.GetCategoryByID(id);

                    comboBoxFoodCategory.SelectedItem = cateogory;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in comboBoxFoodCategory.Items)
                    {
                        if (item.ID == cateogory.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    comboBoxFoodCategory.SelectedIndex = index;
                }
            }
            catch { }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (comboBoxFoodCategory.SelectedItem as Category).ID;
            float price = (float)numericFoodPrice.Value;
            
            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm món vào menu");
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (comboBoxFoodCategory.SelectedItem as Category).ID;
            float price = (float)numericFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa thông tin món thành công");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thông tin món");
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công");
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa thức ăn");
            }
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private void btnSearchFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txbFoodNameSearch.Text);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDownAccountType.Value;

            //string query = "IF EXISTS (SELECT 1 FROM users WHERE username = @username) " +
            //     "SELECT 1 " +
            //     "ELSE " +
            //     "SELECT 0";

            //int result = (int)DataProvider.Instance.ExecuteScalar(query);


            using (SqlConnection connection = new SqlConnection("Data Source=SHIKI;Initial Catalog=QuanLyCafe;Integrated Security=True"))
            {
                connection.Open();

                string sql = "IF EXISTS (SELECT 1 FROM dbo.Account WHERE username = @username) " +
                             "SELECT 1 " +
                             "ELSE " +
                             "SELECT 0";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", userName);

                    int result = (int)command.ExecuteScalar();

                    if (result == 1)
                    {
                        MessageBox.Show("Tài khoản đã tồn tại. \nVui lòng chọn tên đăng nhập khác.");
                    }
                    else
                    {
                        AddAccount(userName, displayName, type);
                    }
                }
            }


            //if (result == 1)
            //{
            //    MessageBox.Show("Tài khoản đã tồn tại");
            //}
            //else
            //{
            //    AddAccount(userName, displayName, type);
            //}
        }



        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDownAccountType.Value;

            EditAccount(userName, displayName, type);
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

            ResetPass(userName);
        }

        private void btnShowCategory_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }

        
        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txbTableName.Text;
            AddTable(name);
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txbTableID.Text);
            DeleteTable(id);
        }

        private void btnShowTable_Click(object sender, EventArgs e)
        {
            LoadListTable();
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txbTableID.Text);
            string name = txbTableName.Text;
            UpdateTable(name, id);
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            AddCategory(txbCategoryName.Text);
        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = int.Parse((string)txbCategoryID.Text);
            DeleteCategory(id);
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txbCategoryID.Text);
            string name = txbCategoryName.Text;
            LoadListFood();
            UpdateCategory(name, id);
        }

        private void comboBoxFoodCategory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewFood_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void fAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadListCategory();
        }
    }
}

