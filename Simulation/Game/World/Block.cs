using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    enum BlockType
    {
        GRASS = 0,
    }

    class Block
    {
        private static Random random = new Random();
        private static Dictionary<BlockType, (string, Rectangle)[]> Blocks = new Dictionary <BlockType, (string, Rectangle)[]> {
            { BlockType.GRASS, new (string, Rectangle)[] {
                ("terrain_atlas", new Rectangle(672, 160, 32, 32)),
                ("terrain_atlas", new Rectangle(704, 160, 32, 32)),
                ("terrain_atlas", new Rectangle(736, 160, 32, 32))
            }}
        };

        private BlockType blockType;
        private Texture2D texture;
        private Rectangle spritePosition;

        public Block(BlockType blockType = BlockType.GRASS)
        {
            this.blockType = blockType;

            int randomTexture = random.Next(0, Blocks[blockType].Length);

            texture = SimulationGame.contentManager.Load<Texture2D>(Blocks[blockType][randomTexture].Item1);
            spritePosition = Blocks[blockType][randomTexture].Item2;
        }

        public void Draw(SpriteBatch spriteBatch, ref Vector2 position)
        {
            spriteBatch.Draw(texture, position, spritePosition, Color.White);
        }
    }
}
