using HarmonyLib;

namespace Polynautica
{
    /// <summary>
    /// TODO: Figure out how to change water color
    /// </summary>
    class WaterscapeVolumePatch
    {
        [HarmonyPatch(typeof(WaterscapeVolume))]
        [HarmonyPatch("Awake")]
        internal class Awake
        {
            [HarmonyPostfix]
            public static void Postfix(WaterscapeVolume __instance)
            {
                WaterscapeVolume volume = __instance;

                volume.causticsAmount = 0.125f;
                volume.causticsScale = 1.5f;

                volume.sunAttenuation = 0.65f;
                volume.sunLightAmount = 75f;


                volume.waterTransmission = 0.575f;
            }
        }
    }
}
