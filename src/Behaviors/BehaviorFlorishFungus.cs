using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace FlorishingFungus.src.Behaviors
{
    internal class BehaviorFlorishFungus : EntityBehavior
    {
        ITreeAttribute decayTree;
        JsonObject typeAttributes;


        public float HoursToDecay { get; set; }
        public int FungusToGrow { get; set; }
        public double TotalHoursDead
        {
            get { return decayTree.GetDouble("totalHoursDead"); }
            set { decayTree.SetDouble("totalHoursDead", value); }
        }


        public BehaviorFlorishFungus(Entity entity) : base(entity)
        {
        }

        public override void Initialize(EntityProperties properties, JsonObject typeAttributes)
        {
            base.Initialize(properties, typeAttributes);

            (entity as EntityAgent).AllowDespawn = false;

            this.typeAttributes = typeAttributes;
            HoursToDecay = typeAttributes["hoursToDecay"].AsFloat(96);
            FungusToGrow = typeAttributes["fungusToGrow"].AsInt(4);

            decayTree = entity.WatchedAttributes.GetTreeAttribute("decay");

            if (decayTree == null)
            {
                entity.WatchedAttributes.SetAttribute("decay", decayTree = new TreeAttribute());
                TotalHoursDead = entity.World.Calendar.TotalHours;
            }
        }

        public override void OnGameTick(float deltaTime)
        {
            if (!entity.Alive && TotalHoursDead + HoursToDecay < entity.World.Calendar.TotalHours)
            {
                EntityPos position = entity.Pos;

                List<BlockPos> SurroundSoil = SurroundingSoil(position);
                if (SurroundSoil.Count > 0)
                {
                    Block mushroom = FindClosestMushroom(position);
                    if (mushroom != null)
                    {
                        SpawnMushrooms(SurroundSoil, mushroom, FungusToGrow);
                    }
                }
            }

            base.OnGameTick(deltaTime);
        }

        public List<BlockPos> SurroundingSoil(EntityPos pos)
        {
            List<BlockPos> soilBlocks = new List<BlockPos>();

            BlockPos standingBlock = pos.AsBlockPos.AddCopy(BlockFacing.DOWN);

            List<BlockPos> potentialPositions = new List<BlockPos>
            {
                standingBlock.AddCopy(BlockFacing.NORTH),
                standingBlock.AddCopy(BlockFacing.SOUTH),
                standingBlock.AddCopy(BlockFacing.EAST),
                standingBlock.AddCopy(BlockFacing.WEST)
            };

            for (int i = 0; i < potentialPositions.Count; i++)
            {
                //get block at position
                var block = entity.World.BlockAccessor.GetBlock(potentialPositions[i]);
                if (block.FirstCodePart() == "soil")
                {
                    soilBlocks.Add(potentialPositions[i]);
                }
            }

            return soilBlocks;
        }

        public Block FindClosestMushroom(EntityPos pos)
        {
            Vec3d centerVec = pos.AsBlockPos.ToVec3d();
            Block closestMushroom = null;
            BlockPos closestMushroomPos = null;
            //check blocks 10 blocks around the entity for the closest mushroom
            for (int x = -10; x <= 10; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    for (int z = -10; z <= 10; z++)
                    {
                        BlockPos checkPos = pos.AsBlockPos.AddCopy(x, y, z);
                        var block = entity.World.BlockAccessor.GetBlock(checkPos);
                        if (block.FirstCodePart() == "mushroom" && block.LastCodePart() == "normal")
                        {
                            if (closestMushroomPos == null)
                            {
                                closestMushroomPos = checkPos;
                            }
                            else
                            {
                                double currentDistance = checkPos.ToVec3d().DistanceTo(centerVec);
                                double closestDistance = closestMushroomPos.ToVec3d().DistanceTo(centerVec);
                                if (currentDistance < closestDistance)
                                {
                                    closestMushroomPos = checkPos;
                                }
                            }
                        }
                    }
                }
            }

                if (closestMushroomPos != null)
                {
                    closestMushroom = entity.World.BlockAccessor.GetBlock(closestMushroomPos);
            }
            return closestMushroom;
        }

        public void SpawnMushrooms(List<BlockPos> SurroundingPos, Block Mushroom, int MushroomAmount)
        {
            if (SurroundingPos.Count == 0 || Mushroom == null || MushroomAmount == 0) return;

            int randomIndex = entity.World.Rand.Next(0, SurroundingPos.Count - 1);

            Block above = entity.World.BlockAccessor.GetBlock(SurroundingPos[randomIndex].AddCopy(BlockFacing.UP));
            if (above.IsReplacableBy(Mushroom))
            {
                entity.World.BlockAccessor.SetBlock(Mushroom.BlockId, SurroundingPos[randomIndex].AddCopy(BlockFacing.UP));
            }

            List<BlockPos> newSurroundingPos = new List<BlockPos>();
            for (int i = 0; i < SurroundingPos.Count; i++)
            {
                if (i != randomIndex)
                {
                    newSurroundingPos.Add(SurroundingPos[i]);
                }
            }
            SpawnMushrooms(newSurroundingPos, Mushroom, MushroomAmount - 1);
        }

        public override string PropertyName()
        {
            return "florishingfungus.florishfungus";
        }
    }
}