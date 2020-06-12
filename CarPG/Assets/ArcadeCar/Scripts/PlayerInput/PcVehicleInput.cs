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

        private float isGasOn;
        private float isBreakOn;

        public float Horizontal { get { return horizontal; } }

        public float IsGasOn { get { return isGasOn; } }
        public float IsBreakOn { get { return isBreakOn; } }
        
        private void Update()
        {
            horizontal = UniInputs.move.x;

            isGasOn = Mathf.Max((float)UniInputs.gas, 0);
            isBreakOn = Mathf.Max(-Input.GetAxis("Vertical"), 0);
        }
    }
}