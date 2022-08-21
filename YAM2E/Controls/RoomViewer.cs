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

    //TODO:unused?
    public bool HasSelection => SelRect.X != -1; //Selection rectangle doesn't have a negative x value

    public bool ShowScreenOutlines { get; set; } = false;

    public bool ShowDuplicateOutlines { get; set; } = true;

    public bool ShowObjects { get; set; } = true;

    public int SelectedScreen { get; set; } = 0;
    private int SelectedScreenOld = 0;
    private List<Rectangle> UniqueScreen { get; } = new List<Rectangle>();

    //Rectangles
    //Red selection rectangle
    public Rectangle RedRect { get; set; }
    private Pen TilePen { get; set; } = new Pen(Globals.SelectedColor, 1);

    //selection rectangle
    public Rectangle SelRect { get; set; }
    private Pen SelectionPen { get; set; } = new Pen(Globals.SelectionColor, 1);

    //screen outline rectangle
    //TODO:unused?
    private Rectangle ScreenRect { get; set; } = new Rectangle();
    private Pen ScreenPen { get; set; } = new Pen(Color.White, 2)
    {
        Alignment = PenAlignment.Inset
    };
    private Pen UniqueScreenPen { get; set; } = new Pen(Globals.SelectedColor, 2)
    { 
        Alignment = PenAlignment.Inset
    };

    private Pen BlackPen { get; set; } = new Pen(Color.Black, 1);

    //Objects
    private Pen ObjectPen { get; set; } = new Pen(Globals.ObjectColor, 2)
    {
        Alignment = PenAlignment.Inset
    };

    public void ResetSelection()
    {
        RedRect = new Rectangle(-1, -1, 0, 0);
        SelRect = new Rectangle(-1, -1, 0, 0);
    }

    public RoomViewer()
    {
        //SetStyle(ControlStyles.Opaque, true);
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SuspendLayout();
        ResumeLayout(false);
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        BackColor = Color.FromArgb(40, 50, 50);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        //change render settings
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

        if (RedRect.X != -1)
            e.Graphics.DrawRectangle(TilePen, RedRect);

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
                    Rectangle rect = new Rectangle(256 * i + 2, 256 * j + 2, 251, 251);
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
                    Rectangle rect = new Rectangle(256 * i, 256 * j, 255, 255);
                    e.Graphics.DrawRectangle(ScreenPen, rect);
                }
            }
        }

        //Draw Unique Screen outlines
        if (UniqueScreen.Count != 0 && ShowDuplicateOutlines)
        {
            foreach (Rectangle r in UniqueScreen)
                e.Graphics.DrawRectangle(UniqueScreenPen, r);
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
                    Rectangle rec = new Rectangle(p.X, p.Y, 16, 16);
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