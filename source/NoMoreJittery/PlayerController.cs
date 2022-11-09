using HarmonyLib;
using UnityEngine;

namespace NoMoreJittery
{
	[HarmonyPatch(typeof(Player))]
	class PlayerPatch
	{
		[HarmonyPatch(nameof(Player.Awake))]
		[HarmonyPostfix()]
		public static void Awake(Player __instance)
		{
			Player player = __instance;
			MainCameraControl cameraControl = GameObject.FindObjectOfType<MainCameraControl>();
			FixedPlayerController newController = __instance.gameObject.AddComponent<FixedPlayerController>();

			UnityEngine.Object.Destroy(__instance.playerController);

			player.playerController = newController;
			cameraControl.playerController = newController;

			player.rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
		}
	}
}