using HarmonyLib;
using UnityEngine;

namespace Polynautica
{
	class ArmsControllerPatch
	{
		[HarmonyPatch(typeof(ArmsController))]
		[HarmonyPatch("GetRelativeVelocity")]
		internal class GetRelativeVelocity
		{
			[HarmonyPrefix]
			public static bool Prefix(PlayerController __instance, ref Vector3 __result)
			{

				Vector3 velocity = __instance.player.playerController.velocity;
				Transform aimingTransform = __instance.player.camRoot.GetAimingTransform();
				Vector3 result = Vector3.zero;
				if (__instance.player.IsUnderwater() || !__instance.player.groundMotor.IsGrounded())
				{
					result = aimingTransform.InverseTransformDirection(velocity);
				}
				else
				{
					Vector3 forward = aimingTransform.forward;
					forward.y = 0f;
					forward.Normalize();
					result.z = Vector3.Dot(forward, velocity);
					Vector3 right = aimingTransform.right;
					right.y = 0f;
					right.Normalize();
					result.x = Vector3.Dot(right, velocity);
				}
				__result = result;
				return true;
			}
		}
	}
}
