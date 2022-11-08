using HarmonyLib;

namespace NoMoreJittery
{
	[HarmonyPatch(typeof(PassSpeedParam))]
	class PassSpeedParamPatch
	{
		[HarmonyPatch(nameof(PassSpeedParam.GetParamValue))]
		[HarmonyPrefix()]
		public static bool Prefix(ref float __result)
		{
			__result = Utils.GetLocalPlayerComp().playerController.velocity.magnitude;
			return true;
		}
	}
}
