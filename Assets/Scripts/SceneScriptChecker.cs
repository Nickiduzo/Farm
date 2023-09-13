using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;


// чекає всі MonoBehaviour скріпти
public class SceneScriptChecker : MonoBehaviour
{
    // папка скріптів
    [SerializeField] private string folderPath = "Assets/Scripts/Scenes/Fishing";
    // скріпти що перевірив (добавляти вручну)
    [SerializeField] private string[] checkedScripts;
    
    private void Start()
    {
        AnalyzeScene();
    }

    private void AnalyzeScene()
    {
        List<string> checkedScriptsList = new List<string>(checkedScripts);
        // скріпти що на сцені
        List<string> scriptsOnScene = GetAllScriptsOnScene();
        // скріпти що перевіряються в папці до шляху
        Dictionary<string,string> scriptsInFolderToPath = GetScriptsInFolder();
        // скріпти що перевіряються в папці
        List<string> untochedScripts = new List<string>(scriptsInFolderToPath.Keys);
        // перевіряєм скріпти які є на сцені видаляєм
        foreach (string script in scriptsOnScene)
        {
            if (untochedScripts.Contains(script))
            {
                untochedScripts.Remove(script);
                //Debug.Log(script);
            }
        }
        // видаляє скріпти що перевірив

        foreach (string script in checkedScriptsList)
        {
            if (untochedScripts.Contains(script))
            {
                untochedScripts.Remove(script);
                //Debug.Log(script);
            }
        }
        // виводим шлях до скріптів які не юзаєм
        foreach (string script in untochedScripts)
        {
            Debug.Log(scriptsInFolderToPath[script]);
        }

    }

    private List<string> GetAllScriptsOnScene()
    {
        List<string> scripts = new List<string>();

        // Отримати всі об'єкти на сцені
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        // Перебрати кожен об'єкт
        foreach (GameObject gameObject in gameObjects)
        {
            // Отримати всі скрипти, прикріплені до об'єкта
            MonoBehaviour[] attachedScripts = gameObject.GetComponents<MonoBehaviour>();

            // Додати скрипти до списку якщо його ще немає
            foreach(MonoBehaviour attachedScript in attachedScripts){
                string scriptName = attachedScript.GetType().Name;
                if (!scripts.Contains(scriptName))
                    scripts.Add(scriptName);
            }
        }

        return scripts;
    }



    public Dictionary<string,string> GetScriptsInFolder()
    {

        Dictionary<string,string>  scriptNames = new Dictionary<string,string>();

        // Отримати всі файли в папці
        string[] files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

        // Перебрати кожен файл та отримати назву скрипта
        foreach (string filePath in files)
        {
            string scriptName = Path.GetFileNameWithoutExtension(filePath);
            scriptNames[scriptName] = filePath;
            
        }

        return scriptNames;
    }

}