using FIMSpace.Basics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FTail
{
    /// <summary>
    /// FM: Class to update all spine animator components in one Update() Tick
    /// Try to use it if you add lots of tail animators to your object (more than 100) this component can boost performance pretty well.
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Tail Animator/Utilities/FTail Animator MassUpdater")]
    public class FTail_Animator_MassUpdater : MonoBehaviour, UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        public string EditorIconPath { get { return "Tail Animator/FTailAnimatorMass Icon"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        /// <summary> Tails deactivated and updated from this component </summary>
        protected List<FTail_AnimatorBase> tails;

        /// <summary> Tails which are waiting for full initialization </summary>
        private List<FTail_AnimatorBase> tailsQueue;

        public bool StopUpdating = false;
        protected bool StopUpdating2 = false;

        [Header("Use this component only when", order = 0)]
        [Space(-11, order = 1)]
        [Header("you use a lot of tail animators", order = 2)]
        [Space(7, order = 3)]
        public EFUpdateClock UpdateClock = EFUpdateClock.Update;

        [Header("If you want update tail animators selectively")]
        public List<Transform> TailAnimatorsFrom;

        [Header("With new instatiated tail animators", order = 0)]
        [Space(-11, order = 1)]
        [Header("list must be refreshed from code", order = 2)]
        [Space(7, order = 3)]
        [Tooltip("Refersh tail animators by using AddTailToUpdate() method")]
        public bool GetFromWholeScene = false;

        private static FTail_Animator_MassUpdater WholeSceneGetter = null;
        public bool DelayedStart = false;

        private bool initialized = false;


        protected virtual IEnumerator Start()
        {
            if (DelayedStart)
            {
                yield return null;
                yield return null;
            }

            tails = new List<FTail_AnimatorBase>();
            tailsQueue = new List<FTail_AnimatorBase>();

            if (GetFromWholeScene)
            {
                if (WholeSceneGetter)
                {
                    Debug.LogError("There is already component which gets tail animators from whole scene! (" + WholeSceneGetter.transform.name + ")");
                    yield break;
                }
                else
                {
                    WholeSceneGetter = this;
                    FTail_AnimatorBase[] ts = FindObjectsOfType<FTail_AnimatorBase>();
                    for (int i = ts.Length - 1; i >= 0; i--) AddTailToUpdate(ts[i]);
                }
            }
            else
            {
                if (TailAnimatorsFrom.Count > 0)
                {
                    for (int i = 0; i < TailAnimatorsFrom.Count; i++)
                    {
                        List<FTail_AnimatorBase> anims = FTransformMethods.FindComponentsInAllChildren<FTail_AnimatorBase>(TailAnimatorsFrom[i]);
                        for (int j = 0; j < anims.Count; j++) AddTailToUpdate(anims[j]);
                    }
                }
                else
                    Debug.LogError("No 'TailAnimatorsFrom' reference transform to get tail animators!");
            }

            initialized = true;

            yield break;
        }

        protected virtual void Update()
        {
            if (UpdateClock != EFUpdateClock.Update) return;
            UpdateTails();
        }

        private void LateUpdate()
        {
            if (UpdateClock != EFUpdateClock.LateUpdate) return;
            UpdateTails();
        }

        private void FixedUpdate()
        {
            if (UpdateClock != EFUpdateClock.FixedUpdate) return;
            UpdateTails();
        }

        private void OnDisable()
        {
            if (tails != null)
                for (int i = 0; i < tails.Count; i++)
                    if (tails[i])
                        tails[i].enabled = true;
        }

        private void OnEnable()
        {
            if (tails != null)
                for (int i = 0; i < tails.Count; i++)
                    if (tails[i])
                        tails[i].enabled = false;
        }

        private void UpdateTails()
        {
            if (!initialized) return;
            if (StopUpdating || StopUpdating2) return;

            for (int i = tails.Count - 1; i >= 0; i--) if (!tails[i]) tails.RemoveAt(i); else tails[i].CalculateOffsets();

            for (int i = tailsQueue.Count - 1; i >= 0; i--)
            {
                if (tailsQueue[i].IsInitialized && !tailsQueue[i].RefreshHelpers)
                {
                    tailsQueue[i].enabled = false;
                    if (!tailsQueue[i].QueueToLastUpdate) tails.Add(tailsQueue[i]); else tails.Insert(0, tailsQueue[i]);
                    tailsQueue.RemoveAt(i);
                }
                else
                    break;
            }
        }

        public void AddTailToUpdate(FTail_AnimatorBase tail)
        {
            if (!tails.Contains(tail)) if (!tailsQueue.Contains(tail)) tailsQueue.Add(tail);
        }
    }
}