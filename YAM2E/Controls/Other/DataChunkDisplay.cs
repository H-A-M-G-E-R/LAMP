﻿using LAMP.Classes;
using LAMP.Classes.M2_Data;
using LAMP.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAMP.Controls.Other
{
    public partial class DataChunkDisplay : UserControl
    {
        public int ChunkID { get; set; }
        public DataChunk Chunk { get; set; }
        public event EventHandler RemoveThisChunk;

        bool canPreview
        {
            get
            {
                if (Chunk.DataType == "Graphics") return true;
                if (Chunk.DataType == "MetatileTable") return true;
                else return false;
            }
        }

        public DataChunkDisplay(DataChunk chunk)
        {
            InitializeComponent();

            this.Chunk = chunk;

            lbl_type.Text = chunk.DataType;
            lbl_range.Text = $"Location: {Format.PointerToString(chunk.DataStart)} - {Format.PointerToString(chunk.DataStart + chunk.DataLength - 1)}.";
            chb_include.Checked = chunk.Include;

            //Checking which Tilesets contain this DataChunk
            List<string> tilesets = new List<string>();
            foreach (Tileset t in Globals.Tilesets) if (Chunk.ContainedInTileset(t)) tilesets.Add(t.Name);

            if (tilesets.Count == 0) return;

            lbl_contains.Text = "Contained in: ";
            for (int i = 0; i < tilesets.Count; i++)
            {
                string s = ",";
                if (i == tilesets.Count - 1) s = "";
                lbl_contains.Text += $"{tilesets[i]}{s} ";
            }

            btn_preview.Visible = true;
        }

        private void chb_include_CheckedChanged(object sender, EventArgs e)
        {
            Chunk.Include = chb_include.Checked;
        }

        private void btn_remove_Click(object sender, EventArgs e) => RemoveThisChunk.Invoke(this, new EventArgs());

        private void btn_preview_Click(object sender, EventArgs e)
        {

        }
    }
}
