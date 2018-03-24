using Microsoft.Xna.Framework;
using Nez;
using System;

namespace SpiderStrider
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Core
    {
        public Game1()
        {
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            Window.AllowUserResizing = true;
            IsFixedTimeStep = true;

            GameManager.LoadTitleScene();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            InputManager.UpdateLastState();
        }
    }
}

