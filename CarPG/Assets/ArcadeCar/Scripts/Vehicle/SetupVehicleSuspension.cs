using UnityEngine;

namespace Vehicle
{
    /// <summary>
    /// Simple monobehaviour that initializes the vehicle suspension springs, based on inspector setup.
    /// 
    /// Edited to take the actual transforms so springs can be changed from the editor
    /// </summary>
    public class SetupVehicleSuspension : MonoBehaviour
    {
        [SerializeField]
        private Transform springFrontLeftTransform;

        [SerializeField]
        private Transform springFrontRightTransform;

        [SerializeField]
        private Transform springRearLeftTransform;

        [SerializeField]
        private Transform springRearRightTransform;

        [SerializeField]
        private VehicleSuspension suspension;

        private void Awake()
        {
            Debug.Assert(suspension != null, "Member \"suspension\" is required.", this);
        }

        private void Start()
        {
            suspension.InitializeSprings(springFrontLeftTransform.localPosition, springFrontRightTransform.localPosition, springRearLeftTransform.localPosition, springRearRightTransform.localPosition);
        }
    }
}