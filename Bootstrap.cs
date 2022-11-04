using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

namespace Polynautica
{
    [QModCore]
    public class Bootstrap
    {
        [QModPatch]
        public static void Patch()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string modName = $"polybroo_{assembly.GetName().Name}";

            Logger.Log(Logger.Level.Info, "Patching" + modName);

            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);

            Logger.Log(Logger.Level.Info, "Patched successfully. Welcome to Polynatica!");
        }
    }
}
