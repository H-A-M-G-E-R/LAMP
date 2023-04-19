﻿using LAMP.Classes;
using LAMP.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAMP.Controls.Other
{
    public partial class RecentFileDisplay : UserControl
    {
        public RecentFileDisplay(String title, String path, bool pinned, RecentFiles parent)
        {
            InitializeComponent();

            lbl_title.Text = title;
            lbl_path.Text = path;

            btn_pin.FlatAppearance.BorderSize = 0;
            if (pinned) btn_pin.Image = Resources.pinned;
            Pinned = pinned;
            Parent = parent;
        }

        private bool Pinned = false;
        private RecentFiles Parent;

        public void SetBackground(Color c)
        {
            pnl_main.BackColor = c;
        }

        private void pnl_main_MouseEnter(object sender, EventArgs e)
        {
            SetBackground(Color.FromArgb(0xFF, 0xd8, 0xd8, 0xd8));
        }

        private void pnl_main_MouseLeave(object sender, EventArgs e)
        {
            SetBackground(Color.FromArgb(0xFF, 0xf0, 0xf0, 0xf0));
        }

        private void lbl_title_Click(object sender, EventArgs e)
        {
            Editor.OpenProjectAndLoad(lbl_path.Text);
        }

        private void btn_pin_Click(object sender, EventArgs e)
        {
            if (Pinned)
            {
                Globals.pinnedFiles.Remove(lbl_path.Text);
            }
            else Globals.pinnedFiles.Add(lbl_path.Text);

            Parent.LoadRecentFileControls();
        }
    }
}
