using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Effects;
using Simulation.Spritesheet;
using Simulation.Util.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Renderer.Effects
{
    public class BlinkRenderer
    {
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime, Blink blink)
        {
            if (blink.InteriorID == SimulationGame.Player.InteriorID && SimulationGame.VisibleArea.Contains(blink.Position))
            {
                if(blink.BlinkAnimation == null)
                {
                    var texture = SimulationGame.ContentManager.Load<Texture2D>(@"Spells\Blink\Blink");
                    var sheet = new Spritesheet.Spritesheet(texture).WithGrid((80, 80)).WithFrameDuration(40).WithCellOrigin(new Point(40, 48));

                    blink.BlinkAnimation = sheet.CreateAnimation((0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (0, 1), (1, 1), (2, 1), (3, 1), (4, 1), (0, 2), (1, 2));
                    blink.BlinkAnimation.Start(Repeat.Mode.Once);
                }

                if(blink.BlinkAnimation.IsStarted)
                {
                    spriteBatch.Draw(blink.BlinkAnimation, blink.Origin.Position.ToVector(), /*scale: new Vector2(2.5f, 2.5f), */layerDepth: GeometryUtils.GetLayerDepthFromPosition(blink.Origin.Position.X, blink.Origin.Position.Y + World.WorldGrid.BlockSize.Y));
                }
            }
        }
    }
}
