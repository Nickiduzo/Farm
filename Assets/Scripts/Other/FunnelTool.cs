using UnityEngine;

/// <summary>
/// класс для инструментария
/// </summary>
public class FunnelTool : MonoBehaviour
{
    private Animator anim;
    private Collider2D collider2D;
    private Vector2 originalPosition, screenBounds;
    private bool worck, goBack;
    SpawnerGround gc;
    float speed = 5;
    /// <summary>
    /// Уничтожение нынешнего объекта, если этот класс повторяется на сцене
    /// </summary>
    void Awake()
    {
        FunnelTool[] controllers = FindObjectsOfType<FunnelTool>();
        if (controllers.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    /// <summary>
    /// Получение основных данных и параметров
    /// </summary>
    void Start()
    {
        anim = GetComponent<Animator>();

        originalPosition = transform.position;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        gc = GameObject.FindGameObjectWithTag("SpawnerGround").GetComponent<SpawnerGround>();
        collider2D = GetComponent<Collider2D>();
    }
    /// <summary>
    /// Метод обновления с проверкой овощей
    /// </summary>
    void Update()
    {
        if (goBack) GoBack();
        else if (worck) Worck();

        //Destruction of the object after completing the task

        if (gc.vegetables > 9)
        {
            Vector2 pos;
            pos = new Vector2(screenBounds.x + 3, transform.position.y);
            originalPosition = pos;
            collider2D.enabled = false;

            if (Vector2.Distance(transform.position, pos) >= 1) Destroy(gameObject, 3f);
        }
    }

    /// <summary>
    /// Движение и смена параметра на false 
    /// </summary>
    void GoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
        anim.SetBool("State", false);
    }
    /// <summary>
    /// Смена параметра на true
    /// </summary>
    void Worck()
    {
        anim.SetBool("State", true);
    }
    /// <summary>
    /// При входе определённых тегов включать параметр worck и отключает goBack
    /// </summary>
    /// <param name="other">коллайдер, что столкнулся с объектом</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Rock") || other.CompareTag("Seedlings"))
        {
            worck = true;
            goBack = false;
        }
    }
    /// <summary>
    /// При выходе определённых тегов включать параметр движения назад и отключает worck
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Rock") || other.CompareTag("Seedlings"))
        {
            worck = false;
            goBack = true;
        }
    }
}