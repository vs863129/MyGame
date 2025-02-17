using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DebugManage : MonoBehaviour
{
    string filename = "";
    private void OnEnable()
    {
        Application.logMessageReceived += Log;
        Debug.Log("A");
    }
    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        CreateConsole();
    }
    public void Log(string LogString,string stackTrace,LogType type)
    {
        if(type!=LogType.Log)
        {
            TextWriter writer = new StreamWriter(filename, true);
            writer.WriteLine("[" + System.DateTime.Now + "]" + type + ": " + LogString);
            writer.Close();
        }
    }
    void CreateConsole()
    {
        string DebugFileName = "Console";
        var folder = Directory.CreateDirectory(Application.dataPath+"/"+ DebugFileName);
        filename = Application.dataPath + "/" + folder.Name + "/" + "/Log.txt";
    }
}
