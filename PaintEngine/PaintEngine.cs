using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Drawing.Imaging;

namespace PaintEngine
{
    class PaintEngine
    {
        //Graphics
        private Bitmap bitmap;
        private Graphics graphics;
        private PictureBox pictureBox;

        //Properties
        public readonly int Width;
        public readonly int Height;
        public Color BackColor = Color.Transparent;
        public PixelFormat PixelFormat = PixelFormat.Format32bppPArgb;

        //Objects
        public List<object> Objects = new List<object>();

        public PaintEngine(Form form, PictureBox pictureBox) {
            bitmap = new Bitmap(
                pictureBox.Width,
                pictureBox.Height,
                PixelFormat
            );
            graphics = Graphics.FromImage(bitmap);
            this.pictureBox = pictureBox;

            Width = bitmap.Width;
            Height = bitmap.Height;
        }


        public void Show() {
                graphics.Clear(BackColor);
                graphics = Draw(0, 0, Width, Height, bitmap, graphics, Objects);
                pictureBox.Image = bitmap;
        }

        public Graphics Draw(int x1, int y1, int width, int height, Bitmap backgroundBitmap, Graphics backgroundGraphics, List<object> Objects) {
            Bitmap currentBitmap = new Bitmap(
                    width,
                    height,
                    backgroundBitmap.PixelFormat
                );
            Graphics currentGraphics = Graphics.FromImage(currentBitmap);

            for (int i = 0; i < Objects.Count; i++)
            {
                if (Objects[i].GetType() == typeof(PaintEngineGrid))
                {
                    currentGraphics = DrawGrid((PaintEngineGrid)Objects[i], currentGraphics);
                }
                else if (Objects[i].GetType() == typeof(PaintEngineRectangle))
                {
                    currentGraphics = DrawRectangle((PaintEngineRectangle)Objects[i], currentGraphics);
                }
                else if (Objects[i].GetType() == typeof(PaintEngineSelection))
                {
                    PaintEngineSelection selection = (PaintEngineSelection)Objects[i];
                    currentGraphics = Draw(
                        selection.X1,
                        selection.Y1,
                        selection.Width,
                        selection.Height,
                        currentBitmap,
                        currentGraphics,
                        selection.Objects
                    );
                }
            }

            backgroundGraphics.DrawImage(currentBitmap, new Point(x1, y1));
            return backgroundGraphics;
        }



        private Graphics DrawGrid(PaintEngineGrid grid, Graphics currentGraphics)
        {
            int cellWidth = grid.CellWidth + grid.Thickness;
            int cellHeight = grid.CellHeight + grid.Thickness;

            if (grid.Thickness > 0) {
                //columns
                for (int i = 0; i < grid.NumberOfColumns + 1; i++)
                {
                    currentGraphics.DrawLine(
                        new Pen(grid.Color, grid.Thickness),
                        cellWidth * i + (grid.Thickness / 2),
                        0,
                        cellWidth * i + (grid.Thickness / 2),
                        cellHeight * grid.NumberOfRows + grid.Thickness - 1
                    );
                }

                //rows
                for (int i = 0; i < grid.NumberOfRows + 1; i++)
                {
                    currentGraphics.DrawLine(
                        new Pen(grid.Color, grid.Thickness),
                        0,
                        cellHeight * i + (grid.Thickness / 2),
                        cellWidth * grid.NumberOfColumns + grid.Thickness - 1,
                        cellHeight * i + (grid.Thickness / 2)
                    );
                }
            }

            //cells
            for (int x = 0; x < grid.Cells.GetLength(0); x++) {
                for (int y = 0; y < grid.Cells.GetLength(1); y++)
                {
                    if (grid.Cells[x, y] != null) {
                        if (grid.Cells[x, y].Selected) {
                            if (grid.Cells[x, y].Thickness > 0) {
                                currentGraphics.DrawRectangle(
                                        new Pen(grid.Cells[x, y].SelectionColor, grid.Cells[x, y].Thickness),
                                        new Rectangle(
                                            cellWidth * x + (grid.Cells[x, y].Thickness / 2),
                                            cellHeight * y + (grid.Cells[x, y].Thickness / 2),
                                            cellWidth,
                                            cellHeight)
                                );
                            }

                            if (grid.Cells[x, y].Fill)
                            {
                                currentGraphics.FillRectangle(
                                    new SolidBrush(grid.Cells[x, y].FillColor),
                                    new RectangleF(
                                        cellWidth * x + (grid.Cells[x, y].Thickness),
                                        cellHeight * y + (grid.Cells[x, y].Thickness),
                                        cellWidth - grid.Cells[x, y].Thickness,
                                        cellHeight - grid.Cells[x, y].Thickness)
                                );
                            }
                        }
                    }
                }
            }

            return currentGraphics;
        }

        private Graphics DrawRectangle(PaintEngineRectangle rect, Graphics currentGraphics)
        {
            if (rect.Fill) {
                currentGraphics.FillRectangle(
                    new SolidBrush(rect.Color),
                    new RectangleF(rect.X1, rect.Y1, rect.Width, rect.Height)
                );
            } else {
                currentGraphics.DrawRectangle(
                    new Pen(rect.Color, rect.Thickness),
                    new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height)
                );
            }

            return currentGraphics;
        }
    }

    class PaintEngineGrid
    {
        public int CellWidth { get; set; }
        public int CellHeight { get; set; }
        public Color Color { get; set; }
        public int Thickness { get; set; }
        public PaintEngineCell[,] Cells { get; set; }
        public readonly int NumberOfColumns;
        public readonly int NumberOfRows;

        public PaintEngineGrid(int cellWidth, int cellHeight, Color color, int thickness, int numberOfColumns, int numberOfRows)
        {
            this.CellWidth = cellWidth;
            this.CellHeight = cellHeight;
            this.Color = color;
            this.Thickness = thickness;
            this.Cells = new PaintEngineCell[numberOfColumns, numberOfRows];
            this.NumberOfColumns = numberOfColumns;
            this.NumberOfRows = numberOfRows;
        }

        public void SelectCells(int x, int y) {
            Cells[x, y] = new PaintEngineCell(true, Color.Orange, Thickness);
        }
    }

    class PaintEngineCell {
        public bool Selected { get; set; }
        public Color SelectionColor { get; set; }
        public Color FillColor { get; set; }
        public int Thickness { get; set; }
        public bool Fill = false;

        public PaintEngineCell(bool selected, Color selectionColor, int thickness){
            this.Selected = selected;
            this.SelectionColor = selectionColor;
            this.FillColor = selectionColor;
            this.Thickness = thickness;
        }
    }

    class PaintEngineRectangle
    {
        public Color Color { get; set; }
        public int Thickness { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Fill { get; set; }

        public PaintEngineRectangle(Color color, int thickness, int x1, int y1, int width, int height, bool fill)
        {
            this.Color = color;
            this.Thickness = thickness;
            this.X1 = x1;
            this.Y1 = y1;
            this.Width = width;
            this.Height = height;
            this.Fill = fill;
        }
    }

    class PaintEngineSelection
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<object> Objects = new List<object>();

        public PaintEngineSelection(int x1, int y1, int width, int height)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.Width = width;
            this.Height = height;
        }
    }

    class PaintEngineAllObjects
    {
        public object Object { get; set; }
        
        public PaintEngineAllObjects(object instanceObject)
        {
            this.Object = instanceObject;
        }
    }
}
