using NLog;
using System;
using System.IO;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Session;
using Sandbox.Game;
using Sandbox.Game.Entities.Character;
using System.Reflection;
using Torch.Managers.PatchManager;

namespace FixNpcLoot
{
    public class FixNpcLoot : TorchPluginBase, IWpfPlugin
    {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private static readonly string CONFIG_FILE_NAME = "FixNpcLootConfig.cfg";

        private FixNpcLootControl _control;
        public UserControl GetControl() => _control ?? (_control = new FixNpcLootControl(this));

        private Persistent<FixNpcLootConfig> _config;
        public FixNpcLootConfig Config => _config?.Data;

        [PatchShim]
        public static class MyInventoryPatch
        {

            public static readonly Logger Log = LogManager.GetCurrentClassLogger();

            internal static readonly MethodInfo update =
                typeof(MyInventory).GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public) ??
                throw new Exception("Failed to find patch method");

            internal static readonly MethodInfo updatePatch =
                typeof(MyInventoryPatch).GetMethod(nameof(ClearPath), BindingFlags.Static | BindingFlags.Public) ??
                throw new Exception("Failed to find patch method");

            public static bool ClearPath(MyInventory __instance)
            {
                var character = __instance.Owner as MyCharacter;
                // If in definition character set EnableSpawnInventoryAsContainer False skip clear inventory because this character is npc
                if (character != null && !character.Definition.EnableSpawnInventoryAsContainer) return false; 
                return true;

            }

            public static void Patch(PatchContext ctx)
            {
                ctx.GetPattern(update).Prefixes.Add(updatePatch);
                Log.Info("Patching Successful MyInventory.Clear (Fix NPC loot)!");
            }
        }

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);

            SetupConfig();

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

            Save();
        }

        private void SessionChanged(ITorchSession session, TorchSessionState state)
        {

            switch (state)
            {

                case TorchSessionState.Loaded:
                    Log.Info("Session Loaded!");
                    break;

                case TorchSessionState.Unloading:
                    Log.Info("Session Unloading!");
                    break;
            }
        }

        private void SetupConfig()
        {

            var configFile = Path.Combine(StoragePath, CONFIG_FILE_NAME);

            try
            {

                _config = Persistent<FixNpcLootConfig>.Load(configFile);

            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            if (_config?.Data == null)
            {

                Log.Info("Create Default Config, because none was found!");

                _config = new Persistent<FixNpcLootConfig>(configFile, new FixNpcLootConfig());
                _config.Save();
            }
        }

        public void Save()
        {
            try
            {
                _config.Save();
                Log.Info("Configuration Saved.");
            }
            catch (IOException e)
            {
                Log.Warn(e, "Configuration failed to save");
            }
        }
    }
}
