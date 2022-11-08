using System.Reflection;
using HarmonyLib;
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

namespace NoMoreJittery
{
    [QModCore]
    public class Main
    {
        [QModPatch]
        public static void Patch()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string modName = $"polybro_{assembly.GetName().Name}";

            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);

            Logger.Log(Logger.Level.Info, "Thanks for using my mod!");
        }
    }
}
