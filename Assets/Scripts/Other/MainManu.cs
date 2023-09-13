using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Класс выхода из главного меню
/// </summary>
public class MainManu : MonoBehaviour
{
    /// <summary>
    /// Выход из игры
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
    /// <summary>
    /// Разрушает все объекты и перезагружает главную сцену
    /// </summary>
    public void StartScene()
    {
        // calls Destroy on all active GameObjects should be sufficient

        GameObject[] GameObjects = FindObjectsOfType<GameObject>();

        for (int i = 0; i < GameObjects.Length; i++)
            Destroy(GameObjects[i]);

        SceneManager.LoadScene(0);
    }
}
