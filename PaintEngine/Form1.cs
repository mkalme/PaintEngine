using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace PaintEngine
{
    public partial class Form1 : Form
    {
        PaintEngine paint;
        PaintEngineGrid grid = new PaintEngineGrid(8, 8, Color.Black, 1, 80, 47);
        //PaintEngineGrid grid = new PaintEngineGrid(8, 8, Color.Black, 0, 85, 52);

        Color[] colors = new Color[] {Color.Orange, Color.Red, Color.Purple, Color.Yellow, Color.Green,
            Color.Honeydew, Color.IndianRed, Color.Khaki, Color.Ivory, Color.LightSteelBlue, Color.MidnightBlue};

        public Form1()
        {
            InitializeComponent();

            paint = new PaintEngine(this, pictureBox1);
            PaintEngineSelection selection = new PaintEngineSelection(40, 13, 800, 500);
            selection.Objects.Add(grid);
            paint.Objects.Add(selection);

            draw();
            paint.Show();

            timer1.Start();
            timer1.Interval = 1000;
        }

        public void draw()
        {
            Random rand = new Random();

            for (int x = 0; x < grid.NumberOfColumns; x++)
            {
                for (int y = 0; y < grid.NumberOfRows; y++)
                {
                    grid.Cells[x, y] = new PaintEngineCell(true, grid.Color, grid.Thickness);
                    grid.Cells[x, y].Fill = true;
                    grid.Cells[x, y].FillColor = colors[rand.Next(colors.Length)];
                }
            }

            //Debug.WriteLine(grid.Cells[10, 10].FillColor.ToString());

            //PaintEngineGrid grid1 = new PaintEngineGrid(10, 10, Color.Wheat, 2, 40, 20);
            //PaintEngineRectangle rect = new PaintEngineRectangle(Color.SteelBlue, 20, 0, 0, 300, 300, false);

            //PaintEngineSelection selection = new PaintEngineSelection(50, 50, 200, 200);
            //selection.Objects.Add(grid);
            //selection.Objects.Add(grid1);
            //selection.Objects.Add(rect);

            paint.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            draw();
        }
    }
}
