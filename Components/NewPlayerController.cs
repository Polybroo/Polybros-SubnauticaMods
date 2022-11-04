using UnityEngine;

namespace Polynautica
{
	/// <summary>
	/// Copy-paste of PlayerController with small fixes
	/// </summary>
	public class NewPlayerController : PlayerController
	{
		public new Transform forwardReference
		{
			get
			{
				return MainCamera.camera.transform;
			}
			private set
			{
			}
		}
		private void Awake()
		{
			player = GetComponent<Player>();
			underWaterController = GetComponent<UnderwaterMotor>();
			underWaterController.playerController = this;
			groundController = GetComponent<GroundMotor>();
			groundController.playerController = this;
		}
		// Token: 0x06000055 RID: 85 RVA: 0x00004350 File Offset: 0x00002550
		private void Start()
		{
			this.underWaterController.SetEnabled(true);
			this.underWaterController.SetEnabled(false);
			this.groundController.SetEnabled(true);
			this.groundController.SetEnabled(false);
			this.groundController.SetControllerRadius(this.controllerRadius);
			this.underWaterController.SetControllerRadius(this.controllerRadius);
			this.activeController = this.groundController;
			this.activeController.SetEnabled(true);
			this.currentControllerHeight = this.standheight;
			this.desiredControllerHeight = this.standheight;
		}

		public new void OnProtoSerialize(ProtobufSerializer serializer)
		{
		}

		public new void OnProtoDeserialize(ProtobufSerializer serializer)
		{
			base.Invoke("HandleControllerStateAfterDeserialization", 0.1f);
		}

		public new bool IsSprinting()
		{
			return base.enabled && this.activeController.IsSprinting();
		}

		public new void SetEnabled(bool enabled)
		{
			this.velocity = Vector3.zero;
			if (this.activeController != null)
			{
				this.activeController.SetVelocity(this.velocity);
			}
			if (!enabled)
			{
				this.underWaterController.SetEnabled(false);
				this.groundController.SetEnabled(false);
			}
			else if (this.activeController != null)
			{
				this.activeController.SetEnabled(true);
			}
			base.enabled = enabled;
		}

		public new void ForceControllerSize()
		{
			this.player.UpdateIsUnderwater();
			bool flag = this.player.IsUnderwaterForSwimming();
			bool flag2 = this.player.GetVehicle();
			this.desiredControllerHeight = ((flag || flag2) ? this.swimheight : this.standheight);
			this.currentControllerHeight = this.desiredControllerHeight;
			this.groundController.SetControllerHeight(this.currentControllerHeight);
			this.underWaterController.SetControllerHeight(this.currentControllerHeight);
		}

		private void HandleControllerStateAfterDeserialization()
		{
			this.HandleUnderWaterState();
			this.HandleControllerState();
		}

		private void HandleControllerState()
		{
			this.groundController.SetEnabled(false);
			this.underWaterController.SetEnabled(false);
			if (this.inVehicle)
			{
				return;
			}
			if (this.underWater)
			{
				this.activeController = (this.player.IsInSub() ? this.groundController : this.underWaterController);
				this.desiredControllerHeight = this.swimheight;
				this.activeController.SetControllerHeight(this.currentControllerHeight);
				this.activeController.SetEnabled(true);
				return;
			}
			this.activeController = this.groundController;
			this.desiredControllerHeight = this.standheight;
			this.activeController.SetControllerHeight(this.currentControllerHeight);
			this.activeController.SetEnabled(true);
		}

		private void HandleUnderWaterState()
		{
			bool flag = this.player.IsUnderwaterForSwimming();
			bool flag2 = this.player.GetVehicle();
			if (this.underWater != flag || this.inVehicle != flag2)
			{
				this.underWater = flag;
				this.inVehicle = flag2;
				this.HandleControllerState();
			}
			this.activeController.SetUnderWater(this.underWater);
		}

