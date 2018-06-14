using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Spritesheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Spells
{
    class Fireball
    {
        private Animation animation;

        public Vector2 Position;
        private Vector2 direction;

        private float velocity = 0.3f; // 10 per second
        private float angle;

        public Fireball(Vector2 Position, Vector2 direction)
        {
            this.direction = direction;
            this.Position = Position;

            angle = (float)Math.Atan2(direction.Y, direction.X) + (float)Math.PI * 0.5f;

            Console.WriteLine(angle);

            Texture2D texture = SimulationGame.contentManager.Load<Texture2D>(@"Spells\Fireball\Lv1UFireballp");
            var sheet = new Simulation.Spritesheet.Spritesheet(texture).WithGrid((15, 29)).WithFrameDuration(120);

            animation = sheet.CreateAnimation((0, 0), (1, 0), (2, 0));
            animation.Start(Repeat.Mode.Loop);
        }

        public void Update(GameTime gameTime)
        {
            Position.X += direction.X * velocity * gameTime.ElapsedGameTime.Milliseconds;
            Position.Y += direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds;

            animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(animation, Position, rotation: angle, scale: new Vector2(1.5f, 1.5f));
        }
    }
}
