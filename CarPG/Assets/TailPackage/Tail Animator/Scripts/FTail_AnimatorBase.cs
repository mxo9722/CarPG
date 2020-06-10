using FIMSpace.Basics;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Base script for tail-like procedural animation
    /// </summary>
    public abstract class FTail_AnimatorBase : MonoBehaviour
    {
        [Header("[ Auto detection if left empty ]", order = 0)]
        [Space(-8f)]
        [Header("[ or put here first bone ]", order = 2)]
        /// <summary> List of tail bones </summary>
        public List<Transform> TailTransforms;

        [Tooltip("When you want pin controll rotation motion to parent instead of first bone in chain")]
        public bool RootToParent = false;

        [Tooltip("When you want to use auto get when you assigning one bone inside inspector window")]
        public bool AutoGetWithOne = true;

        [Tooltip("Safe variable to correctly support dynamic scalling. Not changing much visually but preventing from incorrect scalling when bones inside your model have animated scale.")]
        public bool InitBeforeAnimator = false;

        /// <summary> List of invisible for editor points which represents ghost animation for tail </summary>
        public List<FTail_Point> proceduralPoints;
        public List<FTail_Point> localProceduralPoints;

        [Tooltip("Position speed is defining how fast tail segments will return to target position, it gives animation more underwater/floaty feeling if it's lower")]
        [Range(0f, 60f)]
        public float PositionSpeed = 35f;
        [Tooltip("Rotation speed is defining how fast tail segments will return to target rotation, it gives animation more lazy feeling if it's lower")]
        [Range(0f, 60f)]
        public float RotationSpeed = 20f;
        [Tooltip("If you want to limit stretching/gumminess of position motion when object moves fast. Recommended adjust to go with it under 0.3 value")]
        [Range(0f, 1f)]
        public float MaxStretching = .375f;
        [Tooltip("How Sensitive should be position / rotation change in model to affect tail motion, when cranked up tail acts more like entangling line when down it's more stiff and less rapid, !in most cases 0.5 value is just right!")]
        [Range(0f, 1f)]
        public float Sensitivity = 0.5f;
        [Tooltip("Elastic spring effect good for tails to make them more 'meaty'")]
        [Range(0f, 1f)]
        public float Springiness = 0.0f;

        [Tooltip("Limiting max rotation angle for each tail segment")]
        [Range(1f, 90f)]
        public float AngleLimit = 90.0f;
        [Tooltip("If you need specific axis to be limited, leave unchanged to limit all axes (check which axes affect 'Waving Axis' to know which axis you want limit)")]
        public Vector3 AngleLimitAxis = Vector3.zero;
        [Tooltip("If you want limit axes symmetrically leave this parameter unchanged, if you want limit one direction of axis more than reversed, tweak this parameter")]
        public Vector2 LimitAxisRange = Vector2.zero;
        [Tooltip("If limiting shouldn't be too rapidly performed")]
        [Range(0f, 1f)]
        public float LimitSmoothing = 0.15f;

        [Tooltip("If your object moves too fast you can controll influence of position changes for tail motion with this parameter")]
        [Range(0f, 1f)]
        public float MotionInfluence = 1f;

        [Tooltip("If end of tail is returning to initial position too slow, increate this parameter")]
        [Range(0f, 1f)]
        public float StiffTailEnd = 0.0f;

        //[Range(0f, 1f)]
        //public float DirectionStraighter = 0.0f;

        protected List<Transform> editorGizmoTailList;

        [Tooltip("Automatically changing some tweaking settings to make tail animation look correctly")]
        public bool UseAutoCorrectLookAxis = true;

        [Tooltip("Tail will try to align to shape of bone chain instead of force - straightening it")]
        public bool FullCorrection = true;

        public enum FELookUpMethod { Default, RolledBones, CrossUp, Parental }

        [Tooltip("Use this option when your model is rolling strangely when waving or other stuff (Parental seems to be the most universal but needs some more testing)")]
        public FELookUpMethod LookUpMethod = FELookUpMethod.Default;
        [Tooltip("If using cross up, you need reference for base orientation of character, this should be character's root game object which is facing Z axis")]
        public Transform OrientationReference;
        public bool AnimateCorrections = false;
        [Tooltip("If first bone in chain should be animated")]
        public bool AnimateRoot = true;

        public float LengthMultiplier = 1f;

        [Tooltip("Bones wrong rotations axis corrector")]
        [Space(8f)]
        public Vector3 AxisCorrection = new Vector3(0f, 0f, 1f);
        public Vector3 AxisLookBack = new Vector3(0f, 1f, 0f);

        [HideInInspector]
        public bool ExtraCorrectionOptions = false;
        public Vector3 ExtraFromDirection = new Vector3(0f, 0f, 1f);
        public Vector3 ExtraToDirection = new Vector3(0f, 0f, 1f);

        [Tooltip("This option adding TailReference component to all tail segments, so you can access this component from tail's segment transform")]
        public bool AddTailReferences = false;

        // V1.2
        [Tooltip("Set update clock to LateUpdate if you want to use component over object with own animation")]
        public EFUpdateClock UpdateClock = EFUpdateClock.LateUpdate;

        [Tooltip("SafeDeltaTime can eliminate some chopping when framerate isn't stable")]
        public bool SafeDeltaTime = true;
        protected float deltaTime = 0.016f;

        // V1.2.2
        [Tooltip("To use for example when your model is posed during animations much different than it's initial T-Pose (when you use 'Animate Corrections')")]
        public bool RefreshHelpers = false;

        // V1.2.3
        [Tooltip("Useful when you use other components to affect bones hierarchy and you want this component to follow other component's changes")]
        public bool QueueToLastUpdate = true;

        // V1.2.6 - 1.3.1
        [Tooltip("[Experimental] Using some simple calculations to make tail bend on colliders")]
        public bool UseCollision = false;
        public enum ECollisionMethod { RotationOffset_Old, PositionOffset_New, PositionRotationOffset }
        [Tooltip("Method for detecting collisions, 'RotationOffset' gives you a bit better detecting mesh colliders but is less precise than other methods")]
        public ECollisionMethod CollisionMethod = ECollisionMethod.PositionOffset_New;
        public bool CollideWithOtherTails = false;
        [Tooltip("If you want to continue checking collision if segment collides with one collider (colliding of tail segment with two or more colliders without this option can result in stuttery motion)")]
        public bool DetailedCollision = false;

        public AnimationCurve CollidersScale = AnimationCurve.Linear(0, 1, 1, 1);
        public float CollidersScaleMul = 6.5f;
        [Range(0f, 1f)]
        public float DifferenceScaleFactor = 1f;

        public List<Collider> IgnoredColliders;
        public Vector3 BoxesDimensionsMul = Vector3.one;
        public bool CollidersSameLayer = true;
        [Tooltip("If you add rigidbodies to each tail segment's collider, collision will work on everything but it will be less optimal, you don't have to add here rigidbodies but then you must have not kinematic rigidbodies on objects segments can collide")]
        public bool CollidersAddRigidbody = true;
        public float RigidbodyMass = 1f;

        [FPD_Layers]
        public LayerMask CollidersLayer = 0;


        [Range(-0.5f, 1.2f)]
        [Tooltip("If tail colliding objects should fit to colliders or reflect from them")]
        public float CollisionSwapping = 0.5f;

        public enum ECollisionSpace { World_Slow, Selective_Fast }

        [Tooltip("How collision should be detected, world gives you collision on all world colliders but with more use of cpu (using unity's rigidbodies), 'Selective' gives you possibility to detect collision on selected colliders without using Rigidbodies, it also gives smoother motion (deactivated colliders will still detect collision, unless its game object is disabled)")]
        public ECollisionSpace CollisionSpace = ECollisionSpace.World_Slow;
        public List<Collider> IncludedColliders;
        protected List<FImp_ColliderData_Base> IncludedCollidersData;

        /// <summary> List of collider datas to be checked by every tail segment</summary>
        protected List<FImp_ColliderData_Base> CollidersDataToCheck;

        // V1.2.6
        [Tooltip("If you want to simulate global additional force over tail animation, working not exacly like gravity but tries to mimic this with simple calculations")]
        public Vector3 Curving = Vector3.zero;
        public Vector3 Gravity = Vector3.zero;
        //public AnimationCurve GravityAlongSegments = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        [Tooltip("Rotation offset for first bone in chain, you can use it when enabling 'root to parent' option")]
        public Vector3 RootRotationOffset = Vector3.zero;
        [Tooltip("Position offset for first bone in chain, you can use it when enabling 'root to parent' option")]
        public Vector3 RootPositionOffset = Vector3.zero;

        public FTail_Point RootPoint;

        [HideInInspector]
        [SerializeField]
        private Transform initializationBone1;
        [HideInInspector]
        [SerializeField]
        private Transform initializationBone2;

        /// <summary> Initialization method controll flag </summary>
		protected bool initialized;
        public bool IsInitialized { get; protected set; }

        protected float parentalSpringFactor = 1f;
        protected float parentalSpringFactor2 = 1f;
        protected Vector3 gravityPower;
        protected float sensitivityPower;
        protected bool collisionInitialized = false;
        protected bool forceRefreshCollidersData = false;

        /// <summary> Parent transform of first tail transform </summary>
        protected Transform rootTransform;

        protected bool preAutoCorrect = false;

        protected virtual void Reset()
        {
            IsInitialized = false;
        }

        /// <summary>
        /// Method to initialize component, to have more controll than waiting for Start() method, init can be executed before or after start, as programmer need it.
        /// </summary>
        protected virtual void Init()
        {
            if (initialized) return;

            string name = transform.name;
            if (transform.parent) name = transform.parent.name;

            GenerateRootIfNeeded();

            ConfigureBonesTransforms();

            CoputeHelperVariables();

            PrepareTailPoints();

            if (QueueToLastUpdate) QueueComponentToLastUpdate();

            if (TailTransforms.Count == 1)
            {
                bool wasRoot = RootToParent;
                // WARGING: Setting it automatically when it's detected that only one bone is used in chain
                RootToParent = true;

                if (!wasRoot) LookUpMethod = FELookUpMethod.Parental;

                if (TailTransforms[0].parent == null) Debug.LogError("You want use tail animator on single bone which don't have parent reference transform!");
            }

            prePos = TailTransforms[0].position;

            if (UseCollision) AddColliders();

            initialized = true;
            IsInitialized = true;
        }


        /// <summary>
        /// Calculating helper and fixer variables
        /// </summary>
        protected virtual void CoputeHelperVariables()
        {
            // Precomputing tail look directions for some extra correcting bones structure for animating
            if (TailTransforms.Count > 0)
            {
                if (AddTailReferences)
                {
                    for (int i = 0; i < TailTransforms.Count; i++)
                    {
                        if (TailTransforms[i] == transform) continue;
                        if (!TailTransforms[i].GetComponent<FTail_Reference>()) TailTransforms[i].gameObject.AddComponent<FTail_Reference>().TailReference = this;
                    }
                }
            }

            if (OrientationReference == null) OrientationReference = transform;

            rootTransform = TailTransforms[0].parent;
        }


        /// <summary>
        /// Auto collect tail transforms if they're not defined from inspector
        /// also this is place for override and configure more
        /// </summary>
        protected virtual void ConfigureBonesTransforms()
        {
            AutoGetTailTransforms();
        }

        protected void GenerateRootIfNeeded()
        {
            Transform refer = transform;
            if (TailTransforms != null) if (TailTransforms.Count != 0) refer = TailTransforms[0];

            if (refer != null)
            {
                if (refer.parent == null)
                {
                    // Generating root for tail if there is no root object
                    GameObject rootFixer = new GameObject(name + "-Generated Root");
                    rootFixer.transform.rotation = refer.rotation;

                    if (TailTransforms.Count > 1)
                    {
                        Vector3 backDir = -(refer.InverseTransformPoint(TailTransforms[1].position) - refer.InverseTransformPoint(refer.position)).normalized;
                        rootFixer.transform.position = refer.position + backDir;
                    }
                    else
                        rootFixer.transform.position = refer.position - refer.forward;

                    refer.SetParent(rootFixer.transform, true);
                }
            }
        }


        /// <summary>
        /// Getting child bones to auto define tail structure
        /// </summary>
        public void AutoGetTailTransforms(bool editor = false)
        {
            if (TailTransforms == null) TailTransforms = new List<Transform>();

            bool can = true;
            if (!AutoGetWithOne && !editor) can = false;
            if (!can) return;

            if (TailTransforms.Count < 2)
            {
                Transform lastParent = transform;

                bool boneDefined = true;

                // (V1.1) Start parent
                if (TailTransforms.Count == 0)
                {
                    boneDefined = false;
                    lastParent = transform;
                }
                else lastParent = TailTransforms[0];

                Transform rootTransform = lastParent;

                // 100 iterations because I am scared of while() loops :O so limit to 100 or 1000 if anyone would ever need
                for (int i = TailTransforms.Count; i < 100; i++)
                {
                    if (boneDefined)
                        if (lastParent == rootTransform)
                        {
                            if (lastParent.childCount == 0) break;
                            lastParent = lastParent.GetChild(0);
                            continue;
                        }

                    TailTransforms.Add(lastParent);

                    if (lastParent.childCount > 0) lastParent = lastParent.GetChild(0); else break;
                }
            }
        }


        /// <summary>
        /// Generating invisible points to animate tail freely
        /// </summary>
        protected virtual void PrepareTailPoints()
        {
            if (TailTransforms.Count == 0) Debug.LogError("Zero tail transforms, cannot initialize TailAnimator in " + name);

            proceduralPoints = new List<FTail_Point>();
            localProceduralPoints = new List<FTail_Point>();

            FTail_Point tailPoint;

            // Basic initialization for procedural tail points
            for (int i = 0; i < TailTransforms.Count; i++)
            {
                FTail_Point p = new FTail_Point(TailTransforms[i]) { index = i };
                proceduralPoints.Add(p);
            }

            for (int i = 0; i < TailTransforms.Count - 1; i++)
            {
                proceduralPoints[i].NextPoint = proceduralPoints[i + 1];
            }

            proceduralPoints[TailTransforms.Count - 1].NextPoint = new FTail_Point();

            #region Getting back point for first tail segment

            // Setting next and back point for first bone
            if (proceduralPoints.Count > 1) proceduralPoints[0].NextPoint = proceduralPoints[1];

            tailPoint = proceduralPoints[0];

            FTail_Point rootBackPoint;
            rootBackPoint = new FTail_Point(tailPoint.Transform.parent);

            tailPoint.BackPoint = rootBackPoint;

            // Calculating correction variables for additional back (root) point
            rootBackPoint.NextPoint = tailPoint;
            rootBackPoint.LookDirection = FTail_Point.CalculateLocalForward(rootBackPoint.Transform, rootBackPoint.NextPoint.Transform);
            rootBackPoint.InitLookDirection = rootBackPoint.LookDirection;

            if (rootBackPoint.Transform.parent)
                rootBackPoint.CrossUp = FTail_Point.CalculateCrossUp(rootBackPoint.Transform.parent, rootBackPoint.Transform, OrientationReference);
            else
                rootBackPoint.CrossUp = FTail_Point.CalculateCrossUp(tailPoint.BackPoint.Transform, tailPoint.Transform, OrientationReference);

            rootBackPoint.BoneLength = Vector3.Distance(rootBackPoint.Position, rootBackPoint.NextPoint.Position);
            rootBackPoint.InitBoneLength = rootBackPoint.BoneLength;

            // Bone length for first procedural point
            if (tailPoint.NextPoint != null) tailPoint.BoneLength = Vector3.Distance(tailPoint.Position, tailPoint.NextPoint.Position); else tailPoint.BoneLength = Vector3.Distance(tailPoint.Position, tailPoint.BackPoint.Position);
            tailPoint.InitBoneLength = tailPoint.BoneLength;

            #endregion

            // Initializing back and forward points for tail chain and calculating bone lengths
            for (int i = 1; i < proceduralPoints.Count - 1; i++)
            {
                proceduralPoints[i].NextPoint = proceduralPoints[i + 1];
                proceduralPoints[i].BackPoint = proceduralPoints[i - 1];
                proceduralPoints[i].BoneLength = Vector3.Distance(proceduralPoints[i].Position, proceduralPoints[i].BackPoint.Position);
                proceduralPoints[i].InitBoneLength = proceduralPoints[i].BoneLength;
            }

            // Generating private forward point for last tail point
            if (proceduralPoints.Count > 1)
            {
                tailPoint = proceduralPoints[proceduralPoints.Count - 1];
                tailPoint.BackPoint = proceduralPoints[tailPoint.index - 1];
                tailPoint.BoneLength = tailPoint.BackPoint.BoneLength;
                tailPoint.InitBoneLength = tailPoint.BoneLength;

                FTail_Point forwardPoint = new FTail_Point(tailPoint.Transform);
                forwardPoint.Transform = null;
                forwardPoint.Position = tailPoint.Position + FTail_Point.CalculateLocalForward(tailPoint.BackPoint.Transform, tailPoint.Transform);
                forwardPoint.PreviousPosition = forwardPoint.Position;
                forwardPoint.RotationTargetPos = forwardPoint.Position;
            }

            // Final variables calculations for whole tail points chain
            for (int i = 0; i < proceduralPoints.Count - 1; i++)
            {
                tailPoint = proceduralPoints[i];
                tailPoint.LookDirection = FTail_Point.CalculateLocalForward(tailPoint.Transform, tailPoint.NextPoint.Transform);
                tailPoint.InitLookDirection = tailPoint.LookDirection;

                tailPoint.CrossUp = FTail_Point.CalculateCrossUp(tailPoint.BackPoint.Transform, tailPoint.Transform, OrientationReference);
            }

            tailPoint = proceduralPoints[proceduralPoints.Count - 1];
            tailPoint.CrossUp = FTail_Point.CalculateCrossUp(tailPoint.BackPoint.Transform, tailPoint.Transform, OrientationReference);

        }

        /// <summary>
        /// Initialize component for correct work
        /// </summary>
        protected void Start()
        {
            if (InitBeforeAnimator) Init(); else StartCoroutine(InitInLate());
        }


        /// <summary>
        /// Initializing component after animator frame step
        /// </summary>
        protected System.Collections.IEnumerator InitInLate()
        {
            yield return new WaitForEndOfFrame();
            Init();
            yield break;
        }

        /// <summary>
        /// Main method to put motion calculations methods order for derived classes
        /// </summary>
        public virtual void CalculateOffsets()
        {
            if (SafeDeltaTime)
                deltaTime = Mathf.Lerp(deltaTime, GetClampedSmoothDelta(), 0.03f);
            else
                deltaTime = Time.deltaTime;

            MotionCalculations();
        }

        //protected Matrix4x4 localMatrix;

        protected Vector3 prePos;

        /// <summary>
        /// Calculating tail-like movement animation logic for given transforms list
        /// </summary>
        protected virtual void MotionCalculations()
        {
            //localMatrix = Matrix4x4.TRS(TailTransforms[0].transform.position, Quaternion.identity, Vector3.one);

            #region Pre motion calculation init operations

            if (MotionInfluence < 1f)
            {
                Vector3 diff = (prePos - TailTransforms[0].position) * (1f - MotionInfluence);
                for (int i = 1; i < proceduralPoints.Count; i++)
                {
                    proceduralPoints[i].SetPosition(proceduralPoints[i].Position - diff);
                }
            }

            if (UseCollision)
            {
                if (CollisionSpace == ECollisionSpace.World_Slow) if (!collisionInitialized) AddColliders();
            }

            if (preAutoCorrect != UseAutoCorrectLookAxis)
            {
                ApplyAutoCorrection();
                preAutoCorrect = UseAutoCorrectLookAxis;
            }

            if (FullCorrection)
            {
                if (AnimateCorrections)
                    for (int i = 0; i < TailTransforms.Count; i++) proceduralPoints[i].Correction = TailTransforms[i].localRotation;
                else
                    if (proceduralPoints[0].Correction != proceduralPoints[0].InitialLocalRotation)
                    for (int i = 0; i < TailTransforms.Count; i++) proceduralPoints[i].Correction = proceduralPoints[i].InitialLocalRotation;
            }

            gravityPower = Gravity / 40f;

            if (Sensitivity <= 0.05f)
                sensitivityPower = Mathf.Lerp(10f, 0f, Sensitivity / 0.05f);
            else
            if (Sensitivity <= 0.5f)
                sensitivityPower = Mathf.Lerp(3f, 0f, Sensitivity / 0.5f);
            else
                sensitivityPower = Mathf.Lerp(0f, -0.9f, (Sensitivity - 0.5f) * 2f);

            #endregion


            #region Delta Value Calculations

            float posDelta;
            float rotDelta;
            float muler = 1f;

            if (LookUpMethod == FELookUpMethod.Parental)
            {
                parentalSpringFactor = 2.5f;
                parentalSpringFactor2 = 0.05f;
                muler = 0.8f;
            }
            else
            {
                parentalSpringFactor = 1f;
                parentalSpringFactor2 = 1f;
            }

            if (PositionSpeed > 0f)
            {
                posDelta = deltaTime * PositionSpeed * muler;
                posDelta = Mathf.Lerp(posDelta, posDelta * 2f, Springiness);
            }
            else
                posDelta = 0f;

            rotDelta = deltaTime * RotationSpeed * muler;
            rotDelta = Mathf.Lerp(rotDelta, rotDelta * 1.8f, Springiness);

            #endregion


            #region Chain beginning operations

            int startIndex = 1;

            RootPoint = proceduralPoints[0].BackPoint;

            if (!RootToParent)
            {
                proceduralPoints[0].SetPosition(TailTransforms[0].position);
                proceduralPoints[0].BoneLength = proceduralPoints[0].InitBoneLength;
                proceduralPoints[0].PreCollisionPosition = TailTransforms[0].position;
            }
            else
            {
                startIndex = 0;

                if (RootPoint.Transform)
                {
                    RootPoint.Position = RootPoint.Transform.position + proceduralPoints[0].Transform.TransformVector(RootPositionOffset);

                    if (LookUpMethod == FELookUpMethod.Parental)
                    {
                        RootPoint.Rotation = (RootPoint.Transform.rotation) * Quaternion.Euler(RootRotationOffset);
                        AngleLimiting(RootPoint, ref RootPoint.Rotation);
                    }
                    else
                    {
                        RootPoint.Rotation = (
                            RootPoint.Transform.rotation *
                            (proceduralPoints[0].InitialLocalRotation * Quaternion.FromToRotation(RootPoint.LookDirection, ExtraToDirection))
                            ) * Quaternion.Euler(RootRotationOffset);

                        AngleLimiting(RootPoint, ref RootPoint.Rotation);
                    }
                }

                proceduralPoints[0].BoneLength = RootPoint.BoneLength;
            }


            #endregion


            #region Optimization Wise Update Methods

            if (UseCollision)
            {
                if (CollisionSpace == ECollisionSpace.Selective_Fast)
                {
                    RefreshCollidersDataList();

                    // Letting every tail segment check only enabled colliders by game object
                    CollidersDataToCheck.Clear();

                    for (int i = 0; i < IncludedCollidersData.Count; i++)
                    {
                        if (IncludedCollidersData[i].Collider == null) { forceRefreshCollidersData = true; break; }
                        if (IncludedCollidersData[i].Collider.gameObject.activeInHierarchy)
                        {
                            IncludedCollidersData[i].RefreshColliderData();
                            CollidersDataToCheck.Add(IncludedCollidersData[i]);
                        }
                    }

                    for (int i = startIndex; i < proceduralPoints.Count; i++) // Using Fast Collision Update
                    {
                        float stiffer = StiffTailEndValue(i);
                        Vector3 targetPosition = CalculateTargetPosition(proceduralPoints[i]);
                        targetPosition = PhysicsCalculationsForSegmentSelective(proceduralPoints[i], targetPosition);
                        proceduralPoints[i].SetPosition(Vector3.Lerp(proceduralPoints[i].Position, targetPosition, posDelta * stiffer));

                        // Assigning position to procedural point
                        StretchingLimiting(proceduralPoints[i], targetPosition);

                        Quaternion targetLookRotation = CalculateTargetRotation(proceduralPoints[i]);
                        AngleLimiting(proceduralPoints[i], ref targetLookRotation);

                        proceduralPoints[i].SetRotation(Quaternion.Slerp(proceduralPoints[i].Rotation, targetLookRotation, rotDelta * stiffer));
                    }
                }
                else
                {
                    for (int i = startIndex; i < proceduralPoints.Count; i++) // Using World Collision Update
                    {
                        float stiffer = StiffTailEndValue(i);
                        Vector3 targetPosition = CalculateTargetPosition(proceduralPoints[i]);
                        targetPosition = PhysicsCalculationsForSegmentWorldSpace(proceduralPoints[i], targetPosition);
                        proceduralPoints[i].SetPosition(Vector3.Lerp(proceduralPoints[i].Position, targetPosition, posDelta * stiffer));

                        // Assigning position to procedural point
                        StretchingLimiting(proceduralPoints[i], targetPosition);

                        Quaternion targetLookRotation = CalculateTargetRotation(proceduralPoints[i]);
                        AngleLimiting(proceduralPoints[i], ref targetLookRotation);

                        proceduralPoints[i].SetRotation(Quaternion.Slerp(proceduralPoints[i].Rotation, targetLookRotation, rotDelta * stiffer));
                    }
                }

                // Support for 'RotationOffset' method of collisions
                if (CollisionMethod == ECollisionMethod.RotationOffset_Old)
                    for (int i = 1; i < proceduralPoints.Count; i++)
                    {
                        UseCollisionContact(i);

                        if (proceduralPoints[i].collisionFlags > 0f)
                            proceduralPoints[i].collisionFlags -= deltaTime * 8f;
                        else
                            proceduralPoints[i].collisionOffsets = Vector3.zero;
                    }
            }
            else // Not using colliders update methods
            {
                if (MaxStretching < 1f) // Using stretching Update
                {
                    for (int i = startIndex; i < proceduralPoints.Count; i++)
                    {
                        float stiffer = StiffTailEndValue(i);
                        Vector3 targetPosition = CalculateTargetPosition(proceduralPoints[i]);

                        // Assigning position to procedural point
                        proceduralPoints[i].SetPosition(Vector3.Lerp(proceduralPoints[i].Position, targetPosition, posDelta * stiffer));
                        StretchingLimiting(proceduralPoints[i], targetPosition);

                        Quaternion targetLookRotation = CalculateTargetRotation(proceduralPoints[i]);
                        AngleLimiting(proceduralPoints[i], ref targetLookRotation);

                        proceduralPoints[i].SetRotation(Quaternion.Slerp(proceduralPoints[i].Rotation, targetLookRotation, rotDelta * stiffer));
                    }
                }
                else // Simpliest way (stuff inside CalculateTarget... will be optimized too, in soon updates)
                {
                    for (int i = startIndex; i < proceduralPoints.Count; i++)
                    {
                        float stiffer = StiffTailEndValue(i);
                        //Vector3 pre = proceduralPoints[i].Position - proceduralPoints[i].BackPoint.Position;
                        Vector3 targetPosition = CalculateTargetPosition(proceduralPoints[i]);

                        proceduralPoints[i].SetPosition(Vector3.Lerp(proceduralPoints[i].Position, targetPosition, posDelta * stiffer));
                        Quaternion targetLookRotation = CalculateTargetRotation(proceduralPoints[i]);
                        AngleLimiting(proceduralPoints[i], ref targetLookRotation);

                        //if (i == TailTransforms.Count / 2) Debug.Log("Dist = " + (proceduralPoints[i].Position - proceduralPoints[i].BackPoint.Position).magnitude + " was " + pre.magnitude);

                        proceduralPoints[i].SetRotation(Quaternion.Slerp(proceduralPoints[i].Rotation, targetLookRotation, rotDelta * stiffer));
                    }
                }
            }

            #endregion

            prePos = TailTransforms[0].position;
        }


        /// <summary>
        /// Setting tail transforms positions and rotations in world from procedural points animation
        /// </summary>
        protected virtual void SetTailTransformsFromPoints()
        {
            for (int i = 0; i < TailTransforms.Count; i++)
            {
                TailTransforms[i].position = proceduralPoints[i].Position;
                TailTransforms[i].rotation = proceduralPoints[i].Rotation;
            }
        }


        /// <summary>
        /// Calculating target tail position point, including calculating springyness, collision offsets and limiting stretching
        /// </summary>
        protected virtual Vector3 CalculateTargetPosition(FTail_Point tailPoint)
        {
            Vector3 translationVector = tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection);
            if (tailPoint.InitialLossyScale.magnitude != 0f) tailPoint.ScaleFactor = tailPoint.Transform.lossyScale.magnitude / tailPoint.InitialLossyScale.magnitude;

            // Basic target position for tail segment
            //Vector3 targetPosition = tailPoint.BackPoint.Position + (translationVector * -1f * (tailPoint.BoneLength * LengthMultiplier * tailPoint.ScaleFactor)) + gravityPower * 0.4f /* * GravityAlongSegments.Evaluate((float)tailPoint.index / (float)proceduralPoints.Count)*/;
            Vector3 targetPosition = tailPoint.BackPoint.Position + (translationVector * -1f * (tailPoint.BoneLength * LengthMultiplier * tailPoint.ScaleFactor))/* * GravityAlongSegments.Evaluate((float)tailPoint.index / (float)proceduralPoints.Count)*/;

            float gravityHelper = targetPosition.magnitude;
            //targetPosition += gravityPower * 0.1f * tailPoint.BoneLength * tailPoint.Transform.lossyScale.x;
            targetPosition = targetPosition.normalized * gravityHelper;

            targetPosition = SpringinessCalculations(tailPoint, targetPosition); // To optimize
            tailPoint.RotationTargetPos = Vector3.Lerp(tailPoint.Position, targetPosition, Springiness);
            tailPoint.RotationTargetPos += gravityPower * tailPoint.BoneLength * tailPoint.Transform.lossyScale.x;
            tailPoint.PreCollisionPosition = targetPosition;

            return targetPosition;
        }

        // V1.1 and V1.1.1/2
        /// <summary>
        /// Calculates target rotation for one tail segment
        /// We will override it for some exception calculations like 2D rotation
        /// </summary>
        protected virtual Quaternion CalculateTargetRotation(FTail_Point tailPoint = null)
        {
            Quaternion targetRotation;
            // Many stuff to optimize later

            if (FullCorrection)
            {
                Vector3 startLookPos = tailPoint.BackPoint.Position;
                Vector3 lookingAt = tailPoint.RotationTargetPos;

                //startLookPos += (tailPoint.PreCollisionPosition - tailPoint.Position);

                targetRotation = Quaternion.identity;

                bool rotationCollision = false;

                if (UseCollision)
                    if (CollisionMethod == ECollisionMethod.RotationOffset_Old)
                        if (tailPoint.collisionFlags > 0f) if (tailPoint.collisionOffsets != Vector3.zero) rotationCollision = true;

                if (LookUpMethod != FELookUpMethod.Parental)
                {
                    startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.InitBoneLength * tailPoint.ScaleFactor * sensitivityPower;
                    startLookPos -= (tailPoint.BackPoint.PreCollisionPosition - tailPoint.BackPoint.Position) * CollisionSwapping;

                    if (!rotationCollision)
                    {
                        if (startLookPos - lookingAt != Vector3.zero)
                        {
                            if (LookUpMethod == FELookUpMethod.Default)
                            {
                                //targetRotation = Quaternion.LookRotation(startLookPos - lookingAt, tailPoint.BackPoint.TransformDirection(AxisLookBack));
                                targetRotation = Quaternion.LookRotation(startLookPos - lookingAt, (tailPoint.BackPoint.Rotation * Quaternion.FromToRotation(tailPoint.LookDirection, tailPoint.InitialLocalRotation * Vector3.forward)) * AxisLookBack);
                            }
                            else if (LookUpMethod == FELookUpMethod.RolledBones)
                                targetRotation = Quaternion.LookRotation(startLookPos - lookingAt, tailPoint.BackPoint.TransformDirection(tailPoint.LookBackDirection));
                            else
                                targetRotation = Quaternion.LookRotation(startLookPos - lookingAt, tailPoint.BackPoint.TransformDirection(tailPoint.CrossUp));

                            //startLookPos = tailPoint.BackPoint.PreCollisionPosition;
                            //tailPoint.PreCollisionRotation = Quaternion.LookRotation(startLookPos - lookingAt, tailPoint.BackPoint.TransformDirection(AxisLookBack));
                            //tailPoint.PreCollisionRotation *= Quaternion.FromToRotation(tailPoint.BackPoint.LookDirection, ExtraToDirection);
                        }
                    }
                    else
                    {
                        Vector3 tailDirection = (startLookPos - lookingAt).normalized;
                        Vector3 upwards;

                        if (LookUpMethod == FELookUpMethod.Default)
                            upwards = tailPoint.BackPoint.TransformDirection(AxisLookBack);
                        else if (LookUpMethod == FELookUpMethod.RolledBones)
                            upwards = tailPoint.BackPoint.TransformDirection(tailPoint.LookBackDirection);
                        else
                            upwards = tailPoint.BackPoint.TransformDirection(tailPoint.CrossUp);

                        Vector3 smoothedDirection = Vector3.Slerp(tailDirection, (tailDirection + tailPoint.collisionOffsets).normalized, tailPoint.collisionFlags);
                        targetRotation = Quaternion.LookRotation(smoothedDirection, upwards);
                    }

                    targetRotation *= Quaternion.FromToRotation(tailPoint.BackPoint.LookDirection, ExtraToDirection);
                }
                else // Parental method
                {
                    startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.InitBoneLength * tailPoint.ScaleFactor * sensitivityPower;

                    Vector3 targetPos = lookingAt - startLookPos;

                    if (rotationCollision) targetPos = Vector3.Slerp(targetPos, (targetPos - tailPoint.collisionOffsets).normalized, tailPoint.collisionFlags);

                    Quaternion targetingRot = Quaternion.FromToRotation(tailPoint.BackPoint.TransformDirection(tailPoint.Transform.localPosition), targetPos);

                    targetRotation = targetingRot * tailPoint.BackPoint.Rotation;
                }

                if (Curving != Vector3.zero)
                {
                    float mul = 10 / ((float)tailPoint.index * 4.5f + 1f);
                    targetRotation *= Quaternion.Euler(Curving.y * mul, Curving.x * mul, 0f);
                }

                targetRotation *= tailPoint.Correction;
            }
            else
            {
                targetRotation = BaseCorrectionRotation(tailPoint);
            }

            //targetRotation *= Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Inverse(tailPoint.Rotation) * targetRotation, sensitivityPower - 1f);

            return targetRotation;
        }


        private Quaternion BaseCorrectionRotation(FTail_Point tailPoint)
        {
            Quaternion targetRotation = Quaternion.identity;
            Vector3 startLookPos = tailPoint.BackPoint.Position;
            Vector3 lookingAt = tailPoint.RotationTargetPos;

            //startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.BackPoint.InitBoneLength * sensitivityPower;
            startLookPos += tailPoint.BackPoint.TransformDirection(tailPoint.BackPoint.LookDirection) * tailPoint.InitBoneLength * sensitivityPower;
            startLookPos -= (tailPoint.BackPoint.PreCollisionPosition - tailPoint.BackPoint.Position) * CollisionSwapping;

            bool rotationCollision = false;
            if (UseCollision)
                if (CollisionMethod == ECollisionMethod.RotationOffset_Old)
                    if (tailPoint.collisionFlags > 0f) if (tailPoint.collisionOffsets != Vector3.zero)
                        {
                            Vector3 tailDirection = (startLookPos - lookingAt).normalized;
                            Vector3 smoothedDirection = Vector3.Slerp(tailDirection, (tailDirection + tailPoint.collisionOffsets).normalized, tailPoint.collisionFlags);
                            targetRotation = Quaternion.LookRotation(smoothedDirection, tailPoint.BackPoint.TransformDirection(AxisLookBack));
                            rotationCollision = true;
                        }

            if (!rotationCollision)
                if ((startLookPos - lookingAt) != Vector3.zero)
                    targetRotation = Quaternion.LookRotation(startLookPos - lookingAt, tailPoint.BackPoint.TransformDirection(AxisLookBack * Mathf.Sign(FVectorMethods.VectorSum(AxisCorrection))));


            if (Curving != Vector3.zero)
            {
                float mul = 10 / (tailPoint.index * 4.5f + 1);
                targetRotation *= Quaternion.Euler(Curving.y * mul, Curving.x * mul, 0f);
            }

            if (ExtraCorrectionOptions) targetRotation *= Quaternion.FromToRotation(ExtraFromDirection, ExtraToDirection);

            //if (FullCorrection) if (AnimateCorrections) targetRotation *= tailPoint.Correction * Quaternion.Inverse(tailPoint.InitialRotation);

            return targetRotation;
        }


        /// <summary>
        /// Remove all disconnected transforms when object is destroyed in derived class
        /// </summary>
        protected virtual void OnDestroy()
        {
        }


        // V.1.1.1
        /// <summary>
        /// Auto correcting tail look axes
        /// </summary>
        protected void ApplyAutoCorrection()
        {
            ExtraCorrectionOptions = true;
            AxisCorrection = proceduralPoints[0].LookDirection;
            ExtraFromDirection = AxisCorrection;
        }


        private float GetClampedSmoothDelta()
        {
            return Mathf.Clamp(Time.smoothDeltaTime, 0f, 0.25f);
        }


        /// <summary>
        /// Simple but effective, pushing component to be executed as last in the frame
        /// </summary>
        public void QueueComponentToLastUpdate()
        {
            enabled = false;
            enabled = true;
        }

        /// <summary>
        /// Method reserved for refreshing stuff in some derived classes every time when something is changed in the inspector
        /// </summary>
        public virtual void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (CollisionSpace == ECollisionSpace.Selective_Fast)
                    if (proceduralPoints != null) if (proceduralPoints.Count > 1) for (int i = 0; i < proceduralPoints.Count; i++) proceduralPoints[i].ColliderRadius = GetColliderSphereRadiusFor(i);
            }

        }

        /// <summary>
        /// If tail is too long we making it shorter
        /// </summary>
        protected void StretchingLimiting(FTail_Point tailPoint, Vector3 targetPosition)
        {
            float dist = (tailPoint.Position - targetPosition).magnitude;
            if (dist > 0f)
            {
                float maxDist = tailPoint.BoneLength * 2 * MaxStretching;

                if (dist > maxDist) tailPoint.Position = Vector3.Lerp(tailPoint.Position, targetPosition, Mathf.InverseLerp(dist, 0f, maxDist));
                //else
                //{
                //float maxDist = tailPoint.BoneLength * 2 * MaxStretching;
                //dist = (tailPoint.Position - tailPoint.NextPoint.Position).magnitude;
                //if (dist > maxDist) tailPoint.Position = Vector3.Lerp(tailPoint.Transform.parent.TransformPoint(tailPoint.InitialLocalPosition), tailPoint.Position, Mathf.InverseLerp(dist, 0f, maxDist));
                //}
            }
        }

        protected void AngleLimiting(FTail_Point tailPoint, ref Quaternion targetNewRotation)
        {
            if (AngleLimit < 90f)
            {
                Quaternion targetInLocal = Quaternion.Inverse(tailPoint.Transform.parent.rotation) * targetNewRotation;

                // Comparing initial local rotation with target rotation in local space
                float angleDiffToInitPose;


                if (AngleLimitAxis != Vector3.zero)
                {
                    AngleLimitAxis.Normalize();

                    if (LimitAxisRange.x == LimitAxisRange.y)
                    {
                        angleDiffToInitPose = Mathf.DeltaAngle(
                            Vector3.Scale(tailPoint.InitialLocalRotation.eulerAngles, AngleLimitAxis).magnitude,
                            Vector3.Scale(targetInLocal.eulerAngles, AngleLimitAxis).magnitude);

                        if (angleDiffToInitPose < 0f) angleDiffToInitPose = -angleDiffToInitPose;
                    }
                    else
                    {
                        angleDiffToInitPose = Mathf.DeltaAngle(
                            Vector3.Scale(tailPoint.InitialLocalRotation.eulerAngles, AngleLimitAxis).magnitude,
                            Vector3.Scale(targetInLocal.eulerAngles, AngleLimitAxis).magnitude);

                        if (angleDiffToInitPose > LimitAxisRange.x && angleDiffToInitPose < LimitAxisRange.y) angleDiffToInitPose = 0f;
                        if (angleDiffToInitPose < 0) angleDiffToInitPose = -angleDiffToInitPose;
                    }

                    //angleDiffToInitPose = Quaternion.Angle(tailPoint.InitialLocalRotation, targetInLocal);
                }
                else
                    angleDiffToInitPose = Quaternion.Angle(tailPoint.InitialLocalRotation, targetInLocal);


                float angleLimMul = AngleLimit;

                if (angleDiffToInitPose > angleLimMul)
                {
                    float exceededAngle = Mathf.Abs(Mathf.DeltaAngle(angleDiffToInitPose, angleLimMul));
                    float angleFactor = Mathf.InverseLerp(0f, angleLimMul, exceededAngle); // percentage value from target rotation to limit

                    Quaternion newLocal;

                    if (LimitSmoothing > 0f)
                    {
                        float smooth = Mathf.Lerp(55f, 15f, LimitSmoothing);
                        newLocal = Quaternion.Lerp(targetInLocal, tailPoint.InitialLocalRotation, deltaTime * smooth * angleFactor);
                    }
                    else
                    {
                        newLocal = Quaternion.Lerp(targetInLocal, tailPoint.InitialLocalRotation, angleFactor);
                    }

                    targetNewRotation = tailPoint.Transform.parent.rotation * newLocal; // Converting from local to world space

                    //if (tailPoint.index == 3) Debug.Log("exc: " + exceededAngle + " fac: " + angleFactor + " diff: " + angleDiffToInitPose);
                }
            }
        }


        /// <summary>
        /// Multiplier value for further tail segments to move quicker
        /// </summary>
        protected float StiffTailEndValue(float i)
        {
            return 1f + ((i / 1.5f) / (float)proceduralPoints.Count) * StiffTailEnd;
        }

        /// <summary>
        /// Triggering physics methods for certain tail segment
        /// </summary>
        protected Vector3 PhysicsCalculationsForSegmentWorldSpace(FTail_Point tailPoint, Vector3 targetPosition)
        {
            if ((int)CollisionMethod >= 1)
                UseCollisionContact(tailPoint.index, ref targetPosition);

            float boneLength = (tailPoint.BackPoint.Position - targetPosition).magnitude;

            // If tail is too short be enlarging it
            Vector3 toLook = tailPoint.BackPoint.Position - targetPosition;
            float currentLength = toLook.magnitude;
            if (boneLength < tailPoint.BackPoint.BoneLength * tailPoint.ScaleFactor) tailPoint.Position += toLook * ((currentLength - tailPoint.BackPoint.InitBoneLength) / currentLength);

            return targetPosition;
        }


        /// <summary>
        /// Triggering physics methods for certain tail segment
        /// </summary>
        protected Vector3 PhysicsCalculationsForSegmentSelective(FTail_Point tailPoint, Vector3 targetPosition)
        {
            PushIfSegmentInsideCollider(tailPoint, ref targetPosition);

            float boneLength = (tailPoint.BackPoint.Position - targetPosition).magnitude;

            // If tail is too short be enlarging it
            Vector3 toLook = tailPoint.BackPoint.Position - targetPosition;
            float currentLength = toLook.magnitude;
            if (boneLength < tailPoint.BackPoint.BoneLength * tailPoint.ScaleFactor)
            {
                tailPoint.Position += toLook * ((currentLength - tailPoint.BackPoint.InitBoneLength) / currentLength);
            }

            return targetPosition;
        }

        /// <summary>
        /// Calculating springness for tail segment
        /// </summary>
        protected Vector3 SpringinessCalculations(FTail_Point tailPoint, Vector3 targetPosition)
        {
            if (Springiness > 0f)
            {
                Vector3 backPosDiff = (tailPoint.Position - tailPoint.PreviousPosition) * MotionInfluence;
                Vector3 newPos = tailPoint.Position;

                tailPoint.PreviousPosition = tailPoint.Position;

                Vector3 offset = backPosDiff * (1 - Mathf.Lerp(0.3f / parentalSpringFactor, 0.05f / parentalSpringFactor, Springiness));
                tailPoint.SpringOffset = Vector3.Lerp(tailPoint.SpringOffset, offset, deltaTime * (5f + Springiness * 45f));
                newPos += tailPoint.SpringOffset;

                float restDistance = (tailPoint.BackPoint.Position - newPos).magnitude;

                Matrix4x4 otherLocalToWorld = tailPoint.BackPoint.Transform.localToWorldMatrix;
                otherLocalToWorld.SetColumn(3, tailPoint.BackPoint.Position);
                Vector3 restPos = otherLocalToWorld.MultiplyPoint3x4(tailPoint.Transform.localPosition);

                Vector3 diffPosVector = restPos - newPos;
                offset = diffPosVector * Mathf.Lerp(0.5f * parentalSpringFactor2, 0.2f * parentalSpringFactor2, Springiness);
                newPos += offset;

                Vector3 backp = tailPoint.Position + diffPosVector * Mathf.Lerp(0.5f, 0.2f, Springiness);

                diffPosVector = restPos - newPos;
                float distance = diffPosVector.magnitude;
                float maxDistance = restDistance * (1 - Mathf.Lerp(0.4f, 0.1f, Springiness)) * 2;

                if (distance > maxDistance)
                {
                    offset = diffPosVector * ((distance - maxDistance) / distance);
                    newPos += offset;
                }

                if (MaxStretching < 1f)
                {
                    float dist = Vector3.Distance(targetPosition, newPos);

                    if (dist > 0f)
                    {
                        float maxDist = tailPoint.BoneLength * 2 * MaxStretching;
                        float factor = Mathf.InverseLerp(dist, 0f, maxDist);
                        if (dist > maxDist) newPos = Vector3.Lerp(newPos, Vector3.Lerp(backp, targetPosition, factor), factor);
                    }
                }

                targetPosition = Vector3.Lerp(targetPosition, newPos, Mathf.Lerp(0.3f, 0.8f, Springiness));
            }

            return targetPosition;
        }



        #region V1.2.6 Colliders Support

        /// <summary>
        /// Generating colliders on tail with provided settings
        /// </summary>
        private void AddColliders()
        {
            if (CollisionSpace == ECollisionSpace.World_Slow)
            {
                for (int i = 1; i < TailTransforms.Count; i++)
                {
                    if (CollidersSameLayer) TailTransforms[i].gameObject.layer = gameObject.layer; else TailTransforms[i].gameObject.layer = CollidersLayer;
                }

                for (int i = 1; i < TailTransforms.Count; i++)
                {
                    SphereCollider b = TailTransforms[i].gameObject.AddComponent<SphereCollider>();
                    FTail_CollisionHelper tcol = TailTransforms[i].gameObject.AddComponent<FTail_CollisionHelper>().Init(CollidersAddRigidbody, RigidbodyMass);
                    tcol.TailCollider = b;
                    tcol.Index = i;
                    tcol.ParentTail = this;
                    b.radius = GetColliderSphereRadiusFor(TailTransforms, i);
                    proceduralPoints[i].ColliderRadius = b.radius;
                    proceduralPoints[i].CollisionHelper = tcol;
                }
            }
            else
            {
                for (int i = 0; i < TailTransforms.Count; i++)
                {
                    proceduralPoints[i].ColliderRadius = GetColliderSphereRadiusFor(i);
                }

                IncludedCollidersData = new List<FImp_ColliderData_Base>();
                CollidersDataToCheck = new List<FImp_ColliderData_Base>();
                RefreshCollidersDataList();
            }

            collisionInitialized = true;
        }


        //V1.2.6
        /// <summary>
        /// Collision data sent by single tail segment
        /// </summary>
        internal void CollisionDetection(int index, Collision collision)
        {
            proceduralPoints[index].collisionContacts = collision;
        }

        /// <summary>
        /// Exitting collision
        /// </summary>
        internal void ExitCollision(int index)
        {
            proceduralPoints[index].collisionContacts = null;
        }

        public bool CollisionLookBack = true;

        /// <summary>
        /// Use saved collision contact in right moment when uxecuting update methods
        /// </summary>
        protected void UseCollisionContact(int index, ref Vector3 pos)
        {
            if (proceduralPoints[index].collisionContacts == null) return;
            if (proceduralPoints[index].collisionContacts.contacts.Length == 0) return; // In newest Unity 2018 versions 'Collision' class is generated even there are no collision contacts

            Collision collision = proceduralPoints[index].collisionContacts;
            float thisCollRadius = FImp_ColliderData_Sphere.CalculateTrueRadiusOfSphereCollider(proceduralPoints[index].Transform, proceduralPoints[index].ColliderRadius);

            if (collision.collider)
            {
                SphereCollider collidedSphere = collision.collider as SphereCollider;

                // If we collide sphere we can calculate precise segment offset for it
                if (collidedSphere)
                {
                    FImp_ColliderData_Sphere.PushOutFromSphereCollider(collidedSphere, thisCollRadius, ref pos, Vector3.zero);
                }
                else
                {
                    CapsuleCollider collidedCapsule = collision.collider as CapsuleCollider;

                    // If we collide capsule we can calculate precise segment offset for it
                    if (collidedCapsule)
                    {
                        FImp_ColliderData_Capsule.PushOutFromCapsuleCollider(collidedCapsule, thisCollRadius, ref pos, Vector3.zero);
                    }
                    else
                    {
                        BoxCollider collidedBox = collision.collider as BoxCollider;

                        // If we collide box we can calculate precise segment offset for it
                        if (collidedBox)
                        {
                            if (proceduralPoints[index].CollisionHelper.RigBody)
                            {
                                if (collidedBox.attachedRigidbody)
                                {
                                    if (proceduralPoints[index].CollisionHelper.RigBody.mass > 1f)
                                    {
                                        FImp_ColliderData_Box.PushOutFromBoxCollider(collidedBox, collision, thisCollRadius, ref pos);
                                        Vector3 pusherPos = pos;
                                        FImp_ColliderData_Box.PushOutFromBoxCollider(collidedBox, thisCollRadius, ref pos);

                                        pos = Vector3.Lerp(pos, pusherPos, proceduralPoints[index].CollisionHelper.RigBody.mass / 5f);
                                    }
                                    else
                                        FImp_ColliderData_Box.PushOutFromBoxCollider(collidedBox, thisCollRadius, ref pos);
                                }
                                else
                                    FImp_ColliderData_Box.PushOutFromBoxCollider(collidedBox, thisCollRadius, ref pos);
                            }
                            else
                                FImp_ColliderData_Box.PushOutFromBoxCollider(collidedBox, thisCollRadius, ref pos);
                        }
                        else // If we collide mesh we can't calculate very precise segment offset but we can support it in some way
                        {
                            MeshCollider collidedMesh = collision.collider as MeshCollider;
                            if (collidedMesh)
                            {
                                FImp_ColliderData_Mesh.PushOutFromMeshCollider(collidedMesh, collision, thisCollRadius, ref pos);
                            }
                            else // If we collide terrain we can calculate very precise segment offset because terrain not rotates
                            {
                                TerrainCollider terrain = collision.collider as TerrainCollider;
                                FImp_ColliderData_Terrain.PushOutFromTerrain(terrain, thisCollRadius, ref pos);
                            }
                        }
                    }
                }
            }

            //ExitCollision(index -1);
        }

        /// <summary>
        /// Use saved collision contact in right moment when uxecuting update methods
        /// </summary>
        protected void UseCollisionContact(int index)
        {
            if (proceduralPoints[index].collisionContacts == null) return;
            if (proceduralPoints[index].collisionContacts.contacts.Length == 0) return; // In newest Unity 2018 versions 'Collision' class is generated even there are no collision contacts

            Collision collision = proceduralPoints[index].collisionContacts;
            Vector3 desiredDirection;
            desiredDirection = Vector3.Reflect((proceduralPoints[index - 1].Position - proceduralPoints[index].Position).normalized, collision.contacts[0].normal);

            #region Experiments

            //Quaternion segmentForwQ = Quaternion.LookRotation(proceduralPoints[index - 1].Position - proceduralPoints[index].Position);// * Quaternion.FromToRotation(tailLookDirections[index - 1], ExtraToDirection);
            //desiredDirection = Vector3.Reflect(segmentForwQ.eulerAngles.normalized, collision.contacts[0].normal);

            //Quaternion segmentForwQ = Quaternion.LookRotation(proceduralPoints[index].Position - transform.position);// * Quaternion.FromToRotation(tailLookDirections[index - 1], ExtraToDirection);
            //Quaternion segmentForwQ = proceduralPoints[index].Rotation * Quaternion.FromToRotation(tailLookDirections[index - 1], ExtraToDirection);

            //desiredDirection = Vector3.ProjectOnPlane(segmentForward, collision.contacts[0].normal);
            //desiredDirection = Vector3.Lerp(Vector3.Reflect(segmentForward, collision.contacts[0].normal), desiredDirection, collisionFlagsSlow[index]);

            //Vector3 segmentForward = segmentForwQ.eulerAngles.normalized;


            //desiredDirection = Vector3.Project(segmentForward, collision.contacts[0].normal);
            //Plane collisionPlane = new Plane(collision.contacts[0].normal, collision.contacts[0].point);

            //Vector3 norm = collision.contacts[0].normal;
            //Vector3 dir = segmentForward;
            //Vector3.OrthoNormalize(ref norm, ref dir);


            // desiredDirection = Vector3.ProjectOnPlane((TailTransforms[index].position - TailTransforms[index - 1].position).normalized, collision.contacts[0].normal);
            // Quaternion startDiffQuat = Quaternion.Inverse(proceduralPoints[index].InitialRotation * Quaternion.Inverse( TailTransforms[index].localRotation));

            //if (CollisionLookBack)
            //{
            //    Vector3 backCompensation = segmentForward + (Quaternion.Inverse(TailTransforms[index - 1].localRotation) * proceduralPoints[index - 1].InitialRotation) * Vector3.forward;
            //    desiredDirection -= backCompensation;

            //Quaternion backCompensation = Quaternion.Inverse(segmentForwQ) * (Quaternion.Inverse(TailTransforms[index - 1].localRotation) * proceduralPoints[index - 1].InitialRotation);
            //desiredDirection -= backCompensation * Vector3.forward;
            //}

            //desiredDirection = (transform.rotation * firstBoneInitialRotationQ ) * desiredDirection;
            //desiredDirection = (transform.rotation * firstBoneInitialRotationQ) * desiredDirection;

            //collisionOffsets[index] = (Quaternion.Inverse(transform.rotation) * firstBoneInitialRotationQ) * desiredDirection;

            #endregion

            proceduralPoints[index].collisionOffsets = desiredDirection;
            proceduralPoints[index].collisionFlags = Mathf.Min(1f, proceduralPoints[index].collisionFlags + deltaTime * 15f);
        }


        /// <summary>
        /// Refreshing colliders data for included colliders
        /// </summary>
        public void RefreshCollidersDataList()
        {
            if (IncludedColliders.Count != IncludedCollidersData.Count || forceRefreshCollidersData)
            {
                IncludedCollidersData.Clear();

                for (int i = IncludedColliders.Count - 1; i >= 0; i--)
                {
                    if (IncludedColliders[i] == null)
                    {
                        IncludedColliders.RemoveAt(i);
                        continue;
                    }

                    FImp_ColliderData_Base colData = FImp_ColliderData_Base.GetColliderDataFor(IncludedColliders[i]);
                    IncludedCollidersData.Add(colData);
                }

                forceRefreshCollidersData = false;
            }
        }


        public void PushIfSegmentInsideCollider(FTail_Point tailPoint, ref Vector3 targetPoint)
        {
            if (DetailedCollision)
            {
                for (int i = 0; i < CollidersDataToCheck.Count; i++)
                    CollidersDataToCheck[i].PushIfInside(ref targetPoint, tailPoint.GetRadiusScaled(), Vector3.zero);
            }
            else
            {
                for (int i = 0; i < CollidersDataToCheck.Count; i++)
                {
                    if (CollidersDataToCheck[i].PushIfInside(ref targetPoint, tailPoint.GetRadiusScaled(), Vector3.zero)) return;
                }
            }
        }

        /// <summary>
        /// Calculating automatically scale for colliders on tail, which will be automatically assigned after initialization
        /// </summary>
        protected float GetColliderSphereRadiusFor(int i)
        {
            FTail_Point tailPoint = proceduralPoints[i];
            float refDistance = 1f;
            if (TailTransforms.Count > 1) refDistance = Vector3.Distance(TailTransforms[1].position, TailTransforms[0].position);

            float singleScale = Mathf.Lerp(refDistance, tailPoint.InitBoneLength * 0.5f, DifferenceScaleFactor);
            float div = TailTransforms.Count - 1;
            if (div <= 0f) div = 1f;
            float step = 1f / div;

            return 0.5f * singleScale * CollidersScaleMul * CollidersScale.Evaluate(step * (float)i);
        }

        /// <summary>
        /// Calculating automatically scale for colliders on tail, which will be automatically assigned after initialization
        /// </summary>
        protected float GetColliderSphereRadiusFor(List<Transform> transforms, int i)
        {
            float refDistance = 1f;
            if (transforms.Count > 1) refDistance = Vector3.Distance(transforms[1].position, transforms[0].position);

            float singleScale = Mathf.Lerp(refDistance, Vector3.Distance(transforms[i - 1].position, transforms[i].position) * 0.5f, DifferenceScaleFactor);
            float step = 1f / (float)(transforms.Count - 1);

            return 0.5f * singleScale * CollidersScaleMul * CollidersScale.Evaluate(step * (float)i);
        }


        #endregion


        #region Editor Stuff

        // V1.2.0
