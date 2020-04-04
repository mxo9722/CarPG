using UnityEngine;

namespace PlayerInput
{
    /// <summary>
    /// Encapsulates player input logic to control the vehicles,
    /// exposes readonly flags informing the current input state.
    /// Note: Right and Left are never true at the same time,
    /// same for Break and Gas.
    /// </summary>
    public class PcVehicleInput : MonoBehaviour
    {
        [SerializeField]
        private KeyCode leftKeyCode;

        [SerializeField]
        private KeyCode rightKeyCode;

        [SerializeField]
        private KeyCode gasKeyCode;

        [SerializeField]
        private KeyCode breakKeyCode;

        private float horizontal;

        private bool isGasOn;
        private bool isBreakOn;

        public float Horizontal { get { return horizontal; } }

        public bool IsGasOn { get { return isGasOn; } }
        public bool IsBreakOn { get { return isBreakOn; } }
        
        private void Update()
        {
            horizontal = Input.GetAxis("Horizontal");

            if (Input.GetKey(gasKeyCode))
            {
                isGasOn = true;
                isBreakOn = false;
            }
            else if (Input.GetKey(breakKeyCode))
            {
                isGasOn = false;
                isBreakOn = true;
            }
            else
            {
                isGasOn = false;
                isBreakOn = false;
            }
        }
    }
}