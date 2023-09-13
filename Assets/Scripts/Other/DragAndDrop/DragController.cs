using UnityEngine;
/// <summary>
/// Контроллер для захвата объектов через луч
/// </summary>
public class DragController : MonoBehaviour
{
    public Draggable LastDragged => lastDragged;

    private bool isDragActive = false;

    private Vector2 screenPosition;

    private Vector3 worldPosition;

    private Draggable lastDragged;
    /// <summary>
    /// Получение контроллеров на сцене и если их больше одного, то уничтожает объект
    /// </summary>
    void Awake()
    {
        DragController[] controllers = FindObjectsOfType<DragController>();
        if (controllers.Length > 1)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Если нажали на экран, то получает позицию нажатия и перемещает объект, либо оставляет
    /// </summary>
    void Update()
    {
        if (isDragActive)
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                Drop();
                return;
            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            screenPosition = new Vector2(mousePos.x, mousePos.y);
        }
        else if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }

        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (isDragActive)
        {
            Drag();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null)
            {
                Draggable draggable = hit.transform.gameObject.GetComponent<Draggable>();
                if (draggable != null)
                {
                    lastDragged = draggable;
                    InitDrag();
                }
            }
        }
    }
    /// <summary>
    /// Инициализация захвата и обновление статуса на true
    /// </summary>
    void InitDrag()
    {
        lastDragged.LastPosition = lastDragged.transform.position;
        UpdateDraStatus(true);
    }
    /// <summary>
    /// захват объекта и его перемещение к позиции нажатия
    /// </summary>
    void Drag()
    {
        lastDragged.transform.position = new Vector2(worldPosition.x, worldPosition.y);
    }
    /// <summary>
    /// Обновление статуса на false
    /// </summary>
    void Drop()
    {
        UpdateDraStatus(false);
    }
    /// <summary>
    /// При захвате меняет слой объекта на Dragging, иначе на Default
    /// </summary>
    /// <param name="IsDragging">параметр при захвате</param>
    void UpdateDraStatus(bool IsDragging)
    {
        isDragActive = lastDragged.IsDragging = IsDragging;
        lastDragged.gameObject.layer = IsDragging ? Layer.Dragging : Layer.Default;
    }
}
