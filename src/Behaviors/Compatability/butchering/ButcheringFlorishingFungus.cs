using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;


# nullable disable
namespace FlorishingFungus.src.Behaviors.Compatability.butchering
{
    public class ButcheringFlorishingFungus : CollectibleBehavior
    {
        private ICoreAPI api;

        public int FungusToGrow { get; set; }
        public override void OnLoaded(ICoreAPI api)
        {
            
            base.OnLoaded(api);
            this.api = api;
        }
        public ButcheringFlorishingFungus(CollectibleObject collObj) : base(collObj)
        {
        }

        public override void Initialize(JsonObject properties)
        {
            base.Initialize(properties);

            this.FungusToGrow = properties["fungusToGrow"].AsInt(4);
        }

        public override ItemStack OnTransitionNow(ItemSlot slot, TransitionableProperties props, ref EnumHandling handling)
        {
            //get block at position of the itemstack 
            BlockPos pos = slot.Inventory.Pos;

            //get collectibe name from itemstack
            string collectibleage1 = slot.Itemstack.Collectible.FirstCodePart(1);
            string collectibleage2 = slot.Itemstack.Collectible.FirstCodePart(2);

            if (collectibleage1 == "baby" || collectibleage2 == "baby")
            {
                this.FungusToGrow = 1;
            }
            this.api.Logger.Notification("Transitioning item at position: " + pos);

            if (pos != null)
            {
                List<BlockPos> SurroundSoil = SurroundingSoil(pos);

                if (SurroundSoil.Count > 0)
                {
                    Block Mushroom = FindClosestMushroom(pos);
                    if (Mushroom != null)
                    {
                        SpawnMushrooms(SurroundSoil, Mushroom, this.FungusToGrow);
                    }
                }
            }
            return base.OnTransitionNow(slot, props, ref handling);
        }

        public List<BlockPos> SurroundingSoil(BlockPos pos)
        {
            List<BlockPos> soilBlocks = new List<BlockPos>();

            BlockPos standingBlock = pos.AddCopy(BlockFacing.DOWN);

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
                var block = api.World.BlockAccessor.GetBlock(potentialPositions[i]);
                if (block.FirstCodePart() == "soil")
                {
                    soilBlocks.Add(potentialPositions[i]);
                }
            }

            return soilBlocks;
        }

        public Block FindClosestMushroom(BlockPos pos)
        {
            Vec3d centerVec = pos.ToVec3d();
            Block closestMushroom = null;
            BlockPos closestMushroomPos = null;
            //check blocks 10 blocks around the entity for the closest mushroom
            for (int x = -10; x <= 10; x++)
            {
                for (int y = -2; y <= 2; y++)
                {
                    for (int z = -10; z <= 10; z++)
                    {
                        BlockPos checkPos = pos.AddCopy(x, y, z);
                        var block = api.World.BlockAccessor.GetBlock(checkPos);
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
                closestMushroom = api.World.BlockAccessor.GetBlock(closestMushroomPos);
            }
            return closestMushroom;
        }

        public void SpawnMushrooms(List<BlockPos> SurroundingPos, Block Mushroom, int MushroomAmount)
        {
            if (SurroundingPos.Count == 0 || Mushroom == null || MushroomAmount == 0) return;

            int randomIndex = api.World.Rand.Next(0, SurroundingPos.Count - 1);

            Block above = api.World.BlockAccessor.GetBlock(SurroundingPos[randomIndex].AddCopy(BlockFacing.UP));
            if (above.IsReplacableBy(Mushroom))
            {
                api.World.BlockAccessor.SetBlock(Mushroom.BlockId, SurroundingPos[randomIndex].AddCopy(BlockFacing.UP));
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
    }
}
