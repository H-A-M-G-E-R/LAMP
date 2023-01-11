﻿using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using LAMP.Classes;

namespace LAMP.Controls;

public class RoomViewer : Control
{
    public override Image BackgroundImage
    {
        get => base.BackgroundImage;
        set
        {
            base.BackgroundImage = value;
            Size = base.BackgroundImage.Size;
        }
    }

    public RoomViewer()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SuspendLayout();
        ResumeLayout(false);
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        BackColor = Color.FromArgb(40, 50, 50);
    }

    public bool ShowScreenOutlines { get; set; } = false;
    public bool ShowDuplicateOutlines { get; set; } = true;
    public bool ShowScrollBorders { get; set; } = false;
    public bool ShowObjects { get; set; } = true;

    public int SelectedScreen { get; set; } = 0;
    private int SelectedScreenOld = 0;
    private List<Rectangle> UniqueScreen { get; } = new List<Rectangle>();

    //Rectangles
    //Red selection rectangle
    public Rectangle RedRect { get; set; }
    public Rectangle CursorRect { get; set; }
    private Pen TilePen { get; set; } = new Pen(Globals.SelectedColor, 1);

    //selection rectangle
    public Rectangle SelRect { get; set; }
    private Pen SelectionPen { get; set; } = new Pen(Globals.SelectionColor, 1);

    //screen outline rectangle
    private Pen ScreenPen { get; set; } = new Pen(Color.White, 2)
    {
        Alignment = PenAlignment.Inset
    };
    private Pen UniqueScreenPen { get; set; } = new Pen(Globals.UniqueScreenColor, 2)
    { 
        Alignment = PenAlignment.Inset
    };
    private Pen BorderOutlinePen { get; set; } = new Pen(Globals.BorderColor, 2)
    {
        Alignment = PenAlignment.Inset
    };

    private Pen BlackPen { get; set; } = new Pen(Color.Black, 1);

    //Objects
    private Pen ObjectPen { get; set; } = new Pen(Globals.ObjectColor, 2)
    {
        Alignment = PenAlignment.Inset
    };
    public Rectangle HeldObject { get; set; } = new Rectangle(-1, -1, -1, -1);

    public void ResetSelection()
    {
        RedRect = new Rectangle(-1, -1, 0, 0);
        SelRect = new Rectangle(-1, -1, 0, 0);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        //change render settings
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

        if (RedRect.X != -1 && MainWindow.EditingTiles) e.Graphics.DrawRectangle(TilePen, RedRect);
        if (!MainWindow.EditingTiles) e.Graphics.DrawRectangle(TilePen, CursorRect);

        //duplicate Screen Outlines
        if (SelectedScreen != SelectedScreenOld && ShowDuplicateOutlines)
        {
            //invalidating old outlines
            if (UniqueScreen.Count != 0)
            {
                Rectangle rect = UniqueScreen[0];
                foreach (Rectangle r in UniqueScreen)
                    rect = Editor.UniteRect(rect, r);

                Invalidate(Editor.SetValSize(rect));
            }

            UniqueScreen.Clear();
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Rectangle rect = new Rectangle(256 * i + 2, 256 * j + 2, 252, 252);
                    int nr = j * 16 + i;
                    if (Globals.Areas[Globals.SelectedArea].Screens[nr] != SelectedScreen)
                        continue;
                    UniqueScreen.Add(rect);
                    Invalidate(Editor.SetValSize(rect));
                }
            }

            SelectedScreenOld = SelectedScreen;
        }

        //screen outlines
        if (ShowScreenOutlines)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Rectangle rect = new Rectangle(256 * i, 256 * j, 256, 256);
                    e.Graphics.DrawRectangle(ScreenPen, rect);
                }
            }
        }

        //screen borders
        if (ShowScrollBorders)
        {
            foreach (Rectangle r in Globals.ScrollBorders)
                e.Graphics.DrawRectangle(BorderOutlinePen, r);
        }

        //Draw Unique Screen outlines
        if (UniqueScreen.Count != 0 && ShowDuplicateOutlines)
        {
            foreach (Rectangle r in UniqueScreen)
                e.Graphics.DrawRectangle(UniqueScreenPen, r);
        }

        //Draw held object
        if (MainWindow.heldObject != null)
        {
            e.Graphics.DrawEllipse(ObjectPen, HeldObject);
        }

        //Draw objects
        if (ShowObjects)
        {
            for(int i = 0; i < 256; i++)
            {
                int screen = i + 256 * Globals.SelectedArea;
                if (Globals.Objects[screen].Count == 0) continue;

                foreach (Enemy o in Globals.Objects[screen]) 
                {
                    Point p = o.GetPosition(i);
                    Rectangle rec = new Rectangle(p.X, p.Y, 15, 15);
                    e.Graphics.DrawEllipse(ObjectPen, rec);
                    //e.Graphics.DrawRectangle(ObjectPen, rec);
                    //TODO: Add option to switch been circle and rect
                }
            }
        }

        SelectionPen.DashPattern = BlackPen.DashPattern = new float[] { 2, 3 };
        BlackPen.DashOffset = 2;
        e.Graphics.DrawRectangle(BlackPen, SelRect);
        e.Graphics.DrawRectangle(SelectionPen, SelRect);
        base.OnPaint(e);
    }
}