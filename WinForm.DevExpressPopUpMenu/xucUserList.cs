using DevExpress.XtraBars;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Linq;
using System.Windows.Forms;
using WinForm.DevExpressPopUpMenu.Models;

namespace WinForm.DevExpressPopUpMenu
{
    public partial class xucUserList : DevExpress.XtraEditors.XtraUserControl
    {
        private TestContext db = new TestContext();
        private int selectedID = 0;

        public xucUserList()
        {
            InitializeComponent();
            gridControl1.DataSource = db.Users.ToList();
        }

        private void xucUserList_Load(object sender, EventArgs e)
        {
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Düzenle"));
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Sil"));
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Tümünü Sil"));
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Seçilenleri Sil"));
            gridView1.OptionsBehavior.Editable = false;
            // This property controls whether multi-select feature is enabled
            gridView1.OptionsSelection.MultiSelect = true;
            // Controls whether multiple cells or rows can be selected
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            User user = new User()
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
            };
            db.Users.Add(user);
            db.SaveChanges();
            txtFirstName.Text = "";
            txtLastName.Text = "";
            gridControl1.DataSource = db.Users.ToList();
        }

        private int rowHandle;
        private GridColumn column;

        private void barManager1_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridView view = gridControl1.FocusedView as GridView;
            if (e.Item.Caption == "Düzenle")
            {
                var user = db.Users.FirstOrDefault(x => x.Id == selectedID);
                txtFirstName.Text = user.FirstName;
                txtLastName.Text = user.LastName;
                btnEdit.Visible = true;
                btnSave.Visible = false;
            }
            if (e.Item.Caption == "Sil")
            {
                view.DeleteRow(gridView1.FocusedRowHandle);
                var user = db.Users.FirstOrDefault(x => x.Id == selectedID);
                db.Users.Remove(user);
                db.SaveChanges();
            }
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            selectedID = Convert.ToInt32((sender as GridView).GetFocusedRowCellValue("Id").ToString());
            //   MessageBox.Show("Seçilen ID: " + selectedID.ToString());
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Point);
            if (hitInfo.InRowCell)
            {
                view.FocusedRowHandle = rowHandle = hitInfo.RowHandle;
                column = hitInfo.Column;
                popupMenu1.ShowPopup(barManager1, view.GridControl.PointToScreen(e.Point));
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var user = db.Users.FirstOrDefault(x => x.Id == selectedID);
            user.FirstName = txtFirstName.Text;
            user.LastName = txtLastName.Text;
            db.SaveChanges();
            MessageBox.Show("Düzenleme Yapıldı !");
            btnEdit.Visible = false;
            btnSave.Visible = true;
            txtFirstName.Text = "";
            txtLastName.Text = "";
            gridControl1.DataSource = db.Users.ToList();
        }
    }
}