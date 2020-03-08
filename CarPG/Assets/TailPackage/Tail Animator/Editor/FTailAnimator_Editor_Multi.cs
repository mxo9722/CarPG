using FIMSpace.FEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FIMSpace.FTail
{
    [CustomEditor(typeof(FTail_AnimatorBlending))]
    [CanEditMultipleObjects]
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    public class FTailAnimator_Editor_Blending : FTailAnimator_Editor
    {
        protected SerializedProperty sp_blendOrig;
        protected SerializedProperty sp_blendChain;
        protected SerializedProperty sp_posNotAnim;
        protected SerializedProperty sp_rotNotAnim;
        protected SerializedProperty sp_rotsSelective;

        private bool wasCorr = false;
        private bool wasAnimCorr = false;
        private bool wasRefresh = false;
        private bool drawAnimatorOptions = true;


        protected override void OnEnable()
        {
            base.OnEnable();

            sp_blendOrig = serializedObject.FindProperty("BlendToOriginal");
            sp_blendChain = serializedObject.FindProperty("BlendChainValue");
            sp_posNotAnim = serializedObject.FindProperty("PositionsNotAnimated");
            sp_rotNotAnim = serializedObject.FindProperty("RotationsNotAnimated");
            sp_rotsSelective = serializedObject.FindProperty("SelectiveRotsNotAnimated");
        }


        protected override void DrawSpeedSliders(FTail_AnimatorBase tail)
        {
            base.DrawSpeedSliders(tail);

            EditorGUILayout.BeginVertical(FEditor_Styles.GreenBackground);

            bool connected = false;

            if (!Application.isPlaying)
            {
                if (tail.FullCorrection && tail.AnimateCorrections && tail.RefreshHelpers) connected = true;
            }
            else
                if (tail.FullCorrection && tail.AnimateCorrections) connected = true;

            string preStr = "►";
            if ( drawAnimatorOptions ) preStr = "▼";
            GUILayout.BeginHorizontal(FEditor_Styles.Style(new Color32(255, 225, 255, 35)));
            if (GUILayout.Button(preStr + " Blending and Animator Help Parameters", EditorStyles.miniLabel)) drawAnimatorOptions = !drawAnimatorOptions;
            GUILayout.EndHorizontal();

            if (drawAnimatorOptions)
            {
                Color preCol = GUI.color;
                if (connected) GUI.color = new Color(0.1f, 1f, 0.325f, 0.9f);

                if (GUILayout.Button(new GUIContent("Connect with animator", "This button changing some variables to make component cooperate highly with animator's animation. VARIABLES WHICH ARE CHANGED: RefreshHelpers, FullCorrection, AnimateCorrection"), new GUILayoutOption[1] { GUILayout.MaxHeight(18) }))
                {
                    SwitchConnectWithAnimator(!connected);

                    foreach (var s in Selection.gameObjects)
                    {
                        if (s == tail.gameObject) continue;
                        FTail_AnimatorBlending b = s.GetComponent<FTail_AnimatorBlending>();
                        if (b)
                        {
                            bool enabledIs = false;
                            if (b.FullCorrection && b.AnimateCorrections) enabledIs = true;

                            if (!enabledIs)
                            {
                                b.FullCorrection = true;
                                b.AnimateCorrections = true;
                                b.RefreshHelpers = true;
                            }
                            else
                            {
                                b.FullCorrection = false;
                                b.AnimateCorrections = false;
                            }
                        }
                    }
                }

                GUI.color = preCol;

                FTail_AnimatorBlending tailBlending = (FTail_AnimatorBlending)target;
                EditorGUILayout.Slider(sp_blendOrig, 0f, 1f);

                if (tailBlending.BlendToOriginal > 0f && tailBlending.BlendToOriginal < 1f)
                {
                    if (tailBlending.TailTransforms.Count > 0)
                    {
                        float height = 16f;
                        Rect rect = GUILayoutUtility.GetRect(GUILayoutUtility.GetLastRect().width, height, "TextField");

                        float step = rect.width / (float)tailBlending.TailTransforms.Count;

                        for (int i = 0; i < tailBlending.TailTransforms.Count; i++)
                        {
                            float y = 1 - Mathf.InverseLerp(tailBlending.TailTransforms.Count / 2, tailBlending.TailTransforms.Count + 1, i);

                            float blendValue = 1f;

                            if (tailBlending.BlendChainValue < 1f)
                                blendValue = Mathf.Clamp(tailBlending.BlendChainValue * (float)tailBlending.TailTransforms.Count - i, 0f, 1f);

                            EditorGUI.DrawRect(new Rect(rect.x + 2 + i * step, rect.y + (1 - y) * ((height - 1) / 2), step - 2f, height * y), new Color(0.9f, 0.9f, 0.9f, blendValue * 0.78f));
                        }

                        var centered = GUI.skin.GetStyle("Label");
                        centered.alignment = TextAnchor.UpperCenter;
                        GUI.Label(rect, Mathf.Round(tailBlending.BlendChainValue * 100) + "% Source Animation Chain Blend", centered);
                    }

                    EditorGUILayout.Slider(sp_blendChain, 0f, 1f);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Blend with original for chain blend", MessageType.None);
                    GUILayout.EndHorizontal();
                }

                EditorGUIUtility.labelWidth = 147;
                EditorGUILayout.PropertyField(sp_posNotAnim);
                EditorGUILayout.PropertyField(sp_rotNotAnim);
                EditorGUIUtility.labelWidth = 0;

                if (tailBlending.RotationsNotAnimated)
                {
                    if (tailBlending.SelectiveRotsNotAnimated.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Hit 'Auto' for this option to work", MessageType.Info);
                    }
                    else
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(sp_rotsSelective, true);
                        EditorGUI.indentLevel--;
                    }
                }

                GUILayout.Space(4f);

                EditorGUIUtility.labelWidth = 147;
                EditorGUILayout.PropertyField(sp_queue);
                EditorGUIUtility.labelWidth = 0;
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(4f);
        }

        protected override void OnClickedAuto()
        {
            base.OnClickedAuto();
            OnTailTransformsCountChange();
        }

        protected override void OnTailTransformsCountChange()
        {
            base.OnTailTransformsCountChange();

            FTail_AnimatorBlending blending = target as FTail_AnimatorBlending;
            blending.SelectiveRotsNotAnimated = new System.Collections.Generic.List<bool>();
            for (int i = 0; i < blending.TailTransforms.Count; i++) blending.SelectiveRotsNotAnimated.Add(true);
        }

        /// <summary>
        /// Switches few variables for specific behaviour of component
        /// </summary>
        private void SwitchConnectWithAnimator(bool turnOn)
        {
            FTail_AnimatorBase tailComp = (FTail_AnimatorBase)target;

            if (turnOn)
            {
                wasCorr = tailComp.FullCorrection;
                wasAnimCorr = tailComp.AnimateCorrections;
                wasRefresh = tailComp.RefreshHelpers;

                tailComp.FullCorrection = true;
                tailComp.AnimateCorrections = true;
                tailComp.RefreshHelpers = true;
            }
            else
            {
                tailComp.FullCorrection = wasCorr;
                tailComp.AnimateCorrections = wasAnimCorr;
                tailComp.RefreshHelpers = wasRefresh;
            }
        }
    }
}