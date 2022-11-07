using HarmonyLib;
using UnityEngine;

namespace Polynautica
{
	class MainCameraV2Patch
	{
		[HarmonyPatch(typeof(MainCameraV2))]
		[HarmonyPatch("Start")]
		internal class Start
		{
			[HarmonyPostfix]
			public static void Postfix(MainCameraV2 __instance)
			{
				var traverse = Traverse.Create(__instance);
				Traverse<int> dcm = traverse.Field<int>("defaultCullingMask");
				//dcm.Value &= ~(1 << 8);
				//__instance.RestoreCullingMask();

				GameObject copy = GameObject.Instantiate(SNCameraRoot.main.guiCam.gameObject);

				Transform parent = SNCameraRoot.main.mainCam.transform.parent;
				copy.transform.parent = parent;
				copy.transform.localEulerAngles = Vector3.zero;
				copy.transform.localPosition = Vector3.zero;
				Camera cam = copy.GetComponent<Camera>();
				cam.cullingMask = LayerMask.GetMask("Viewmodel");
				cam.clearFlags = CameraClearFlags.Depth;
				cam.depth = 2;
				cam.depthTextureMode = DepthTextureMode.None & DepthTextureMode.Depth & DepthTextureMode.DepthNormals & DepthTextureMode.MotionVectors;
				cam.renderingPath = RenderingPath.DeferredShading;
				cam.enabled = false;
			}
		}
	}
}
