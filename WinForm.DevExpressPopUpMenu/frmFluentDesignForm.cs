using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinForm.DevExpressPopUpMenu
{
    public partial class frmFluentDesignForm : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        public frmFluentDesignForm()
        {
            InitializeComponent();
            fluentDesignFormContainer1.Controls.Clear();
            fluentDesignFormContainer1.Controls.Add(new xucUserList() { Dock = DockStyle.Fill });
        }
    }
}