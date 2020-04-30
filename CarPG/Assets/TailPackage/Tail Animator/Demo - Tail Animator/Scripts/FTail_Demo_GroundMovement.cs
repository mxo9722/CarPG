using System.Collections.Generic;
using UnityEngine;


namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Check my package 'GroundFitter' which providing much more customizable movement than this
    /// </summary>
    public class FTail_Demo_GroundMovement : MonoBehaviour
    {
        #region  Fitting

        [Header("Check my other Package 'Ground Fitter' for", order = 0)]
        [Space(-7f, order = 1)]
        [Header("more customizable ground fit movement", order = 2)]
        public float RotationYAxis = 0f;

        [Range(1f, 30f)]
        public float FittingSpeed = 6f;

        public float RaycastHeightOffset = 0.5f;
        public float RaycastCheckRange = 5f;

        public float LookAheadRaycast = 0f;
        public float AheadBlend = 0.5f;

        public float YOffset = 0f;

        [Space(8f)]
        public LayerMask GroundLayerMask = 1 << 0;
        public bool RelativeLookUp = true;
        [Range(0f, 1f)]
        public float RelativeLookUpBias = 0.25f;

        public RaycastHit LastRaycast { get; protected set; }
        protected Quaternion helperRotation = Quaternion.identity;

        protected float delta;


        #region Methods

        private Vector3 GetUpVector()
        {
            if (RelativeLookUp)
            {
                return Vector3.Lerp(transform.up, Vector3.up, RelativeLookUpBias);
            }
            else return Vector3.up;
        }

        protected virtual void FitToGround()
        {
            RaycastHit aheadHit = new RaycastHit();

            if (LookAheadRaycast != 0f)
                Physics.Raycast(transform.position + GetUpVector() * RaycastHeightOffset + transform.forward * LookAheadRaycast, -GetUpVector(), out aheadHit, RaycastCheckRange + YOffset, GroundLayerMask, QueryTriggerInteraction.Ignore);

            RefreshLastRaycast();

            if (LastRaycast.transform)
            {
                Quaternion fromTo = Quaternion.FromToRotation(Vector3.up, LastRaycast.normal);

                if (aheadHit.transform)
                {
                    Quaternion aheadFromTo = Quaternion.FromToRotation(Vector3.up, aheadHit.normal);
                    fromTo = Quaternion.Lerp(fromTo, aheadFromTo, AheadBlend);
                }

                helperRotation = Quaternion.Slerp(helperRotation, fromTo, delta * FittingSpeed);
            }
            else // If nothing is under our legs we rotate object smoothly to zero rotations
                helperRotation = Quaternion.Slerp(helperRotation, Quaternion.identity, delta * FittingSpeed);

            RotationCalculations();

            if (LastRaycast.transform)
                transform.position = LastRaycast.point + Vector3.up * YOffset;
        }

        internal void RotationCalculations()
        {
            Quaternion targetRotation = helperRotation * Quaternion.AngleAxis(RotationYAxis, Vector3.up);
            transform.rotation = targetRotation;
        }

        internal RaycastHit CastRay()
        {
            RaycastHit outHit;
            Physics.Raycast(transform.position + GetUpVector() * RaycastHeightOffset, -GetUpVector(), out outHit, RaycastCheckRange + Mathf.Abs(YOffset), GroundLayerMask, QueryTriggerInteraction.Ignore);
            return outHit;
        }

        internal void RefreshLastRaycast()
        {
            LastRaycast = CastRay();
        }

        #endregion

        #endregion

        #region Movement

        protected bool fittingEnabled = true;

        [Header("> Movement <")]
        public float BaseSpeed = 3f;
        public float RotateToTargetSpeed = 6f;
        public float SprintingSpeed = 10f;

        protected float ActiveSpeed = 0f;
        public float AccelerationSpeed = 10f;
        public float DecelerationSpeed = 10f;

        public float JumpPower = 7f;
        public float gravity = 15f;
        public bool MultiplySprintAnimation = false;

        internal float YVelocity;
        protected bool inAir = false;
        protected float gravityOffset = 0f;

        internal bool MoveForward = false;
        internal bool Sprint = false;
        internal float RotationOffset = 0f;

        protected string lastAnim = "";

        protected Animator animator;

        protected bool animatorHaveAnimationSpeedProp = false;
        protected float initialYOffset;

        protected Vector3 holdJumpPosition;
        protected float freezeJumpYPosition;

        private bool oneAnimation = false;

        #region Methods


        /// <summary>
        /// Preparin initial stuff
        /// </summary>
        protected virtual void InitMovement()
        {
            animator = GetComponentInChildren<Animator>();

            if (animator)
            {
                if (HasParameter(animator, "AnimationSpeed")) animatorHaveAnimationSpeedProp = true;
                if (!animator.HasState(0, Animator.StringToHash("Idle"))) oneAnimation = true;
            }

            RotationYAxis = transform.rotation.eulerAngles.y;
            initialYOffset = YOffset;
            RefreshLastRaycast();

            RotationOffset = 0f;
            Sprint = false;
            MoveForward = false;
        }


        protected virtual void UpdateMovement()
        {
            delta = Time.deltaTime;
            HandleInput();
            HandleGravity();
            HandleAnimations();
            HandleTransforming();
        }

        protected virtual void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Jump();

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.LeftShift)) Sprint = true; else Sprint = false;

                RotationOffset = 0f;
                if (Input.GetKey(KeyCode.A)) RotationOffset = -90;
                if (Input.GetKey(KeyCode.D)) RotationOffset = 90;
                if (Input.GetKey(KeyCode.S)) RotationOffset = 180;

                MoveForward = true;
            }
            else
            {
                Sprint = false;
                MoveForward = false;
            }
        }

        protected virtual void HandleGravity()
        {
            if (fittingEnabled)
            {
                if (YOffset > initialYOffset)
                    YOffset += YVelocity * delta;
                else
                    YOffset = initialYOffset;
            }
            else
                YOffset += YVelocity * delta;

            if (inAir) YVelocity -= gravity * delta;

            if (fittingEnabled)
            {
                if (!LastRaycast.transform)
                {
                    if (!inAir)
                    {
                        inAir = true;
                        holdJumpPosition = transform.position;
                        freezeJumpYPosition = holdJumpPosition.y;
                        YVelocity = -1f;
                        fittingEnabled = false;
                    }
                }
                else
                    if (YVelocity > 0f)
                {
                    inAir = true;
                }
            }

            if (inAir)
            {
                if (fittingEnabled) fittingEnabled = false;

                if (YVelocity < 0f)
                {
                    RaycastHit hit = CastRay();

                    if (hit.transform)
                    {
                        if (transform.position.y + (YVelocity * delta) <= hit.point.y + initialYOffset + 0.05f)
                        {
                            YOffset -= (hit.point.y - freezeJumpYPosition);
                            HitGround();
                        }
                    }
                }
                else
                {
                    RaycastHit hit = CastRay();

                    if (hit.transform)
                    {
                        if (hit.point.y - 0.1f > transform.position.y)
                        {
                            YOffset = initialYOffset;
                            YVelocity = -1f;
                            HitGround();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handling switching animation clips of Animator
        /// </summary>
        protected virtual void HandleAnimations()
        {
            if (ActiveSpeed > 0.15f)
            {
                if (Sprint)
                    CrossfadeTo("Run", 0.25f);
                else
                    CrossfadeTo("Walk", 0.25f);
            }
            else
            {
                CrossfadeTo("Idle", 0.25f);
            }

            if (oneAnimation)
            {
                // If object is in air we just slowing animation speed to zero
                if (animatorHaveAnimationSpeedProp)
                    if (inAir) FAnimatorMethods.LerpFloatValue(animator, "AnimationSpeed", 0f, 30f);
                    else
                        FAnimatorMethods.LerpFloatValue(animator, "AnimationSpeed", MultiplySprintAnimation ? (ActiveSpeed / BaseSpeed) : Mathf.Min(1f, (ActiveSpeed / BaseSpeed)), 30f);
            }
            else
            {
                // If object is in air we just slowing animation speed to zero
                if (animatorHaveAnimationSpeedProp)
                    if (inAir) FAnimatorMethods.LerpFloatValue(animator, "AnimationSpeed", 0f);
                    else
                        FAnimatorMethods.LerpFloatValue(animator, "AnimationSpeed", MultiplySprintAnimation ? (ActiveSpeed / BaseSpeed) : Mathf.Min(1f, (ActiveSpeed / BaseSpeed)));
            }
        }


        /// <summary>
        /// Refreshing some switching to new landing position varibles, useful in custom coding
        /// </summary>
        protected void RefreshHitGroundVars(RaycastHit hit)
        {
            holdJumpPosition = hit.point;
            freezeJumpYPosition = hit.point.y;
            YOffset = Mathf.Abs(LastRaycast.point.y - transform.position.y);
        }


        /// <summary>
        /// Calculating changes for transform
        /// </summary>
        protected virtual void HandleTransforming()
        {
            if (fittingEnabled)
            {
                if (LastRaycast.transform)
                {
                    transform.position = LastRaycast.point + YOffset * Vector3.up;
                    holdJumpPosition = transform.position;
                    freezeJumpYPosition = holdJumpPosition.y;
                }
                else
                    inAir = true;
            }
            else
            {
                holdJumpPosition.y = freezeJumpYPosition + YOffset;
            }

            if (MoveForward)
            {
                if (!fittingEnabled)
                {
                    RotationYAxis = Mathf.LerpAngle(RotationYAxis, Camera.main.transform.eulerAngles.y + RotationOffset, delta * RotateToTargetSpeed * 0.15f);
                    RotationCalculations();
                }
                else
                    RotationYAxis = Mathf.LerpAngle(RotationYAxis, Camera.main.transform.eulerAngles.y + RotationOffset, delta * RotateToTargetSpeed);


                if (!Sprint)
                    ActiveSpeed = Mathf.Lerp(ActiveSpeed, BaseSpeed, delta * AccelerationSpeed);
                else
                    ActiveSpeed = Mathf.Lerp(ActiveSpeed, SprintingSpeed, delta * AccelerationSpeed);
            }
            else
            {
                if (ActiveSpeed > 0f)
                    ActiveSpeed = Mathf.Lerp(ActiveSpeed, -0.01f, delta * DecelerationSpeed);
                else ActiveSpeed = 0f;
            }

            holdJumpPosition += ((transform.forward * ActiveSpeed)) * delta;
            transform.position = holdJumpPosition;
        }

        /// <summary>
        /// Method executed when object is landing on ground from beeing in air lately
        /// </summary>
        protected virtual void HitGround()
        {
            RefreshLastRaycast();
            fittingEnabled = true;
            inAir = false;
            freezeJumpYPosition = 0f;
        }

        /// <summary>
        /// Trigger this method so object will jump
        /// </summary>
        public virtual void Jump()
        {
            YVelocity = JumpPower;
            YOffset += JumpPower * Time.deltaTime / 2f;
        }

        /// <summary>
        /// Crossfading to target animation with protection of playing same animation over again
        /// </summary>
        protected virtual void CrossfadeTo(string animation, float transitionTime = 0.25f)
        {
            if (!animator) return;

            if (oneAnimation) return;

            if (!animator.HasState(0, Animator.StringToHash(animation)))
            {
                // Preventing holding shift for sprint and starting walking freeze on idle  
                if (animation == "Run") animation = "Walk"; else return;
            }

            if (lastAnim != animation)
            {
                animator.CrossFadeInFixedTime(animation, transitionTime);
                lastAnim = animation;
            }
        }

        /// <summary>
        /// Checking if animator have parameter with choosed name
        /// </summary>
        public static bool HasParameter(Animator animator, string paramName)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName)
                    return true;
            }
            return false;
        }

        #endregion



        #endregion

        protected virtual void Start()
        {
            InitMovement();
        }

        protected virtual void Update()
        {
            delta = Time.deltaTime;

            if ( fittingEnabled ) FitToGround();

            UpdateMovement();
        }

    }
}
