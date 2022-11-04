using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace Polynautica
{
	/// <summary>
	/// TODO: Make smooth camera control
	/// </summary>
	class MainCameraControlPatch
    {
        [HarmonyPatch(typeof(MainCameraControl))]
        [HarmonyPatch("Update")]
        internal class Update
        {
            [HarmonyPrefix]
            public static bool Prefix(MainCameraControl __instance)
			{
				//Debug.Log("MainCameraControl loop works!");

				Traverse instanceTraverse = Traverse.Create(__instance);

				Traverse<PlayerController> playerController = instanceTraverse.Field<PlayerController>("playerController");
				Traverse<UnderWaterTracker> underWaterTracker = instanceTraverse.Field<UnderWaterTracker>("underWaterTracker");

				Traverse<float> swimCameraAnimation = instanceTraverse.Field<float>("swimCameraAnimation");
				Traverse<float> smoothedSpeed = instanceTraverse.Field<float>("smoothedSpeed");
				Traverse<float> camShake = instanceTraverse.Field<float>("camShake");
				Traverse<float> strafeTilt = instanceTraverse.Field<float>("strafeTilt");
				Traverse<float> impactBob = instanceTraverse.Field<float>("impactBob");
				Traverse<float> impactForce = instanceTraverse.Field<float>("impactForce");
				Traverse<float> viewModelLockedYaw = instanceTraverse.Field<float>("viewModelLockedYaw");

				Traverse<bool> wasInLockedMode = instanceTraverse.Field<bool>("wasInLockedMode");
				Traverse<bool> wasInLookAroundMode = instanceTraverse.Field<bool>("wasInLookAroundMode");

				if (underWaterTracker.Value.isUnderWater)
				{
					swimCameraAnimation.Value = Mathf.Clamp01(swimCameraAnimation.Value + Time.deltaTime);
				}
				else
				{
					swimCameraAnimation.Value = Mathf.Clamp01(swimCameraAnimation.Value - Time.deltaTime);
				}
				float num = __instance.minimumY;
				float num2 = __instance.maximumY;
				Vector3 velocity = playerController.Value.velocity;
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool inExosuit = Player.main.inExosuit;
				bool flag4 = uGUI_BuilderMenu.IsOpen();
				if (Player.main != null)
				{
					flag = Player.main.GetPDA().isInUse;
					flag3 = (Player.main.motorMode == Player.MotorMode.Vehicle);
					flag2 = (flag || flag3 || __instance.cinematicMode);
					if (XRSettings.enabled && VROptions.gazeBasedCursor)
					{
						flag2 = (flag2 || flag4);
					}
				}
				if (flag2 != wasInLockedMode.Value || __instance.lookAroundMode != wasInLookAroundMode.Value)
				{
					__instance.camRotationX = 0f;
					__instance.camRotationY = 0f;
					wasInLockedMode.Value = flag2;
					wasInLookAroundMode.Value = __instance.lookAroundMode;
				}
				bool flag5 = (!__instance.cinematicMode || (__instance.lookAroundMode && !flag)) && __instance.mouseLookEnabled && (flag3 || AvatarInputHandler.main == null || AvatarInputHandler.main.IsEnabled() || Builder.isPlacing);
				if (flag3 && !XRSettings.enabled && !inExosuit)
				{
					flag5 = false;
				}
				Transform transform = __instance.transform;
				float num3 = (float)((flag || __instance.lookAroundMode || Player.main.GetMode() == Player.Mode.LockedPiloting) ? 1 : -1);
				if (!flag2 || (__instance.cinematicMode && !__instance.lookAroundMode))
				{
					__instance.cameraOffsetTransform.localEulerAngles = UWE.Utils.LerpEuler(__instance.cameraOffsetTransform.localEulerAngles, Vector3.zero, Time.deltaTime * 5f);
				}
				else
				{
					transform = __instance.cameraOffsetTransform;
					__instance.rotationY = Mathf.LerpAngle(__instance.rotationY, 0f, Time.deltaTime * 10f);
					__instance.transform.localEulerAngles = new Vector3(-__instance.rotationY, __instance.rotationX, 0f);
					__instance.cameraUPTransform.localEulerAngles = UWE.Utils.LerpEuler(__instance.cameraUPTransform.localEulerAngles, Vector3.zero, Time.deltaTime * 30f);
				}
				if (!XRSettings.enabled)
				{
					Vector3 localPosition = __instance.cameraOffsetTransform.localPosition;
					localPosition.z = Mathf.Clamp(localPosition.z + Time.deltaTime * num3 * 0.25f, 0f + __instance.camPDAZStart, __instance.camPDAZOffset + __instance.camPDAZStart);
					__instance.cameraOffsetTransform.localPosition = localPosition;
				}
				Vector2 vector = Vector2.zero;
				if (flag5 && FPSInputModule.current.lastGroup == null)
				{
					vector = GameInput.GetLookDelta();
					if (XRSettings.enabled && VROptions.disableInputPitch)
					{
						vector.y = 0f;
					}
					if (inExosuit)
					{
						vector.x = 0f;
					}
					vector *= Player.main.mesmerizedSpeedMultiplier;
				}

				instanceTraverse.Method("UpdateCamShake").GetValue();
				if (__instance.cinematicMode && !__instance.lookAroundMode)
				{
					__instance.camRotationX = Mathf.LerpAngle(__instance.camRotationX, 0f, Time.deltaTime * 2f);
					__instance.camRotationY = Mathf.LerpAngle(__instance.camRotationY, 0f, Time.deltaTime * 2f);
					__instance.transform.localEulerAngles = new Vector3(-__instance.camRotationY + camShake.Value, __instance.camRotationX, 0f);
				}
				else if (flag2)
				{
					if (!XRSettings.enabled)
					{
						bool flag6 = !__instance.lookAroundMode || flag;
						bool flag7 = !__instance.lookAroundMode || flag;
						Vehicle vehicle = Player.main.GetVehicle();
						if (vehicle != null)
						{
							flag6 = (vehicle.controlSheme != Vehicle.ControlSheme.Mech || flag);
						}
						__instance.camRotationX += vector.x;
						__instance.camRotationY += vector.y;
						__instance.camRotationX = Mathf.Clamp(__instance.camRotationX, -60f, 60f);
						__instance.camRotationY = Mathf.Clamp(__instance.camRotationY, -60f, 60f);
						if (flag7)
						{
							__instance.camRotationX = Mathf.LerpAngle(__instance.camRotationX, 0f, Time.deltaTime * 10f);
						}
						if (flag6)
						{
							__instance.camRotationY = Mathf.LerpAngle(__instance.camRotationY, 0f, Time.deltaTime * 10f);
						}
						__instance.cameraOffsetTransform.localEulerAngles = new Vector3(-__instance.camRotationY, __instance.camRotationX + camShake.Value, 0f);
					}
				}
				else
				{
					__instance.rotationX += vector.x;
					__instance.rotationY += vector.y;
					__instance.rotationY = Mathf.Clamp(__instance.rotationY, __instance.minimumY, __instance.maximumY);
					__instance.cameraUPTransform.localEulerAngles = new Vector3(Mathf.Min(0f, -__instance.rotationY + camShake.Value), 0f, 0f);
					transform.localEulerAngles = new Vector3(Mathf.Max(0f, -__instance.rotationY + camShake.Value), __instance.rotationX, 0f);
				}
				instanceTraverse.Method("UpdateStrafeTilt").GetValue();
				Vector3 localEulerAngles = __instance.transform.localEulerAngles + new Vector3(0f, 0f, __instance.cameraAngleMotion.y * __instance.cameraTiltMod + strafeTilt.Value + camShake.Value * 0.5f);
				float num4 = 0f - __instance.skin;
				if (!flag2 && instanceTraverse.Method("GetCameraBob").GetValue<bool>())
				{
					float to = Mathf.Min(1f, velocity.magnitude / 5f);
					smoothedSpeed.Value = UWE.Utils.Slerp(smoothedSpeed.Value, to, Time.deltaTime);
					num4 += (Mathf.Sin(Time.time * 6f) - 1f) * (0.02f + smoothedSpeed.Value * 0.15f) * swimCameraAnimation.Value;
				}
				if (impactForce.Value > 0f)
				{
					impactBob.Value = Mathf.Min(0.9f, impactBob.Value + impactForce.Value * Time.deltaTime);
					impactForce.Value -= Mathf.Max(1f, impactForce.Value) * Time.deltaTime * 5f;
				}
				num4 -= impactBob.Value;
				if (impactBob.Value > 0f)
				{
					impactBob.Value = Mathf.Max(0f, impactBob.Value - Mathf.Pow(impactBob.Value, 0.5f) * Time.deltaTime * 3f);
				}
				__instance.transform.localPosition = new Vector3(0f, num4, 0f);
				__instance.transform.localEulerAngles = localEulerAngles;
				if (Player.main.motorMode == Player.MotorMode.Vehicle)
				{
					__instance.transform.localEulerAngles = Vector3.zero;
				}
				Vector3 localEulerAngles2 = new Vector3(0f, __instance.transform.localEulerAngles.y, 0f);
				Vector3 localPosition2 = __instance.transform.localPosition;
				if (XRSettings.enabled)
				{
					if (flag2 && !flag3)
					{
						localEulerAngles2.y = viewModelLockedYaw.Value;
					}
					else
					{
						localEulerAngles2.y = 0f;
					}
					if (!flag3 && !__instance.cinematicMode)
					{
						if (!flag2)
						{
							Quaternion rotation = playerController.Value.forwardReference.rotation;
							localEulerAngles2.y = (__instance.gameObject.transform.parent.rotation.GetInverse() * rotation).eulerAngles.y;
						}
						localPosition2 = __instance.gameObject.transform.parent.worldToLocalMatrix.MultiplyPoint(playerController.Value.forwardReference.position);
					}
				}
				__instance.viewModel.transform.localEulerAngles = localEulerAngles2;
				__instance.viewModel.transform.localPosition = localPosition2;

				return false;
            }
		}
		/*[HarmonyPatch(typeof(MainCameraControl))]
		[HarmonyPatch("Awake")]
		internal class Awake
		{
			[HarmonyPostfix]
			public static void Postfix(MainCameraControl __instance)
			{
			}
		}*/
	}

}
