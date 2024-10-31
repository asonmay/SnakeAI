using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeuralNetworkLibrary;
using System;
using System.Security.Cryptography;

namespace SnakeAI
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SnakeGame[] games;
        private GeneticTrainer<SnakeGame> trainer;
        private int trainingSpeed;
        private Point seenNetworks;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 720;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Random random = new Random();

            trainingSpeed = 5;
            int gameSize = 120;
            int gridSize = 10;
            int min = -2;
            int max = 2;
            double mutationRate = 0.025;
            TimeSpan speed = TimeSpan.FromMilliseconds(150);
            ActivationFunction activationFunc = new ActivationFunction(ActivationAndErrorFunctions.TanH, ActivationAndErrorFunctions.TanHDerivative);
            ErrorFunction errorFunc = new ErrorFunction(ActivationAndErrorFunctions.MeanSquareError, ActivationAndErrorFunctions.MeanSquaredErrorDerivative);

            seenNetworks = new Point(6,6);
            games = new SnakeGame[250];
            for(int x = 0; x < games.Length; x++)
            {
                NeuralNetwork net = new NeuralNetwork(activationFunc, errorFunc, 6, 8, 8, 2);
                net.Randomize(random, min, max);
                games[x] = new SnakeGame(RandomColor(random), Vector2.Zero, new Point(gridSize, gridSize), new Point(gameSize / gridSize, gameSize / gridSize), net, min, max, speed, Color.Gray);
            }

            trainer = new GeneticTrainer<SnakeGame>(min, max, mutationRate, games, 0.2, 0.1);
        }

        private Color RandomColor(Random random)
        {
            return new Color(random.Next(25) * 10, random.Next(25) * 10, random.Next(25) * 10);
        }

        private void UpdateGames(GameTime gameTime)
        {
            for (int x = 0; x < trainer.Networks.Length; x++)
            {
                trainer.Networks[x].Update(gameTime);
            }
        }

        private void DrawGames(SnakeGame[] nets)
        {
            for (int x = 0; x < seenNetworks.X; x++)
            {
                for (int y = 0; y < seenNetworks.Y; y++)
                {
                    int index = x * seenNetworks.X + y;
                    if (nets[index] != null)
                    {
                        Grid grid = trainer.Networks[index].Grid;
                        nets[index].Grid.Pos = new Vector2(x * grid.Size.X * grid.TileSize.X, y * grid.Size.Y * grid.TileSize.X);
                        nets[index].Draw(spriteBatch);
                    }
                }
            }
        }

        private SnakeGame[] GetGamesToDraw()
        {
            SnakeGame[] games = new SnakeGame[seenNetworks.X * seenNetworks.Y];
            int index = 0;
            for(int i = 0; i < trainer.Networks.Length; i++)
            {
                if (!trainer.Networks[i].IsGameOver)
                {
                    games[index] = trainer.Networks[i];
                    index++;
                }
                if (index >= seenNetworks.X * seenNetworks.Y)
                {
                    break;
                }
            }
            return games;
        }

        private bool AreAllDead()
        {
            for(int i = 0; i < trainer.Networks.Length; i++)
            {
                if (!trainer.Networks[i].IsGameOver)
                {
                    return false;
                }
            }
            return true;
        }

        private void ResetGames()
        {
            for (int i = 0; i < trainer.Networks.Length; i++)
            {
                trainer.Networks[i].Reset();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for(int i = 0; i < trainingSpeed; i++)
            {
                UpdateGames(gameTime);
            }

            if (AreAllDead())
            {
                trainer.Train(new Random());
                ResetGames();
            }

            Window.Title = $"Gen: {trainer.Generation.ToString()}";
            if(Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                trainingSpeed = 1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                trainingSpeed = 3;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D5))
            {
                trainingSpeed = 5;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.D9))
            {
                trainingSpeed = 9;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.D0))
            {
                trainingSpeed = 65;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (trainingSpeed < 25)
            {
                DrawGames(GetGamesToDraw());
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
