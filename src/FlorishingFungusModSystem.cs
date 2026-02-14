using FlorishingFungus.src.Behaviors;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace FlorishingFungus.src
{
    public class FlorishingFungusModSystem : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            api.RegisterEntityBehaviorClass(Mod.Info.ModID + ".florishfungus", typeof(BehaviorFlorishFungus));
        }

    }
}