#if UNITY_EDITOR

        // Set it to false if you don't want any gizmo
        public static bool drawMainGizmo = true;
        public bool drawGizmos = false;


        /// <summary>
        /// Getting list of transforms in editor mode if we don't defined chain yet
        /// </summary>
        protected List<Transform> GetEditorGizmoTailList()
        {
            editorGizmoTailList = new List<Transform>();

            if (TailTransforms != null && TailTransforms.Count > 1)
            {
                editorGizmoTailList = TailTransforms;
            }
            else
            {
                Transform lastParent = transform;
                bool boneDefined = true;

                if (TailTransforms == null || TailTransforms.Count == 0)
                {
                    boneDefined = false;
                    lastParent = transform;
                }
                else lastParent = TailTransforms[0];

                Transform rootTransform = lastParent;

                for (int i = editorGizmoTailList.Count; i < 100; i++)
                {
                    if (boneDefined)
                        if (lastParent == rootTransform)
                        {
                            if (lastParent == null) break;
                            if (lastParent.childCount == 0) break;
                            lastParent = lastParent.GetChild(0);
                            continue;
                        }

                    editorGizmoTailList.Add(lastParent);

                    if (lastParent.childCount > 0) lastParent = lastParent.GetChild(0); else break;
                }
            }

            return editorGizmoTailList;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            //if (proceduralPoints != null)
            //{
            //    if (CollisionSpace == ECollisionSpace.Selective_Fast)
            //    {
            //        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.4f);
            //        for (int i = 0; i < proceduralPoints.Count; i++)
            //        {
            //            //Gizmos.DrawWireSphere(proceduralPoints[i].Transform.position, FImp_ColliderData_Sphere.CalculateTrueRadiusOfSphereCollider(proceduralPoints[i].Transform, proceduralPoints[i].ColliderRadius));
            //            Gizmos.DrawWireSphere(proceduralPoints[i].Transform.position, proceduralPoints[i].GetRadiusScaled() );
            //        }
            //    }
            //}

            if (!Application.isPlaying)
                if (UseCollision)
                {
                    GetEditorGizmoTailList();

                    Color preCol = Gizmos.color;
                    Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.7f);

                    for (int i = 1; i < editorGizmoTailList.Count; i++)
                    {
                        if (editorGizmoTailList[i] == null) continue;
                        Gizmos.matrix = Matrix4x4.TRS(editorGizmoTailList[i].position, editorGizmoTailList[i].rotation, editorGizmoTailList[i].lossyScale);
                        Gizmos.DrawWireSphere(Vector3.zero, GetColliderSphereRadiusFor(editorGizmoTailList, i));
                    }

                    Gizmos.color = preCol;
                }
        }

        protected virtual void OnDrawGizmos()
        {

            //if (UseCollision)
            //    if (proceduralPoints != null)
            //    {
            //        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.4f);
            //        for (int i = 0; i < proceduralPoints.Count; i++)
            //        {
            //            Gizmos.DrawWireSphere(proceduralPoints[i].Transform.position, proceduralPoints[i].GetRadiusScaled());
            //        }
            //    }


            if (!drawMainGizmo) return;

            Gizmos.DrawIcon(transform.position, "FIMSpace/FTail/SPR_TailAnimatorGizmoIcon.png", true);
        }
#endif
        #endregion
    }
}
