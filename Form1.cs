using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {


        // The universe array
        bool[,] universe = new bool[5, 5];
        bool[,] scratchPad = new bool[5, 5];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Universe Width and Height for FromClosed
        int uniWidth;
        int uniHeight;

        // Alive cell count
        int aliveCells = 0;

        //Checking for type of Boundary Type, Finite is set as default
        bool isToroidal = false;

        // For View tap
        bool isNeighborCount = true;
        bool isGrid = true;
        bool isHud = true;

        //Default Seed
        int seed = 2001;

        public Form1()
        {

            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            //For Reload, Rest, and FromClosed function
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            seed = Properties.Settings.Default.Seed;
            uniWidth = Properties.Settings.Default.universeWidth;
            uniHeight = Properties.Settings.Default.universeHeight;
            universe = new bool[uniWidth, uniHeight];
            scratchPad = new bool[uniWidth, uniHeight];
        }

        private void LivingCells()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                    {
                        aliveCells++;
                    }
                }
            }
            if (aliveCells <= 0) aliveCells = 0;
        }
        private void RandomizeTime()
        {
            // Randomaize from time

            Random rand = new Random();

            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int rNumber = rand.Next(0, 2);
                    if (rNumber == 0) universe[x, y] = true;
                    else universe[x, y] = false;
                }
            }
            LivingCells();
            graphicsPanel1.Invalidate();
        }
        private void RandomizeSeed()
        {
            //Randomize from a seed

            Random rSeed = new Random(seed);

            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int rNumber = rSeed.Next(0, 2);
                    if (rNumber == 0) universe[x, y] = true;
                    else universe[x, y] = false;
                }
            }
            LivingCells();
            graphicsPanel1.Invalidate();
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            int count;

            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {

                    if (isToroidal) count = CountNeighborsToroidal(x, y);
                    else count = CountNeighborsFinite(x, y);
                    scratchPad[x, y] = false;
                    // The rules
                    if (universe[x, y] == true)
                    {

                        if (count < 2)
                        {
                            scratchPad[x, y] = false;
                            aliveCells--;
                        }
                        else if (count > 3)
                        {
                            scratchPad[x, y] = false;
                            aliveCells--;
                        }
                        else if (count == 2 || count == 3)
                        {
                            scratchPad[x, y] = true;

                        }

                    }
                    else if (universe[x, y] == false)
                    {

                        if (count == 3)
                        {

                            scratchPad[x, y] = true;


                        }
                    }

                }
            }

            //Copy from scratchPad to universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            LivingCells();

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            graphicsPanel1.Invalidate();
        }


        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {

            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            //Displaying count neighbor
            Font font = new Font("Arial", 10f);
            string boundaryType = string.Empty;
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            int neighbor;

            // Displaying the Hud
            Rectangle hudRect = Rectangle.Empty;
            hudRect.X = 0 * cellWidth;
            hudRect.Y = 90 + cellHeight;
            hudRect.Width = 5 * cellWidth;
            hudRect.Height = 5 * cellHeight;
            Font hudFont = new Font("Arial", 12f);
            StringFormat hudFormat = new StringFormat();
            hudFormat.Alignment = StringAlignment.Near;
            hudFormat.LineAlignment = StringAlignment.Near;
            string hud = string.Empty;
            hud = "Generation : " + generations + "\nCell count : " + aliveCells.ToString() + "\nBoundary Type : " + boundaryType + "\nUniverse Size : { Width = " + universe.GetLength(0) + ", Height = " + universe.GetLength(1) + "}";


            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;

                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    if (isToroidal) { neighbor = CountNeighborsToroidal(x, y); boundaryType = "Toroidal"; }
                    else { neighbor = CountNeighborsFinite(x, y); boundaryType = "Finite"; }

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);

                    }

                    // Outline the cell with a pen
                    if (isGrid) e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    // Display Neighbors count 
                    if (isNeighborCount) e.Graphics.DrawString(neighbor.ToString(), font, Brushes.Black, cellRect, stringFormat);

                }
            }
            // Update status strip for Interval, Seed, and Lving Cells
            toolStripStatusLabelInterval.Text = "Interval : " + timer.Interval.ToString();
            toolStripStatusLabelSeed.Text = "Seed : " + seed.ToString();
            toolStripStatusAlive.Text = "Alive : " + aliveCells.ToString();
            // Display the Hud
            if (isHud) e.Graphics.DrawString(hud, hudFont, Brushes.IndianRed, hudRect, hudFormat);

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {

            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];
                LivingCells();

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) continue;

                    if (xCheck < 0) continue;
                    if (yCheck < 0) continue;
                    if (xCheck >= xLen) continue;
                    if (yCheck >= yLen) continue;

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;

                    }

                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0) continue;
                    if (xCheck < 0) xCheck = xLen - 1;
                    if (yCheck < 0) yCheck = yLen - 1;
                    if (xCheck >= xLen) xCheck = 0;
                    if (yCheck >= yLen) yCheck = 0;

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;

                    }


                }
            }
            return count;
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // The Play Button
            timer.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // The Pause Button
            timer.Enabled = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // The Next Button
            NextGeneration();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            // The Clear Button
            for (int y = 0; y < universe.GetLength(1); y++)
            {

                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    aliveCells = 0;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Saving a Cells file
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");
                //String[,] sUniverse = new String[5, 5];
                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(0); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(1); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += "O";

                        }
                        else if (universe[x, y] == false)
                        {
                            currentRow += ".";

                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.

                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opening a Cells File
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0; 

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row == "!") continue;


                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    if (row != "!")
                    {
                        maxHeight += 1;
                    }
                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.

                }
                maxHeight -= 1;
                maxWidth = maxHeight;
                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                int yPos = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!")) continue;
                    // If the row is not a comment then 

                    // it is a row of cells and needs to be iterated through.


                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos] == 'O') universe[xPos, yPos] = true;
                        if (row[xPos] == '.') universe[xPos, yPos] = false;
                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                    }
                    yPos += 1;

                }
                graphicsPanel1.Invalidate();
                // Close the file.
                reader.Close();
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The Exit Strip Menu from File
            this.Close();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The Randomize cell by time, Strip Menu from Randomize
            RandomizeTime();
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The Back Color Change, Strip Menu from Setting

            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }

        }

        private void cellsColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The Cells Color Change, Strip Menu from Setting

            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }

        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The Grid Color Change, Strip Menu from Setting

            ColorDialog dlg = new ColorDialog();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The Randomize cell by seed, Strip Menu from Randomize

            ModalDialog dlg = new ModalDialog();
            dlg.SetNumber(seed);
            if (DialogResult.OK == dlg.ShowDialog())
            {
                seed = dlg.GetNumber();
                RandomizeSeed();
                graphicsPanel1.Invalidate();
            }


        }

        private void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // the Options tap is were to change the Time Interval number, and Universe Width and Hieght, Strip Menu from Setting 
            Options_Dialog dlg = new Options_Dialog();
            dlg.SetTimer(timer.Interval);
            dlg.SetNumberWidth(universe.GetLength(0));
            dlg.SetNumberHeight(universe.GetLength(1));

            if (DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval = dlg.GetTimer();
                if (universe.GetLength(0) != dlg.GetNumberWidth() || universe.GetLength(1) != dlg.GetNumberHeight())
                {
                    universe = new bool[dlg.GetNumberWidth(), dlg.GetNumberHeight()];
                    scratchPad = new bool[dlg.GetNumberWidth(), dlg.GetNumberHeight()];
                }
                graphicsPanel1.Invalidate();
            }

        }

        private void finitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Change bool value to false in isToroidal for Finite to be called from graphicPanel1 
            isToroidal = false;
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Change bool value to true in isToroidal for Toroidal to be called from graphicPanel1 
            isToroidal = true;
            graphicsPanel1.Invalidate();
        }

        private void neigborhoodCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Change bool value in isNeighborCount for it be Display, Strip Menu form View
            if (isNeighborCount) isNeighborCount = false;
            else isNeighborCount = true;
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Change bool value in isGrid for it be Display, Strip Menu form View
            if (isGrid) isGrid = false;
            else isGrid = true;
            graphicsPanel1.Invalidate();
        }


        private void optionsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // For Contest sensitive Menu Option, is reusing the function from the Strip Menu Option
            optionsToolStripMenuItem1_Click(sender, e);
        }

        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // For Contest sensitive Menu Color, is reusing the function from the Strip Menu Back Color
            backColorToolStripMenuItem_Click(sender, e);
        }

        private void cellsColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // For Contest sensitive Menu Color, is reusing the function from the Strip Menu Cells Color
            cellsColorToolStripMenuItem_Click(sender, e);
        }

        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // For Contest sensitive Menu Color, is reusing the function from the Strip Menu Grid Color
            gridToolStripMenuItem_Click(sender, e);
        }

        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Change bool value in isHud for it be Display, Strip Menu form View
            if (isHud) isHud = false;
            else isHud = true;
            graphicsPanel1.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Save value when Closing 

            Properties.Settings.Default.PanelColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Seed = seed;
            Properties.Settings.Default.universeWidth = universe.GetLength(0);
            Properties.Settings.Default.universeHeight = universe.GetLength(1);
            Properties.Settings.Default.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // it will rest it default value from Settings.settings file from Properties
            Properties.Settings.Default.Reset();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            seed = Properties.Settings.Default.Seed;
            uniWidth = Properties.Settings.Default.universeWidth;
            uniHeight = Properties.Settings.Default.universeHeight;
            if (universe.GetLength(0) != uniWidth || universe.GetLength(1) != uniHeight)
            {
                universe = new bool[uniWidth, uniHeight];
                scratchPad = new bool[uniWidth, uniHeight];
            }
            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // it will load value from the previous save 
            Properties.Settings.Default.Reload();
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            seed = Properties.Settings.Default.Seed;
            uniWidth = Properties.Settings.Default.universeWidth;
            uniHeight = Properties.Settings.Default.universeHeight;
            if (universe.GetLength(0) != uniWidth || universe.GetLength(1) != uniHeight)
            {
                universe = new bool[uniWidth, uniHeight];
                scratchPad = new bool[uniWidth, uniHeight];
            }
            graphicsPanel1.Invalidate();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            // the New Button, is reusing the Clear Button fuction
            toolStripButton1_Click_1(sender, e);
            generations = 0;
            // but this one set Living cells at zero
            aliveCells = 0;
            graphicsPanel1.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The New Strip Menu From File, is reusing the new Button fuction
            newToolStripButton_Click(sender, e);
        }
    }
}
