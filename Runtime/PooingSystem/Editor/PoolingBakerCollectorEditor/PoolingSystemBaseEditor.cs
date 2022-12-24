#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace Services.Optimization.PoolingSystem.ScriptEditor
{
    [CustomEditor(typeof(PoolingSystemBaseHnadler))]
    public class PoolingSystemBaseEditor : Editor
    {
        PoolingSystemBaseHnadler _systemBaseHandler;
        Dictionary<int, AnimBool> _animBools = new Dictionary<int, AnimBool>();

        float _miniBtn = 30f;

        public override void OnInspectorGUI()
        {
            _systemBaseHandler = target as PoolingSystemBaseHnadler;
            if (!_systemBaseHandler) return;

             EditorAssitance.VerticalGroup(EditorGUIKey.GroupBox, () =>
             {
                 PoolingEditorCache.objectPreview = (ObjectPreview)EditorGUILayout.EnumPopup("Preview Object",PoolingEditorCache.objectPreview);
                 EditorGUILayout.PropertyField(serializedObject.FindProperty("_initMethod"));
                 EditorGUILayout.PropertyField(serializedObject.FindProperty("_container"));
             });

            //Check Confix
            EditorAssitance.VerticalGroup(EditorGUIKey.GroupBox, () =>
            {
                int count = _systemBaseHandler.BakerLayers.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!_animBools.ContainsKey(i)) _animBools.Add(i, new AnimBool());
                    DrawLayerItem(i);
                }

                EditorAssitance.HorizontalLine();

                if (GUILayout.Button("New Layer"))
                {
                    CreateLayer newLayer = new CreateLayer();
                    newLayer.SetupValue();
                    _systemBaseHandler.BakerLayers.Add(newLayer);
                }
            });

            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(_systemBaseHandler, "BakerCollector");

            if(GUI.changed) EditorUtility.SetDirty(_systemBaseHandler);

            Repaint();
        }

        void DrawLayerItem(int index)
        {
            if (index > _systemBaseHandler.BakerLayers.Count - 1) 
                return;

            var layer = _systemBaseHandler.BakerLayers[index];
            bool isContineus = true;

            EditorAssitance.FoldoutGroup(_animBools[index], layer.layerName, () =>
            {
                if (!isContineus)
                    return;

                EditorAssitance.VerticalGroup(EditorGUIKey.GroupBox, () =>
                {
                    layer.layerName = EditorGUILayout.TextField("Layer Name", layer.layerName);
                    layer.source = (CreateSource)EditorGUILayout.EnumPopup("Soruce Loader", layer.source);

                    if(layer.source == CreateSource.Defualt)
                    {
                        for (int i = 0; i < layer.poolingCreaters.Count; i++)
                        {
                            DrawItem(index, i);
                        }

                        EditorAssitance.HorizontalLine();

                        if (GUILayout.Button("+"))
                        {
                            layer.poolingCreaters.Add(new PoolingCreater());
                        }

                    }
                    else if(layer.source == CreateSource.LoadPath)
                    {
                        EditorAssitance.VerticalGroup(EditorGUIKey.GroupBox, () =>
                        {
                            layer.sourcePath = EditorGUILayout.TextField("Assets Path", layer.sourcePath);
                        });
                    }

                });

                _systemBaseHandler.BakerLayers[index] = layer;

            }, () => 
            { 
                if(GUILayout.Button("X", GUILayout.Width(_miniBtn)))
                {
                    _systemBaseHandler.BakerLayers.RemoveAt(index);
                    isContineus = false;
                }
            });
        }

        void DrawItem(int layerIndex, int createrIndex)
        {
            var layer = _systemBaseHandler.BakerLayers[layerIndex];
            var creater = layer.poolingCreaters[createrIndex];
            var id = creater.profile ? creater.profile.ID : "Profile is null";

            var isConfix = !PoolingEditorCache.AddObjectCacheSucces(creater.profile);
            bool isContinues = true;

            EditorAssitance.VerticalGroup(EditorGUIKey.GroupBox, () =>
            {
                EditorAssitance.HorizontalGroup(GUI.skin.box, () =>
                {
                    EditorGUILayout.LabelField("ID : " + id);
                    if (GUILayout.Button("-",GUILayout.Width(_miniBtn)))
                    {
                        layer.poolingCreaters.RemoveAt(createrIndex);
                        isContinues = false;
                        return;
                    }
                });

                if (!isContinues)
                    return;

                creater.profile = (PoolingProfile)EditorGUILayout.ObjectField("Profile", creater.profile, typeof(PoolingProfile), true);

                if (creater.profile && creater.profile.Prefab && PoolingEditorCache.objectPreview == ObjectPreview.On)
                {
                    EditorAssitance.HorizontalLine();
                    EditorAssitance.DrawPrefviewObject(creater.profile.Prefab.gameObject, 80);
                }

                if(isConfix)
                    EditorGUILayout.HelpBox("ID of this profile are same with other profile id please check profiles id", MessageType.Error);

                layer.poolingCreaters[createrIndex] = creater;
                _systemBaseHandler.BakerLayers[layerIndex] = layer;
            });
        }
    }
}
#endif
