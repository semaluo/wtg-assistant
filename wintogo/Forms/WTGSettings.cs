﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wintogo.Forms
{
    public partial class WTGSettings : Form
    {
        public WTGSettings()
        {
            InitializeComponent();
        }

        private void WTGSettings_Load(object sender, EventArgs e)
        {
            //UserSetWTGSettingItems wsi = new UserSetWTGSettingItems();
            propertyGrid1.SelectedObject = WTGOperation.userSettings;
        }
    }
}