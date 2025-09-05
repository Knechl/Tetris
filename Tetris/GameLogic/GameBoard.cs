using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Tetris.GameLogic
{
    public class GameBoard
    {
        private Canvas gameCanvas;

        public int[,] board = new int[10, 20]; // Größe des Feldes
        public Color[,] boardColors = new Color[10, 20]; // Farben für jeden Block

        // Aktuelles fallendes Tetromino
        private int[,] currentTetromino;
        private Color currentTetrominoColor;
        private int tetrominoX = 3; // Start X-Position (Mitte)
        private int tetrominoY = 0; // Start Y-Position (oben)

        // Scores
        public int lines = 0;
        public int score = 0;
        public TimeSpan gameTime = TimeSpan.Zero;

        // Events für UI Updates
        public event Action<int, int, TimeSpan> ScoreUpdated;

        private Random random = new Random();

        public GameBoard(Canvas canvas)
        {
            gameCanvas = canvas;
            InitializeBoard();
            SpawnNewTetromino();
        }

        private void InitializeBoard()
        {
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    board[x, y] = 0;
                    boardColors[x, y] = Colors.Transparent;
                }
            }
        }

        // Spawnt ein neues zufälliges Tetromino
        public void SpawnNewTetromino()
        {
            int blockType = random.Next(0, 7); // 0-6 für alle 7 Block-Typen

            // Daten für den Block-Typ holen
            int[,] shape;
            Color color;

            if (blockType == 0)
            {
                shape = Tetromino.IBlock;
                color = Tetromino.IBlockColor;
            }
            else if (blockType == 1)
            {
                shape = Tetromino.OBlock;
                color = Tetromino.OBlockColor;
            }
            else if (blockType == 2)
            {
                shape = Tetromino.TBlock;
                color = Tetromino.TBlockColor;
            }
            else if (blockType == 3)
            {
                shape = Tetromino.SBlock;
                color = Tetromino.SBlockColor;
            }
            else if (blockType == 4)
            {
                shape = Tetromino.ZBlock;
                color = Tetromino.ZBlockColor;
            }
            else if (blockType == 5)
            {
                shape = Tetromino.LBlock;
                color = Tetromino.LBlockColor;
            }
            else
            {
                shape = Tetromino.JBlock;
                color = Tetromino.JBlockColor;
            }

            // Tetromino setzen
            currentTetromino = shape;
            currentTetrominoColor = color;
            tetrominoX = 3;
            tetrominoY = 0;

            // Prüfen ob Spiel vorbei ist
            if (!CanMoveTetromino(0, 0))
            {
                // Spiel Ende

                lines = 0;
                score = 0;
                gameTime = TimeSpan.Zero;
                // UI benachrichtigen
                ScoreUpdated?.Invoke(lines, score, gameTime);
                InitializeBoard();
            }
        }

        // Bewegt aktuelles Tetromino nach unten
        public bool MoveTetromino()
        {
            if (CanMoveTetromino(0, 1))
            {
                tetrominoY++;
                return true;
            }
            else
            {
                // am boden
                PlaceCurrentTetromino();
                check(); // Prüft ob eine zeile voll ist
                SpawnNewTetromino(); // Neues Tetromino spawnen
                return false;
            }
        }

        // Input
        public void MoveTetromino(int deltaX, int deltaY)
        {
            if (CanMoveTetromino(deltaX, deltaY))
            {
                tetrominoX += deltaX;
                tetrominoY += deltaY;
            }
        }

        // Rotation
        public void RotateTetromino()
        {
            int height = currentTetromino.GetLength(0);
            int width = currentTetromino.GetLength(1);

            // Backup erstellen
            int[,] backup = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    backup[y, x] = currentTetromino[y, x];
                }
            }
            
            // Rotation
            int[,] rotated = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rotated[x, height - 1 - y] = currentTetromino[y, x];
                }
            }

            // Das rotierte Array zuweisen
            currentTetromino = rotated;

            // Kollisionsprüfung
            if (!CanMoveTetromino(0, 0))
            {
                currentTetromino = backup; // Zurücksetzen
            }
        }

        // Kann bewegt werden?
        private bool CanMoveTetromino(int deltaX, int deltaY)
        {
            int newX = tetrominoX + deltaX;
            int newY = tetrominoY + deltaY;

            int height = currentTetromino.GetLength(0);
            int width = currentTetromino.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (currentTetromino[y, x] == 1)
                    {
                        int boardX = newX + x;
                        int boardY = newY + y;

                        // Prüfen ob außerhalb des Spielfelds
                        if (boardX < 0 || boardX >= 10 || boardY >= 20)
                            return false;

                        // Prüft ob da bereits ein Tetromino liegt
                        if (boardY >= 0 && board[boardX, boardY] == 1)
                            return false;
                    }
                }
            }

            return true;
        }

        // Platziert aktuelles Tetromino permanent
        private void PlaceCurrentTetromino()
        {
            int height = currentTetromino.GetLength(0);
            int width = currentTetromino.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (currentTetromino[y, x] == 1)
                    {
                        int boardX = tetrominoX + x;
                        int boardY = tetrominoY + y;

                        if (boardX >= 0 && boardX < 10 && boardY >= 0 && boardY < 20)
                        {
                            board[boardX, boardY] = 1;
                            boardColors[boardX, boardY] = currentTetrominoColor;
                        }
                    }
                }
            }
        }

        /* Grafik Logic*/
        public void ZeichneArray()
        {
            gameCanvas.Children.Clear();

            // Board
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (board[x, y] == 1)
                    {
                        MaleBlock(x, y, boardColors[x, y]);
                    }
                }
            }

            // Tetromino zeichnen
            if (currentTetromino != null)
            {
                int height = currentTetromino.GetLength(0);
                int width = currentTetromino.GetLength(1);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (currentTetromino[y, x] == 1)
                        {
                            int screenX = tetrominoX + x;
                            int screenY = tetrominoY + y;

                            // Nur zeichnen wenn sichtbar
                            if (screenX >= 0 && screenX < 10 && screenY >= 0 && screenY < 20)
                            {
                                MaleBlock(screenX, screenY, currentTetrominoColor);
                            }
                        }
                    }
                }
            }
        }

        public void check()
        {
            int linesCleared = 0;
            // Zeile 0
            if ((board[0, 0] == 1) && (board[1, 0] == 1) && (board[2, 0] == 1) && (board[3, 0] == 1) && (board[4, 0] == 1) && (board[5, 0] == 1) && (board[6, 0] == 1) && (board[7, 0] == 1) && (board[8, 0] == 1) && (board[9, 0] == 1)) { ZeileLoeschen(0); linesCleared++; }
            // Zeile 1
            if ((board[0, 1] == 1) && (board[1, 1] == 1) && (board[2, 1] == 1) && (board[3, 1] == 1) && (board[4, 1] == 1) && (board[5, 1] == 1) && (board[6, 1] == 1) && (board[7, 1] == 1) && (board[8, 1] == 1) && (board[9, 1] == 1)) { ZeileLoeschen(1); linesCleared++; }
            // Zeile 2
            if ((board[0, 2] == 1) && (board[1, 2] == 1) && (board[2, 2] == 1) && (board[3, 2] == 1) && (board[4, 2] == 1) && (board[5, 2] == 1) && (board[6, 2] == 1) && (board[7, 2] == 1) && (board[8, 2] == 1) && (board[9, 2] == 1)) { ZeileLoeschen(2); linesCleared++; }
            // Zeile 3
            if ((board[0, 3] == 1) && (board[1, 3] == 1) && (board[2, 3] == 1) && (board[3, 3] == 1) && (board[4, 3] == 1) && (board[5, 3] == 1) && (board[6, 3] == 1) && (board[7, 3] == 1) && (board[8, 3] == 1) && (board[9, 3] == 1)) { ZeileLoeschen(3); linesCleared++; }
            // Zeile 4
            if ((board[0, 4] == 1) && (board[1, 4] == 1) && (board[2, 4] == 1) && (board[3, 4] == 1) && (board[4, 4] == 1) && (board[5, 4] == 1) && (board[6, 4] == 1) && (board[7, 4] == 1) && (board[8, 4] == 1) && (board[9, 4] == 1)) { ZeileLoeschen(4); linesCleared++; }
            // Zeile 5
            if ((board[0, 5] == 1) && (board[1, 5] == 1) && (board[2, 5] == 1) && (board[3, 5] == 1) && (board[4, 5] == 1) && (board[5, 5] == 1) && (board[6, 5] == 1) && (board[7, 5] == 1) && (board[8, 5] == 1) && (board[9, 5] == 1)) { ZeileLoeschen(5); linesCleared++; }
            // Zeile 6
            if ((board[0, 6] == 1) && (board[1, 6] == 1) && (board[2, 6] == 1) && (board[3, 6] == 1) && (board[4, 6] == 1) && (board[5, 6] == 1) && (board[6, 6] == 1) && (board[7, 6] == 1) && (board[8, 6] == 1) && (board[9, 6] == 1)) { ZeileLoeschen(6); linesCleared++; }
            // Zeile 7
            if ((board[0, 7] == 1) && (board[1, 7] == 1) && (board[2, 7] == 1) && (board[3, 7] == 1) && (board[4, 7] == 1) && (board[5, 7] == 1) && (board[6, 7] == 1) && (board[7, 7] == 1) && (board[8, 7] == 1) && (board[9, 7] == 1)) { ZeileLoeschen(7); linesCleared++; }
            // Zeile 8
            if ((board[0, 8] == 1) && (board[1, 8] == 1) && (board[2, 8] == 1) && (board[3, 8] == 1) && (board[4, 8] == 1) && (board[5, 8] == 1) && (board[6, 8] == 1) && (board[7, 8] == 1) && (board[8, 8] == 1) && (board[9, 8] == 1)) { ZeileLoeschen(8); linesCleared++; }
            // Zeile 9
            if ((board[0, 9] == 1) && (board[1, 9] == 1) && (board[2, 9] == 1) && (board[3, 9] == 1) && (board[4, 9] == 1) && (board[5, 9] == 1) && (board[6, 9] == 1) && (board[7, 9] == 1) && (board[8, 9] == 1) && (board[9, 9] == 1)) { ZeileLoeschen(9); linesCleared++; }
            // Zeile 10
            if ((board[0, 10] == 1) && (board[1, 10] == 1) && (board[2, 10] == 1) && (board[3, 10] == 1) && (board[4, 10] == 1) && (board[5, 10] == 1) && (board[6, 10] == 1) && (board[7, 10] == 1) && (board[8, 10] == 1) && (board[9, 10] == 1)) { ZeileLoeschen(10); linesCleared++; }
            // Zeile 11
            if ((board[0, 11] == 1) && (board[1, 11] == 1) && (board[2, 11] == 1) && (board[3, 11] == 1) && (board[4, 11] == 1) && (board[5, 11] == 1) && (board[6, 11] == 1) && (board[7, 11] == 1) && (board[8, 11] == 1) && (board[9, 11] == 1)) { ZeileLoeschen(11); linesCleared++; }
            // Zeile 12
            if ((board[0, 12] == 1) && (board[1, 12] == 1) && (board[2, 12] == 1) && (board[3, 12] == 1) && (board[4, 12] == 1) && (board[5, 12] == 1) && (board[6, 12] == 1) && (board[7, 12] == 1) && (board[8, 12] == 1) && (board[9, 12] == 1)) { ZeileLoeschen(12); linesCleared++; }
            // Zeile 13
            if ((board[0, 13] == 1) && (board[1, 13] == 1) && (board[2, 13] == 1) && (board[3, 13] == 1) && (board[4, 13] == 1) && (board[5, 13] == 1) && (board[6, 13] == 1) && (board[7, 13] == 1) && (board[8, 13] == 1) && (board[9, 13] == 1)) { ZeileLoeschen(13); linesCleared++; }
            // Zeile 14
            if ((board[0, 14] == 1) && (board[1, 14] == 1) && (board[2, 14] == 1) && (board[3, 14] == 1) && (board[4, 14] == 1) && (board[5, 14] == 1) && (board[6, 14] == 1) && (board[7, 14] == 1) && (board[8, 14] == 1) && (board[9, 14] == 1)) { ZeileLoeschen(14); linesCleared++; }
            // Zeile 15
            if ((board[0, 15] == 1) && (board[1, 15] == 1) && (board[2, 15] == 1) && (board[3, 15] == 1) && (board[4, 15] == 1) && (board[5, 15] == 1) && (board[6, 15] == 1) && (board[7, 15] == 1) && (board[8, 15] == 1) && (board[9, 15] == 1)) { ZeileLoeschen(15); linesCleared++; }
            // Zeile 16
            if ((board[0, 16] == 1) && (board[1, 16] == 1) && (board[2, 16] == 1) && (board[3, 16] == 1) && (board[4, 16] == 1) && (board[5, 16] == 1) && (board[6, 16] == 1) && (board[7, 16] == 1) && (board[8, 16] == 1) && (board[9, 16] == 1)) { ZeileLoeschen(16); linesCleared++; }
            // Zeile 17
            if ((board[0, 17] == 1) && (board[1, 17] == 1) && (board[2, 17] == 1) && (board[3, 17] == 1) && (board[4, 17] == 1) && (board[5, 17] == 1) && (board[6, 17] == 1) && (board[7, 17] == 1) && (board[8, 17] == 1) && (board[9, 17] == 1)) { ZeileLoeschen(17); linesCleared++; }
            // Zeile 18
            if ((board[0, 18] == 1) && (board[1, 18] == 1) && (board[2, 18] == 1) && (board[3, 18] == 1) && (board[4, 18] == 1) && (board[5, 18] == 1) && (board[6, 18] == 1) && (board[7, 18] == 1) && (board[8, 18] == 1) && (board[9, 18] == 1)) { ZeileLoeschen(18); linesCleared++; }
            // Zeile 19
            if ((board[0, 19] == 1) && (board[1, 19] == 1) && (board[2, 19] == 1) && (board[3, 19] == 1) && (board[4, 19] == 1) && (board[5, 19] == 1) && (board[6, 19] == 1) && (board[7, 19] == 1) && (board[8, 19] == 1) && (board[9, 19] == 1)) { ZeileLoeschen(19); linesCleared++; }

            if (linesCleared > 0)
            {
                lines += linesCleared;
                score += linesCleared * 100; // 100 Punkte pro Zeile

                // UI benachrichtigen
                ScoreUpdated?.Invoke(lines, score, gameTime);
            }
        }

        /* Hilfsmethode zum Malen*/
        private void MaleBlock(int x, int y, Color farbe)
        {
            Rectangle block = new Rectangle();
            block.Width = 24;
            block.Height = 24;
            block.Fill = new SolidColorBrush(farbe);
            block.Stroke = Brushes.Black;
            block.StrokeThickness = 1;

            Canvas.SetLeft(block, x * 24);
            Canvas.SetTop(block, y * 24);

            gameCanvas.Children.Add(block);
        }

        private void ZeileLoeschen(int zeile)
        {
            for (int y = zeile; y > 0; y--)
            {
                for (int x = 0; x < 10; x++)
                {
                    board[x, y] = board[x, y - 1];
                    boardColors[x, y] = boardColors[x, y - 1];
                }
            }

            for (int x = 0; x < 10; x++)
            {
                board[x, 0] = 0;
                boardColors[x, 0] = Colors.Transparent;
            }
        }

        private void LoescheBlock(int x, int y)
        {
            for (int i = gameCanvas.Children.Count - 1; i >= 0; i--)
            {
                var element = gameCanvas.Children[i];
                if (element is Rectangle rect)
                {
                    double left = Canvas.GetLeft(rect);
                    double top = Canvas.GetTop(rect);
                    if (left == x * 24 && top == y * 24)
                    {
                        gameCanvas.Children.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}