using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using System.Globalization;

namespace QuanLyCafe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();

            LoadCategory();

            LoadComboboxTable(comboBoxSwitchTable);

        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fProfile f = new fProfile(LoginAccount);
            f.UpdateAccount += F_UpdateAccount;
            f.ShowDialog();
        }


        private void F_UpdateAccount(object? sender, AccountEvent e)
        {
            tàiKhoảnToolStripMenuItem.Text = "Tài khoản: " + e.Acc.DisplayName;
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.InsertFood += F_InsertFood;
            f.DeleteFood += F_DeleteFood;
            f.UpdateFood += F_UpdateFood;
            f.ShowDialog();
        }

        private void F_UpdateFood(object? sender, EventArgs e)
        {
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).ID);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).ID);
        }

        private void F_DeleteFood(object? sender, EventArgs e)
        {
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).ID);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).ID);

            LoadTable();
        }

        private void F_InsertFood(object? sender, EventArgs e)
        {
            LoadFoodListByCategoryID((comboBoxCategory.SelectedItem as Category).ID);
            if (listViewBill.Tag != null)
                ShowBill((listViewBill.Tag as Table).ID);
        }

        private void fTableManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn đăng xuất khỏi tài khoản?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        void LoadTable()
        {
            flowLayoutPnlTable.Controls.Clear();

            List<Table> tableList = TableDAO.Instance.LoadTableList();
            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                flowLayoutPnlTable.Controls.Add(btn);

                btn.Click += btn_Click;
                btn.Tag= item;

                switch(item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.LightGreen;
                        break;

                    default:
                        btn.BackColor = Color.Red;
                        break;
                }

            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            listViewBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }

        void ShowBill(int id)
        {
            listViewBill.Items.Clear();

            List<QuanLyCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);

            float totalPrice = 0;

            foreach (QuanLyCafe.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());

                totalPrice += item.totalPrice;

                listViewBill.Items.Add(lsvItem);
            }
            //CultureInfo culture = new CultureInfo("vi-VN");

            //Thread.CurrentThread.CurrentCulture = culture;

            txbTotalPrice.Text = totalPrice.ToString(/*"c", culture*/);

        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            comboBoxCategory.DataSource = listCategory;
            comboBoxCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            comboBoxFood.DataSource = listFood;
            comboBoxFood.DisplayMember = "Name";

        }

        private void flowLayoutPnlTable_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listViewBill_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if(cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCategoryID(id);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = listViewBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);

            int foodID = (comboBoxFood.SelectedItem as Food).ID;

            int count = (int)numericUpDownFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);
            LoadTable();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = listViewBill.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);

            int discount = (int)numericDiscount.Value;

            double totalPrice = Convert.ToDouble(txbTotalPrice.Text);

            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc thanh toán hóa đơn cho bàn {0}\nTổng tiền - (Tổng tiền / 100) x Giảm giá\n=> {1} - ({1} / 100) x {2} = {3}đ", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }

        private void btnDiscount_Click(object sender, EventArgs e)
        {

        }

        private void txbTotalPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (listViewBill.Tag as Table).ID;

            int id2 = (comboBoxSwitchTable.SelectedItem as Table).ID;

            if (MessageBox.Show(string.Format("Xác nhận chuyển bàn {0} qua {1}", (listViewBill.Tag as Table).Name, (comboBoxSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);
                LoadTable();
            }          
        }

        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            tàiKhoảnToolStripMenuItem.Text += ": " + LoginAccount.DisplayName;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadCategory();
            LoadTable();
            //LoadComboboxTable();
            //LoadFoodListByCategoryID();
        }

        private void comboBoxSwitchTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
