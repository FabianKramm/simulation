using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.Renderer.Entities
{
    public class LivingEntityRendererInformationFactory
    {
        public static LivingEntityRendererInformation createPlayerRenderInformation()
        {
            var texture = SimulationGame.contentManager.Load<Texture2D>("player");
            var sheet = new Spritesheet.Spritesheet(texture).WithGrid((32, 48)).WithCellOrigin(new Point(16, 48)).WithFrameDuration(160);
            var rendererInformation = new LivingEntityRendererInformation(
                sheet.CreateAnimation((0, 0), (1, 0), (2, 0), (3, 0)),
                sheet.CreateAnimation((0, 3), (1, 3), (2, 3), (3, 3)),
                sheet.CreateAnimation((0, 1), (1, 1), (2, 1), (3, 1)),
                sheet.CreateAnimation((0, 2), (1, 2), (2, 2), (3, 2))
            );

            return rendererInformation;
        }
    }
}
