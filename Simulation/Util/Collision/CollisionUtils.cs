using Microsoft.Xna.Framework;
using Simulation.Game.Enums;
using Simulation.Game.Objects;
using Simulation.Game.Objects.Entities;
using Simulation.Game.World;
using Simulation.Util.Geometry;
using System.Collections.Generic;

namespace Simulation.Util.Collision
{
    public class CollisionUtils
    {
        public static bool IsSightBlocked(HitableObject origin, HitableObject target, int lineWidth)
        {
            if (origin.InteriorID != target.InteriorID)
                return true;

            Point originPoint = origin.Position.ToPoint();
            Point targetPoint = target.Position.ToPoint();

            Rect unionRect = ShapeCollision.ConvertLineToRect(originPoint, targetPoint);
            List<HitableObject> hittedObjects = GetHittedObjects(unionRect, origin.InteriorID, origin);

            Vector2[] polyRect = GeometryUtils.GetRectangleFromLine(originPoint, targetPoint, lineWidth);

            foreach (var hitableObject in hittedObjects)
                if (hitableObject != target && ShapeCollision.RectIntersectsPoly(hitableObject.HitBoxBounds, polyRect))
                    return true;

            // Check if blocks are of type hitable && if they intersect with the polyRect
            Point topLeft = GeometryUtils.GetChunkPosition(unionRect.Left, unionRect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point bottomRight = GeometryUtils.GetChunkPosition(unionRect.Right, unionRect.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    if (GetHitBoxTypeFromBlock(blockX, blockY, origin.InteriorID) == HitBoxType.HITABLE_BLOCK && ShapeCollision.RectIntersectsPoly(new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y), polyRect))
                        return true;

            return false;
        }

        public static bool IsSightBlocked(HitableObject origin, HitableObject target)
        {
            if (origin.InteriorID != target.InteriorID)
                return true;

            Vector2 originVector = origin.Position.ToVector();
            Vector2 targetVector = target.Position.ToVector();

            Rect unionRect = ShapeCollision.ConvertLineToRect(originVector, targetVector);
            List<HitableObject> hittedObjects = GetHittedObjects(unionRect, origin.InteriorID, origin);

            foreach(var hitableObject in hittedObjects)
                if (hitableObject != target && ShapeCollision.LineIntersectsRectangle(originVector, targetVector, hitableObject.HitBoxBounds))
                    return true;

            // Check if blocks are of type hitable
            Point topLeft = GeometryUtils.GetChunkPosition(unionRect.Left, unionRect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
            Point bottomRight = GeometryUtils.GetChunkPosition(unionRect.Right, unionRect.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

            for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    if (GetHitBoxTypeFromBlock(blockX, blockY, origin.InteriorID) == HitBoxType.HITABLE_BLOCK && ShapeCollision.LineIntersectsRectangle(originVector, targetVector, new Rect(blockX * WorldGrid.BlockSize.X, blockY * WorldGrid.BlockSize.Y, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y)))
                        return true;

            return false;
        }

        public static LivingEntity GetClosestLivingTarget(Circle hitboxBounds, string interiorId, HitableObject origin, int maxAggro)
        {
            Rect rectangleHitboxBounds = new Rect(hitboxBounds.CenterX - hitboxBounds.Radius, hitboxBounds.CenterY - hitboxBounds.Radius, 2 * hitboxBounds.Radius, 2 * hitboxBounds.Radius);
            LivingEntity livingTarget = GetClosestLivingTarget(rectangleHitboxBounds, interiorId, origin, maxAggro);

            return hitboxBounds.Intersects(livingTarget.HitBoxBounds) ? livingTarget : null;
        }

        public static LivingEntity GetClosestLivingTarget(Rect hitboxBounds, string interiorId, HitableObject origin, int maxAggro)
        {
            ThreadingUtils.assertMainThread();

            HitableObject closestTarget = null;
            float closestDistance = float.PositiveInfinity;

            if (interiorId == Interior.Outside)
            {
                // Check collision with interactive && contained objects
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(hitboxBounds.Left, hitboxBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(hitboxBounds.Right, hitboxBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkX, chunkY);

                        if (worldGridChunk.OverlappingObjects != null)
                            foreach (HitableObject hitableObject in worldGridChunk.OverlappingObjects)
                                if (hitableObject is LivingEntity && 
                                    hitableObject != origin && 
                                    ((LivingEntity)origin).GetAggroTowardsEntity((LivingEntity)hitableObject) <= maxAggro && 
                                    hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                                {
                                    var distance = GeometryUtils.GetDiagonalDistance(hitableObject.Position.X, hitableObject.Position.Y, origin.Position.X, origin.Position.Y);

                                    if(distance < closestDistance)
                                    {
                                        closestDistance = distance;
                                        closestTarget = hitableObject;
                                    }
                                }
                                    

                        if (worldGridChunk.ContainedObjects != null)
                            foreach (var hitableObject in worldGridChunk.ContainedObjects)
                                if (hitableObject is LivingEntity && 
                                    hitableObject != origin &&
                                    ((LivingEntity)origin).GetAggroTowardsEntity((LivingEntity)hitableObject) <= maxAggro &&
                                    hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                                {
                                    var distance = GeometryUtils.GetDiagonalDistance(hitableObject.Position.X, hitableObject.Position.Y, origin.Position.X, origin.Position.Y);

                                    if (distance < closestDistance)
                                    {
                                        closestDistance = distance;
                                        closestTarget = hitableObject;
                                    }
                                }
                    }
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(interiorId);

                foreach (var hitableObject in interior.ContainedObjects)
                    if (hitableObject is LivingEntity && 
                        hitableObject != origin &&
                        ((LivingEntity)origin).GetAggroTowardsEntity((LivingEntity)hitableObject) <= maxAggro &&
                        hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                    {
                        var distance = GeometryUtils.GetDiagonalDistance(hitableObject.Position.X, hitableObject.Position.Y, origin.Position.X, origin.Position.Y);

                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = hitableObject;
                        }
                    }
            }

            return (LivingEntity)closestTarget;
        }

        public static List<LivingEntity> GetLivingHittedObjects(Circle hitboxBounds, string interiorId, HitableObject origin, int maxAggro)
        {
            Rect rectangleHitboxBounds = new Rect(hitboxBounds.CenterX - hitboxBounds.Radius, hitboxBounds.CenterY - hitboxBounds.Radius, 2 * hitboxBounds.Radius, 2 * hitboxBounds.Radius);
            List<LivingEntity> hittedObjects = GetLivingHittedObjects(rectangleHitboxBounds, interiorId, origin);

            for (int i = 0; i < hittedObjects.Count; i++)
            {
                if (hitboxBounds.Intersects(hittedObjects[i].HitBoxBounds) == false || ((LivingEntity)origin).GetAggroTowardsEntity(hittedObjects[i]) > maxAggro)
                {
                    hittedObjects.RemoveAt(i);
                    i--;
                }
            }

            return hittedObjects;
        }

        public static List<LivingEntity> GetLivingHittedObjects(Rect hitboxBounds, string interiorId, HitableObject origin, int maxAggro)
        {
            List<LivingEntity> hittedObjects = GetLivingHittedObjects(hitboxBounds, interiorId, origin);

            for (int i = 0; i < hittedObjects.Count; i++)
            {
                if (((LivingEntity)origin).GetAggroTowardsEntity(hittedObjects[i]) > maxAggro)
                {
                    hittedObjects.RemoveAt(i);
                    i--;
                }
            }

            return hittedObjects;
        }

        public static List<LivingEntity> GetLivingHittedObjects(Circle hitboxBounds, string interiorId, HitableObject origin)
        {
            Rect rectangleHitboxBounds = new Rect(hitboxBounds.CenterX - hitboxBounds.Radius, hitboxBounds.CenterY - hitboxBounds.Radius, 2 * hitboxBounds.Radius, 2 * hitboxBounds.Radius);
            List<LivingEntity> hittedObjects = GetLivingHittedObjects(rectangleHitboxBounds, interiorId, origin);

            for (int i = 0; i < hittedObjects.Count; i++)
            {
                if (hitboxBounds.Intersects(hittedObjects[i].HitBoxBounds) == false)
                {
                    hittedObjects.RemoveAt(i);
                    i--;
                }
            }

            return hittedObjects;
        }

        public static List<LivingEntity> GetLivingHittedObjects(Rect hitboxBounds, string interiorId, HitableObject origin)
        {
            ThreadingUtils.assertMainThread();
            List<LivingEntity> hittedObjecs = new List<LivingEntity>();

            if (interiorId == Interior.Outside)
            {
                // Check collision with interactive && contained objects
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(hitboxBounds.Left, hitboxBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(hitboxBounds.Right, hitboxBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkX, chunkY);

                        if (worldGridChunk.OverlappingObjects != null)
                            foreach (HitableObject hitableObject in worldGridChunk.OverlappingObjects)
                                if (hitableObject is LivingEntity && hitableObject.IsHitable && hitableObject != origin && hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                                    if (hittedObjecs.Contains((LivingEntity)hitableObject) == false)
                                        hittedObjecs.Add((LivingEntity)hitableObject);

                        if (worldGridChunk.ContainedObjects != null)
                            foreach (var hitableObject in worldGridChunk.ContainedObjects)
                                if (hitableObject is LivingEntity && hitableObject.IsHitable && hitableObject != origin && hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                                    if (hittedObjecs.Contains((LivingEntity)hitableObject) == false)
                                        hittedObjecs.Add((LivingEntity)hitableObject);
                    }
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(origin.InteriorID);

                foreach (var hitableObject in interior.ContainedObjects)
                    if (hitableObject is LivingEntity && hitableObject.IsHitable && hitableObject != origin && hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                        if (hittedObjecs.Contains((LivingEntity)hitableObject) == false)
                            hittedObjecs.Add((LivingEntity)hitableObject);
            }

            return hittedObjecs;
        }

        public static List<HitableObject> GetHittedObjects(Circle hitboxBounds, string interiorId, HitableObject origin)
        {
            Rect rectangleHitboxBounds = new Rect(hitboxBounds.CenterX - hitboxBounds.Radius, hitboxBounds.CenterY - hitboxBounds.Radius, 2 * hitboxBounds.Radius, 2 * hitboxBounds.Radius);
            List<HitableObject> hittedObjects = GetHittedObjects(rectangleHitboxBounds, interiorId, origin);
            
            for(int i=0;i<hittedObjects.Count;i++)
            {
                if(hitboxBounds.Intersects(hittedObjects[i].HitBoxBounds) == false)
                {
                    hittedObjects.RemoveAt(i);
                    i--;
                }
            }

            return hittedObjects;
        }

        public static List<HitableObject> GetHittedObjects(Rect hitboxBounds, string interiorId, HitableObject origin)
        {
            ThreadingUtils.assertMainThread();
            List<HitableObject> hittedObjecs = new List<HitableObject>();

            if (interiorId == Interior.Outside)
            {
                // Check collision with interactive && contained objects
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(hitboxBounds.Left, hitboxBounds.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(hitboxBounds.Right, hitboxBounds.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkX, chunkY);

                        if (worldGridChunk.OverlappingObjects != null)
                            foreach (HitableObject hitableObject in worldGridChunk.OverlappingObjects)
                                if (hitableObject != origin && hitableObject.IsHitable && hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                                    if (hittedObjecs.Contains(hitableObject) == false)
                                        hittedObjecs.Add(hitableObject);

                        if (worldGridChunk.ContainedObjects != null)
                            foreach (var hitableObject in worldGridChunk.ContainedObjects)
                                if (hitableObject != origin && hitableObject.IsHitable && hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                                    if (hittedObjecs.Contains(hitableObject) == false)
                                        hittedObjecs.Add(hitableObject);
                    }
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(interiorId);

                foreach (var hitableObject in interior.ContainedObjects)
                    if (hitableObject != origin && hitableObject.IsHitable && hitableObject.HitBoxBounds.Intersects(hitboxBounds))
                        if (hittedObjecs.Contains(hitableObject) == false)
                            hittedObjecs.Add(hitableObject);
            }

            return hittedObjecs;
        }

        public static bool IsHitableBlockHitted(Rect hitboxBounds, string interiorID = null)
        {
            ThreadingUtils.assertMainThread();

            if (interiorID == Interior.Outside)
            {
                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.GetChunkPosition(hitboxBounds.Left, hitboxBounds.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.GetChunkPosition(hitboxBounds.Right, hitboxBounds.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        Point chunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkPos.X, chunkPos.Y);

                        BlockType blockType = worldGridChunk.GetBlockType(blockX, blockY);

                        if (GetHitBoxTypeFromBlock(blockType) == HitBoxType.HITABLE_BLOCK)
                            return true;
                    }
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(interiorID);

                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.GetChunkPosition(hitboxBounds.Left, hitboxBounds.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.GetChunkPosition(hitboxBounds.Right, hitboxBounds.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        BlockType blockType = interior.GetBlockType(blockX, blockY);

                        if (GetHitBoxTypeFromBlock(blockType) == HitBoxType.HITABLE_BLOCK)
                            return true;
                    }
            }

            return false;
        }

        public static bool IsRectBlockedFast(HitableObject origin, Rect rect)
        {
            ThreadingUtils.assertMainThread();

            if (origin.InteriorID == Interior.Outside)
            {
                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.GetChunkPosition(rect.Left, rect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.GetChunkPosition(rect.Right, rect.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        Point chunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkPos.X, chunkPos.Y);

                        if (!SimulationGame.World.WalkableGrid.IsBlockWalkable(blockX, blockY))
                            return true;
                    }

                return false;
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(origin.InteriorID);

                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.GetChunkPosition(rect.Left, rect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.GetChunkPosition(rect.Right, rect.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        if (blockX < 0 || blockX >= interior.Dimensions.X)
                            return true;
                        if (blockY < 0 || blockY >= interior.Dimensions.Y)
                            return true;

                        BlockType blockType = interior.GetBlockType(blockX, blockY);

                        if (CollisionUtils.GetBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                            return true;
                    }

                foreach (var hitableObject in interior.ContainedObjects)
                    if (hitableObject.BlockingType == BlockingType.BLOCKING && hitableObject.IsHitable && hitableObject != origin && hitableObject.BlockingBounds.Intersects(rect))
                        return true;

                return false;
            }
        }

        public static bool IsRectBlockedAccurate(HitableObject origin, Rect rect)
        {
            ThreadingUtils.assertMainThread();

            if(origin.InteriorID == Interior.Outside)
            {
                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.GetChunkPosition(rect.Left, rect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.GetChunkPosition(rect.Right, rect.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        Point chunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkPos.X, chunkPos.Y);

                        BlockType blockType = worldGridChunk.GetBlockType(blockX, blockY);

                        if (CollisionUtils.GetBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                            return true;
                    }

                // Check collision with interactive && contained objects
                Point chunkTopLeft = GeometryUtils.GetChunkPosition(rect.Left, rect.Top, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);
                Point chunkBottomRight = GeometryUtils.GetChunkPosition(rect.Right, rect.Bottom, WorldGrid.WorldChunkPixelSize.X, WorldGrid.WorldChunkPixelSize.Y);

                for (int chunkX = chunkTopLeft.X; chunkX <= chunkBottomRight.X; chunkX++)
                    for (int chunkY = chunkTopLeft.Y; chunkY <= chunkBottomRight.Y; chunkY++)
                    {
                        WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkX, chunkY);

                        if (worldGridChunk.OverlappingObjects != null)
                            foreach (HitableObject hitableObject in worldGridChunk.OverlappingObjects)
                                if (hitableObject.BlockingType == BlockingType.BLOCKING && hitableObject.IsHitable && hitableObject != origin && hitableObject.BlockingBounds.Intersects(rect))
                                    return true;

                        if (worldGridChunk.ContainedObjects != null)
                            foreach (var hitableObject in worldGridChunk.ContainedObjects)
                                if (hitableObject.BlockingType == BlockingType.BLOCKING && hitableObject.IsHitable && hitableObject != origin && hitableObject.BlockingBounds.Intersects(rect))
                                    return true;
                    }

                return false;
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(origin.InteriorID);

                // Check if blocks are of type blocking
                Point topLeft = GeometryUtils.GetChunkPosition(rect.Left, rect.Top, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);
                Point bottomRight = GeometryUtils.GetChunkPosition(rect.Right, rect.Bottom, WorldGrid.BlockSize.X, WorldGrid.BlockSize.Y);

                for (int blockX = topLeft.X; blockX <= bottomRight.X; blockX++)
                    for (int blockY = topLeft.Y; blockY <= bottomRight.Y; blockY++)
                    {
                        if (blockX < 0 || blockX >= interior.Dimensions.X)
                            return true;
                        if (blockY < 0 || blockY >= interior.Dimensions.Y)
                            return true;

                        BlockType blockType = interior.GetBlockType(blockX, blockY);

                        if (CollisionUtils.GetBlockingTypeFromBlock(blockType) == BlockingType.BLOCKING)
                            return true;
                    }

                foreach (var hitableObject in interior.ContainedObjects)
                    if (hitableObject.BlockingType == BlockingType.BLOCKING && hitableObject.IsHitable && hitableObject != origin && hitableObject.BlockingBounds.Intersects(rect))
                        return true;

                return false;
            }
        }

        public static BlockingType GetBlockingTypeFromBlock(BlockType blockType)
        {
            switch(blockType)
            {
                case BlockType.NONE:
                case BlockType.GRASS_WATERHOLE:
                    return BlockingType.BLOCKING;
                default:
                    return BlockingType.NOT_BLOCKING;
            }
        }

        public static HitBoxType GetHitBoxTypeFromBlock(BlockType blockType)
        {
            switch (blockType)
            {
                case BlockType.NONE:
                case BlockType.GRASS_WATERHOLE:
                    return HitBoxType.HITABLE_BLOCK;
                default:
                    return HitBoxType.NO_HITBOX;
            }
        }

        public static BlockingType GetBlockingTypeFromBlock(int blockX, int blockY, string interiorID)
        {
            if (interiorID == Interior.Outside)
            {
                Point chunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkPos.X, chunkPos.Y);
                BlockType blockType = worldGridChunk.GetBlockType(blockX, blockY);

                return GetBlockingTypeFromBlock(blockType);
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(interiorID);
                BlockType blockType = interior.GetBlockType(blockX, blockY);

                return GetBlockingTypeFromBlock(blockType);
            }
        }

        public static HitBoxType GetHitBoxTypeFromBlock(int blockX, int blockY, string interiorID)
        {
            if (interiorID == Interior.Outside)
            {
                Point chunkPos = GeometryUtils.GetChunkPosition(blockX, blockY, WorldGrid.WorldChunkBlockSize.X, WorldGrid.WorldChunkBlockSize.Y);
                WorldGridChunk worldGridChunk = SimulationGame.World.GetFromChunkPoint(chunkPos.X, chunkPos.Y);
                BlockType blockType = worldGridChunk.GetBlockType(blockX, blockY);

                return GetHitBoxTypeFromBlock(blockType);
            }
            else
            {
                Interior interior = SimulationGame.World.InteriorManager.Get(interiorID);
                BlockType blockType = interior.GetBlockType(blockX, blockY);

                return GetHitBoxTypeFromBlock(blockType);
            }
        }
    }
}
