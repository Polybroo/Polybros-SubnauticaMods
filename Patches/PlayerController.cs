using HarmonyLib;
using UnityEngine;

namespace Polynautica
{
	/// <summary>
	/// Find out why quick slots and cinematic animations won't work
	/// </summary>
	[HarmonyPatch(typeof(PlayerController))]
	class PlayerControllerPatch
	{
		[HarmonyPatch(nameof(PlayerController.FixedUpdate))]
		[HarmonyPrefix()]
		public static bool FixedUpdate(PlayerController __instance) => false;

		[HarmonyPatch(nameof(PlayerController.Start))]
		[HarmonyPrefix()]
		public static void Start(PlayerController __instance)
		{
			NewPlayerController controller = __instance.gameObject.AddComponent<NewPlayerController>();
			Player.main.playerController = controller;

			GameObject.FindObjectOfType<MainCameraControl>().playerController = controller;

			UnityEngine.Object.Destroy(__instance);
		}
	}
}
