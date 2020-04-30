using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(FTail_AnimatorBase))]
    [CanEditMultipleObjects]
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    public class FTailAnimator_Editor_Base : Editor
    {
        protected static bool drawGizmoSwitcher = true;

        protected static bool drawDefaultInspector = false;
        protected static bool drawTailBones = false;
        protected static bool drawTuningParams = false;
        protected static bool drawPhysicalParams = false;
        protected static bool drawExtraParameters = false;
        protected static bool drawFromTo = false;
        protected static bool drawAutoFixOption = true;
        protected static bool drawAnimOptions = true;
        protected Texture2D breakLineTail = null;
        protected bool wasRoot = false;

        protected SerializedProperty sp_posSpeeds;
        protected SerializedProperty sp_rotSpeeds;
        protected SerializedProperty sp_maxDist;
        protected SerializedProperty sp_Springiness;
        protected SerializedProperty sp_Sensitivity;
        protected SerializedProperty sp_useAutoCorr;

        protected SerializedProperty sp_addRefs;
        protected SerializedProperty sp_stretch;
        protected SerializedProperty sp_fullCorrect;
        protected SerializedProperty sp_rollBones;
        protected SerializedProperty sp_orientRef;
        protected SerializedProperty sp_axisCorr;
        protected SerializedProperty sp_axisBack;
        protected SerializedProperty sp_extraCorr;
        protected SerializedProperty sp_fromdir;
        protected SerializedProperty sp_todir;
        protected SerializedProperty sp_animate;
        protected SerializedProperty sp_animateRoot;
        protected SerializedProperty sp_refr;
        protected SerializedProperty sp_smoothdelta;
        protected SerializedProperty sp_queue;
        protected SerializedProperty sp_rootp;
        protected SerializedProperty sp_autoone;
        protected SerializedProperty sp_useCollision;
        protected SerializedProperty sp_CollisionMethod;
        protected SerializedProperty sp_curving;
        protected SerializedProperty sp_gravity;
        protected SerializedProperty sp_colScale;
        protected SerializedProperty sp_colScaleMul;
        protected SerializedProperty sp_colBoxDim;
        protected SerializedProperty sp_colDiffFact;
        protected SerializedProperty sp_colWithOther;
        protected SerializedProperty sp_colIgnored;
        protected SerializedProperty sp_colSameLayer;
        protected SerializedProperty sp_colCustomLayer;
        protected SerializedProperty sp_colAddRigs;
        protected SerializedProperty sp_RootRotationOffset;
        protected SerializedProperty sp_RootPositionOffset;
        protected SerializedProperty sp_CollisionSpace;
        protected SerializedProperty sp_CollisionSwapping;
        protected SerializedProperty sp_DetailedCollision;
        protected SerializedProperty sp_IncludedColliders;
        protected SerializedProperty sp_RigidbodyMass;

        protected SerializedProperty sp_AngleLimit;
        protected SerializedProperty sp_StiffTailEnd;
        protected SerializedProperty sp_AngleLimitAxis;
        protected SerializedProperty sp_AngleLimitAxisTo;
        protected SerializedProperty sp_LimitSmoothing;
        protected SerializedProperty sp_MotionInfluence;

        

        //protected SerializedProperty sp_GravAlong;

        protected virtual void OnEnable()
        {
            sp_posSpeeds = serializedObject.FindProperty("PositionSpeed");
            sp_rotSpeeds = serializedObject.FindProperty("RotationSpeed");
            sp_maxDist = serializedObject.FindProperty("MaxStretching");
            sp_Springiness = serializedObject.FindProperty("Springiness");
            sp_Sensitivity = serializedObject.FindProperty("Sensitivity");
            sp_useAutoCorr = serializedObject.FindProperty("UseAutoCorrectLookAxis");

            sp_addRefs = serializedObject.FindProperty("AddTailReferences");
            sp_stretch = serializedObject.FindProperty("LengthMultiplier");
            sp_fullCorrect = serializedObject.FindProperty("FullCorrection");
            sp_rollBones = serializedObject.FindProperty("LookUpMethod");
            sp_orientRef = serializedObject.FindProperty("OrientationReference");
            sp_axisCorr = serializedObject.FindProperty("AxisCorrection");
            sp_axisBack = serializedObject.FindProperty("AxisLookBack");
            sp_extraCorr = serializedObject.FindProperty("ExtraCorrectionOptions");
            sp_fromdir = serializedObject.FindProperty("ExtraFromDirection");
            sp_todir = serializedObject.FindProperty("ExtraToDirection");
            sp_animate = serializedObject.FindProperty("AnimateCorrections");
            sp_animateRoot = serializedObject.FindProperty("AnimateRoot");
            sp_refr = serializedObject.FindProperty("RefreshHelpers");
            sp_smoothdelta = serializedObject.FindProperty("SafeDeltaTime");
            sp_queue = serializedObject.FindProperty("QueueToLastUpdate");
            sp_rootp = serializedObject.FindProperty("RootToParent");
            sp_autoone = serializedObject.FindProperty("AutoGetWithOne");
            sp_useCollision = serializedObject.FindProperty("UseCollision");
            sp_CollisionMethod = serializedObject.FindProperty("CollisionMethod");
            sp_curving = serializedObject.FindProperty("Curving");
            sp_gravity = serializedObject.FindProperty("Gravity");
            sp_colScale = serializedObject.FindProperty("CollidersScale");
            sp_colScaleMul = serializedObject.FindProperty("CollidersScaleMul");
            sp_colBoxDim = serializedObject.FindProperty("BoxesDimensionsMul");
            sp_colDiffFact = serializedObject.FindProperty("DifferenceScaleFactor");
            sp_colWithOther = serializedObject.FindProperty("CollideWithOtherTails");
            sp_RigidbodyMass = serializedObject.FindProperty("RigidbodyMass");


            sp_colIgnored = serializedObject.FindProperty("IgnoredColliders");
            sp_colSameLayer = serializedObject.FindProperty("CollidersSameLayer");
            sp_colCustomLayer = serializedObject.FindProperty("CollidersLayer");
            sp_colAddRigs = serializedObject.FindProperty("CollidersAddRigidbody");
            sp_RootRotationOffset = serializedObject.FindProperty("RootRotationOffset");
            sp_RootPositionOffset = serializedObject.FindProperty("RootPositionOffset");
            sp_CollisionSpace = serializedObject.FindProperty("CollisionSpace");
            sp_CollisionSwapping = serializedObject.FindProperty("CollisionSwapping");
            sp_IncludedColliders = serializedObject.FindProperty("IncludedColliders");
            sp_DetailedCollision = serializedObject.FindProperty("DetailedCollision");
            sp_IncludedColliders = serializedObject.FindProperty("IncludedColliders");

            sp_AngleLimit = serializedObject.FindProperty("AngleLimit");
            sp_StiffTailEnd = serializedObject.FindProperty("StiffTailEnd");
            sp_AngleLimitAxis = serializedObject.FindProperty("AngleLimitAxis");
            sp_AngleLimitAxisTo = serializedObject.FindProperty("LimitAxisRange");
            sp_LimitSmoothing = serializedObject.FindProperty("LimitSmoothing");
            sp_MotionInfluence = serializedObject.FindProperty("MotionInfluence");

            //sp_GravAlong = serializedObject.FindProperty("GravityAlongSegments");
        }

        public override void OnInspectorGUI()
        {
            // Update component from last changes
            serializedObject.Update();

            FTail_AnimatorBase tailComp = (FTail_AnimatorBase)target;

            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);
            EditorGUILayout.BeginHorizontal();
            drawDefaultInspector = GUILayout.Toggle(drawDefaultInspector, "Default inspector");

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 80;
            drawGizmoSwitcher = GUILayout.Toggle(drawGizmoSwitcher, "View Gizmo Switch");

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            DrawingStack(tailComp);

            // Apply changed parameters variables
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawingStack(FTail_AnimatorBase tail)
        {
            if (drawDefaultInspector)
            {
                GUILayout.Space(5f);
                DrawDefaultInspector();
            }
            else
            {
                serializedObject.Update();
                Undo.RecordObject(tail, "TailAnimator Parameters");

                GUILayout.Space(3f);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                DrawTailList(tail);

                DrawSpeedSliders(tail);

                DrawTuningParameters(tail);

                EditorGUILayout.EndVertical();

                if (drawGizmoSwitcher) DrawBottomTailBreakLine();

                if (GUI.changed) tail.OnValidate();

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected void DrawTailList(FTail_AnimatorBase tail)
        {
            GUILayout.BeginHorizontal(FEditor_Styles.BlueBackground);
            // Long text as tooltip to save space in inspectors
            EditorGUILayout.LabelField(new GUIContent("ENTER HERE FOR INFO TOOLTIP (no in playmode)", "Put under 'Tail Bones' first bone of tail - component will use children transform to get rest tail bones, or left empty, then tail structure will be created starting from this transform, also you can put here for example 3 bones so only this transforms will be animated"));
            GUILayout.EndHorizontal();

            // Extra info for Tail bones array viewer
            string extraInfo = "";
            bool red = false;


            if (tail.TailTransforms == null) tail.TailTransforms = new System.Collections.Generic.List<Transform>();

            if (tail.TailTransforms.Count > 0)
            {
                if (tail.TailTransforms[0] == null)
                {
                    extraInfo = " - NULL BONE!";
                    red = true;
                }
                else
                if (tail.TailTransforms.Count == 1)
                {
                    if (tail.AutoGetWithOne)
                    {
                        if (drawTailBones)
                            extraInfo = "  (1 - Auto Get)";
                        else
                            extraInfo = "  (1 - Auto Child Transforms)";
                    }
                    else
                    {
                        extraInfo = "  (Only one bone)";
                    }
                }
                else
                {
                    bool nullDetected = false;
                    for (int i = 1; i < tail.TailTransforms.Count; i++)
                    {
                        if (tail.TailTransforms[i] == null)
                        {
                            nullDetected = true;
                            break;
                        }
                    }

                    if (nullDetected)
                    {
                        extraInfo = "   (SOME NULLS!)";
                        red = true;
                    }
                }
            }
            else
            {
                if (drawTailBones)
                    extraInfo = "  (No Bone - Auto Get)";
                else
                    extraInfo = "  (No Bone - Auto Get)";
            }

            if (extraInfo == "") extraInfo = " (" + tail.TailTransforms.Count + ")";


            if (red)
                GUILayout.BeginVertical(FEditor_Styles.RedBackground);
            else
                GUILayout.BeginVertical(FEditor_Styles.GrayBackground);


            GUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;


            drawTailBones = EditorGUILayout.Foldout(drawTailBones, "Tail Bones" + extraInfo, true);

            if (tail.TailTransforms.Count == 1)
                EditorGUILayout.PropertyField(sp_autoone, new GUIContent("", "When you want to use auto get when you assigning one bone inside inspector window (Not working with waving - then you have to rotate parent transform in your own way to get same effect)"), new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(18) });

            if (drawTailBones)
            {
                DrawAddTailButtons(tail);

                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                for (int i = 0; i < tail.TailTransforms.Count; i++)
                {
                    tail.TailTransforms[i] = (Transform)EditorGUILayout.ObjectField("Tail Bone [" + i + "]", tail.TailTransforms[i], typeof(Transform), true);
                }

                EditorUtility.SetDirty(target);

                EditorGUI.indentLevel--;
            }
            else
            {
                if (tail.TailTransforms.Count == 0) DrawAddTailButtons(tail);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(FEditor_Styles.GrayBackground);

            EditorGUIUtility.labelWidth = 155;
            EditorGUILayout.PropertyField(sp_rootp);
            EditorGUIUtility.labelWidth = 0;
            GUILayout.EndVertical();

            if (tail.RootToParent != wasRoot)
                if (tail.RootToParent)
                {
                    tail.LookUpMethod = FTail_AnimatorBase.FELookUpMethod.Parental;

                    foreach (var s in Selection.gameObjects)
                    {
                        if (s == tail.gameObject) continue;
                        FTail_Animator b = s.GetComponent<FTail_Animator>();
                        if (b)
                        {
                            bool enabledIs = false;
                            if (b.RootToParent) if (b.LookUpMethod == FTail_AnimatorBase.FELookUpMethod.Default) enabledIs = true;
                            if (enabledIs) b.LookUpMethod = FTail_AnimatorBase.FELookUpMethod.Parental;
                        }
                    }
                }

            wasRoot = tail.RootToParent;

            GUILayout.Space(1f);

            EditorGUI.indentLevel--;

            EditorGUIUtility.labelWidth = 0;
        }

        protected void DrawAddTailButtons(FTail_AnimatorBase tail)
        {
            // V1.2.2
            if (GUILayout.Button("Auto", new GUILayoutOption[2] { GUILayout.MaxWidth(48), GUILayout.MaxHeight(14) }))
            {
                OnClickedAuto();
            }

            if (GUILayout.Button("+", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
            {
                if (tail.TailTransforms.Count == 0) drawTailBones = true;

                tail.TailTransforms.Add(null);
                EditorUtility.SetDirty(target);
                OnTailTransformsCountChange();
            }

            if (GUILayout.Button("-", new GUILayoutOption[2] { GUILayout.MaxWidth(28), GUILayout.MaxHeight(14) }))
            {
                if (tail.TailTransforms.Count > 0)
                {
                    tail.TailTransforms.RemoveAt(tail.TailTransforms.Count - 1);
                    EditorUtility.SetDirty(target);
                    OnTailTransformsCountChange();
                }
            }
        }

        protected virtual void DrawSpeedSliders(FTail_AnimatorBase tail)
        {
            //EditorGUILayout.HelpBox("Elasticity Behaviour Parameters", MessageType.None);

            string preStr = "►";
            if (drawAnimOptions) preStr = "▼";
            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            if (GUILayout.Button(preStr + " Aniamtion & Elasticity Parameters", EditorStyles.miniLabel)) drawAnimOptions = !drawAnimOptions;
            GUILayout.EndHorizontal();

            if (drawAnimOptions)
            {
                EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

                //EditorGUILayout.Slider(sp_posSpeeds, 0f, 60f);
                //EditorGUILayout.Slider(sp_rotSpeeds, 0f, 60f);
                EditorGUILayout.PropertyField(sp_posSpeeds);
                EditorGUILayout.PropertyField(sp_rotSpeeds);
                EditorGUILayout.PropertyField(sp_maxDist);
                EditorGUILayout.PropertyField(sp_Springiness);

                EditorGUILayout.EndVertical();

                if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_queue);

                GUILayout.Space(1f);

                EditorGUIUtility.labelWidth = 0;
            }
        }

        protected virtual void DrawTuningParameters(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 130;

            EditorGUILayout.BeginVertical(FEditor_Styles.LBlueBackground);

            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            drawTuningParams = EditorGUILayout.Foldout(drawTuningParams, "Tuning Parameters", true);
            GUILayout.EndHorizontal();

            if (drawTuningParams)
            {
                GUILayout.Space(8f);
                DrawTuningParametersGUI(tail);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }



        protected virtual void DrawTuningParametersGUI(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.PropertyField(sp_stretch);

            if (tail.FullCorrection && tail.AnimateCorrections) EditorGUILayout.PropertyField(sp_refr);
            FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

            //if (drawAutoFixOption)
            {
                EditorGUIUtility.labelWidth = 170;
                //EditorGUILayout.LabelField(new GUIContent("Full Correction - previously 'Auto go through all bones'"));
                EditorGUILayout.PropertyField(sp_fullCorrect, new GUIContent(new GUIContent("Full Correction", "If automatic orientation fix should be calculated for each bones separately (previously named 'Auto go through all bones')")));
            }

            if (tail.FullCorrection)
            {
                //if (!tail.RolledBones)
                EditorGUILayout.PropertyField(sp_animate, new GUIContent("Animate Corrections", "When you want corrections to match animation in realtime"));

                if (tail.AnimateCorrections)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sp_animateRoot);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(sp_rollBones, new GUIContent("LookUp Method", "Use this option when your model is rolling strangely when waving or other stuff"));
                if (tail.LookUpMethod == FTail_AnimatorBase.FELookUpMethod.CrossUp)
                {
                    if (Application.isPlaying) GUI.enabled = false;
                    EditorGUIUtility.labelWidth = 190;
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(sp_orientRef);
                    EditorGUI.indentLevel--;
                    EditorGUIUtility.labelWidth = 170;
                    if (Application.isPlaying) GUI.enabled = true;
                }
            }

            if (tail.UpdateClock != Basics.EFUpdateClock.FixedUpdate)
            {
                FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));
                EditorGUILayout.PropertyField(sp_smoothdelta);
            }

            if (tail.RootToParent)
            {
                EditorGUIUtility.labelWidth = 140;
                EditorGUILayout.PropertyField(sp_RootPositionOffset);
                EditorGUILayout.PropertyField(sp_RootRotationOffset);
            }

            FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

            // More Advanced Parameters tab
            GUILayout.BeginVertical(FEditor_Styles.BlueBackground);

            GUILayout.BeginHorizontal();
            drawExtraParameters = EditorGUILayout.Foldout(drawExtraParameters, new GUIContent("Advanced Parameters"), true);

            if (drawAutoFixOption)
            {
                if (!Application.isPlaying)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUIUtility.labelWidth = 80;
                    EditorGUILayout.PropertyField(sp_useAutoCorr, new GUIContent("Automatic", ""));
                }
            }

            GUILayout.EndHorizontal();


            if (drawExtraParameters)
            {
                if (tail.LookUpMethod == FTail_AnimatorBase.FELookUpMethod.Default)
                {
                    EditorGUIUtility.labelWidth = 135;
                    if (!tail.FullCorrection)
                        EditorGUILayout.PropertyField(sp_axisCorr, new GUIContent("Axis Correction", "[Advanced] Bones wrong rotations axis corrector"));
                    EditorGUILayout.PropertyField(sp_axisBack, new GUIContent("Axis LookBack", "[Advanced] Look rotation transform direction reference"));
                    EditorGUIUtility.labelWidth = 0;
                }

                GUILayout.Space(6f);

                GUILayout.BeginVertical(FEditor_Styles.LGrayBackground);
                GUILayout.BeginHorizontal();
                drawFromTo = EditorGUILayout.Foldout(drawFromTo, new GUIContent("Additional FromTo", "Click on toggle to enable using this option"), true);

                GUILayout.FlexibleSpace();
                bool preExtr = tail.ExtraCorrectionOptions;

                EditorGUILayout.PropertyField(sp_extraCorr, new GUIContent(""), GUILayout.MaxWidth(45f));
                if (preExtr != tail.ExtraCorrectionOptions) if (tail.ExtraCorrectionOptions) drawFromTo = true; else drawFromTo = false;
                GUILayout.EndHorizontal();

                if (drawFromTo)
                    if (!tail.FullCorrection)
                    {
                        EditorGUIUtility.labelWidth = 117;
                        EditorGUILayout.PropertyField(sp_fromdir, new GUIContent("From Axis", "From rotation transforming. Extra repair parameters for rotating tail in unusual axes space."));
                        EditorGUILayout.PropertyField(sp_todir, new GUIContent("To Axis", ""));
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(sp_todir, new GUIContent("To Axis", ""));
                    }

                GUILayout.EndVertical();
                GUILayout.EndVertical();

                DrawInAdvTweaking();

                //EditorGUI.indentLevel--;
            }
            else
                GUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void DrawPhysicalOptionsTab(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 130;

            EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.9f, 0.5f, 0.2f, 0.15f)));

            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal(FEditor_Styles.LGrayBackground);
            drawPhysicalParams = EditorGUILayout.Foldout(drawPhysicalParams, "Physical & Experimental", true);
            GUILayout.EndHorizontal();

            if (drawPhysicalParams)
            {
                GUILayout.Space(8f);
                DrawPhysicalParametersGUI(tail);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }


        protected virtual void DrawPhysicalParametersGUI(FTail_AnimatorBase tail)
        {
            EditorGUIUtility.labelWidth = 140;

            if (tail.UseCollision) EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

            EditorGUILayout.PropertyField(sp_useCollision);

            if (!Application.isPlaying)
            {
                if (tail.UseCollision)
                {
                    EditorGUILayout.PropertyField(sp_CollisionSpace);


                    if (tail.CollisionSpace == FTail_AnimatorBase.ECollisionSpace.World_Slow)
                    {
                        EditorGUILayout.HelpBox("Collision support is experimental and not working fully correct yet. When entering playmode colliders will be generated as in editor preview", MessageType.None);
                        EditorGUILayout.HelpBox("Tail should have assigned layer which is not colliding with itself", MessageType.Info);
                        FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));
                        EditorGUILayout.PropertyField(sp_CollisionMethod);
                    }
                    else
                        EditorGUILayout.HelpBox("Collision support is experimental and not working fully correct yet.", MessageType.Info);


                    if (tail.CollisionSpace == FTail_AnimatorBase.ECollisionSpace.World_Slow)
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.Space(2f);
                        EditorGUIUtility.labelWidth = 190;
                        EditorGUILayout.PropertyField(sp_colWithOther);
                        EditorGUIUtility.labelWidth = 140;
                        EditorGUILayout.PropertyField(sp_colAddRigs, new GUIContent("Add Rigidbodies", "If you add rigidbodies to each tail segment's collider, collision will work on everything but it will be less optimal, you don't have to add here rigidbodies but then you must have not kinematic rigidbodies on objects segments can collide"));
                        if (tail.CollidersAddRigidbody)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUIUtility.labelWidth = 152;
                            EditorGUILayout.PropertyField(sp_RigidbodyMass);
                            EditorGUI.indentLevel--;
                        }
                        GUILayout.Space(4f);

                        EditorGUIUtility.labelWidth = 180;
                        EditorGUILayout.PropertyField(sp_colSameLayer);

                        if (!tail.CollidersSameLayer)
                        {
                            EditorGUI.indentLevel++;
                            EditorGUIUtility.labelWidth = 140;
                            EditorGUILayout.PropertyField(sp_colCustomLayer);
                            EditorGUI.indentLevel--;
                        }

                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.PropertyField(sp_colIgnored, true);

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                        Color c = GUI.color;
                        GUILayout.BeginVertical();
                        if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                        if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), EditorStyles.toolbarButton)) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                        GUI.color = c;
                        GUILayout.EndVertical();

                        EditorGUILayout.PropertyField(sp_IncludedColliders, true);
                        EditorGUILayout.EndVertical();
                        // EditorGUI.indentLevel--;
                    }


                    FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

                    if (tail.CollisionMethod == FTail_AnimatorBase.ECollisionMethod.RotationOffset_Old)
                    {
                        EditorGUILayout.HelpBox("'RotationOffset method required bigger collider size than it's mesh", MessageType.None);
                    }

                    EditorGUILayout.PropertyField(sp_colScaleMul, new GUIContent("Scale Multiplier"));
                    EditorGUILayout.PropertyField(sp_colScale, new GUIContent("Scale Curve"));
                    EditorGUILayout.PropertyField(sp_colDiffFact, new GUIContent("Auto Curve"));
                }
            }
            else // In Playmode
            {
                if (tail.UseCollision)
                {
                    if (tail.CollisionSpace == FTail_AnimatorBase.ECollisionSpace.World_Slow)
                    {
                        EditorGUILayout.PropertyField(sp_CollisionMethod);
                        FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

                        EditorGUI.indentLevel++;
                        EditorGUIUtility.labelWidth = 190;
                        EditorGUILayout.PropertyField(sp_colWithOther);
                        GUILayout.Space(4f);
                        EditorGUIUtility.labelWidth = 0;
                        //EditorGUILayout.PropertyField(sp_colIgnored, true);
                        EditorGUI.indentLevel--;
                        GUILayout.Space(3f);
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical(FEditor_Styles.Emerald);

                        Color c = GUI.color;
                        GUILayout.BeginVertical();
                        if (ActiveEditorTracker.sharedTracker.isLocked) GUI.color = new Color(0.44f, 0.44f, 0.44f, 0.8f); else GUI.color = new Color(0.95f, 0.95f, 0.99f, 0.9f);
                        if (GUILayout.Button(new GUIContent("Lock Inspector for Drag & Drop Colliders", "Drag & drop colliders to 'Included Colliders' List from the hierarchy"), EditorStyles.toolbarButton)) ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                        GUI.color = c;
                        GUILayout.EndVertical();

                        EditorGUILayout.PropertyField(sp_IncludedColliders, true);
                        EditorGUILayout.EndVertical();
                        // EditorGUI.indentLevel--;

                        EditorGUILayout.HelpBox("Rescalling in playmode available only in editor not in build", MessageType.Warning);
                        EditorGUILayout.PropertyField(sp_colScaleMul, new GUIContent("Scale Multiplier"));
                        EditorGUILayout.PropertyField(sp_colScale, new GUIContent("Scale Curve"));
                        EditorGUILayout.PropertyField(sp_colDiffFact, new GUIContent("Auto Curve"));
                        GUILayout.Space(3f);
                    }
                }
            }

            if (tail.UseCollision)
            {
                FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));

                //if (tail.LookUpMethod == FTail_AnimatorBase.FELookUpMethod.Parental && tail.FullCorrection)
                //{
                //    GUI.enabled = false;
                //    tail.CollisionSwapping = 0.5f;
                    EditorGUILayout.HelpBox("Swapping is not available with 'Parental' correction algorithm", MessageType.None);
                //}

                EditorGUILayout.PropertyField(sp_CollisionSwapping);
                GUILayout.Space(3f);

                if (GUI.enabled == false) GUI.enabled = true;

                if (tail.CollisionSpace == FTail_AnimatorBase.ECollisionSpace.Selective_Fast)
                {
                    EditorGUILayout.PropertyField(sp_DetailedCollision);
                    GUILayout.Space(3f);
                }

                EditorGUILayout.EndVertical();
            }


            GUILayout.Space(5f);

            EditorGUIUtility.labelWidth = 140;
            EditorGUILayout.PropertyField(sp_curving);
            EditorGUILayout.PropertyField(sp_gravity);
            EditorGUILayout.PropertyField(sp_StiffTailEnd);
            //EditorGUILayout.PropertyField(sp_GravAlong);

            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void OnClickedAuto()
        {
            FTail_AnimatorBase tail = target as FTail_AnimatorBase;

            if (tail.TailTransforms.Count <= 1)
            {
                tail.AutoGetTailTransforms(true);
            }
            else
            {
                if (tail.TailTransforms[0] == null)
                    tail.AutoGetTailTransforms(true);
                else
                {
                    bool isnull = false;

                    for (int i = 0; i < tail.TailTransforms.Count; i++)
                        if (tail.TailTransforms[i] == null)
                        {
                            isnull = true;
                            break;
                        }

                    if (isnull)
                    {
                        for (int i = 1; i < tail.TailTransforms.Count; i++)
                        {
                            if (tail.TailTransforms[i - 1].childCount == 0) break;
                            tail.TailTransforms[i] = tail.TailTransforms[i - 1].GetChild(0);
                        }
                    }
                    else
                    {
                        Transform first = tail.TailTransforms[0];
                        tail.TailTransforms.Clear();
                        tail.TailTransforms.Add(first);
                        tail.AutoGetTailTransforms(true);
                    }
                }
            }
        }

        protected virtual void DrawInAdvTweaking()
        {
            FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.6f));
            EditorGUIUtility.labelWidth = 155;
            if (!Application.isPlaying) EditorGUILayout.PropertyField(sp_addRefs);
            EditorGUIUtility.labelWidth = 0;
        }

        protected virtual void OnTailTransformsCountChange()
        {

        }

        protected void DrawBottomTailBreakLine()
        {
            if (breakLineTail == null) breakLineTail = Resources.Load("FTail_BreakLineTail", typeof(Texture2D)) as Texture2D;
            Rect rect = GUILayoutUtility.GetRect(128f, breakLineTail.height * 1f);
            GUILayout.BeginHorizontal();
            GUI.DrawTexture(rect, breakLineTail, ScaleMode.StretchToFill, true, 1f);
            FTail_AnimatorBase tail = (FTail_AnimatorBase)target;
            tail.drawGizmos = GUILayout.Toggle(tail.drawGizmos, new GUIContent("", "Toggle to switch drawing gizmos"));
            GUILayout.EndHorizontal();
        }
    }
}
