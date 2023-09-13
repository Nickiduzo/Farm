using UnityEngine;
/// <summary>
/// Класс посадки семян
/// </summary>
public class Seedlings : MonoBehaviour
{
    private Collider2D collider2D;

    public GameObject toolObjekt;

    public int numberToSpawn = 5;
    public GameObject prefSpownObjekt;

    private bool angry, chill = false;

    private Vector2 originalPosition;
    public int woater;
    bool flag = false, chekHole = false, cheakWoater = false;
    private Vector2 screenBounds, pos;

    SpawnerGround sg;

    /// <summary>
    /// Получить данные и найти разрешение экрана
    /// </summary>
    void Start()
    {
        sg = GameObject.FindGameObjectWithTag("SpawnerGround").GetComponent<SpawnerGround>();
        originalPosition = transform.position;
        collider2D = GetComponent<Collider2D>();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        sg.seedlings++;
        pos = new Vector2(screenBounds.x - 3, -screenBounds.y + 3);

    }

    /// <summary>
    /// Проверка на количество и спавн объекта в случае, если количество равно 10
    /// </summary>
    void Update()
    {

        if (cheakWoater)
            woater++;

        if (woater == 10)
        {
            cheakWoater = false;
            if (!flag)
            {
                float x = transform.position.x;
                float y = transform.position.y;
                Vector2 pos;

                for (int i = 0; i < numberToSpawn; i++)
                {
                    float posX = Random.Range(-x, x);
                    float posY = Random.Range(-y, y);

                    pos = new Vector2(posX, posY + i);
                    collider2D.enabled = false;
                    Instantiate(prefSpownObjekt, pos, prefSpownObjekt.transform.rotation);
                }

                flag = true;
            }
        }
        //check for spawn tool
        if (chekHole && !GameObject.FindGameObjectWithTag("Packeg"))
        {
            Vector2 pos;
            pos = new Vector2(screenBounds.x - 3, -screenBounds.y + 3);
            Instantiate(toolObjekt, pos, prefSpownObjekt.transform.rotation);
            chekHole = false;
        }

    }


    /// <summary>
    /// Проверка на прикосновение к яме и инструменту
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Holes"))
        {
            chekHole = true;

        }

        if (collision.CompareTag("Tools"))
        {
            cheakWoater = true;
        }

    }
}
