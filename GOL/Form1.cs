using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GOL
{
    public partial class Form1 : Form
    {
        int uniHeight = 10;
        int uniWidth = 10;
        int interval = 100;
        // The universe array
        bool[,] universe = new bool[10, 10];

        //The scratchpad array
        bool[,] scratchpad = new bool[10, 10];


        // Drawing colors
        Color backColor = Color.White;
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = interval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            toolStripStatusLabelInterval.Text = $"Interval = {interval.ToString()}";
        }

        //Counting for Finite
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

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                        continue;

                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                        continue;
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                        continue;
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                        continue;
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                        continue;

                    if (universe[xCheck, yCheck] == true)
                        count++;

                }

            }

            return count;

        }

        //Counting for Torodial
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

                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                        continue;
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                        xCheck = xLen - 1;
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                        yCheck = yLen - 1;
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                        xCheck = 0;
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                        yCheck = 0;


                    if (universe[xCheck, yCheck] == true) count++;

                }

            }

            return count;

        }

        //Swap universe and scratchpad
        private void Swap()
        {
            //copy from scratchpad to universe
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = scratchpad[x, y];
                }
            }
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            //
            scratchpad = new bool[universe.GetLength(0), universe.GetLength(1)];
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count;
                    if (torodialToolStripMenuItem.Checked && !finiteToolStripMenuItem.Checked)
                        count = CountNeighborsToroidal(x, y);
                    else
                        count = CountNeighborsFinite(x, y);

                    //Apply rules
                    if (universe[x, y] != true)
                    {
                        if (count == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (count == 2 || count == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                        else
                        {
                            scratchpad[x, y] = false;
                        }
                    }
                }
            }

            //copy from scratchpad to universe
            Swap();

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //Update status strip Alive
            UpdateLabelAlive();
            //redraw panel
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        
        //Updates Alive status strip when called
        private void UpdateLabelAlive()
        {
            int alive = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y] == true)
                        alive++;
                }
            }
            toolStripStatusLabelAlive.Text = "Alive = " + alive;
        }
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            //Background color
            graphicsPanel1.BackColor = backColor;

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            //HUD String Format
            StringFormat HudString = new StringFormat();
            HudString.Alignment = StringAlignment.Near;
            HudString.LineAlignment = StringAlignment.Far;

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    //RectangleF.
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;


                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen only if grid is checkd
                    if (gridToolStripMenuItem.Checked == true)
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    //Neighbor count in cell, only triggers if checked
                    if (neighborCountToolStripMenuItem.Checked)
                    {
                        Font font = new Font("Arial", 10f);

                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        int neighbors;
                        if (torodialToolStripMenuItem.Checked && !finiteToolStripMenuItem.Checked)
                            neighbors = CountNeighborsToroidal(x, y);
                        else
                            neighbors = CountNeighborsFinite(x, y);
                        if (universe[x,y] == false)
                        {
                            if (neighbors == 3)
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            else if (neighbors != 0)
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                        }
                        else
                        {
                            if (neighbors == 2 || neighbors == 3)
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            else
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, cellRect, stringFormat);
                        }
                       
                    }
                }
            }

            //HUD
            if (hudToolStripMenuItem.Checked)
            {
                string hudtext;
                int alive = 0;
                Font font = new Font("Arial", 16f);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                            alive++;
                    }
                }
                Rectangle HUD = new Rectangle(0, 3, graphicsPanel1.Width, graphicsPanel1.Height);
                if (torodialToolStripMenuItem.Checked)
                    hudtext = $"Generation = {generations}\nCell Count = {alive}\nBoundary type = Toroidal\nInterval = {interval}\n" +
                        $"Universe Width = {uniWidth} Universe Height = {uniHeight}";
                else
                    hudtext = $"Generation = {generations}\nCell Count = {alive}\nBoundary type = Finite\nInterval = {interval}\n" +
                        $"Universe Width = {uniWidth} Universe Height = {uniHeight}";
                Brush brush = new SolidBrush(Color.Orange);
                e.Graphics.DrawString(hudtext, font, brush, HUD, HudString);
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                //FLOATS!!!
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state

                universe[x, y] = !universe[x, y]; // Clicking out of bounds breaks here

                //Update Alive
                UpdateLabelAlive();

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
                //Invalidate will be called everytime we repaint
                //NEVER PLACE IN PAINT
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Next Button
        private void nextToolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        //Play
        private void playToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        //Pause
        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        //clears universe
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;

                }
            }
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        //Randomize from time
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Randomizes based on time
            Random rng = new Random();
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int num = rng.Next(0, 2);
                    if (num == 0)
                        scratchpad[x, y] = true;
                    else
                        scratchpad[x, y] = false;
                }
            }
            Swap();
            UpdateLabelAlive();
            graphicsPanel1.Invalidate();
        }

        //Save Function
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                writer.WriteLine($"!{DateTime.Now.ToString()}");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then write O (Capital o)
                        if (universe[x, y] == true)
                            writer.Write("O");
                        // Else if the universe[x,y] is dead then Write '.' (period)
                        else
                            writer.Write(".");
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine("");
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        //Open/Import
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

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
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    else
                    {
                        // Increment the maxHeight variable for each row read.
                        maxHeight++;

                        // Get the length of the current row string
                        // and adjust the maxWidth variable if necessary.
                        maxWidth = row.Length;

                    }
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                scratchpad = new bool[maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                int yPos = 0;
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    else
                    {

                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                scratchpad[xPos, yPos] = true;
                            }
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            else
                            {
                                scratchpad[xPos, yPos] = false;
                            }
                        }
                        //Increment y after each row
                        yPos++;
                    }

                }
                //Swap Scratchpad and Universe
                Swap();
                // Close the file.
                reader.Close();
                graphicsPanel1.Invalidate();
            }
        }

        //button uses menu logic
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem_Click(sender, e);
        }

        //Uses Open method
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }
        //Uses Open method
        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem_Click(sender, e);
        }
        //Random from seed
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Dialog box
            SeedDialog seedDialog = new SeedDialog();
            seedDialog.ShowDialog();

            //Randomizing
            Random rng = new Random(seedDialog.GetSeed());
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int num = rng.Next(0, 2);
                    if (num == 0)
                        scratchpad[x, y] = true;
                    else
                        scratchpad[x, y] = false;
                }
            }
            Swap();
            UpdateLabelAlive();
            graphicsPanel1.Invalidate();
        }
        //Options menu
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.ShowDialog();
            uniWidth = options.GetWidth();
            uniHeight = options.GetHegiht();
            interval = options.GetInterval();
            universe = new bool[uniWidth,uniHeight];
            scratchpad = new bool[uniWidth,uniHeight];
            if (interval > 0)
                timer.Interval = interval;
            graphicsPanel1.Invalidate();
            toolStripStatusLabelInterval.Text = $"Interval = {interval.ToString()}";
        }

        //View Menu
        //Toggle Grid
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem.Checked == true)
            {
                gridToolStripMenuItem.Checked = false;
                gridColorToolStripMenuItem1.Checked = false;
                //Turn off grid

            }
            else
            {
                gridToolStripMenuItem.Checked = true;
                gridColorToolStripMenuItem1.Checked = true;
                //Turn on grid
            }
            graphicsPanel1.Invalidate();
        }
        //Toggle Neighbor count
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (neighborCountToolStripMenuItem.Checked == true)
            {
                neighborCountToolStripMenuItem.Checked = false;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }
        //Toroidal Menu
        private void torodialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!torodialToolStripMenuItem.Checked)
            {
                torodialToolStripMenuItem.Checked = true;
                finiteToolStripMenuItem.Checked = false;
            }
        }
        //Finite menu
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!finiteToolStripMenuItem.Checked)
            {
                finiteToolStripMenuItem.Checked = true;
                torodialToolStripMenuItem.Checked = false;
            }
        }
        //Hud menu
        private void hUDToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (hUDToolStripMenuItem1.Checked)
            {
                hUDToolStripMenuItem1.Checked = false;
                hudToolStripMenuItem.Checked = false;
            }
            else
            {
                hUDToolStripMenuItem1.Checked = true;
                hudToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        //Run Menu
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            playToolStripButton_Click(sender, e);
        }
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pauseToolStripButton_Click(sender, e);
        }
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nextToolStripButton_Click(sender, e);
        }

        //Color Menu
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            backColor = colorDialog.Color;
            graphicsPanel1.Invalidate();
        }
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            cellColor = colorDialog.Color;
            graphicsPanel1.Invalidate();
        }
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            gridColor = colorDialog.Color;
            graphicsPanel1.Invalidate();
        }

        //Context Menu Below
        private void backColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            backColorToolStripMenuItem_Click(sender, e);
            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            cellColorToolStripMenuItem_Click(sender, e);
            graphicsPanel1.Invalidate();
        }

        private void gridColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gridColorToolStripMenuItem_Click(sender , e);
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (gridColorToolStripMenuItem1.Checked)
            {
                gridColorToolStripMenuItem1.Checked = false;
                gridColorToolStripMenuItem.Checked = false;
            }
            else
            {
                gridColorToolStripMenuItem1.Checked = true;
                gridColorToolStripMenuItem.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void neighborToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (neighborToolStripMenuItem1.Checked)
            {
                neighborToolStripMenuItem1.Checked = false;
                neighborCountToolStripMenuItem.Checked = false;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked= true;
                neighborToolStripMenuItem1.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }

        private void hudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hudToolStripMenuItem.Checked)
            {
                hudToolStripMenuItem.Checked = false;
                hUDToolStripMenuItem1.Checked = false;
            }
            else
            {
                hudToolStripMenuItem.Checked = true;
                hUDToolStripMenuItem1.Checked = true;
            }
            graphicsPanel1.Invalidate();
        }
    }
}
