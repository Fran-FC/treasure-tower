using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugRoomRenderer))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DebugRoomRenderer drr = (DebugRoomRenderer)target;
    }
}
