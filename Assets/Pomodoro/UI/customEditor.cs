
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Contador))]
public class customEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Contador contador = (Contador)target;

        if(GUILayout.Button("Asignar tiempo"))
        {
            contador.AsignarContadorGUI();
        }
    }

}
