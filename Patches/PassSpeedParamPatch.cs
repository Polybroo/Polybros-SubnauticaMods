using HarmonyLib;

namespace Polynautica
{
	class PassSpeedParamPatch
	{
		[HarmonyPatch(typeof(PassSpeedParam))]
		[HarmonyPatch("GetParamValue")]
		internal class GetParamValue
		{
			[HarmonyPrefix]
			public static bool Prefix(ref float __result)
			{
				__result = Utils.GetLocalPlayerComp().playerController.velocity.magnitude;
				return true;
			}
		}
	}
}
