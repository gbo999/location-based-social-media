/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using InfinityCode.uPano.Directions;
using InfinityCode.uPano.Editors.Utils;
using InfinityCode.uPano.HotSpots;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.uPano.Editors.VisualEditors.InteractiveElements
{
    public class InteractiveElementSettings : EditorWindow
    {
        internal static InteractiveElementSettings wnd;
        private Vector2 scrollPosition;

        public static void CloseWindow()
        {
            if (wnd != null) wnd.Close();
        }

        private void OnDestroy()
        {
            wnd = null;
        }

        private void OnEnable()
        {
            wnd = this;
        }

        private void OnGUI()
        {
            InteractiveElementNode node = VisualInteractiveElementEditor.activeNode;
            if (node == null) return;

            EditorGUI.BeginChangeCheck();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (node.type == InteractiveElementNode.ElementType.hotSpot) DrawHotSpotSettings(node);
            else if (node.type == InteractiveElementNode.ElementType.direction) DrawDirectionSettings(node);

            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck()) VisualInteractiveElementEditor.Redraw();
        }

        private void DrawDirectionSettings(InteractiveElementNode node)
        {
            Direction direction = node.direction;
            int index = VisualInteractiveElementEditor.directionManager.IndexOf(direction);

            if (index == -1) return;

            VisualInteractiveElementEditor.serializedDirectionManager.Update();
            SerializedProperty item = VisualInteractiveElementEditor.serializedDirectionItems.GetArrayElementAtIndex(index);

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_title"));

            EditorGUI.BeginChangeCheck();
            SerializedProperty prefabProperty = item.FindPropertyRelative("_prefab");
            EditorGUILayout.PropertyField(prefabProperty);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying) direction.prefab = prefabProperty.objectReferenceValue as GameObject;

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_pan"));

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_scale"));

            DrawQuickActions(item);
            DrawEvents(item);

            VisualInteractiveElementEditor.serializedDirectionManager.ApplyModifiedProperties();
        }

        private void DrawHotSpotSettings(InteractiveElementNode node)
        {
            HotSpot hotSpot = node.hotSpot;
            int index = VisualInteractiveElementEditor.hotSpotManager.IndexOf(hotSpot);

            if (index == -1 || VisualInteractiveElementEditor.serializedHotSpotManager == null) return;

            VisualInteractiveElementEditor.serializedHotSpotManager.Update();
            SerializedProperty item = VisualInteractiveElementEditor.serializedHotSpotItems.GetArrayElementAtIndex(index);

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_title"));

            EditorGUI.BeginChangeCheck();
            SerializedProperty prefabProperty = item.FindPropertyRelative("_prefab");
            EditorGUILayout.PropertyField(prefabProperty);
            if (EditorGUI.EndChangeCheck() && Application.isPlaying) hotSpot.prefab = prefabProperty.objectReferenceValue as GameObject;

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_pan"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("_tilt"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("_lookToCenter"));

            EditorGUI.BeginChangeCheck();
            Quaternion rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", hotSpot.rotation.eulerAngles));
            if (EditorGUI.EndChangeCheck())
            {
                hotSpot.rotation = rotation;
                VisualInteractiveElementEditor.serializedHotSpotManager.Update();
            }

            EditorGUILayout.PropertyField(item.FindPropertyRelative("_scale"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("_distanceMultiplier"));

            DrawQuickActions(item);
            DrawEvents(item);

            VisualInteractiveElementEditor.serializedHotSpotManager.ApplyModifiedProperties();
        }

        private static void DrawQuickActions(SerializedProperty item)
        {
            EditorUtils.GroupLabel("Quick Actions");

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 180;

            DrawQuickActionTargetPanorama(item);
            DrawQuickActionTooltip(item);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }

        private static void DrawQuickActionTooltip(SerializedProperty item)
        {
            if (VisualInteractiveElementEditor.activeNode.type != InteractiveElementNode.ElementType.hotSpot) return;

            SerializedProperty tooltip = item.FindPropertyRelative("tooltip");
            EditorGUILayout.PropertyField(tooltip);
            if (string.IsNullOrEmpty(tooltip.stringValue)) return;

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(item.FindPropertyRelative("tooltipAction"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("tooltipPrefab"));
            EditorGUI.indentLevel--;
        }

        private static void DrawQuickActionTargetPanorama(SerializedProperty item)
        {
            SerializedProperty loadPanoPrefab = item.FindPropertyRelative("loadPanoramaPrefab");
            EditorGUILayout.PropertyField(loadPanoPrefab);

            SerializedProperty targetPano = item.FindPropertyRelative("switchToPanorama");
            EditorGUILayout.PropertyField(targetPano);

            if (loadPanoPrefab.objectReferenceValue == null && targetPano.objectReferenceValue == null) return;

            if (targetPano.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(item.FindPropertyRelative("copyPanTilt"));
            }

            EditorGUILayout.PropertyField(item.FindPropertyRelative("beforeTransitionPrefab"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("afterTransitionPrefab"));
        }

        private static void DrawEvents(SerializedProperty item)
        {
            EditorUtils.GroupLabel("Events");

            EditorGUILayout.PropertyField(item.FindPropertyRelative("ignoreGlobalActions"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("OnClick"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("OnPointerDown"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("OnPointerUp"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("OnPointerEnter"));
            EditorGUILayout.PropertyField(item.FindPropertyRelative("OnPointerExit"));
        }

        internal static void OpenWindow()
        {
            GetWindow<InteractiveElementSettings>(true, "Interactive Element Settings", false);
        }

        internal static void Redraw()
        {
            if (wnd == null) return;

            if (VisualInteractiveElementEditor.activeNode == null) CloseWindow();
            else wnd.Repaint();
        }
    }
}