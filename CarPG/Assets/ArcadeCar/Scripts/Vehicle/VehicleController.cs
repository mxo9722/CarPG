using UnityEngine;
using Cinemachine;
using System;

namespace Vehicle
{
    /// <summary>
    /// Logic for controlling the arcade vehicle, that includes:
    /// steering, gas, brake, traction, air drag, antirolling using center of mass.
    /// Receives input data throught public methods, which can be called by the player
    /// or by AI, for example.
    /// </summary>
    public class VehicleController : MonoBehaviour
    {
        [SerializeField]
        public Rigidbody myRigidbody;

        [SerializeField]
        private BoxCollider boxCollider;

        [SerializeField]
        private VehicleSuspension suspension;

        /// <summary>
        /// Center of mass applied to the rigidbody on awake.
        /// </summary>
        [SerializeField]
        private Transform centerOfMassTransform;

        [SerializeField]
        private Transform wallCorrectionTransform;
        /// <summary>
        /// Transform that holds relative position where gas and brake forces are applied.
        /// Note: Can be shifted for alternative styles of vehicles to give different weight transfer feeling.
        /// </summary>
        [SerializeField]
        private Transform enginePowerTransform;

        /// <summary>
        /// Transform that holds relative position where steering left/right forces are applied.
        /// Note: Can be shifted for alternative styles of vehicles to give different weight transfer feeling.
        /// </summary>
        [SerializeField]
        private Transform steeringTransform;

        /// <summary>
        /// The maximum velocity of the vehicle in metters/seconds.
        /// </summary>
        [SerializeField]
        public float maximumVelocity;

        [SerializeField]
        public float accelerationFactor;

        [SerializeField]
        private float brakeFactor;

        [SerializeField]
        public float steerFactor;

        //private int stayGroundedFrames = 5;

        //[SerializeField]
        //private CinemachineFreeLook cameraLogic;
        /// <summary>
        /// Sideways traction based on the movement of the vehicle.
        /// Note: can be tweaked for a more drifting feeling, for example.
        /// </summary>
        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float sidewaysTraction;

        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float angularTraction;

        /// <summary>
        /// Range between 0.0f and 1.0f
        /// </summary>
        [SerializeField]
        private float gasInput;

        /// <summary>
        /// Range between 0.0f and 1.0f
        /// </summary>
        [SerializeField]
        private float brakeInput;

        /// <summary>
        /// Range between -1.0f and 1.0f
        /// </summary>
        public float steerInput;

        [SerializeField]
        private bool isMovingForward;
        private Vector3 straightVelocity;
        public float straightVelocityMagnitude;

        public float SteerInput { get { return steerInput; } }
        public float BrakeInput { get { return brakeInput; } }
        public bool IsMovingForward { get { return isMovingForward; } }
        public float StraightVelocityMagnitude { get { return straightVelocityMagnitude; } }

        private void Awake()
        {
            Debug.Assert(myRigidbody != null, "Member \"myRigidbody\" is required.", this);
            Debug.Assert(boxCollider != null, "Member \"boxCollider\" is required.", this);
            Debug.Assert(suspension != null, "Member \"suspension\" is required.", this);
            Debug.Assert(centerOfMassTransform != null, "Member \"centerOfMassTransform\" is required.", this);
            Debug.Assert(enginePowerTransform != null, "Member \"enginePowerTransform\" is required.", this);
            Debug.Assert(steeringTransform != null, "Member \"steeringTransform\" is required.", this);

            myRigidbody.centerOfMass = centerOfMassTransform.localPosition;
        }

