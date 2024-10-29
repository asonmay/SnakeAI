using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeAI
{
    public class Grid
    {
        public Vector2 Pos { get; set; }
        public Point TileSize { get; private set; }
        public Point Size { get; private set; }
        public Snake Snake { get; private set; }
        public Point Apple {  get; set; }
        public Color Color { get; set; }

        private Color appleColor;
        public Grid(Vector2 pos, Point tileSize, Point size, Color appleColor, Color color)
        {
            Pos = pos;
            TileSize = tileSize;
            Size = size;
            this.appleColor = appleColor;
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(new RectangleF(Pos, new SizeF(TileSize.X * Size.X, TileSize.Y * Size.Y)), Color);
            spriteBatch.DrawRectangle(new RectangleF(Pos, new SizeF(TileSize.X * Size.X, TileSize.Y * Size.Y)), Color.Black, 2);
            spriteBatch.FillRectangle(new RectangleF(Pos.X + TileSize.Y * Apple.X, Pos.Y + TileSize.Y * Apple.Y, TileSize.X, TileSize.Y), appleColor);
        }
    }
}
