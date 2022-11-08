using UnityEngine;

namespace NoMoreJittery
{
	public class FixedPlayerController : PlayerController
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
            if (this.player.rigidBody.isKinematic) base.UpdateController();
            if (base.activeController != base.groundController) base.velocity = base.player.rigidBody.velocity;
            
        }
        private void FixedUpdate()
        {
            base.HandleUnderWaterState();
            if (!base.player.rigidBody.isKinematic) base.UpdateController();
        }
    }
}
