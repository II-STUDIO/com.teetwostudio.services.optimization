#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace Services.Optimization.MeshAnimationSystem
{
    [ExecuteInEditMode]
    public class MeshAnimator : MonoBehaviour
    {
        public MeshAnimatorController controller;

        public bool autoUpdate = true;

        private MeshFilter[] meshFilter;
        private MeshAnimationState currentState;
        private TransitionInfo transitionInfo_ref;
        private ParameterInfo parameter;

        private CountDown frameRateCountdown;
        private Condition condition_ref;

        private float frameFrequency;
        private bool isRegistedUpdate = false;

        private int currentStateIndex = 0;
        private int currentMeshIndex = 0;
        private int meshIndexCount = 0;

        private Dictionary<string, int> sateIndexHash = new Dictionary<string, int>();

#if UNITY_EDITOR
        public Coroutine EditorPlayRountine { get; set; }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            if (EditorPlayRountine == null)
                return;

            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
#endif
        private void Awake()
        {
            currentStateIndex = 0;
            currentMeshIndex = 0;
            currentState = null;

            if (!controller)
                return;

            currentStateIndex = controller.defaultStateIndex;
            frameFrequency = 1f / controller.fps;
            frameRateCountdown.onComplete = RefreshMesh;

            meshFilter = GetComponentsInChildren<MeshFilter>();

            if (controller.states.Count > 0)
            {
                currentState = controller.states[currentStateIndex];
                meshIndexCount = currentState.animation.meshesCollection[0].meshes.Count - 1;
            }

            RefreshMesh();
        }

        private void OnEnable()
        {
            if (autoUpdate)
            {
                isRegistedUpdate = true;
                frameRateCountdown.Start(frameFrequency);
                SystemBaseUpdater.Instance.AddUpdater(UpdateFrame);
            }
        }

        private void OnDisable()
        {
            if (autoUpdate || isRegistedUpdate)
            {
                isRegistedUpdate = false;
                frameRateCountdown.Clear();
                SystemBaseUpdater.Instance.RemoveUpdater(UpdateFrame);
            }
        }

        public Mesh GetMesh(int stateIndex, int meshIndex)
        {
            if (controller.states.Count == 0)
                return null;

            if (controller.states[stateIndex].animation.meshesCollection.Count == 0)
                return null;

            return controller.states[stateIndex].animation.meshesCollection[0].meshes[meshIndex];
        }

        public int StringToStateIndex(string animationName)
        {
            int count = controller.states.Count;

            for (int i = 0; i < count; i++)
            {
                if (animationName == controller.states[i].animation.name)
                    return i;
            }

            Debug.LogErrorFormat($"The animation name <{animationName}> is not exited");
            return -1;
        }

        public int AnimationToStateIndex(MeshAnimation animation)
        {
            return StringToStateIndex(animation.name);
        }

        public void PlayState(MeshAnimation animation)
        {
            PlayState(animation.name);
        }

        public void PlayState(string animationName)
        {
            if(sateIndexHash.TryGetValue(animationName, out int index))
            {
                PlayState(index);
                return;
            }

            int targetIndex = StringToStateIndex(animationName);
            if (targetIndex < 0)
            {
                Debug.LogErrorFormat($"The animation name <{animationName}> is not exited");
                return;
            }

            sateIndexHash.Add(animationName, targetIndex);

            PlayState(targetIndex);
        }

        public void PlayState(int stateIndex)
        {
            if (stateIndex == currentStateIndex)
                return;

            currentMeshIndex = 0;
            currentStateIndex = stateIndex;
            currentState = controller.states[currentStateIndex];
            meshIndexCount = currentState.animation.meshesCollection[0].meshes.Count - 1;

            frameRateCountdown.Clear();

            RefreshMesh();
        }

        public void UpdateFrame(float deltaTime)
        {
            frameRateCountdown.Update(deltaTime);
        }

        private void RefreshMesh()
        {
            frameRateCountdown.Start(frameFrequency);

            if (currentState == null)
                return;

            int collectionCount = currentState.animation.meshesCollection.Count;

            if (collectionCount == 1)
                meshFilter[0].mesh = currentState.animation.meshesCollection[0].meshes[currentMeshIndex];
            else
            {
                for(int i = 0; i < collectionCount; i++)
                {
                    meshFilter[i].mesh = currentState.animation.meshesCollection[i].meshes[currentMeshIndex];
                }
            }

            if (currentMeshIndex < meshIndexCount - 1)
                currentMeshIndex++;
            else
            {
                if (currentState.animation.isLoop)
                {
                    currentMeshIndex = 0;
                    return;
                }

                int newStateIndex = GetNextStateIndex();

                PlayState(newStateIndex);
            }
        }

        private int GetNextStateIndex()
        {
            if (currentState.transitionInfos == null)
                return currentStateIndex;

            int infoCount = currentState.transitionInfos.Count;
            if (infoCount == 0)
                return currentStateIndex;

            if(infoCount == 1)
            {
                transitionInfo_ref = currentState.transitionInfos[0];
                return CheckValideCondition(transitionInfo_ref) ? transitionInfo_ref.targetStateIndex : currentStateIndex;
            }

            for(int i = 0; i < infoCount; i++)
            {
                transitionInfo_ref = currentState.transitionInfos[0];

                if (CheckValideCondition(transitionInfo_ref))
                    return transitionInfo_ref.targetStateIndex;
            }

            return currentStateIndex;
        }

        private bool CheckValideCondition(TransitionInfo transitionInfo)
        {
            int count = transitionInfo.conditions.Count;

            if (count > 0)
            {
                for(int i = 0; i < count; i++)
                {
                    condition_ref = transitionInfo.conditions[i];

                    if (condition_ref.IsValide(parameter))
                        return true;
                }

                return false;
            }

            return true;
        }
    }
}