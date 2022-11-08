using HarmonyLib;
using UnityEngine;

namespace NoMoreJittery
{
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
			Player player = Player.main;
			MainCameraControl camControl = GameObject.FindObjectOfType<MainCameraControl>();
			FixedPlayerController newController = __instance.gameObject.AddComponent<FixedPlayerController>();

			player.playerController = newController;
			camControl.playerController = newController;
			player.rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

			UnityEngine.Object.Destroy(__instance);
		}
	}
}