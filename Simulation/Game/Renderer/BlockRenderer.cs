using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public enum BlockRenderType
    {
        GRASS_01 = 0,
        GRASS_02,
        GRASS_03,
        GRASS_04,
        GRASS_WATERHOLE,
    }

    public class BlockRenderer
    { 
        private static Dictionary<BlockRenderType, (string, Rectangle)> Blocks = new Dictionary<BlockRenderType, (string, Rectangle)> {
            { BlockRenderType.GRASS_01, ("terrain_atlas", new Rectangle(672, 160, 32, 32)) },
            { BlockRenderType.GRASS_02, ("terrain_atlas", new Rectangle(704, 160, 32, 32)) },
            { BlockRenderType.GRASS_03, ("terrain_atlas", new Rectangle(736, 160, 32, 32)) },
            { BlockRenderType.GRASS_04, ("terrain_atlas", new Rectangle(708, 96, 32, 32)) },
            { BlockRenderType.GRASS_WATERHOLE, ("terrain_atlas", new Rectangle(192, 288, 32, 32)) }
        };

        public static void Draw(SpriteBatch spriteBatch, Block block)
        {
            spriteBatch.Draw(SimulationGame.contentManager.Load<Texture2D>(Blocks[block.blockType].Item1), new Vector2(block.blockBounds.X, block.blockBounds.Y), Blocks[block.blockType].Item2, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            if(block.ambientObjects != null)
                foreach (DrawableObject ambientObject in block.ambientObjects)
                    ambientObject.Draw(spriteBatch);
        }
    }
}