        private void FixedUpdate()
        {
            if (myRigidbody.velocity.magnitude <= 1 || suspension.AreAllSpringsGrounded())
            {
                myRigidbody.drag = 0;
                myRigidbody.angularDrag = .05f;
            }
            //myRigidbody.AddForceAtPosition(-transform.up * 10000, transform.position); //lol disable gravity and uncomment this for a wild ride
            if (suspension.AreAllSpringsGrounded())
            {
                
                //if (cameraLogic.m_BindingMode != Cinemachine.CinemachineTransposer.BindingMode.LockToTargetNoRoll)
                //    cameraLogic.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.LockToTargetNoRoll;


                Vector3 projectedForward = suspension.GetSuspensionProjectedForwardDirection();
                Vector3 projectedRight = Vector3.Cross(Vector3.up, projectedForward);
                straightVelocity = projectedForward * Vector3.Dot(myRigidbody.velocity, projectedForward);
                straightVelocityMagnitude = straightVelocity.magnitude;
                isMovingForward = Vector3.Dot(projectedForward, straightVelocity.normalized) > 0.0f;

                if (gasInput > 0.0f)
                {
                    // Positive Gas Input
                    // Top forward speed if
                    if (!isMovingForward || straightVelocityMagnitude < maximumVelocity)
                    {
                        myRigidbody.AddForceAtPosition(
                            projectedForward * accelerationFactor * gasInput,
                            enginePowerTransform.position,
                            ForceMode.Acceleration);
                        

                        //TODO: Make some bool that determines if we need to clamp this
                        //we clamp if we're accelerating from under max speed so we don't overshoot
                        //probably in lateupdate?

                        //myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, maximumVelocity); //lets you have high acceleration but low max speed
                    }
                }
                else if (gasInput < 0.0f)
                {
                    // Negative Gas Input
                    // Top backward speed if
                    if (isMovingForward || straightVelocityMagnitude < maximumVelocity * 0.4f)
                    {
                        myRigidbody.AddForceAtPosition(
                            projectedForward * accelerationFactor * 0.7f * gasInput,
                            enginePowerTransform.position,
                            ForceMode.Acceleration);
                    }
                }

                // Steering
                if (!Mathf.Approximately(0.0f, steerInput) && straightVelocityMagnitude > 0.0f)
                {
                    if (isMovingForward)
                    {
                        myRigidbody.AddForceAtPosition(steerInput * projectedRight * steerFactor * Mathf.Clamp(StraightVelocityMagnitude, 0, 25), steeringTransform.position, ForceMode.Acceleration);
                        //clamping the velocity magnitude so speed boosts don't affect steering too much
                        //a better solution would be to have a variable that is added to max velocity instead of changing it itself but stephen code bad


                        if (gasInput == 1 && straightVelocityMagnitude < 1.0f) //we're probably stuck on a wall, magnitude won't help us anymore and there is no god
                        {
                            myRigidbody.AddForceAtPosition(steerInput * projectedRight * steerFactor * 100, wallCorrectionTransform.position, ForceMode.Acceleration);
                        }
                    }
                    else
                    {
                        // Moving backwards (reverse the steer) To provide a better feeling, we do an
                        // extra test for steering when the Vehicle is moving back, so that if the
                        // vehicle is still moving backwards because of physics, but the player is
                        // already doing input for moving forward(gas positive), then we don't turn
                        // at all.
                        if (gasInput < 0.0f)
                        {
                            // Case is moving backwards and player still giving input for keep moving backwards, that is when we rear (reverse) steer
                            myRigidbody.AddForceAtPosition(steerInput * -projectedRight * steerFactor * StraightVelocityMagnitude, steeringTransform.position, ForceMode.Acceleration);
                        }
                    }
                }

                if (brakeInput > 0.0f)
                {
                    if (straightVelocityMagnitude > 0.0f && isMovingForward)
                    {
                        myRigidbody.AddForceAtPosition(
                            -projectedForward * brakeInput * brakeFactor,
                            enginePowerTransform.position,
                            ForceMode.Acceleration);
                    }
                }

                // Angular traction
                myRigidbody.AddTorque(-new Vector3(0.0f, myRigidbody.angularVelocity.y, 0.0f) * angularTraction, ForceMode.Acceleration);

                // Sideways traction
                Vector3 contrarySidewaysVelocity = -Vector3.Project(myRigidbody.velocity, transform.right);
                if (contrarySidewaysVelocity.sqrMagnitude > 0.0f)
                {
                    myRigidbody.AddForce(contrarySidewaysVelocity * sidewaysTraction, ForceMode.Acceleration);
                }

                // Wheels Rolling Resistance, turn front and back traction (so vehicle doesnt tend to move front/back forever)
                const float WHEELS_ROLLING_RESISTANCE_COEFFICIENT = 2.0f;
                Vector3 wheelsRollingResistanceForce = -(straightVelocity * WHEELS_ROLLING_RESISTANCE_COEFFICIENT);
                myRigidbody.AddForce(wheelsRollingResistanceForce, ForceMode.Acceleration);
            }
            else
            {
                //if (cameraLogic.m_BindingMode != Cinemachine.CinemachineTransposer.BindingMode.WorldSpace)
                //    cameraLogic.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.WorldSpace;

                //if (stayGroundedFrames < 100)
                //{
                //    myRigidbody.AddForceAtPosition(-transform.up * 10000, steeringTransform.position);
                //    stayGroundedFrames++;
                //}
                isMovingForward = false;

                /**if (gasInput != 0.0f || brakeInput > 0.0f) //W and S inputs used for pitching in the air
                {
                    myRigidbody.AddRelativeTorque(-new Vector3((gasInput * -.1f) + (brakeInput * .1f), 0.0f, 0.0f), ForceMode.VelocityChange);
                }**/

                if (!Mathf.Approximately(0.0f, steerInput) && straightVelocityMagnitude > 0.0f) //turning inputs used for rolling in the air TODO: Change to Q and E
                {
                    myRigidbody.AddRelativeTorque(-new Vector3(0.0f, 0.0f, steerInput * .1f), ForceMode.VelocityChange);
                }
            }

            // Air Resistance / Aerodynamic drag
            const float AERODYNAMIC_DRAG_MODIFIER = 0.003f;
            //myRigidbody.AddForce(-myRigidbody.velocity.normalized * boxCollider.size.magnitude * AERODYNAMIC_DRAG_MODIFIER * myRigidbody.velocity.sqrMagnitude,ForceMode.Acceleration);
        }

        public void Gas(float gasInput)
        {
            this.gasInput = Mathf.Clamp(gasInput, -1.0f, 1.0f);
        }

        public void Brake(float brakeInput)
        {
            this.brakeInput = Mathf.Clamp01(brakeInput);
        }

        public void Steer(float steerInput)
        {
            this.steerInput = Mathf.Clamp(steerInput, -1.0f, 1.0f);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Wall")
            {
                //myRigidbody.AddForceAtPosition(transform.right * steerInput * 200, steeringTransform.position, ForceMode.Acceleration);
                //GameObject go = new GameObject();
                //Instantiate(go, collision.contacts[0].point, new Quaternion());
                
                Vector3 bounceForce = Vector3.Normalize(wallCorrectionTransform.position - collision.contacts[0].point) * collision.relativeVelocity.magnitude;
                bounceForce.y = 0;
                myRigidbody.AddForceAtPosition(bounceForce * 5, wallCorrectionTransform.position, ForceMode.Acceleration);
            }

            if (!suspension.AreAllSpringsGrounded() && collision.gameObject.tag != "Enemy")
            {
                myRigidbody.drag = 1;
                myRigidbody.angularDrag = myRigidbody.angularVelocity.magnitude;
            }
        }
    }
}