using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworkLibrary;
using MonoGame.Extended;
using System.Xml.Linq;

namespace SnakeAI
{
    public enum Directions
    {
        Left,
        Right,
        Forward,
        Back
    }

    public class Snake : Sprite
    {
        public LinkedList<Point> Points { get; private set; }
        public Directions currentDirection { get; set; }
        public int numberOfMoves { get; set; }

        private Grid grid;
        private Color color;
        private Point startingPos;
        private TimeSpan speed;
        private TimeSpan timer;
        private Point[] lastPos;
        
        public Point Head
        {
            get
            {
                return Points.First();
            }
        }

        public Snake(Color color, Point startingPos, Grid grid, TimeSpan speed)
            :base(null, Vector2.Zero, color, SpriteEffects.None, 1, 0, Rectangle.Empty, Vector2.Zero)
        {
            Points = new LinkedList<Point>();
            Points.AddFirst(startingPos);
            this.color = color;
            this.startingPos = startingPos;
            this.grid = grid;
            this.speed = speed;
            lastPos = new Point[4];
        }

        public void Reset()
        {
            Points = new LinkedList<Point>();
            Points.AddFirst(startingPos);
        }

        public double Update(GameTime gameTime, double fitness, Point apple)
        {
            timer += gameTime.ElapsedGameTime;
            if(timer >= speed)
            {
                Point past = Points.First();
                Move();
                double pastDist = Math.Abs(past.X - apple.X) + Math.Abs(past.Y - apple.Y);
                double currentDist = Math.Abs(Points.First().X - apple.X) + Math.Abs(Points.First().Y - apple.Y);
                fitness += pastDist < currentDist ? -10 : 5;
                //if (lastPos[0] == Points.First() || lastPos[3] == Points.First())
                //{
                //    fitness -= 300;
                //}
                timer = TimeSpan.Zero;
                numberOfMoves++;
                
                //for(int i = lastPos.Length - 1; i > 0; i--)
                //{
                //    lastPos[i - 1] = lastPos[i];
                //}
                //lastPos[0] = past;
            }
            return fitness;
        }

        public void Move()
        {
            switch (currentDirection)
            {
                case Directions.Left:
                    Points.AddFirst(new Point(Points.First().X - 1, Points.First().Y));
                    break;

                case Directions.Right:
                    Points.AddFirst(new Point(Points.First().X + 1, Points.First().Y));
                    break;

                case Directions.Forward:
                    Points.AddFirst(new Point(Points.First().X, Points.First().Y + 1));
                    break;

                case Directions.Back:
                    Points.AddFirst(new Point(Points.First().X, Points.First().Y - 1));
                    break;
            }
            Points.RemoveLast();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Point current = Points.First();
            for (int i = 0; i < Points.Count; i++)
            {
                Point position = new Point((int)(grid.Pos.X + current.X * grid.TileSize.X), (int)(grid.Pos.Y + current.Y * grid.TileSize.Y));
                spriteBatch.FillRectangle(new Rectangle(position, grid.TileSize), color);
            }
        }
    }
}
