using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace QuickPool
{
    [CustomPropertyDrawer(typeof(Pool))]
    public class PoolDrawer : PropertyDrawer
    {
        private float lineHeight { get { return EditorGUIUtility.singleLineHeight; } }

        private DespawnMode despawnMode;
        bool useCustomRoot;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prefabProperty = property.FindPropertyRelative("prefab");
            SerializedProperty sizeProperty = property.FindPropertyRelative("size");
            SerializedProperty allowGrowthProperty = property.FindPropertyRelative("allowGrowth");
            SerializedProperty useCustomRootProperty = property.FindPropertyRelative("useCustomRoot");
            SerializedProperty rootProperty = property.FindPropertyRelative("m_Root");
            SerializedProperty despawnModeProperty = property.FindPropertyRelative("mode");
            SerializedProperty despawnPosProperty = property.FindPropertyRelative("despawnPos");
            SerializedProperty audioSourceHandlingProperty = property.FindPropertyRelative("audioSourceHandling");
            SerializedProperty particleSystemHandlingProperty = property.FindPropertyRelative("particleSystemHandling");

            GameObject prefab = (GameObject)prefabProperty.objectReferenceValue;
            string prefabName = prefab != null ? prefab.name : "None";
            despawnMode = (DespawnMode)despawnModeProperty.enumValueIndex;
            useCustomRoot = useCustomRootProperty.boolValue;

            //base.OnGUI(position, property, label);
            DrawHeader(position, prefabName, property);
            GUI.Box(position, "", GUI.skin.box);
            position.y += 5;
            if (property.isExpanded)
            {
                DrawField(position, prefabProperty, 1);
                DrawField(position, sizeProperty, 2);
                DrawField(position, allowGrowthProperty, 3);
                DrawEnumMaskField(position, audioSourceHandlingProperty, 4, "Audio Source");
                DrawEnumMaskField(position, particleSystemHandlingProperty, 5, "Particle system");
                DrawField(position, useCustomRootProperty, 6);
                if (useCustomRoot)
                    DrawField(position, rootProperty, 7);
                DrawField(position, despawnModeProperty, useCustomRootProperty.boolValue ? 8 : 7);
                if (despawnMode == DespawnMode.Move)
                    DrawField(position, despawnPosProperty, useCustomRootProperty.boolValue ? 9 : 8);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            var pool = GetPoolObject(property);

            float height = lineHeight;
            if(property.isExpanded)
            {
                height += lineHeight * 7;
                if (pool.useCustomRoot)
                    height += lineHeight;
                if (pool.mode == DespawnMode.Move)
                    height += lineHeight;
            }
            return height + 2;
        }

        private void DrawHeader(Rect position, string poolName, SerializedProperty property)
        {
            var headerPosition = new Rect(position.x, position.y, position.width, 20);
            GUI.Box(headerPosition, "", EditorStyles.toolbar);

            var headerLabel = new Rect(headerPosition.x + 15, headerPosition.y, headerPosition.width - 140, headerPosition.height);
            property.isExpanded = EditorGUI.Foldout(headerLabel, property.isExpanded, poolName);

            var clearBtnRect = new Rect(headerLabel.x + headerLabel.width, headerLabel.y, 40, headerLabel.height);

            if (GUI.Button(clearBtnRect, "Clear", EditorStyles.toolbarButton))
                GetPoolObject(property).ClearAndDestroy();

            var preinstantiateBtnRect = new Rect(headerLabel.x + headerLabel.width + 40, headerLabel.y, 80, headerLabel.height);

            if (GUI.Button(preinstantiateBtnRect, "Preinstantiate", EditorStyles.toolbarButton))
                GetPoolObject(property).Initialize();

        }

        private void DrawField(Rect position, SerializedProperty property, int index)
        {
            var fieldRect = new Rect(position.x + 5, position.y + lineHeight * index, position.width - 5, lineHeight);
            EditorGUI.PropertyField(fieldRect, property);
        }

        private void DrawEnumMaskField(Rect position, SerializedProperty property, int index, string text)
        {
            var fieldRect = new Rect(position.x + 5, position.y + lineHeight * index, position.width - 5, lineHeight);
            property.intValue = Utils.DrawBitMaskField(fieldRect, property.intValue, typeof(ComponentHandlingType), new GUIContent(text));
        }


        private Pool GetPoolObject(SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            var filedName = fieldInfo.Name;
            var type = obj.GetType();

            return (Pool)type.GetField(filedName).GetValue(obj);
        }
    }

    public class PropertyField
    {

        public void Draw()
        {

        }
    }
}