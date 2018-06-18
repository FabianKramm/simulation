using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.Hud;
using Simulation.Spritesheet;
using Simulation.Util;
using System;

namespace Simulation.Game.Effect
{
    public class Fireball: Effect
    {
        private Animation flying;
        private Animation impact;

        private float velocity = 0.3f; // 10 per second
        private float angle;

        public Vector2 position;
        private Vector2 direction;

        private bool hasHitTarget = false;

        public Fireball(LivingEntity origin, Vector2 target, Vector2? relativeOriginPosition = null) : base(origin)
        {
            Vector2 _relativeOriginPosition = relativeOriginPosition ?? Vector2.Zero;

            position = Vector2.Add(origin.position, _relativeOriginPosition);

            direction = new Vector2(target.X - position.X, target.Y - position.Y);
            direction.Normalize();

            position.X += (direction.X * World.World.BlockSize.X);
            position.Y += (direction.Y * World.World.BlockSize.Y);

            angle = (float)Math.Atan2(direction.Y, direction.X) + (float)Math.PI * 0.5f;

            GameConsole.WriteLine("", angle + "");

            Texture2D texture = SimulationGame.contentManager.Load<Texture2D>(@"Spells\Fireball\Lv1UFireballp");
            var sheet = new Simulation.Spritesheet.Spritesheet(texture).WithGrid((15, 29)).WithFrameDuration(120).WithCellOrigin(new Point(7, 0));

            flying = sheet.CreateAnimation((0, 0), (1, 0), (2, 0));
            flying.Start(Repeat.Mode.Loop);

            Texture2D explode = SimulationGame.contentManager.Load<Texture2D>(@"Spells\Fireball\Explode");
            sheet = new Simulation.Spritesheet.Spritesheet(explode).WithGrid((61, 60)).WithFrameDuration(100).WithCellOrigin(new Point(30, 30));

            impact = sheet.CreateAnimation((0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0), (9, 0));
        }

        public override void Update(GameTime gameTime)
        {
            if(!hasHitTarget)
            {
                position.X += (direction.X * velocity * gameTime.ElapsedGameTime.Milliseconds);
                position.Y += (direction.Y * velocity * gameTime.ElapsedGameTime.Milliseconds);

                if (SimulationGame.worldDimensions.Contains(position))
                {
                    var rotateVector = new Vector2(position.X, position.Y + 7.5f);
                    var rotatedPoint = GeometryUtils.Rotate(angle, ref position, ref rotateVector);
                    var collisionRect = new Rectangle((int)(rotatedPoint.X - 7.5f), (int)(rotatedPoint.Y - 7.5f), 15, 15);
                    var touchedBlocks = SimulationGame.world.getTouchedWorldBlocks(ref collisionRect);

                    foreach (var block in touchedBlocks)
                        foreach (var entity in block.hitableObjects)
                        {
                            if (entity == origin) continue;

                            if (collisionRect.Intersects(entity.hitBoxBounds))
                            {
                                hasHitTarget = true;
                                impact.Start(Repeat.Mode.Once);
                            }
                        }

                    flying.Update(gameTime);
                }
                else
                {
                    isFinished = true;
                }
            }
            else
            {
                impact.Update(gameTime);

                if(!impact.IsStarted)
                {
                    isFinished = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(SimulationGame.visibleArea.Contains(position))
            {
                if(hasHitTarget)
                {
                    if(impact.IsStarted)
                    {
                        spriteBatch.Draw(impact, position, scale: new Vector2(1.5f, 1.5f), layerDepth: GeometryUtils.getLayerDepthFromYPosition(position.Y + World.World.BlockSize.Y));
                    }
                }
                else
                {
                    spriteBatch.Draw(flying, position, rotation: angle, scale: new Vector2(1.5f, 1.5f), layerDepth: GeometryUtils.getLayerDepthFromYPosition(position.Y));
                }
            }

            if(SimulationGame.isDebug)
            {
                var rotateVector = new Vector2(position.X, position.Y + 7.5f);
                var rotatedPoint = GeometryUtils.Rotate(angle, ref position, ref rotateVector);

                SimulationGame.primitiveDrawer.Rectangle(new Rectangle((int)(rotatedPoint.X - 7.5f), (int)(rotatedPoint.Y - 7.5f), 15, 15), Color.Red);
            }
        }
    }
}
