using UnityEngine;
using UnityEditor;

/**
 * 演示在Project窗口扩展右键菜单的功能
 *  
 */

public class EditorToolTest
{
    [MenuItem("Assets/ToolTest/打印选中项名")]
    static void Tool1()
    {
        Debug.Log(Selection.activeObject.name);
    }

    [MenuItem("Assets/ToolTest/创建/立方体")]
    static void CreateCube()
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
}
