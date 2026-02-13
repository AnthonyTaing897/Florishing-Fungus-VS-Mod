using FlorishingFungus.src.Behaviors;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace FlorishingFungus.src
{
    public class FlorishingFungusModSystem : ModSystem
    {

        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public override void Start(ICoreAPI api)
        {
            Mod.Logger.Notification("Hello from template mod: " + api.Side);

            api.RegisterEntityBehaviorClass(Mod.Info.ModID + ".florishfungus", typeof(BehaviorFlorishFungus));
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            Mod.Logger.Notification("Hello from template mod server side: " + Lang.Get("florishingfungus:hello"));
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            Mod.Logger.Notification("Hello from template mod client side: " + Lang.Get("florishingfungus:hello"));
        }

    }
}
