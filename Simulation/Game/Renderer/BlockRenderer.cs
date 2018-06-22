using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public class BlockRenderer
    { 
        private static Dictionary<BlockType, (string, Rectangle)> Blocks = new Dictionary<BlockType, (string, Rectangle)> {
            { BlockType.GRASS_01, ("terrain_atlas", new Rectangle(672, 160, 32, 32)) },
            { BlockType.GRASS_02, ("terrain_atlas", new Rectangle(704, 160, 32, 32)) },
            { BlockType.GRASS_03, ("terrain_atlas", new Rectangle(736, 160, 32, 32)) },
            { BlockType.GRASS_04, ("terrain_atlas", new Rectangle(708, 96, 32, 32)) },
            { BlockType.GRASS_WATERHOLE, ("terrain_atlas", new Rectangle(192, 288, 32, 32)) }
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
