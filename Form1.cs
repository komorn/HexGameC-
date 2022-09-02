using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexGame
{
    /*	
         HEX GAME
         Natália Komorníková, 2. ročník, Bioinformatika
         Programovaní v C# zápočtový program
   */
    
    public partial class FormWindow : Form
    {
        enum State { START, GAMEPC, GAMEUSER, WIN, WINPC, END }
        State state;

        Font SmallFont = new Font("Cambria", 10);
        Font MediumFont = new Font("Cambria", 16);
        Font BigFont = new Font("Cambria", 18);

        private static int numOfRows = 6;
        private static int numOfCols = 6;

        private int red = 1; //counting how many buttons are red
        int blue = 0;
        
        private Tuple<int, int> lastMovePC;
        private Tuple<int, int> lastMoveUSER;

        private string winner;

        private int[,,] valuesUSER = new int[6, 6, 2] { { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } } };

        private int[,,] valuesPC = new int[6, 6, 2] { { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } },
        { { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 }, { 500, 0 } } };

        public string[,] colors = null;
        private List<string> routeUSER;
        private List<string> routePC;

        private string[,] createColorsArr()
        {
            // function to create colors array where we will check which field is colored with what color

            string[,] colors = new string[6,6];

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    colors[i, j] = " ";
                }
            }
            return colors;
        }

        List<HexagonalButton> buttons = new List<HexagonalButton>();
        

        private void createAccessibleNameOfButtons()
        {
            // function to create AccessibleName of hexagonal buttons - using this I can get to each button's position in the game field

            int i = 5;
            int j = 5;

            foreach (Control c in Controls)
            {
                if (c is HexagonalButton hb)
                {
                    Tuple<int, int> ij = new Tuple<int, int>(i, j);
                    buttons.Add(hb);
                    hb.AccessibleName = ij.ToString();
                    j--;

                    if (j == -1)
                    {
                        j = 5;
                        i--;
                    }
                } 
            }
        }

        List<int> distanceToLeftSide = new List<int>();
        List<int> distanceToRightSide = new List<int>();
        List<int> distanceToBottomSide = new List<int>();
        List<int> distanceToUpperSide = new List<int>();

        public int mxd;
        public int mxh;

        public int maxDepth = 8; // max depth of the search used by computer
        

        private void SetValue(string mark)
        {
            // function which evaluates how good/bad is this position for user for every field in the whole game field - it uses two functions (numberOfRoadsLeft & numberOfRoadsRight)

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    valuesUSER[i, j, 0] = 0;
                    valuesUSER[i, j, 1] = 0;

                    if (colors[i, j] == " ") 
                    {
                        distanceToLeftSide.Clear();
                        distanceToRightSide.Clear();

                        if (j == 0) //first column
                        {
                            mxd = maxDepth;
                            mxh = 0;
                            valuesUSER[i, j, 1] = NumOfRoadsRight(i, j, mark, -1, -1, 0, 0, mxd); // evaluates the number of roads from the first column to the right
                            NumOfRoadsLeft(i, j, mark, -1, -1, 0, 0, mxh);

                        }

                        else if (j == 5) //last column
                        {
                            mxh = maxDepth;
                            mxd = 0;
                            valuesUSER[i, j, 1] = NumOfRoadsLeft(i, j, mark, -1, -1, 0, 0, mxh); // evaluates the number of roads from the last column to the left
                            NumOfRoadsRight(i, j, mark, -1, -1, 0, 0, mxd);
                        }

                        else
                        {
                            for (int k = 1; k < maxDepth + 1; k++)
                            {
                                mxd = k;
                                mxh = maxDepth - k;
                                valuesUSER[i, j, 1] += ((NumOfRoadsLeft(i, j, mark, -1, -1, 0, 0, mxh)) *
                                    (NumOfRoadsRight(i, j, mark, -1, -1, 0, 0, mxd)));
                                // the number of roads from every other column to the left + to the right
                            }
                        }

                        if ((distanceToLeftSide.Count != 0) && (distanceToRightSide.Count != 0))
                        {
                            valuesUSER[i, j, 0] = (distanceToLeftSide.Min()) + (distanceToRightSide.Min());
                        }

                        else
                        {
                            valuesUSER[i , j, 0] = 500;
                        }

                    }

                }
            }

            return;
        }


        private int NumOfRoadsLeft(int row, int col, string mark, int prevRow, int prevCol, int numRoads, int depth, int mx)
        {
            // function that evaluates the number of roads to the left side and also the distance of these roads

            List<Tuple<int, int>> neighs = new List<Tuple<int, int>>();

            neighs = GetNonMatchingNeighbours(row, col, mark);

            if (colors[row, col] != "r" && col != 0)    
            {
                depth += 1;
            }

            if (col == 0)
            {
                if (colors[row, col] == "r" && depth != 0)      
                {
                    depth -= 1;
                }
                distanceToLeftSide.Add(depth);
            }

            if (col != 0 && depth < mx)
            {
                foreach (var neighbour in neighs)
                {
                    if ((neighbour.Item2 == col - 1) || ((neighbour.Item2 == col) && ((neighbour.Item1 != prevRow) || (neighbour.Item2 != prevCol))))
                    {
                        if (neighbour.Item2 == 0)
                        {
                            numRoads += 1;
                        }

                        numRoads += NumOfRoadsLeft(neighbour.Item1, neighbour.Item2, mark, row, col, 0, depth, mx);

                        if (colors[neighbour.Item1, neighbour.Item2] == "r" && col == 1)          
                        {
                            numRoads = 0;
                        }

                    }
                }
            }

            return numRoads;
        }

        private int NumOfRoadsRight(int row, int col, string mark, int prevRow, int prevCol, int numRoads, int depth, int mx)
        {
            // function that evaluates the number of roads to the right side and also the distance of these roads

            List<Tuple<int, int>> neighs = new List<Tuple<int, int>>();

            neighs = GetNonMatchingNeighbours(row, col, mark);

            if (colors[row, col] != "r" && col != 5)   
            {
                depth += 1;
            }

            if (col == 5)
            {
                if (colors[row, col] == "r" && depth != 0)      
                {
                    depth -= 1;
                }
                distanceToRightSide.Add(depth);
            }

            if (col != 5 && depth < mx)
            {
                foreach (var neighbour in neighs)
                {
                    if ((neighbour.Item2 == col + 1) || ((neighbour.Item2 == col) && ((neighbour.Item1 != prevRow) || (neighbour.Item2 != prevCol))))
                    {
                        if (neighbour.Item2 == 5)
                        {
                            numRoads += 1;
                        }

                        numRoads += NumOfRoadsRight(neighbour.Item1, neighbour.Item2, mark, row, col, 0, depth, mx);

                        if (colors[neighbour.Item1, neighbour.Item2] == "r" && col == 4)       
                        {
                            numRoads = 0;
                        }

                    }
                }
            }

            return numRoads;
        }

        private void SetValueForPC(string mark)
        {
            // function which evaluates how good/bad is this position for PC for every field in the whole game field - it uses two functions (numberOfRoadsUp & numberOfRoadsDown),
            // this function only goes to the depth equal to 3 because that is enough to research possible win to the half of all moves

            for (int i = 0; i < numOfRows; i++)
            {
                for (int j = 0; j < numOfCols; j++)
                {
                    valuesPC[i, j, 0] = 0;
                    valuesPC[i, j, 1] = 0;

                    if (colors[i, j] == " ")
                    {
                        distanceToBottomSide.Clear();
                        distanceToUpperSide.Clear();

                        if (i == 0) //first row
                        {
                            mxd = 3;
                            mxh = 0;
                            valuesPC[i, j, 1] = NumOfRoadsDown(i, j, mark, -1, -1, 0, 0, mxd);
                            NumOfRoadsUp(i, j, mark, -1, -1, 0, 0, mxh);

                        }

                        else if (i == 5) //last row
                        {
                            mxh = 3;
                            mxd = 0;
                            valuesPC[i, j, 1] = NumOfRoadsUp(i, j, mark, -1, -1, 0, 0, mxh);
                            NumOfRoadsDown(i, j, mark, -1, -1, 0, 0, mxd);
                        }

                        else
                        {
                            for (int k = 1; k < maxDepth - 3 + 1; k++)
                            {
                                mxd = k;
                                mxh = maxDepth - 3 - k;
                                valuesPC[i, j, 1] += ((NumOfRoadsUp(i, j, mark, -1, -1, 0, 0, mxh)) *
                                    (NumOfRoadsDown(i, j, mark, -1, -1, 0, 0, mxd)));

                            }
                        }

                        if ((distanceToUpperSide.Count != 0) && (distanceToBottomSide.Count != 0))
                        {
                            valuesPC[i, j, 0] = (distanceToUpperSide.Min()) + (distanceToBottomSide.Min());
                        }
                        else
                        {
                            valuesPC[i, j, 0] = 500;
                        }

                    }

                }
            }

            return;
        }

        private int NumOfRoadsUp(int row, int col, string mark, int prevRow, int prevCol, int numRoads, int depth, int mx)
        {
            // function that evaluates the number of roads to the upper side and also the distance of these roads

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            neighbours = GetNonMatchingNeighbours(row, col, mark);

            if (colors[row, col] != "b" && row != 0)   
            {
                depth += 1;
            }

            if (row == 0)
            {
                if (colors[row, col] == "b" && depth != 0)   
                {
                    depth -= 1;
                }
                distanceToUpperSide.Add(depth);
            }

            if (row != 0 && depth < mx)
            {
                foreach (var neighbour in neighbours)
                {
                    if ((neighbour.Item1 == row - 1) || ((neighbour.Item1 == row) && ((neighbour.Item1 != prevRow) || (neighbour.Item2 != prevCol))))
                    {
                        if (neighbour.Item1 == 0)
                        {
                            numRoads += 1;
                        }

                        numRoads += NumOfRoadsUp(neighbour.Item1, neighbour.Item2, mark, row, col, 0, depth, mx);
                    }
                }
            }

            return numRoads;
        }

        private int NumOfRoadsDown(int row, int col, string mark, int prevRow, int prevCol, int numRoads, int depth, int mx)
        {
            // function that evaluates the number of roads to the bottom side and also the distance of these roads

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            neighbours = GetNonMatchingNeighbours(row, col, mark);

            if (colors[row, col] != "b" && row != 5)   
            {
                depth += 1;
            }

            if (row == 5)
            {
                if (colors[row, col] == "b" && depth != 0)    
                {
                    depth -= 1;
                }
                distanceToBottomSide.Add(depth);
            }

            if (row != 5 && depth < mx)
            {
                foreach (var neighbour in neighbours)
                {
                    if ((neighbour.Item1 == row + 1) || ((neighbour.Item1 == row) && ((neighbour.Item1 != prevRow) || (neighbour.Item2 != prevCol))))
                    {
                        if (neighbour.Item1 == 5)
                        {
                            numRoads += 1;
                        }

                        numRoads += NumOfRoadsDown(neighbour.Item1, neighbour.Item2, mark, row, col, 0, depth, mx);
                    }
                }
            }

            return numRoads;
        }


        public FormWindow()
        {
            InitializeComponent();
            createAccessibleNameOfButtons();
            colors = createColorsArr();
            state = SetState(State.START);
        }

        State SetState(State newState)
        {
            // function to switch which controls are / aren't visible and enabled in each state of the game

            switch (newState)
            {
                case State.START:
                    LPlayersGuide.Visible = false;
                    LPlayersGuide.Text = " The game HEX is a game for two players " +
                        "\n where each player is supposed to connect opposite " +
                        "\n sides of the game plane. " +
                        "\n Players change after each move and each player " +
                        "\n changes the color of one hexagon in his/hers move. " +
                        "\n If a hexagon is already occupied the color cannot be changed." +
                        "\n It is possible to undo one move which removes the last move " +
                        "\n of the player and the last move of PC."+
                        "\n The game always ends with a winner it can never be tied." +
                        "\n To start the game select who beggins.";
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    buttonMenu.Visible = false;
                    buttonEndGame.Visible = false;
                    BInfo.Visible = true;
                    BInfo.Enabled = true;
                    BInfo.Width = 150;
                    BInfo.Height = 55;
                    BInfo.Text = "TUTORIAL";
                    LMove.Visible = true;
                    LMove.Text = "Select who beggins the game: ";
                    RBUser.Visible = true;
                    RBPC.Visible = true;
                    button1.Visible = true;
                    button1.Enabled = true;
                    button1.Width = 250;
                    button1.Height = 100;
                    button1.Left = (this.ClientSize.Width - button1.Width) / 2;
                    button1.Top = ((this.ClientSize.Height - button1.Height) / 2) -100 ;
                    button1.BackColor = Color.Black;
                    BInfo.Left = ((this.ClientSize.Width - BInfo.Width) / 2);
                    BInfo.Top = ((this.ClientSize.Height - RBUser.Height) / 2) + 95;
                    BInfo.BackColor = Color.Black;
                    LMove.Left = (this.ClientSize.Width - LMove.Width) / 2;
                    LMove.Top = ((this.ClientSize.Height - LMove.Height) / 2) ;
                    RBUser.Left = ((this.ClientSize.Width - RBUser.Width) / 2) -40 ;
                    RBUser.Top = ((this.ClientSize.Height - RBUser.Height) / 2) + 50;
                    RBUser.BackColor = Color.Transparent;
                    RBPC.BackColor = Color.Transparent;
                    LMove.BackColor = Color.Transparent;
                    RBPC.Left = RBUser.Left + 100;
                    RBPC.Top = RBUser.Top;
                    LWinner.Visible = false;
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                    pictureBox3.Visible = false;
                    pictureBox4.Visible = false;

                    foreach (var but in buttons)
                    {
                        but.Visible = false;
                        but.Enabled = false;
                    }
                    pictureBoxBackground.Visible = true;
                    break;
                case State.GAMEPC:
                    LPlayersGuide.Visible = false;
                    button1.Visible = false;
                    button3.BackColor = Color.Black;
                    button4.BackColor = Color.Black;
                    button3.ForeColor = Color.MediumOrchid;
                    button4.ForeColor = button3.ForeColor;
                    button3.Visible = true;
                    button4.Visible = true;
                    buttonMenu.Visible = false;
                    buttonEndGame.Visible = false;
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox3.Visible = true;
                    pictureBox4.Visible = true;
                    LMove.Visible = false;
                    RBPC.Visible = false;
                    RBUser.Visible = false;
                    BInfo.Visible = false;
                    int count = 0;

                    foreach (var but in buttons)
                    {
                        but.Enabled = false;
                        but.Visible = true;
                        if (but.BackColor == Color.Red)
                        {
                            count += 1;
                        }
                    }
                    button2.Visible = false;
                    LMove.Visible = false;
                    LMove.Text = "Select who beggins the game: ";
                    RBUser.Visible = true;
                    RBPC.Visible = true;

                    if (count == red)
                    {
                        Tuple<int, int> rowcol = gamePC();
                        lastMovePC = Tuple.Create(rowcol.Item1, rowcol.Item2);
                        PcButtonClick(rowcol);
                        if (state == State.WIN)
                        {
                            newState = SetState(State.WINPC);
                        }
                        red++;
                    }
                    else if (RBPC.Checked == true)
                    {
                        Tuple<int, int> rowcol = gamePC();
                        PcButtonClick(rowcol);
                    }
                    Tuple<bool, HashSet<Tuple<int, int>>> bfsPC = BfsHexPC("b");

                    if (bfsPC.Item1 == false)
                    {
                        newState = SetState(State.GAMEUSER);
                    }
                    pictureBoxBackground.Visible = true;
                    break;
                case State.GAMEUSER:
                    LPlayersGuide.Visible = false;
                    button1.Visible = false;
                    button3.BackColor = Color.Black;
                    button4.BackColor = Color.Black;
                    button3.ForeColor = Color.MediumOrchid;
                    button4.ForeColor = button3.ForeColor;
                    button3.Visible = true;
                    button4.Visible = true;
                    buttonMenu.Visible = false;
                    buttonEndGame.Visible = false;
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox3.Visible = true;
                    pictureBox4.Visible = true;
                    LMove.Visible = false;
                    RBPC.Visible = false;
                    RBUser.Visible = false;
                    BInfo.Visible = false;

                    foreach (var but in buttons)
                    {
                        but.Enabled = true;
                        but.Visible = true;
                    }
                    pictureBoxBackground.Visible = true;
                    break;
                case State.WIN:
                    LPlayersGuide.Visible = false;
                    LMove.Visible = false;
                    RBPC.Visible = false;
                    RBUser.Visible = false;
                    button1.Visible = false;
                    button2.Visible = false;
                    button2.Enabled = true;
                    button2.Width = 250;
                    button2.Height = 100;
                    button2.Left = (this.ClientSize.Width - button2.Width) / 2;
                    button2.Top = ((this.ClientSize.Height - button2.Height) / 2)+100;
                    button2.BackColor = Color.Black;
                    button2.ForeColor = Color.Turquoise;
                    button3.Visible = false;
                    button4.Visible = false;
                    buttonEndGame.Visible = false;
                    LWinner.Font = MediumFont;
                    LWinner.ForeColor = Color.Red;
                    LWinner.Visible = true;
                    LWinner.Left = ((this.ClientSize.Width - LWinner.Width) / 2) + 280;
                    LWinner.Top = ((this.ClientSize.Height - LWinner.Height) / 2);
                    buttonMenu.Top = LWinner.Top + 50;
                    buttonMenu.Left = LWinner.Left + 120;
                    buttonMenu.Width = 150;
                    buttonMenu.Height = 65;
                    buttonMenu.BackColor = Color.Black;
                    buttonMenu.ForeColor = Color.MediumSlateBlue;
                    buttonMenu.Font = BigFont;
                    buttonMenu.Visible = true;

                    foreach (var but in buttons)
                    {
                        but.Enabled = false;
                        but.Visible = true;
                    }
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox3.Visible = true;
                    pictureBox4.Visible = true;
                    pictureBoxBackground.Visible = true;
                    HighlightRouteOfWinner(routeUSER);
                    break;
                case State.WINPC:
                    LPlayersGuide.Visible = false;
                    LMove.Visible = false;
                    RBPC.Visible = false;
                    RBUser.Visible = false;
                    button1.Visible = false;
                    button2.Visible = false;
                    button2.Enabled = true;
                    button2.Width = 250;
                    button2.Height = 100;
                    button2.Left = (this.ClientSize.Width - button2.Width) / 2;
                    button2.Top = ((this.ClientSize.Height - button2.Height) / 2) + 150;
                    button2.BackColor = Color.Black;
                    button2.ForeColor = Color.Turquoise;
                    buttonEndGame.Visible = false;
                    LWinner.Font = MediumFont;
                    LWinner.ForeColor = Color.Blue;
                    LWinner.Visible = true;
                    LWinner.Left = ((this.ClientSize.Width - LWinner.Width) / 2) + 280;
                    LWinner.Top = ((this.ClientSize.Height - LWinner.Height) / 2);
                    button3.Visible = false;
                    button4.Visible = false;
                    buttonMenu.Top = LWinner.Top + 50;
                    buttonMenu.Left = LWinner.Left + 120;
                    buttonMenu.Width = 150;
                    buttonMenu.Height = 65;
                    buttonMenu.BackColor = Color.Black;
                    buttonMenu.ForeColor = Color.MediumSlateBlue;
                    buttonMenu.Font = BigFont;
                    buttonMenu.Visible = true;

                    foreach (var but in buttons)
                    {
                        but.Enabled = false;
                        but.Visible = true;
                    }
                    pictureBox1.Visible = true;
                    pictureBox2.Visible = true;
                    pictureBox3.Visible = true;
                    pictureBox4.Visible = true;
                    pictureBoxBackground.Visible = true;
                    HighlightRouteOfWinner(routePC);
                    break;
                case State.END:
                    LPlayersGuide.Visible = false;
                    button2.Visible = true;
                    button2.Enabled = true;
                    button2.Width = 150;
                    button2.Height = 70;
                    button2.Left = (this.ClientSize.Width - button2.Width) / 2;
                    button2.Top = ((this.ClientSize.Height - button2.Height) / 2)+100;
                    button2.BackColor = Color.Black;
                    button2.ForeColor = Color.Turquoise;
                    buttonEndGame.Visible = true;
                    buttonEndGame.Text = "END GAME";
                    buttonEndGame.Enabled = true;
                    buttonEndGame.Width = 200;
                    buttonEndGame.Height = 60;
                    buttonEndGame.Left = (this.ClientSize.Width - buttonEndGame.Width) / 2;
                    buttonEndGame.Top = button2.Top + 95;
                    buttonEndGame.BackColor = Color.Black;
                    buttonEndGame.ForeColor = Color.Red;
                    buttonEndGame.Font = MediumFont;
                    buttonMenu.Visible = false;
                    LWinner.Text += "!!";
                    LWinner.Visible = true;
                    LWinner.Font = BigFont;
                    LWinner.Left = ((this.ClientSize.Width - LWinner.Width) / 2);
                    LWinner.Top = ((this.ClientSize.Height - LWinner.Height) / 2);
                    LMove.Visible = false;
                    RBPC.Visible = false;
                    RBUser.Visible = false;

                    foreach (var but in buttons)
                    { 
                        but.Enabled = false;
                        but.Visible = false;
                    }
                    pictureBox1.Visible = false;
                    pictureBox2.Visible = false;
                    pictureBox3.Visible = false;
                    pictureBox4.Visible = false;
                    pictureBoxBackground.Visible = true;
                    break;
                default:
                    break;
            }
            return newState;
        }

        private void hexagonalButton3_Click(object sender, EventArgs e)
        {

        }

        private void UserButtonClick (object sender, EventArgs e)
        {
            // function that changes the properties of the chosen button of that user clicks

            foreach(var but in buttons)
            {
                but.Text = "HEX";
                but.ForeColor = Color.SlateBlue;
            }

            if (state == State.GAMEUSER)
            {
                if (((HexagonalButton)sender).BackColor == Color.Blue || ((HexagonalButton)sender).BackColor == Color.Red)
                {
                    MessageBox.Show("This hexagon is already occupied. Choose another one.");
                    state = SetState(State.GAMEUSER);
                    return;
                }
                
                ((HexagonalButton)sender).BackColor = System.Drawing.Color.Red;
                ((HexagonalButton)sender).Text = "HEX";
                ((HexagonalButton)sender).Enabled = false;

                string position = ((HexagonalButton)sender).AccessibleName;
                string[] splitPos = position.Split(",");
                int i = int.Parse(splitPos[0].Substring(1));
                int j = int.Parse(splitPos[1].Remove(splitPos[1].Length - 1));
                lastMoveUSER = Tuple.Create(i, j);
                colors[i, j] = "r";                  // change to RED

                Tuple<bool, HashSet<Tuple<int, int>>> bfsUSER = BfsHexUSER("r");
                routeUSER = HexWin(bfsUSER.Item2, "r");

                if (bfsUSER.Item1 == true)
                {
                    state = SetState(State.WIN);
                    HexWin(bfsUSER.Item2, "r");
                    return;
                }

                state = SetState(State.GAMEPC);
            }

            else if (state == State.START)
            {
                ((HexagonalButton)sender).BackColor = System.Drawing.Color.LightGray;
                ((HexagonalButton)sender).Text = "HEX";
                MessageBox.Show("Pre spustenie hry stlačte tlačítko START");
            }

            else if (state == State.GAMEPC)
            {
                ((HexagonalButton)sender).BackColor = System.Drawing.Color.LightGray;
                ((HexagonalButton)sender).Text = "HEX";
            }


        }

        private Tuple<int, int> gamePC()
        {
            // function that returns row & col which makes the move of the PC using the functions that evaluate how good or bad is each field for both user and PC
            // this function chooses the best move that blocks the user and secondly is the best move for the PC

            SetValue("b");
            SetValueForPC("r");

            Tuple<int, int, int, int> bestMove = new Tuple<int, int, int, int>(0, 0, 0, 10);

            for (int row = 0; row < numOfRows; row++)
            {
                for (int col = 0; col < numOfCols; col++)
                {
                    if (colors[row, col] == " ")
                    {
                        if (valuesPC[row, col, 0] == 0) //PC missing 1 move to win
                        {
                            Tuple<int, int, int, int> move = new Tuple<int, int, int, int>(row, col, valuesPC[row, col, 1], valuesPC[row, col, 0]);
                            bestMove = move;
                        }
                        else
                        {
                            if (valuesUSER[row, col, 0] < bestMove.Item4)
                            {
                                var move = Tuple.Create(row, col, valuesUSER[row, col, 1], valuesUSER[row, col, 0]);
                                bestMove = move;
                            }
                            else if (valuesUSER[row, col, 0] == bestMove.Item4 &&
                                valuesUSER[row, col, 1] >= bestMove.Item3)
                            {
                                var move = Tuple.Create(row, col, valuesUSER[row, col, 1], valuesUSER[row, col, 0]);
                                bestMove = move;
                            }
                        }
                    }
                    
                }
            }

            var rowCol = Tuple.Create(bestMove.Item1, bestMove.Item2);

            colors[bestMove.Item1, bestMove.Item2] = "b";

            return rowCol;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // start button event

            if (RBUser.Checked == true)
            {
                state = SetState(State.GAMEUSER);
            }
            else if (RBPC.Checked == true)
            {
                state = SetState(State.GAMEPC);
            }
            
        }

        private void BInfo_Click(object sender, EventArgs e)
        {
            // tutorial button event

            LPlayersGuide.BackColor = Color.Transparent;
            LPlayersGuide.Visible = true;

            if (state == State.START)
            {
                LPlayersGuide.Left = ((this.ClientSize.Width - LPlayersGuide.Width) / 2) +3;
                LPlayersGuide.Top = ((this.ClientSize.Height - LPlayersGuide.Height) / 2) + 245;
            }
        }
        
        public List<Tuple<int, int>> moves = new List<Tuple<int, int>>
        {
            // function that creates the list of all possible directions of connection with a button

            Tuple.Create( -1, -1 ),
            Tuple.Create( -1, 0 ),
            Tuple.Create( 0, 1 ),
            Tuple.Create( 1, 0 ),
            Tuple.Create( 1, 1 ),
            Tuple.Create( 0, -1 ),

        };

        
        public List<Tuple<int, int>> GetMatchingNeighbours(int row, int col, string mark) 
        {
            // function that returns all neighbours with the same color as the given button in row & col

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            foreach (var move in moves)
            {
                int i = move.Item1;
                int j = move.Item2;

                if ((row + i >= 0) && (row + i < numOfRows) && (col + j >= 0) && (col + j < numOfCols))
                {
                    if (colors[row + i,col + j] == mark)
                    {
                        neighbours.Add(Tuple.Create(row + i, col + j));
                    }
                } 
            }

            return neighbours;
        }

        public List<Tuple<int, int>> GetNonMatchingNeighbours(int row, int col, string mark)
        {
            // function that returns all neighbours with different color of the given button or empty neighbour of given row & col

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            foreach (var move in moves)
            {
                int i = move.Item1;
                int j = move.Item2;

                if ((row + i >= 0) && (row + i < numOfRows) && (col + j >= 0) && (col + j < numOfCols))
                {
                    if (colors[row + i, col + j] != mark)
                    {
                        neighbours.Add(Tuple.Create(row + i, col + j));
                    }
                }
            }

            return neighbours;
        }

        void PcButtonClick(Tuple<int,int> button)
        {
            // function that changes the color and other properties of the chosen button of PC's move

            Tuple<bool,HashSet<Tuple<int, int>>> bfsPC = BfsHexPC("b");

            routePC = HexWin(bfsPC.Item2, "b");

            string row = button.Item1.ToString();
            string col = button.Item2.ToString();
            string move = "("+row+ ", " + col+")";        //(1, 1)

            foreach (var hb in buttons)
            {
                if (hb.AccessibleName == move)
                {
                    hb.BackColor = Color.Blue;
                    hb.Text = "HEX";
                    blue++;
                }
            }

            if (bfsPC.Item1 == true)
            {
                state = SetState(State.WINPC);
                HexWin(bfsPC.Item2, "b");
                return;
            }
            else if (bfsPC.Item1 == false)
            {
                state = SetState(State.GAMEUSER);
            }
            
        }

        public Tuple<bool, HashSet<Tuple<int,int>>> BfsHexUSER(string mark)
        {
            // breadth first search to check if the right and left sides are connected - if yes USER wins the game

            Queue<Tuple<int, int>> queueLeft = new Queue<Tuple<int, int>>();
            Queue<Tuple<int, int>> queueRight = new Queue<Tuple<int, int>>();
            HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();
            _ = new List<Tuple<int, int>>();

            for (int i =0; i < numOfRows; i++)
            {
                if (colors[i,0] == mark)
                {
                    Tuple<int,int> pos = Tuple.Create(i, 0);
                    queueLeft.Enqueue(pos);
                }
            }

            for (int i = 0; i < numOfRows; i++)
            {
                if (colors[i, 5] == mark)
                {
                    Tuple<int, int> pos = Tuple.Create(i, numOfRows-1);
                    queueRight.Enqueue(pos);
                }
            }

            while (queueLeft.Count != 0)
            {
                Tuple<int, int> rowCol = queueLeft.Dequeue();

                if (visited.Contains(rowCol))
                {
                    continue;
                }

                visited.Add(rowCol);
                int row = rowCol.Item1;
                int col = rowCol.Item2;
                List<Tuple<int, int>> neighs = GetMatchingNeighbours(row, col, mark);

                foreach (var neighbour in neighs)
                {
                    if (!visited.Contains(neighbour))
                    {
                        if (queueRight.Contains(neighbour))
                        {
                            visited.Add(neighbour);
                            Tuple<bool, HashSet<Tuple<int, int>>> winRes = Tuple.Create(true, visited);
                            winner = "user";
                            return winRes;
                        }

                    }
                    queueLeft.Enqueue(neighbour);
                }

            }

            Tuple<bool, HashSet<Tuple<int, int>>> failRes = Tuple.Create(false, visited);
            return failRes;
        }

        public Tuple<bool, HashSet<Tuple<int, int>>> BfsHexPC(string mark)
        {
            // breadth first search to check if the top and bottom sides are connected - if yes PC wins the game

            Queue<Tuple<int, int>> queueTop = new Queue<Tuple<int, int>>();
            Queue<Tuple<int, int>> queueBottom = new Queue<Tuple<int, int>>(); 
            HashSet<Tuple<int, int>> visited = new HashSet<Tuple<int, int>>();
            _ = new List<Tuple<int, int>>();

            for (int i = 0; i < numOfCols; i++)
            {
                if (colors[0, i] == mark)
                {
                    Tuple<int, int> pos = Tuple.Create(0, i);
                    queueTop.Enqueue(pos);
                }
            }

            for (int i = 0; i < numOfCols; i++)
            {
                if (colors[5, i] == mark)
                {
                    Tuple<int, int> pos = Tuple.Create(numOfCols-1, i);
                    queueBottom.Enqueue(pos);
                    
                }
            }

            while (queueTop.Count != 0)
            {
                Tuple<int, int> rowCol = queueTop.Dequeue();

                if (visited.Contains(rowCol))
                {
                    continue;
                }

                visited.Add(rowCol);
                int row = rowCol.Item1;
                int col = rowCol.Item2;
                List<Tuple<int, int>> neighs = GetMatchingNeighbours(row, col, mark);

                foreach (var neighbour in neighs)
                {
                    if (!visited.Contains(neighbour))
                    {
                        if (queueBottom.Contains(neighbour))
                        {
                            visited.Add(neighbour);
                            Tuple<bool, HashSet<Tuple<int, int>>> winRes = Tuple.Create(true, visited);
                            winner = "pc";
                            return winRes;
                        }
                    }
                    queueTop.Enqueue(neighbour);
                }

            }

            Tuple<bool, HashSet<Tuple<int, int>>> failRes = Tuple.Create(false, visited);
            return failRes;
        }

        private void RemoveLastMove(Tuple<int,int> lastU, Tuple <int,int> lastPC)
        {
            // function to remove the last move of the PC

            int rowU = lastU.Item1;     
            int colU = lastU.Item2;

            int rowPC = lastPC.Item1;
            int colPC = lastPC.Item2;

            colors[rowU, colU] = " ";
            colors[rowPC, colPC] = " ";

            string rowUSER = lastU.Item1.ToString();
            string colUSER = lastU.Item2.ToString();
            string moveUSER = "(" + rowUSER + ", " + colUSER + ")";        //(1, 1)

            string rowPCs = lastPC.Item1.ToString();
            string colPCs = lastPC.Item2.ToString();
            string movePC = "(" + rowPCs + ", " + colPCs + ")";

            foreach (var hb in buttons)
            {
                if (hb.AccessibleName == moveUSER)
                {
                    hb.BackColor = Color.LightGray;
                    hb.Text = "HEX";
                    red--;
                }
                else if(hb.AccessibleName == movePC)
                {
                    hb.BackColor = Color.LightGray;
                    hb.Text = "HEX";
                }
            }

            lastMovePC = null;
            lastMoveUSER = null;
        }

        private List<string> HexWin(HashSet<Tuple<int, int>> route, string player)
        {
            // function that creates the label that gives us the winner and returns all the moves of the winning route

            List<string> moves = new List<string>();
            
            if (player == "b")
            {
                LWinner.ForeColor = Color.Blue;
                LWinner.Text = "BLUE PLAYER IS THE WINNER";

                foreach (var but in route)
                {
                    string row = but.Item1.ToString();
                    string col = but.Item2.ToString();
                    string accessName = "(" + row + ", " + col + ")";
                    moves.Add(accessName);
                }
            }
            else if(player == "r")
            {

                LWinner.ForeColor = Color.Red;
                LWinner.Text = "RED PLAYER IS THE WINNER";

                foreach (var but in route)
                {
                    string row = but.Item1.ToString();
                    string col = but.Item2.ToString();
                    string accessName = "(" + row + ", " + col + ")";
                    moves.Add(accessName);
                }
            } 

            return moves;

        }

        private void UndoLastMove(object sender, EventArgs e)
        {
            // function to undo the last move of the USER it removes the move using function RemoveLastMove

            if (state == State.WIN)
            {
                Tuple<bool, HashSet<Tuple<int, int>>> bfsPC = BfsHexPC("b");
                Tuple<bool, HashSet<Tuple<int, int>>> bfsU = BfsHexUSER("r");

                if (bfsU.Item1 == true)
                {
                    MessageBox.Show("Unable to undo last move - RED player is the WINNER.");
                }

                else if (bfsPC.Item1 == true)
                {
                    MessageBox.Show("Unable to undo last move - BLUE player is the WINNER.");
                }

                return;
            }
            else if (red == 1)
            {
                MessageBox.Show("No moves to undo.");
                return;
            }

            RemoveLastMove(lastMoveUSER,lastMovePC);
        }


        private void ShowLastPCMove(object sender, EventArgs e)
        {
            // function that shows us the last move of the PC in the game field

            if (lastMovePC == null || lastMoveUSER == null)
            {
                MessageBox.Show("No PC move to show beacuse of UNDO.");
                return;
            }

            button4.Visible = false;
            string rowPCs = lastMovePC.Item1.ToString();
            string colPCs = lastMovePC.Item2.ToString();
            string movePC = "(" + rowPCs + ", " + colPCs + ")";

            foreach(var hb in buttons)
            {
                if (hb.AccessibleName == movePC)
                {
                    hb.Text = "HERE";
                    hb.ForeColor = System.Drawing.Color.SpringGreen;
                }
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // function to restart the game

            colors = createColorsArr();
            state = SetState(State.START);

            foreach (var but in buttons)
            {
                but.BackColor = Color.LightGray;
                but.ForeColor = Color.SlateBlue;
                but.Text = "HEX";
            }

            red = 1;

        }

        private void HighlightRouteOfWinner(List<string> moves)
        {
            // function to highlight the winner's route

            foreach (var hb in buttons)
            {
                if (moves.Contains(hb.AccessibleName))
                {
                    hb.BackColor = Color.GreenYellow;
                    hb.Text = "HEX";
                    red--;
                }
            }
        }

        private void buttonMenu_Click(object sender, EventArgs e)
        {
            // event for the button MENU that gets us to END game state where we can chose if we want to play again or to exit the game

            state = SetState(State.END);
        }

        private void buttonEndGame_Click(object sender, EventArgs e)
        {
            // event for the button end game - exiting the application

            Application.Exit();
        }
    }
}