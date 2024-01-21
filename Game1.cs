using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame_schmup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace test
{
    public class Game1 : Game
    {
        Texture2D leavesTexture;
        Texture2D spiderTexture;
        Texture2D mothTexture;
        Texture2D winTexture;
        Texture2D loseTexture;

        SoundEffect backgroundAudio;
        SoundEffect winAudio;
        SoundEffect loseAudio;
        SoundEffect projAudio;
        SoundEffect enemyHitAudio;
        SoundEffect playerHitAudio;
        SoundEffect gameStartAudio;

        SoundEffectInstance win = null;
        SoundEffectInstance lose = null;
        SoundEffectInstance shoot = null;
        SoundEffectInstance hit = null;
        SoundEffectInstance ouch = null;
        SoundEffectInstance start = null;
        SoundEffectInstance musicInstance = null;

        List<Moth> mothsList = new List<Moth>() { 
            new Moth(new Vector2(960, 140), "moth"),
            new Moth(new Vector2(800, 360), "moth"),
            new Moth(new Vector2(960, 580), "moth")
        };

        List<Projectile> projectilesList = new List<Projectile>();

        List<Life> livesList = new List<Life>()
        {
            new Life(new Vector2(100, 0), "spider_silhouette"),
            new Life(new Vector2(200, -25), "spider_silhouette"),
            new Life(new Vector2(300, -50), "spider_silhouette")
        };

        Vector2 half_spider_dimensions;
        Vector2 spiderPos = new Vector2(200, 360);
        Vector2 half_leaves_dimensions;
        Vector2 half_moth_dimensions;
        Vector2 half_proj_dimensions;

        float spiderSpeed = 200.0f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        bool hitSpace = false;
        bool spaceReleased = false;
        bool firstRun = true;
        int lostCount = 0;
        int wonCount = 0;
        int startCount = 0;

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
            winTexture = Content.Load<Texture2D>("winScreen");
            loseTexture = Content.Load<Texture2D>("gameOver");
            spiderTexture = Content.Load<Texture2D>("spider");
            mothTexture = Content.Load<Texture2D>("moth");

            backgroundAudio = Content.Load<SoundEffect>("Sky Game Menu");
            gameStartAudio = Content.Load<SoundEffect>("mission_start");
            winAudio = Content.Load<SoundEffect>("mission_complete");
            loseAudio = Content.Load<SoundEffect>("game_over");
            enemyHitAudio = Content.Load<SoundEffect>("chew");
            playerHitAudio = Content.Load<SoundEffect>("ouch");
            projAudio = Content.Load<SoundEffect>("zap");

            shoot = projAudio.CreateInstance();
            win = winAudio.CreateInstance();
            lose = loseAudio.CreateInstance();
            start = gameStartAudio.CreateInstance();
            hit = enemyHitAudio.CreateInstance();
            ouch = playerHitAudio.CreateInstance();

            if (firstRun)
            {
                musicInstance = backgroundAudio.CreateInstance();
                musicInstance.IsLooped = true;
                musicInstance.Play();
            }

            foreach (var moth in mothsList)
            {
                moth.texture = Content.Load<Texture2D>(moth.texture_name);
            }

            foreach (var projectile in projectilesList)
            {
                projectile.texture = Content.Load<Texture2D>(projectile.texture_name);
            }

            foreach (var life in livesList)
            {
                life.texture = Content.Load<Texture2D>(life.texture_name);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            if (firstRun)
            {
                start.Play();
                firstRun = false;
            }
            //move player
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

            //move moths
            foreach (var moth in mothsList)
            {
                if (moth.position.X < 0)
                {
                    moth.position.X = 1300;
                }
                moth.position.X -= moth.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            //shoot projectile
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && hitSpace == false)
            {
                hitSpace = true;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && hitSpace)
            {
                spaceReleased = true;
            }
            if (hitSpace && spaceReleased)
            {
                shoot.Play();

                spaceReleased = false;
                hitSpace = false;
                Console.WriteLine("pressed space, shoot projectile");
                projectilesList.Add(new Projectile(spiderPos, "spiderweb2"));
                LoadContent();
            }

            //move projectile
            foreach (var proj in projectilesList)
            {
                proj.position.X += proj.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // check collisions between projectiles and moths (expression for collision from chatGPT)
            Projectile deleteMeProj = null;
            Moth deleteMeMoth = null;
            foreach (var proj in projectilesList)
            {
                foreach (var moth in mothsList)
                {
                    bool overlap_x = proj.position.X + half_proj_dimensions.X > moth.position.X - half_moth_dimensions.X 
                        && proj.position.X - half_proj_dimensions.X < moth.position.X + half_moth_dimensions.X;
                    bool overlap_y = proj.position.Y + half_proj_dimensions.Y > moth.position.Y - half_moth_dimensions.Y 
                        && proj.position.Y - half_proj_dimensions.Y < moth.position.Y + half_moth_dimensions.Y;

                    if (overlap_x && overlap_y)
                    {
                        deleteMeProj = proj;
                        deleteMeMoth = moth;
                    }
                }
            }
            if (deleteMeProj != null && deleteMeMoth != null)
            {
                hit.Play();

                projectilesList.Remove(deleteMeProj);
                mothsList.Remove(deleteMeMoth);
            }
            deleteMeMoth = null; //reset variable

            //check collisions between moths and spider (expression for collision from chatGPT)
            foreach (var moth in mothsList)
            {
                bool isInsideX = moth.position.X - half_moth_dimensions.X <= spiderPos.X && spiderPos.X <= moth.position.X + half_moth_dimensions.X;
                bool isInsideY = moth.position.Y - half_moth_dimensions.Y <= spiderPos.Y && spiderPos.Y <= moth.position.Y + half_moth_dimensions.Y;

                // successful collision, so remove life
                if (isInsideX && isInsideY)
                {
                    ouch.Play();

                    deleteMeMoth = moth;
                    livesList.Remove(livesList[livesList.Count - 1]);
                }
            }
            mothsList.Remove(deleteMeMoth);
            deleteMeMoth = null;

            //restart game on escape press
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                startCount++;
                restartGame();
            }
            
            //play endgame audio
            if (wonCount == 1)
            {
                win.Play();
            }

            else if (lostCount == 1)
            {
                lose.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            half_spider_dimensions = new Vector2(spiderTexture.Width, spiderTexture.Height) * 0.5f;
            half_leaves_dimensions = new Vector2(leavesTexture.Width, leavesTexture.Height) * 0.5f;
            half_moth_dimensions = new Vector2(mothTexture.Width, mothTexture.Height) * 0.5f;
            half_proj_dimensions = new Vector2(25, 25);

            _spriteBatch.Begin();
            _spriteBatch.Draw(leavesTexture, new Vector2(0,0), Color.White);
            _spriteBatch.Draw(spiderTexture, spiderPos - half_spider_dimensions, Color.White);

            foreach (var moth in mothsList)
            {
                _spriteBatch.Draw(moth.texture, moth.position - half_moth_dimensions, Color.White);
            }

            foreach (var projectile in projectilesList)
            {
                _spriteBatch.Draw(projectile.texture, projectile.position - half_proj_dimensions, Color.White);
            }

            foreach (var life in livesList)
            {
                _spriteBatch.Draw(life.texture, life.position - half_proj_dimensions, Color.White);
            }

            if (mothsList.Count == 0)
            {
                wonCount++;
                _spriteBatch.Draw(winTexture, new Vector2(0, 0), Color.White);
            }

            if (livesList.Count == 0)
            {
                lostCount++;
                _spriteBatch.Draw(loseTexture, new Vector2(0, 0), Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void restartGame()
        {
            lostCount = 0;
            wonCount = 0;

            if (startCount == 1)
                start.Play();

            mothsList.Clear();
            mothsList = new List<Moth>() {
                new Moth(new Vector2(960, 140), "moth"),
                new Moth(new Vector2(800, 360), "moth"),
                new Moth(new Vector2(960, 580), "moth")
            };

            projectilesList.Clear();

            livesList.Clear();
            livesList = new List<Life>()
            {
                new Life(new Vector2(100, 0), "spider_silhouette"),
                new Life(new Vector2(200, -25), "spider_silhouette"),
                new Life(new Vector2(300, -50), "spider_silhouette")
            };

            spiderPos = new Vector2(200, 360);

            LoadContent();
        }
    }
}
