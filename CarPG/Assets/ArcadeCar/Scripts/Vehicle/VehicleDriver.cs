using PlayerInput;
using UnityEngine;

namespace Vehicle
{
    /// <summary>
    /// The vehicle driver represents an entity which reads player input,
    /// and drives the selected vehicle controller.
    /// </summary>
    public class VehicleDriver : MonoBehaviour
    {
        [SerializeField]
        private PcVehicleInput input;

        [SerializeField]
        private VehicleController controller;

        [SerializeField]
        private float steeringInputRate;

        private float steering;

        private void Update()
        {
            if (input != null && controller != null)
            {
                if (input.Breaks>input.Gas)
                {
                    if (!Mathf.Approximately(controller.StraightVelocityMagnitude, 0.0f))
                    {
                        if (controller.IsMovingForward)
                        {
                            controller.Gas(0.0f);
                            controller.Brake(input.Breaks);
                        }
                        else
                        {
                            // Gas backwards
                            controller.Gas(-input.Breaks);
                            controller.Brake(0.0f);
                        }
                    }
                    else // carEngine.IsStationary, 0 straight(front back) velocity
                    {
                        // Gas backwards
                        controller.Gas(-input.Breaks);
                        controller.Brake(0.0f);
                    }
                }
                else if (input.Gas>0)
                {
                    controller.Gas(input.Gas);
                    controller.Brake(0.0f);
                }
                else
                {
                    controller.Gas(0.0f);
                    controller.Brake(0.0f);
                }

                steering = input.Horizontal;

                controller.Steer(steering);
            }
        }

        public void SetVehicleController(VehicleController controller)
        {
            this.controller = controller;
        }
    }
}