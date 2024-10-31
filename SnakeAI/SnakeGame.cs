using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content.Tiled;
using NeuralNetworkLibrary;

namespace SnakeAI
{
    public class SnakeGame : INeuralNetwork
    {
        public double Fitness { get; set; }
        public NeuralNetwork Network { get; set; }
        public bool IsGameOver { get; private set; }
        public Grid Grid { get; set; }

        private Snake snake;       
        private TimeSpan speed;

        public SnakeGame(Color color, Vector2 pos, Point size, Point tileSize, NeuralNetwork net, int min, int max, TimeSpan speed, Color gridColor)
        {
            Grid = new Grid(pos, tileSize, size, color, gridColor);
            snake = new Snake(color, new Point(size.X/2, size.Y/2), Grid, speed);
            Network = net;
            RandomizeApple();
            Network.Randomize(new Random(), min, max);
            this.speed = speed;
        }

        private void RandomizeApple()
        {
            Random random = new Random();
            Grid.Apple = new Point(random.Next(Grid.Size.X), random.Next(Grid.Size.Y));
        }

        private Directions GetDirection(double[] values)
        {
            if (Math.Abs(values[0]) > Math.Abs(values[1]))
            {
                return values[0] > 0 ? Directions.Right : Directions.Left;
            }
            else
            {
                return values[1] > 0 ? Directions.Forward : Directions.Back;
            }
        }

        private double GetNoramlizeValue(double min, double max, double nMin, double nMax, double current)
        {
            return (current - min) / (max - min) * (nMax - nMin) + nMin;
        }

        private void UpdateSnakeDirection()
        {
            Vector2 appleDif = new Vector2(Grid.Apple.X - snake.Head.X, Grid.Apple.Y - snake.Head.Y);
            double wallDif1 = GetNoramlizeValue(0, Grid.Size.X, 0, 1, snake.Head.X);
            double wallDif2 = GetNoramlizeValue(0, Grid.Size.X, 0, 1, snake.Head.Y);
            double wallDif3 = GetNoramlizeValue(0, Grid.Size.X, 0, 1, Grid.Size.X - snake.Head.X);
            double wallDif4 = GetNoramlizeValue(0, Grid.Size.X, 0, 1, Grid.Size.Y - snake.Head.Y);

            appleDif = new Vector2((float)GetNoramlizeValue(0, Grid.Size.X, 0, 1, appleDif.X), (float)GetNoramlizeValue(0, Grid.Size.Y, 0, 1, appleDif.Y));

            double[] inputs = [appleDif.X, appleDif.Y, wallDif1, wallDif2, wallDif3, wallDif4];
            double[] raw = Network.Compute(inputs);
            snake.CurrentDirection = GetDirection(raw);
        }

        public void Update(GameTime gameTime)
        {
            if(!IsGameOver)
            {
                UpdateSnakeDirection();
                Fitness = snake.Update(gameTime, Fitness, Grid.Apple);
               
                if (snake.Head.X == Grid.Apple.X && snake.Head.Y == Grid.Apple.Y)
                {
                    RandomizeApple();
                    snake.NumberOfMoves = 0;
                    Fitness += 50;
                    snake.AddToTail();
                }

                if (snake.IsTouchingItself())
                {
                    IsGameOver = true;
                    Fitness -= 100;
                }

                if (snake.Head.X < 0 || snake.Head.Y < 0 || snake.Head.X > Grid.Size.X - 1 || snake.Head.Y > Grid.Size.Y - 1)
                {
                    IsGameOver = true;
                    Fitness -= 100;
                }

                if (snake.NumberOfMoves > 20)
                {
                    Fitness -= 25;
                    IsGameOver = true;
                }
            }
        }

        public void Draw(SpriteBatch sp)
        {
            if(!IsGameOver)
            {
                Grid.Draw(sp);
                snake.Draw(sp);
            }
        }

        public void Reset()
        {
            snake.Reset();
            snake.NumberOfMoves = 0;
            IsGameOver = false;
            Fitness = 0;
            Grid.Color = Color.Gray;
            RandomizeApple();
        }
    }
}
