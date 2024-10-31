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
        public Directions CurrentDirection { get; set; }
        public int NumberOfMoves { get; set; }

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

        public void AddToTail()
        {
            switch(CurrentDirection)
            {
                case Directions.Left:
                    Points.AddLast(new Point(Points.Last().X + 1, Points.Last().Y));
                    break;
                case Directions.Right:
                    Points.AddLast(new Point(Points.Last().X - 1, Points.Last().Y));
                    break;
                case Directions.Forward:
                    Points.AddLast(new Point(Points.Last().X + 1, Points.Last().Y));
                    break;
                case Directions.Back:
                    Points.AddLast(new Point(Points.Last().X - 1, Points.Last().Y));
                    break;
            }
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
               
                timer = TimeSpan.Zero;
                NumberOfMoves++; 
            }
            return fitness;
        }

        public void Move()
        {
            switch (CurrentDirection)
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

        public bool IsTouchingItself()
        {
            LinkedListNode<Point> current = Points.First.Next;
            for (int i = 1; i < Points.Count; i++)
            {
                if(Points.First.Value == current.Value)
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            LinkedListNode<Point> current = Points.First;
            for (int i = 0; i < Points.Count; i++)
            {
                Point position = new Point((int)(grid.Pos.X + current.Value.X * grid.TileSize.X), (int)(grid.Pos.Y + current.Value.Y * grid.TileSize.Y));
                spriteBatch.FillRectangle(new Rectangle(position, grid.TileSize), color);
                current = current.Next;
            }
        }
    }
}
