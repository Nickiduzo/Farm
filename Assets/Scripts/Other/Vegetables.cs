using UnityEngine;

/// <summary>
/// Класс овощей и их взаидействий с игрой
/// </summary>
public class Vegetables : MonoBehaviour
{
    SpawnerGround sg;
    private Collider2D collider2D;
    private SpriteRenderer renderer;
    private Vector2 originalPosition, screenBounds;
    public GameObject prefWorms;
    public GameObject prefBasket;

    private bool flag = false;

    public int numberToSpawn = 5;


    /// <summary>
    /// Получение параметров и разрешения экрана
    /// </summary>
    void Start()
    {
        originalPosition = transform.position;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        renderer = GetComponent<SpriteRenderer>();
        Spawwen(prefWorms);
        sg = GameObject.FindGameObjectWithTag("SpawnerGround").GetComponent<SpawnerGround>();
        sg.vegetables++;

        collider2D = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Если объектов нет по тегам и условии, то спавнит объект
    /// </summary>
    private void Update()
    {
        if (!GameObject.FindGameObjectWithTag("Jarglass") && !GameObject.FindGameObjectWithTag("Worms") &&
            !GameObject.FindGameObjectWithTag("Bugs"))
            if (!flag)
            {
                SpawnerBasket(prefBasket);
                flag = true;
            }
    }
    /// <summary>
    /// При прикосновении к корзине
    /// </summary>
    /// <param name="collision">коллайдер вошедшего объекта</param>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Basket")
        {
            renderer.flipY = true;

            collider2D.enabled = false;
        }
    }

    /// <summary>
    /// Спавнит объект по позиции экрана, но с заданым количеством
    /// </summary>
    /// <param name="prefSpownObjekt">объект, что нужно заспавнить</param>
    void Spawwen(GameObject prefSpownObjekt)
    {
        //setting the spawning position
        float x = screenBounds.x + 1;
        float y = screenBounds.y + 1;

        Vector2 pos;


        for (int i = 0; i < numberToSpawn; i++)
        {
            float posX = Random.Range(-x, x);
            float posY = Random.Range(-y, y);

            pos = new Vector2(posX - i, posY + i);
            Instantiate(prefSpownObjekt, pos, prefSpownObjekt.transform.rotation);
        }
    }
    /// <summary>
    /// Спавнит объект по позиции экрана
    /// </summary>
    /// <param name="gameObject">объект, что нужно заспавнить</param>
    void SpawnerBasket(GameObject gameObject)
    {
        Vector2 pos;

        float posX = -screenBounds.x;
        float posY = -screenBounds.y + 1;

        pos = new Vector2(posX, posY);
        Instantiate(gameObject, pos, gameObject.transform.rotation);
    }
}