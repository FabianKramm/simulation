using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.MetaData;
using Simulation.Game.Objects.Entities;
using Simulation.Game.Serialization;
using Simulation.Util;
using Simulation.Util.Collision;
using Simulation.Util.Geometry;
using System.Diagnostics;

namespace Simulation.Game.World
{
    public class Interior: WorldPart
    {
        public static string Outside = null;

        [Serialize]
        public string ID;

        // Used for json
        private Interior() { }

        public Interior(Point dimensions)
        {
            ID = Util.Util.GetUUID();
            Dimensions = dimensions;
            blockingGrid = new int[dimensions.X, dimensions.Y];
        }

        public void ChangeDimensions(Point newDimensions)
        {
            var newBlockingGrid = new int[newDimensions.X, newDimensions.Y];
            var newBounds = new Rectangle(0, 0, newDimensions.X * WorldGrid.BlockSize.X, newDimensions.Y * WorldGrid.BlockSize.Y);

            for (int blockX=0;blockX < newDimensions.X;blockX++)
                for (int blockY = 0; blockY < newDimensions.Y; blockY++)
                {
                    if(blockX >= Dimensions.X || blockY >= Dimensions.Y)
                    {
                        newBlockingGrid[blockX, blockY] = 0;
                        continue;
                    }
                    else
                    {
                        newBlockingGrid[blockX, blockY] = blockingGrid[blockX, blockY];
                    }
                }

            if(ContainedObjects != null)
                for (int i = 0; i < ContainedObjects.Count; i++)
                {
                    var containedObject = ContainedObjects[i];

                    if (newBounds.Contains(containedObject.Position.ToPoint()) == false)
                    {
                        if(containedObject is DurableEntity)
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot change dimensions because Durable Entity would be destroyed!");
                            return;
                        }

                        containedObject.DisconnectFromWorld();
                        containedObject.Destroy();

                        i--;
                    }
                }

            if (AmbientObjects != null)
                for (int i=0;i<AmbientObjects.Count;i++)
                {
                    var ambientObject = AmbientObjects[i];

                    if (newBounds.Contains(ambientObject.Position.ToPoint()) == false)
                    {
                        ambientObject.DisconnectFromWorld();
                        ambientObject.Destroy();

                        i--;
                    }
                }

            if(WorldLinks != null)
            {
                WorldLink[] wordLinks = new WorldLink[WorldLinks.Count];
                WorldLinks.Values.CopyTo(wordLinks, 0);

                foreach (var worldLink in wordLinks)
                {
                    if (newBounds.Contains(worldLink.ToRealWorldPositionFrom().ToPoint()) == false)
                    {
                        SimulationGame.World.UpdateWorldLink(worldLink);
                    }
                }
            }

            blockingGrid = newBlockingGrid;
            Dimensions = newDimensions;
        }

        public override int GetBlockType(int blockX, int blockY)
        {
            if (blockX < 0 || blockX >= Dimensions.X || blockY < 0 || blockY >= Dimensions.Y)
                return BlockType.None;

            return blockingGrid[blockX, blockY];
        }

        public override void SetBlockType(int blockX, int blockY, int blockType)
        {
            blockingGrid[blockX, blockY] = blockType;
        }

        public override void AddWorldLink(WorldLink worldLink)
        {
            Debug.Assert(worldLink.FromInteriorID == ID, "FromInteriorID is different than interior ID!");
            Debug.Assert(worldLink.FromBlock.X >= 0 && worldLink.FromBlock.X < Dimensions.X && worldLink.FromBlock.Y >= 0 && worldLink.FromBlock.Y < Dimensions.Y, "FromBlock is has wrong size!");
            
            base.AddWorldLink(worldLink);
        }

        public bool IsBlockWalkable(int blockX, int blockY)
        {
            ThreadingUtils.assertChildThread();

            if(CollisionUtils.GetBlockingTypeFromBlock(GetBlockType(blockX, blockY)) == BlockingType.BLOCKING)
            {
                return false;
            }

            Rect blockBounds = new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            if (ContainedObjects != null)
            {
                lock(ContainedObjects)
                {
                    foreach (var containedObject in ContainedObjects)
                    {
                        lock(containedObject)
                        {
                            if (containedObject.BlockingType == BlockingType.BLOCKING && containedObject.BlockingBounds.Intersects(blockBounds))
                                return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
