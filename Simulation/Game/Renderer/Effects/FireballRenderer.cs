using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Effects;
using Simulation.Spritesheet;
using Simulation.Util.Geometry;

namespace Simulation.Game.Renderer.Effects
{
    public class FireballRendererInformation : EffectRendererInformation
    {
        public Animation Flying;
        public Animation Impact;

        public FireballRendererInformation(Fireball fireball)
        {
            var texture = SimulationGame.ContentManager.Load<Texture2D>(@"Spells\Fireball\Lv1UFireballp");
            var sheet = new Spritesheet.Spritesheet(texture).WithGrid((15, 29)).WithFrameDuration(120).WithCellOrigin(new Point(7, 0));

            Flying = sheet.CreateAnimation((0, 0), (1, 0), (2, 0));
            Flying.Start(Repeat.Mode.Loop);
        }

        public void StartImpactAnimation()
        {
            var explode = SimulationGame.ContentManager.Load<Texture2D>(@"Spells\Fireball\Explode");
            var sheet = new Spritesheet.Spritesheet(explode).WithGrid((61, 60)).WithFrameDuration(100).WithCellOrigin(new Point(30, 30));

            Impact = sheet.CreateAnimation((0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0), (9, 0));
            Impact.Start(Repeat.Mode.Once);
        }
    }

    public class FireballRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, Fireball fireball)
        {
            if (fireball.InteriorID == SimulationGame.Player.InteriorID && SimulationGame.VisibleArea.Contains(fireball.Position))
            {
                if (fireball.effectRendererInformation == null)
                    fireball.effectRendererInformation = new FireballRendererInformation(fireball);

                FireballRendererInformation fireballRendererInformation = (FireballRendererInformation)fireball.effectRendererInformation;

                if (fireball.HasHitTarget)
                {
                    if (fireballRendererInformation.Impact == null)
                        fireballRendererInformation.StartImpactAnimation();

                    fireballRendererInformation.Impact.Update(gameTime);

                    if (fireballRendererInformation.Impact.IsStarted)
                    {
                        spriteBatch.Draw(fireballRendererInformation.Impact, fireball.Position, scale: new Vector2(1.5f, 1.5f), layerDepth: GeometryUtils.getLayerDepthFromPosition(fireball.Position.X, fireball.Position.Y + World.WorldGrid.BlockSize.Y));
                    }
                    else
                    {
                        fireballRendererInformation.IsFinished = true;
                    }
                }
                else
                {
                    fireballRendererInformation.Flying.Update(gameTime);

                    spriteBatch.Draw(fireballRendererInformation.Flying, fireball.Position, rotation: fireball.Angle, scale: new Vector2(1.5f, 1.5f), layerDepth: GeometryUtils.getLayerDepthFromPosition(fireball.Position.X, fireball.Position.Y));
                }

                if (SimulationGame.IsDebug)
                {
                    var rotateVector = new Vector2(fireball.Position.X, fireball.Position.Y + 7.5f);
                    var rotatedPoint = GeometryUtils.Rotate(fireball.Angle, fireball.Position, ref rotateVector);

                    SimulationGame.PrimitiveDrawer.Rectangle(new Rectangle((int)(rotatedPoint.X - 7.5f), (int)(rotatedPoint.Y - 7.5f), 15, 15), Color.Red);
                }
            }
        }
    }
}
