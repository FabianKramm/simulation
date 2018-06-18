using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Base;
using System;
using System.Collections.Generic;

namespace Simulation.Game.World
{
    public enum BlockType
    {
        GRASS = 0,
        GRASS_WATERHOLE = 1,
    }

    public enum BlockingType
    {
        NOT_BLOCKING = 0,
        BLOCKING
    }

    public enum HitBoxType
    {
        NO_HITBOX = 0,
        STATIC_OBJECT,
        MOVING_OBJECT,
        LIVING_ENTITY
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

        public BlockType blockType;
        private Texture2D texture;
        private Rectangle spritePosition;
        private Vector2 worldPosition;

        public Rectangle blockBounds
        {
            get; private set;
        }

        public Point position
        {
            get; private set;
        }

        public BlockingType blockingType
        {
            get; private set;
        }

        // Objects that are hitable and can block the path
        public List<HitableObject> hitableObjects = new List<HitableObject>();

        // Not yet defined
        // public List<HitBoxRectangle> interactiveObjects = new List<HitBoxRectangle>();

        // Objects that should be drawn on this tile
        public List<DrawableObject> ambientObjects = new List<DrawableObject>();

        public Block(Point position, BlockType blockType = BlockType.GRASS)
        {
            this.blockType = blockType;
            this.position = position;
            worldPosition = new Vector2(position.X * World.BlockSize.X, position.Y * World.BlockSize.Y);
            blockBounds = new Rectangle(position.X * World.BlockSize.X, position.Y * World.BlockSize.Y, World.BlockSize.X, World.BlockSize.Y);

            int randomTexture = random.Next(0, Blocks[blockType].Length);

            texture = SimulationGame.contentManager.Load<Texture2D>(Blocks[blockType][randomTexture].Item1);
            spritePosition = Blocks[blockType][randomTexture].Item2;

            if(blockType == BlockType.GRASS_WATERHOLE)
            {
                blockingType = BlockingType.BLOCKING;
            }
            else
            {
                blockingType = BlockingType.NOT_BLOCKING;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, worldPosition, spritePosition, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
           
            foreach (DrawableObject ambientObject in ambientObjects)
                ambientObject.Draw(spriteBatch);
        }
    }
}
