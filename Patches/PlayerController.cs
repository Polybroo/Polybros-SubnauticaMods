using HarmonyLib;
using UnityEngine;

namespace Polynautica
{
	/// <summary>
	/// Find out why quick slots and cinematic animations won't work
	/// </summary>
	class PlayerControllerPatch
	{
		[HarmonyPatch(typeof(PlayerController))]
		[HarmonyPatch("FixedUpdate")]
		internal class FixedUpdate
		{
			[HarmonyPrefix]
			public static bool Prefix(PlayerController __instance)
			{
				return false;
			}
		}
		[HarmonyPatch(typeof(PlayerController))]
		[HarmonyPatch("Start")]
		internal class Start
		{
			[HarmonyPrefix]
			public static void Prefix(PlayerController __instance)
			{
				NewPlayerController controller = __instance.gameObject.AddComponent<NewPlayerController>();
				Traverse player = Traverse.Create(Player.main);
				player.Property("playerController").SetValue(controller);

				Traverse cameraControl = Traverse.Create(GameObject.FindObjectOfType<MainCameraControl>());
				cameraControl.Field("playerController").SetValue(controller);

				UnityEngine.Object.Destroy(__instance);
			}
		}
	}
}
