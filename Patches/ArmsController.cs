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
				Traverse armsController = Traverse.Create(__instance);
				Traverse<Player> player = armsController.Field<Player>("player");

				Vector3 velocity = player.Value.playerController.velocity;
				Transform aimingTransform = player.Value.camRoot.GetAimingTransform();
				Vector3 result = Vector3.zero;
				if (player.Value.IsUnderwater() || !player.Value.groundMotor.IsGrounded())
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
