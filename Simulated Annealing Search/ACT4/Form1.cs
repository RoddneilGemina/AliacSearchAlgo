using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace ACT4
{
    public partial class Form1 : Form
    {
        private const int n = 6;
        private int side;
        private SixState startState;
        private SixState currentState;
        private int moveCounter;
        private int[,] heuristicTable;
        private ArrayList bestMoves;
        private Point? chosenMove;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            side = pictureBox1.Width / n;
            startState = GenerateRandomSixState();
            currentState = new SixState(startState);
            moveCounter = 0;
            UpdateUI();
        }

        private void UpdateUI()
        {
            pictureBox1.Refresh();
            pictureBox2.Refresh();

            label1.Text = $"Attacking pairs: {GetAttackingPairs(startState)}";
            label3.Text = $"Attacking pairs: {GetAttackingPairs(currentState)}";
            label4.Text = $"Moves: {moveCounter}";

            heuristicTable = GetHeuristicTable(currentState);
            bestMoves = GetBestMoves(heuristicTable);

            listBox1.Items.Clear();
            foreach (Point move in bestMoves)
            {
                listBox1.Items.Add(move);
            }

            chosenMove = bestMoves.Count > 0 ? ChooseMove(bestMoves) : null;
            label2.Text = $"Chosen move: {chosenMove}";
            label5.Text = $"Possible Moves (H={heuristicTable[0, 0]})";
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e.Graphics, startState, Brushes.Fuchsia, Brushes.Blue);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard(e.Graphics, currentState, Brushes.Fuchsia, Brushes.Black);
        }

        private void DrawBoard(Graphics graphics, SixState state, Brush queenBrush, Brush squareBrush)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    graphics.FillRectangle((i + j) % 2 == 0 ? squareBrush : Brushes.White, i * side, j * side, side, side);
                    if (j == state.Y[i])
                        graphics.FillEllipse(queenBrush, i * side, j * side, side, side);
                }
            }
        }

        private SixState GenerateRandomSixState()
        {
            Random random = new Random();
            return new SixState(random.Next(n), random.Next(n), random.Next(n), random.Next(n), random.Next(n), random.Next(n));
        }

        private int GetAttackingPairs(SixState state)
        {
            int attackers = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (state.Y[i] == state.Y[j] || Math.Abs(state.Y[i] - state.Y[j]) == j - i)
                        attackers++;
                }
            }
            return attackers;
        }

        private int[,] GetHeuristicTable(SixState state)
        {
            int[,] table = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    SixState possibleState = new SixState(state) { Y = { [i] = j } };
                    table[i, j] = GetAttackingPairs(possibleState);
                }
            }
            return table;
        }

        private ArrayList GetBestMoves(int[,] heuristicTable)
        {
            ArrayList bestMoves = new ArrayList();
            int bestHeuristic = heuristicTable[0, 0];
            Random random = new Random();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (heuristicTable[i, j] < bestHeuristic)
                    {
                        bestHeuristic = heuristicTable[i, j];
                        bestMoves.Clear();
                    }
                    if (heuristicTable[i, j] == bestHeuristic && currentState.Y[i] != j)
                    {
                        bestMoves.Add(new Point(i, j));
                    }
                }
            }

            return bestMoves;
        }

        private Point ChooseMove(ArrayList possibleMoves)
        {
            Random random = new Random();
            return (Point)possibleMoves[random.Next(possibleMoves.Count)];
        }

        private void ExecuteMove(Point move)
        {
            currentState.Y[move.X] = move.Y;
            moveCounter++;
            UpdateUI();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (chosenMove != null && GetAttackingPairs(currentState) > 0)
            {
                ExecuteMove((Point)chosenMove);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            while (GetAttackingPairs(currentState) > 0 && chosenMove != null)
            {
                ExecuteMove((Point)chosenMove);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InitializeGame();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
    }
}
