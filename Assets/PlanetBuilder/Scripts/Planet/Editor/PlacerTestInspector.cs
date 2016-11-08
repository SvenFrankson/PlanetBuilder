using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlacerTest))]
public class PlacerTestInspector : Editor {

    private PlacerTest storedTarget;
    private PlacerTest Target
    {
        get
        {
            if (this.storedTarget == null)
            {

                this.storedTarget = (PlacerTest)target;
            }

            return storedTarget;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Test WorldPosTo..."))
        {
            Target.TestWorldPosTo();
        }
        if (GUILayout.Button("Test PutData..."))
        {
            Target.TestPutData();
        }
    }
}
