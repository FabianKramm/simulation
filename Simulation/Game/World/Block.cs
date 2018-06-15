using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.World
{
    public enum BlockType
    {
        GRASS = 0,
        GRASS_WATERHOLE = 1,
    }

    public enum CollisionType
    {
        NO_COLLISION = 0,
        UNPASSABLE = 1,
    }

    public class Block
    {
        private static Random random = new Random();
        private static Dictionary<BlockType, (string, Rectangle)[]> Blocks = new Dictionary <BlockType, (string, Rectangle)[]> {
            { BlockType.GRASS, new (string, Rectangle)[] {
                ("terrain_atlas", new Rectangle(672, 160, 32, 32)),
                ("terrain_atlas", new Rectangle(704, 160, 32, 32)),
                ("terrain_atlas", new Rectangle(736, 160, 32, 32)),
                ("terrain_atlas", new Rectangle(708, 96, 32, 32))
            }},
            { BlockType.GRASS_WATERHOLE, new (string, Rectangle)[] {
                ("terrain_atlas", new Rectangle(192, 288, 32, 32))
            }}
        };

        private BlockType blockType;
        private Texture2D texture;
        private Rectangle spritePosition;
        public Rectangle blockBounds
        {
            get; private set;
        }

        public Point position
        {
            get; private set;
        }

        public CollisionType collisionType
        {
            get; private set;
        }


        public Block(Point position, BlockType blockType = BlockType.GRASS)
        {
            this.blockType = blockType;
            this.position = position;
            blockBounds = new Rectangle(position.X * World.BlockSize.X, position.Y * World.BlockSize.Y, World.BlockSize.X, World.BlockSize.Y);

            int randomTexture = random.Next(0, Blocks[blockType].Length);

            texture = SimulationGame.contentManager.Load<Texture2D>(Blocks[blockType][randomTexture].Item1);
            spritePosition = Blocks[blockType][randomTexture].Item2;

            if(blockType == BlockType.GRASS_WATERHOLE)
            {
                collisionType = CollisionType.UNPASSABLE;
            }
            else
            {
                collisionType = CollisionType.NO_COLLISION;
            }
        }

        public void Draw(SpriteBatch spriteBatch, ref Vector2 position)
        {
            spriteBatch.Draw(texture, position, spritePosition, Color.White);
        }
    }
}
