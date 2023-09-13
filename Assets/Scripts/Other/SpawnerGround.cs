using UnityEngine;

/// <summary>
/// Класс спавна на земле
/// </summary>
public class SpawnerGround : MonoBehaviour
{
    public int numberToSpawn;

    //Objects for svavan
    public GameObject prefGround;
    public GameObject toolShelingObjekt;
    public GameObject prefHole, prefRoke;
    private Vector2 originalPosition;
    public int vegetables, graund, seedlings, worms, bugs, rock, fish, bottel = 0;
    private Vector2 screenBounds;
    private float posStateX, posStateY;

    //plane on which spawning takes place
    /// <summary>
    /// Если число спавна задано, то спавнить объекты, иначе создать яму и камень
    /// </summary>
    void Start()
    {
        if (numberToSpawn != 0)
        {
            originalPosition = transform.position;
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            Vector2 pos;
            pos = new Vector2(screenBounds.x - 3, -screenBounds.y + 3);
            Instantiate(toolShelingObjekt, pos, toolShelingObjekt.transform.rotation);
            if (numberToSpawn > 1)
                spawnObjects();
            else
            {
                posStateX = screenBounds.x / 2 - 5;
                posStateY = screenBounds.y / 2 - 6;
                Vector2 originalPositionHole = new Vector2(posStateX, posStateY);

                Instantiate(prefHole, originalPositionHole, prefHole.transform.rotation);

                Vector2 originalPositionRoke = new Vector2(posStateX, posStateY + 1);
                Instantiate(prefRoke, originalPositionRoke, prefRoke.transform.rotation);
            }
        }
    }

    /// <summary>
    /// Спавн объектов по числу и по позиции экрана с номером спавна
    /// </summary>
    public void spawnObjects()
    {
        Vector2 pos;
        float screenX, screenY;


        for (int i = 0; i < numberToSpawn; i++)
        {
            screenX = (-screenBounds.x + 7) + i * 5;

            screenY = (-screenBounds.y / 2);
            pos = new Vector2(screenX, screenY);

            Instantiate(prefGround, pos, prefGround.transform.rotation);
        }
    }
}