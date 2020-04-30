using FIMSpace.Basics;
using FIMSpace.FEditor;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(FTail_Animator))]
    [CanEditMultipleObjects]
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    public class FTailAnimator_Editor : FTailAnimator_Editor_Base
    {
        protected bool drawWavingParams = false;

        protected SerializedProperty sp_upClock;
        protected SerializedProperty sp_discTransf;

        protected SerializedProperty sp_wavType;
        protected SerializedProperty sp_useWav;
        protected SerializedProperty sp_cosAd;
        protected SerializedProperty sp_wavSp;
        protected SerializedProperty sp_wavRa;
        protected SerializedProperty sp_wavAx;
        protected SerializedProperty sp_tailRotOff;
        protected SerializedProperty sp_altWave;

        protected override void OnEnable()
        {
            base.OnEnable();

            sp_upClock = serializedObject.FindProperty("UpdateClock");
            sp_discTransf = serializedObject.FindProperty("DisconnectTransforms");

            sp_altWave = serializedObject.FindProperty("AlternateWave");
            sp_wavType = serializedObject.FindProperty("WavingType");
            sp_useWav = serializedObject.FindProperty("UseWaving");
            sp_cosAd = serializedObject.FindProperty("CosinusAdd");
            sp_wavSp = serializedObject.FindProperty("WavingSpeed");
            sp_wavRa = serializedObject.FindProperty("WavingRange");
            sp_wavAx = serializedObject.FindProperty("WavingAxis");
            sp_tailRotOff = serializedObject.FindProperty("TailRotationOffset");
        }

        protected override void DrawingStack(FTail_AnimatorBase tail)
        {
            if (drawDefaultInspector)
            {
                GUILayout.Space(5f);
                DrawDefaultInspector();
            }
            else
            {
                serializedObject.Update();

                GUILayout.Space(3f);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                DrawTailList(tail);
                DrawSpeedSliders(tail);

                if (!tail.RootToParent) DrawWavingOptions((FTail_Animator)tail);

                DrawTuningParameters(tail);

                bool canItPhysics = true;
                if (tail.RootToParent) if (tail.TailTransforms.Count == 1) if (!tail.AutoGetWithOne) canItPhysics = false;

                if (canItPhysics) DrawPhysicalOptionsTab(tail);

                EditorGUILayout.EndVertical();

                if (drawGizmoSwitcher) DrawBottomTailBreakLine();

                if (GUI.changed) tail.OnValidate();

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override void DrawSpeedSliders(FTail_AnimatorBase tail)
        {
            GUILayout.BeginVertical(FEditor_Styles.LNavy);
            //EditorGUILayout.HelpBox("Elasticity Behaviour Parameters", MessageType.None);

            string preStr = "►";
            if (drawAnimOptions) preStr = "▼";
            GUILayout.BeginHorizontal(FEditor_Styles.Style(new Color32(255, 225, 255, 35)));
            //GUILayout.BeginHorizontal(FEditor_Styles.Style(new Color(0.8f,0.8f,0.8f, 0.45f) ));
            if (GUILayout.Button(preStr + " Animation & Elasticity Parameters", EditorStyles.miniLabel)) drawAnimOptions = !drawAnimOptions;
            GUILayout.EndHorizontal();

            if (drawAnimOptions)
            {
                Color preCol = GUI.color;

                GUIStyle smallStyle = new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Italic };
                GUI.color = new Color(1f, 1f, 1f, 0.7f);


                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("                                       Smooth", smallStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Rapid                  ", smallStyle);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(-8f);

                EditorGUIUtility.labelWidth = 115;
                GUI.color = preCol;
                //GUI.color = new Color(0.93f, 1f, 0.93f, 0.9f);
                EditorGUILayout.PropertyField(sp_posSpeeds);

                EditorGUILayout.PropertyField(sp_rotSpeeds);

                GUILayout.Space(2f);
                EditorGUILayout.EndVertical();


                FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.25f));
                //GUILayout.Space(6f);


                //if ( tail.UseCollision ) if ( tail.LookUpMethod == FTail_AnimatorBase.FELookUpMethod.Parental ) if (tail.Springiness > 0.1f) GUI.color = new Color(0.95f, 0.85f, 0.8f, 0.9f);

                GUI.color = new Color(1f, 1f, 1f, 0.7f);
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("                                       Calm", smallStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Bouncy                  ", smallStyle);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(-8f);
                GUI.color = preCol;
                EditorGUILayout.PropertyField(sp_Springiness);
                GUI.color = new Color(1f, 1f, 1f, 0.7f);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("                                       Stiff", smallStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label("Wavy                  ", smallStyle);
                EditorGUILayout.EndHorizontal();
                GUI.color = preCol;
                GUILayout.Space(-8f);
                EditorGUILayout.EndVertical();

                if (tail.UseCollision) if (tail.LookUpMethod == FTail_AnimatorBase.FELookUpMethod.Parental) if (tail.Sensitivity < 0.485f || tail.Sensitivity > 0.75f) GUI.color = new Color(0.95f, 0.85f, 0.8f, 0.9f);
                EditorGUILayout.PropertyField(sp_Sensitivity);
                GUI.color = preCol;

                FEditor_Styles.DrawUILine(new Color(0.5f, 0.5f, 0.5f, 0.25f));

                if ((tail.Springiness > 0.5f && tail.MaxStretching > 0.3f)) GUI.color = new Color(0.8f, 1f, 0.8f, 0.9f);
                else
                if ((tail.AngleLimit < 90 && tail.MaxStretching > 0.2f)) GUI.color = new Color(1f, 0.8f, 0.8f, 0.9f);

                EditorGUILayout.PropertyField(sp_maxDist);
                GUI.color = preCol;
                //if (tail.AngleLimit >= 180) GUI.color = preCol * new Color(1f, 1f, 1f, 0.6f);
                EditorGUILayout.PropertyField(sp_AngleLimit);
                if (tail.AngleLimit < 90)
                {
                    //EditorGUI.indentLevel++;
                    if (tail.AngleLimitAxis == Vector3.zero) GUI.color = preCol * new Color(1f, 1f, 1f, 0.6f);
                    EditorGUILayout.PropertyField(sp_AngleLimitAxis);
                    GUI.color = preCol;

                    if (tail.AngleLimitAxis != Vector3.zero)
                    {
                        if (tail.LimitAxisRange.x == tail.LimitAxisRange.y) GUI.color = preCol * new Color(1f, 1f, 1f, 0.6f);
                        EditorGUILayout.MinMaxSlider(new GUIContent("Range", "If you want limit axes symmetrically leave this parameter unchanged, if you want limit one direction of axis more than reversed, tweak this parameter"),
                            ref tail.LimitAxisRange.x, ref tail.LimitAxisRange.y, -90f, 90f);
                        //EditorGUILayout.PropertyField(sp_AngleLimitAxisTo);
                        GUI.color = preCol;
                    }

                    EditorGUILayout.PropertyField(sp_LimitSmoothing);

                    //EditorGUI.indentLevel--;
                }

                GUI.color = preCol;

                EditorGUILayout.PropertyField(sp_MotionInfluence);

                EditorGUIUtility.labelWidth = 0;
                GUILayout.Space(5f);

                // V1.2
                FTail_Animator tailSimple = tail as FTail_Animator;
                FTail_AnimatorBlending tailBlending = tail as FTail_AnimatorBlending;
                if (!tailBlending) if (tailSimple != null)
                    {
                        EditorGUILayout.BeginVertical(FEditor_Styles.GrayBackground);

                        //GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
                        //if (!Application.isPlaying)
                        //    EditorGUILayout.HelpBox("Use late update order for animated objects", MessageType.Info);
                        //else
                        //EditorGUILayout.HelpBox("  Use late update order for animated objects", MessageType.None);

                        //GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);

                        EditorGUIUtility.labelWidth = 97;
                        EditorGUILayout.PropertyField(sp_upClock, new GUIContent("Update Order"));
                        EditorGUIUtility.labelWidth = 0;

                        if (!Application.isPlaying)
                            if (tail.UpdateClock != EFUpdateClock.LateUpdate)
                            {
                                GUILayout.FlexibleSpace();
                                GUI.color = new Color(1f, 1f, 1f, 0.7f);
                                float width = (float)typeof(EditorGUIUtility).GetProperty("contextWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null, null);

                                if (width > 375)
                                    GUILayout.Label(new GUIContent("Use LateUpdate for animated objects", "Use LateUpdate order for animated objects (animated by unity Animator or Animation components) or use FTail_AnimatorBlending component instead of FTail_Animator"), smallStyle);
                                else if (width > 310)
                                    GUILayout.Label(new GUIContent("Use LateUpdate for...", "Use LateUpdate order for animated objects (animated by unity Animator or Animation components) or use FTail_AnimatorBlending component instead of FTail_Animator"), smallStyle);
                                else
                                    GUILayout.Label(new GUIContent("Put Cursor Here", "(Tooltip) Use LateUpdate order for animated objects (animated by unity Animator or Animation components) or use FTail_AnimatorBlending component instead of FTail_Animator"), smallStyle);

                                GUI.color = preCol;
                            }

                        GUILayout.EndHorizontal();

                        if (!Application.isPlaying)
                        {
                            EditorGUIUtility.labelWidth = 147;
                            EditorGUILayout.PropertyField(sp_queue);
                        }
                        EditorGUIUtility.labelWidth = 0;

                        EditorGUILayout.EndVertical();
                    }

                GUILayout.Space(1f);

                EditorGUIUtility.labelWidth = 0;
            }

            GUILayout.EndVertical();
        }

        protected virtual void DrawWavingOptions(FTail_Animator tail)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical(FEditor_Styles.Style(new Color(0.5f, 0.2f, 0.9f, 0.1f)));

            GUILayout.BeginHorizontal(FEditor_Styles.LBlueBackground);
            drawWavingParams = EditorGUILayout.Foldout(drawWavingParams, "Waving Options", true);

            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 90;

            if (!tail.RootToParent)
                EditorGUILayout.PropertyField(sp_useWav);

            GUILayout.EndHorizontal();

            if (!tail.RootToParent)
                if (drawWavingParams)
                {
                    EditorGUIUtility.labelWidth = 165;

                    EditorGUILayout.PropertyField(sp_wavType);

                    if (tail.WavingType == FTail_Animator.FEWavingType.Simple)
                    {
                        GUILayout.Space(5f);
                        EditorGUILayout.PropertyField(sp_cosAd);
                        GUILayout.Space(5f);
                    }
                    else
                    {
                        GUILayout.Space(5f);
                        EditorGUILayout.PropertyField(sp_altWave);
                        GUILayout.Space(5f);
                    }

                    EditorGUILayout.PropertyField(sp_wavSp);
                    EditorGUILayout.PropertyField(sp_wavRa);
                    GUILayout.Space(5f);


                    Color preCol = GUI.color;

                    if (tail.WavingType == FTail_Animator.FEWavingType.Advanced)
                        if (tail.WavingAxis.x == 0f || tail.WavingAxis.y == 0f || tail.WavingAxis.z == 0f)
                        {
                            EditorGUILayout.HelpBox("With advanced waving, try use all axes", MessageType.None);
                            GUI.color = new Color(1f, 1f, 0.8f, 0.95f);
                        }

                    EditorGUILayout.PropertyField(sp_wavAx);

                    GUI.color = preCol;


                    EditorGUILayout.PropertyField(sp_tailRotOff);
                }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUIUtility.labelWidth = 0;
        }

        protected override void DrawInAdvTweaking()
        {
            base.DrawInAdvTweaking();

            if (!Application.isPlaying)
            {
                EditorGUIUtility.labelWidth = 174;
                EditorGUILayout.PropertyField(sp_discTransf);
            }
        }
    }
}