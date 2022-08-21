﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LAMP.Classes.M2_Data;
using System.Drawing;

namespace LAMP.Classes
{
    internal class Tile
    {
        public Tile() { }

        public GameScreen Screen { get; set; }
        public int ScreenNr { get; set; }
        public int Area { get; set; }
        public Point Position { get; set; }

        public void ReplaceTile(byte tileID)
        {
            int tx = Position.X / 16;
            int ty = Position.Y / 16;
            int count = ty * 16 + tx;
            Screen.Data[count] = tileID;
        }
    }
}
