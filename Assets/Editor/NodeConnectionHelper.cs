using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeConnectionHelper : EditorWindow
{
    PowerConnection connectionB;
    PowerConnection connectionL;
    PowerConnection connectionT;
    PowerConnection connectionR;

    [MenuItem("Window/PowerNodeHelper")]
    public static void ShowWindow()
    {
        GetWindow<NodeConnectionHelper>("Conection helper");
    }

    private void OnGUI()
    {
        GUILayout.Label("Connections");
        connectionB = (PowerConnection)EditorGUILayout.ObjectField(connectionB, typeof(PowerConnection), true);
        if (GUILayout.Button("Set up connection"))
        {
            SetUpConnection(0, connectionB);
        }
        connectionL = (PowerConnection)EditorGUILayout.ObjectField(connectionL, typeof(PowerConnection), true);
        if (GUILayout.Button("Set up connection"))
        {
            SetUpConnection(1, connectionL);
        }
        connectionT = (PowerConnection)EditorGUILayout.ObjectField(connectionT, typeof(PowerConnection), true);
        if (GUILayout.Button("Set up connection"))
        {
            SetUpConnection(2, connectionT);
        }
        connectionR = (PowerConnection)EditorGUILayout.ObjectField(connectionR, typeof(PowerConnection), true);
        if (GUILayout.Button("Set up connection"))
        {
            SetUpConnection(3, connectionR);
        }
    }

    void SetUpConnection(int position, PowerConnection connector = null)
    {
        if (Selection.count == 1)
        {
            GameObject obj = (GameObject)Selection.objects[0];
            PowerNode node = obj.GetComponent<PowerNode>();
            if (node != null)
            {
                if (node.GetNeighbor(position) != null)
                {
                    PowerConnection connection = node.GetNeighbor(position);
                    if (connection.GetInput() == null)
                    {
                        connection.SetInput(node);
                    }
                    else
                    {
                        connection.SetOutput(node.gameObject);
                        connection.SetUpLines();
                    }
                }
                else
                {
                    if (connector == null)
                    {
                        Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Connection.prefab", typeof(GameObject));
                        if (prefab == null)
                        {
                            Debug.LogError("Prefab not found");
                        }
                        Vector3 pos;
                        switch (position)
                        {
                            case 0:
                                pos = new Vector3(node.transform.position.x, node.transform.position.y - 1, 0);
                                break;
                            case 1:
                                pos = new Vector3(node.transform.position.x - 1, node.transform.position.y, 0);
                                break;
                            case 2:
                                pos = new Vector3(node.transform.position.x, node.transform.position.y + 1, 0);
                                break;
                            case 3:
                                pos = new Vector3(node.transform.position.x + 1, node.transform.position.y, 0);
                                break;
                            default:
                                pos = new Vector3(node.transform.position.x, node.transform.position.y - 1, 0);
                                break;
                        }
                        GameObject newObj = (GameObject)Instantiate(prefab, pos, new Quaternion(0, 0, 0, 0));
                        connector = newObj.GetComponent<PowerConnection>();
                        connector.SetInput(node);
                        //Undo.RecordObject(node.gameObject, "PowerNode references set");
                        node.SetNeighbour(position, connector);

                    }
                    else
                    {
                        //Undo.RecordObject(node.gameObject, "PowerNode references set");
                        node.SetNeighbour(position, connector);
                        
                        connector.SetOutput(node.gameObject);
                        connector.Awake();
                        connector.SetUpLines();
                    }
                    
                }
            }
            else
            {
                Debug.LogWarning("NodeConnectionHelper: This operation only works on PowerNode objects");
            }
        }
        else
        {
            Debug.LogWarning("NodeConnectionHelper: Have only one node selected at a time");
        }
    }
}
