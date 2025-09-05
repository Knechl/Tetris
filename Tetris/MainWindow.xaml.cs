using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tetris.GameLogic;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        // GameBoard
        private GameBoard gameBoard;

        // Timer für automatisches Fallen
        private DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();

            // GameBoard erstellen
            gameBoard = new GameBoard(GameBoard);
            gameBoard.ScoreUpdated += (lines, score, time) => UpdateUI();

            // Timer einrichten
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(800); // Alle 800ms fallen lassen
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Tastatureingaben aktivieren
            this.KeyDown += MainWindow_KeyDown;
            this.Focusable = true;
            this.Focus();

            // Erstes Zeichnen
            gameBoard.ZeichneArray();
        }

        // Timer Event
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameBoard.MoveTetromino(); // Lässt aktuelles Tetromino nach unten fallen
            gameBoard.ZeichneArray(); // Neu zeichnen
            gameBoard.gameTime = gameBoard.gameTime.Add(TimeSpan.FromMilliseconds(800));
            UpdateUI();
        }

        private void UpdateUI()
        {
            LinesText.Text = gameBoard.lines.ToString();
            ScoreText.Text = gameBoard.score.ToString();
            TimeText.Text = $"{gameBoard.gameTime.Minutes:D2}:{gameBoard.gameTime.Seconds:D2}";
        }

        // Tastatureingaben verarbeiten
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A:
                    gameBoard.MoveTetromino(-1, 0); // Links
                    break;

                case Key.D:
                    gameBoard.MoveTetromino(1, 0); // Rechts
                    break;

                case Key.S:
                    gameBoard.MoveTetromino(0, 1); // Runter
                    break;

                case Key.W:
                    gameBoard.RotateTetromino(); // Drehung
                    break;

                case Key.Space:
                    // Hard Drop
                    while (gameBoard.MoveTetromino())
                    {
                        // Bis zum Boden
                    }
                    break;

                case Key.P:
                    // Pause
                    if (gameTimer.IsEnabled)
                    {
                        gameTimer.Stop();
                    }
                    else
                    {
                        gameTimer.Start();
                    }
                    break;
            }

            // Neu zeichnen
            gameBoard.ZeichneArray();
        }
    }
}