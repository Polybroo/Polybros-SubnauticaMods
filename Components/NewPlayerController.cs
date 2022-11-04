using UnityEngine;

namespace Polynautica
{
	/// <summary>
	/// Copy-paste of PlayerController with small fixes
	/// </summary>
	public class NewPlayerController : PlayerController
    {
        private void Awake()
        {
            base.player = GetComponent<Player>();
            base.underWaterController = GetComponent<UnderwaterMotor>();
            base.underWaterController.playerController = this;
            base.groundController = GetComponent<GroundMotor>();
            base.groundController.playerController = this;
        }
        private void Start()
        {
            base.underWaterController.SetEnabled(enabled: true);
            base.underWaterController.SetEnabled(enabled: false);
            base.groundController.SetEnabled(enabled: true);
            base.groundController.SetEnabled(enabled: false);
            base.groundController.SetControllerRadius(controllerRadius);
            base.underWaterController.SetControllerRadius(controllerRadius);
            base.activeController = base.groundController;
            base.activeController.SetEnabled(enabled: true);
        }

        private void Update()
        {
            if (base.activeController is UnderwaterMotor)
                return;
            base.UpdateController();
        }
        private void FixedUpdate()
        {
            if (base.activeController is UnderwaterMotor)
                base.UpdateController();
        }
    }
}
