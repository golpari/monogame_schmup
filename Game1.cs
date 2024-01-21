using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace test
{
    public class Game1 : Game
    {
        Texture2D leavesTexture;
        Texture2D spiderTexture;

        SoundEffect backgroundAudio;
        SoundEffectInstance musicInstance = null;

        List<Moth> mothsList = new List<Moth>() { 
            new Moth(new Vector2(960, 140), "moth"),
            new Moth(new Vector2(800, 360), "moth"),
            new Moth(new Vector2(960, 580), "moth")
        };

        Vector2 half_spider_dimensions;
        Vector2 spiderPos = new Vector2(200, 360);
        Vector2 half_leaves_dimensions;
        Vector2 half_moth_dimensions;

        float spiderSpeed = 200.0f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            leavesTexture = Content.Load<Texture2D>("leaves");
            spiderTexture = Content.Load<Texture2D>("spider");
            backgroundAudio = Content.Load<SoundEffect>("Sky Game Menu");
            musicInstance = backgroundAudio.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Play();

            foreach (var moth in mothsList)
            {
                moth.texture = Content.Load<Texture2D>(moth.texture_name);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Console.WriteLine("moving right");
                spiderPos.X += spiderSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Console.WriteLine("moving left");
                spiderPos.X -= spiderSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Console.WriteLine("moving up");
                spiderPos.Y -= spiderSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Console.WriteLine("moving down");
                spiderPos.Y += spiderSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            foreach (var moth in mothsList)
            {
                if (moth.position.X < 0)
                {
                    moth.position.X = 1300;
                }
                moth.position.X -= moth.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            half_spider_dimensions = new Vector2(spiderTexture.Width, spiderTexture.Height) * 0.5f;
            half_leaves_dimensions = new Vector2(leavesTexture.Width, leavesTexture.Height) * 0.5f;
            half_moth_dimensions = new Vector2(mothsList[0].texture.Width, mothsList[0].texture.Height) * 0.5f;

            _spriteBatch.Begin();
            _spriteBatch.Draw(leavesTexture, new Vector2(0,0), Color.White);
            _spriteBatch.Draw(spiderTexture, spiderPos - half_spider_dimensions, Color.White);

            foreach (var moth in mothsList)
            {
                _spriteBatch.Draw(moth.texture, moth.position - half_moth_dimensions, Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
