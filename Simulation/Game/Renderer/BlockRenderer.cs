using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulation.Game.Enums;
using Simulation.Game.MetaData;
using Simulation.Game.World;
using System.Collections.Generic;

namespace Simulation.Game.Renderer
{
    public class BlockRenderer
    { 
        public static void Draw(SpriteBatch spriteBatch, int realX, int realY, int blockId)
        {
            if (blockId == BlockType.None) return;

            Color color = GameRenderer.BlendColor;
            var blockType = BlockType.lookup[blockId];

            if(SimulationGame.IsDebug && SimulationGame.Player.InteriorID == Interior.Outside && !SimulationGame.World.WalkableGrid.IsPositionWalkable(realX, realY))
            {
                color = Color.Red;
            }

            spriteBatch.Draw(SimulationGame.ContentManager.Load<Texture2D>(blockType.SpritePath), 
                new Vector2(realX, realY), new Rectangle(blockType.SpritePostion, blockType.SpriteBounds), color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