		public new void SetMotorMode(Player.MotorMode newMotorMode)
		{
			float forwardMaxSpeed = 5f;
			float backwardMaxSpeed = 5f;
			float strafeMaxSpeed = 5f;
			float underWaterGravity = 0f;
			float swimDrag = this.defaultSwimDrag;
			bool canSwim = true;
			if (newMotorMode == Player.MotorMode.Seaglide)
			{
				forwardMaxSpeed = 25f;
				backwardMaxSpeed = 5f;
				strafeMaxSpeed = 5f;
				swimDrag = 2.5f;
			}
			else if (newMotorMode == Player.MotorMode.Mech)
			{
				forwardMaxSpeed = 4.5f;
				backwardMaxSpeed = 4.5f;
				strafeMaxSpeed = 4.5f;
				underWaterGravity = 7.2f;
				canSwim = false;
			}
			else if (newMotorMode == Player.MotorMode.Run || newMotorMode == Player.MotorMode.Walk)
			{
				forwardMaxSpeed = 3.5f;
				backwardMaxSpeed = 5f;
				strafeMaxSpeed = 5f;
			}
			this.underWaterController.forwardMaxSpeed = forwardMaxSpeed;
			this.underWaterController.backwardMaxSpeed = backwardMaxSpeed;
			this.underWaterController.strafeMaxSpeed = strafeMaxSpeed;
			this.underWaterController.underWaterGravity = underWaterGravity;
			this.underWaterController.swimDrag = swimDrag;
			this.underWaterController.canSwim = canSwim;
			this.groundController.forwardMaxSpeed = forwardMaxSpeed;
			this.groundController.backwardMaxSpeed = backwardMaxSpeed;
			this.groundController.strafeMaxSpeed = strafeMaxSpeed;
			this.groundController.underWaterGravity = underWaterGravity;
			this.groundController.canSwim = canSwim;
		}

		public new bool TestHasSpace(Vector3 position)
		{
			RaycastHit raycastHit;
			return !Physics.CapsuleCast(position, position, this.controllerRadius + 0.01f, Vector3.up, out raycastHit, this.currentControllerHeight, -524289, QueryTriggerInteraction.Ignore);
		}

		public new bool WayToPositionClear(Vector3 position, GameObject ignoreObj = null, bool ignoreLiving = false)
		{
			Vector3 point = base.transform.position - Vector3.up * 0.5f * this.currentControllerHeight;
			Vector3 point2 = base.transform.position + Vector3.up * 0.5f * this.currentControllerHeight;
			Vector3 value = position - base.transform.position;
			int num = UWE.Utils.CapsuleCastIntoSharedBuffer(point, point2, this.controllerRadius + 0.01f, Vector3.Normalize(value), value.magnitude, -524289, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = UWE.Utils.sharedHitBuffer[i];
				GameObject gameObject = raycastHit.collider.gameObject;
				if ((!(ignoreObj != null) || !UWE.Utils.IsAncestorOf(ignoreObj, gameObject)) && (!ignoreLiving || !(gameObject.GetComponentInParent<Living>() != null)))
				{
					return false;
				}
			}
			return true;
		}

		public new void UpdateController()
		{
			this.HandleUnderWaterState();
			float num = UWE.Utils.Slerp(this.currentControllerHeight, this.desiredControllerHeight, Time.deltaTime * 2f);
			float num2 = num - this.currentControllerHeight;
			bool flag = true;
			if (num2 > 0f)
			{
				Vector3 vector = base.transform.position + new Vector3(0f, this.currentControllerHeight * 0.5f, 0f);
				RaycastHit raycastHit;
				flag = !Physics.CapsuleCast(vector, vector, this.controllerRadius + 0.01f, Vector3.up, out raycastHit, num2, -524289);
			}
			if (flag)
			{
				this.currentControllerHeight = num;
			}
			this.underWaterController.SetControllerHeight(this.currentControllerHeight);
			this.groundController.SetControllerHeight(this.currentControllerHeight);
			this.velocity = this.activeController.UpdateMove();
		}

		private void FixedUpdate()
		{
			if (activeController is UnderwaterMotor)
				this.UpdateController();
		}
		private void Update()
        {
			if (activeController is UnderwaterMotor)
				return;
			this.UpdateController();
        }

		[AssertNotNull]
		public new Player player;

		public new float standheight = 1.5f;

		public new float swimheight = 0.5f;

		public new Vector3 velocity = Vector3.zero;

		public new PlayerMotor underWaterController;

		public new PlayerMotor groundController;

		public new PlayerMotor activeController;

		public new float controllerRadius = 0.3f;

		public new float defaultSwimDrag = 2.5f;

		public new bool inputEnabled = true;

		private float currentControllerHeight = 1.5f;

		private float desiredControllerHeight = 1.5f;

		private bool underWater;

		private bool inVehicle;
	}
}
