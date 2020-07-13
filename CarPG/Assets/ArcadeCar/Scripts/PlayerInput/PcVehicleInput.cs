using UnityEngine;

    /// <summary>
    /// Encapsulates player input logic to control the vehicles,
    /// exposes readonly flags informing the current input state.
    /// Note: Right and Left are never true at the same time,
    /// same for Break and Gas.
    /// </summary>
    public class PcVehicleInput : MonoBehaviour
    {
        private float horizontal;

        private float gas;
        private float breaks;

        public float Horizontal { get { return horizontal; } }

        public float Gas { get { return gas; } }
        public float Breaks { get { return breaks; } }
        
        private void Update()
        {
            horizontal = UniInputs.move.x;

            gas = (float)UniInputs.gas;
            breaks = (float)UniInputs.breaks;
        }
    }