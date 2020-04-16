using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WinForm.DevExpressPopUpMenu.Models;

namespace WinForm.DevExpressPopUpMenu
{
    public partial class xucUserList : XtraUserControl
    {
        #region Define

        private TestContext db = new TestContext();
        private int selectedID = 0;
        private int rowHandle;
        private GridColumn column;

        #endregion Define

        #region Constructor

        public xucUserList()
        {
            InitializeComponent();
            GridViewFill();
        }

        #endregion Constructor

        #region xucUserList

        private void xucUserList_Load(object sender, EventArgs e)
        {
            #region popupMenu1

            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Düzenle"));
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Sil"));
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Seçilenleri Sil"));
            popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, "Tümünü Sil"));

            #endregion popupMenu1

            #region ColumnSizeAuto

            //Kolon genişlikleri otomatik genişletiliyor.
            gridControl1.ForceInitialize();
            gridView1.BestFitColumns();

            #endregion ColumnSizeAuto

            #region gridView1

            //Bu özellik kolonun otomatik olup olmayacağını kontrol eder.
            gridView1.OptionsView.ColumnAutoWidth = true;
            //Gridviewdeki düzenleme (Edit) özelliğini kontrol eder.
            gridView1.OptionsBehavior.Editable = false;
            // Bu özellik çoklu seçim özelliğinin etkin olup olmadığını kontrol eder
            gridView1.OptionsSelection.MultiSelect = true;
            // Birden fazla hücre veya satır seçilip seçilemeyeceğini kontrol eder
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;

            #endregion gridView1

            #region Buttons

            btnEdit.Visible = false;
            btnSave.Visible = true;

            #endregion Buttons
        }

        #endregion xucUserList

        #region Butonlar

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
            GridViewFill();
        }

        private void GridViewFill()
        {
            gridControl1.DataSource = db.Users.ToList();
            //Gridview de Sol taraftaki boşluk kaldırılıyor.
            gridView1.OptionsView.ShowIndicator = false;
            //Seçilen Checkbox Uzunluğu
            gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 25;
            //Kolonu gizleme
            gridView1.Columns[0].Visible = false; //Id
            //Kolona Tooltip ekleme
            gridView1.Columns[1].ToolTip = "ToolTip";
            //Kolonu uzunluğunu verme
            gridView1.Columns[1].MaxWidth = 125;
            gridView1.Columns[2].MaxWidth = 125;
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
            GridViewFill();
        }

        private void btnSelectedRows_Click(object sender, EventArgs e)
        {
            //Seçili Satırları Getir
            int count = 0;
            string selectedRowID = "";
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (gridView1.IsRowSelected(i))
                {
                    selectedRowID += gridView1.GetRowCellValue(i, gridView1.Columns["Id"]).ToString() + "\n";
                }
                else
                {
                    count++;
                    if (count == gridView1.DataRowCount)
                    {
                        XtraMessageBox.Show("Lütfen Bir Seçim Yapınız !", "İşlem Başarısız !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (!String.IsNullOrEmpty(selectedRowID))
            {
                XtraMessageBox.Show(selectedRowID, "İşlem Başarılı !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion Butonlar

        #region PopupMenu

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
                GridViewFill();
            }

            if (e.Item.Caption == "Tümünü Sil")
            {
                var users = db.Users.ToList();
                db.Users.RemoveRange(users);
                db.SaveChanges();
                XtraMessageBox.Show("Tüm Kullanıcılar Silindi !'", "İşlem Başarılı !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                GridViewFill();
            }

            if (e.Item.Caption == "Seçilenleri Sil")
            {
                string selectedRowList = "";
                for (int i = 0; i < gridView1.DataRowCount; i++)
                {
                    if (gridView1.IsRowSelected(i))
                    {
                        selectedRowList += gridView1.GetRowCellValue(i, gridView1.Columns["Id"]).ToString() + " - " + gridView1.GetRowCellValue(i, gridView1.Columns["FirstName"]).ToString() + " " + gridView1.GetRowCellValue(i, gridView1.Columns["LastName"]).ToString() + "\n";
                        int selectedRowID = Convert.ToInt32(gridView1.GetRowCellValue(i, gridView1.Columns["Id"]));
                        var user = db.Users.FirstOrDefault(x => x.Id == selectedRowID);
                        db.Users.Remove(user);
                        db.SaveChanges();
                    }
                }
                GridViewFill();

                XtraMessageBox.Show("Silenenlerin Listesi:\n" + selectedRowList, "İşlem Başarılı !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            selectedID = Convert.ToInt32((sender as GridView).GetFocusedRowCellValue("Id").ToString());
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Point);
            if (hitInfo.InRowCell)
            {
                view.FocusedRowHandle = rowHandle = hitInfo.RowHandle;
                column = hitInfo.Column;
                popupMenu1.ShowPopup(barManager1, view.GridControl.PointToScreen(e.Point));
            }
        }

        private void btnGridListRefresh_Click(object sender, EventArgs e)
        {
            GridViewFill();
        }

        #endregion PopupMenu
    }
}