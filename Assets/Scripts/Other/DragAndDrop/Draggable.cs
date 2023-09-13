using UnityEngine;
/// <summary>
/// Класс изменения местоположения объекта при определённых условиях
/// </summary>
public class Draggable : MonoBehaviour
{
    public bool IsDragging;

    public Vector3 LastPosition;

    private Collider2D collider;

    private DragController dragController;

    private float movementTime = 15f;
    private System.Nullable<Vector3> movementDestination;
    /// <summary>
    /// Получение коллайдера и контроллера захвата
    /// </summary>
    private void Start()
    {
        collider = GetComponent<Collider2D>();
        dragController = FindObjectOfType<DragController>();
    }
    /// <summary>
    /// Обновление местоположения
    /// </summary>
    private void FixedUpdate()
    {
        if (movementDestination.HasValue)
        {
            if (IsDragging)
            {
                movementDestination = null;
                return;
            }

            if (transform.position == movementDestination)
            {
                gameObject.layer = Layer.Default;
                movementDestination = null;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, movementDestination.Value, movementTime * Time.fixedDeltaTime);
            }
        }
    }
    /// <summary>
    /// Получение местоположение в зависимости от тегов объекта с которым столкнулся
    /// </summary>
    /// <param name="other">коллайдер объекта</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Draggable collidedDraggable = other.GetComponent<Draggable>();

        if (collidedDraggable != null && dragController.LastDragged.gameObject == gameObject)
        {
            ColliderDistance2D colliderDistance2D = other.Distance(collider);
            Vector3 diff = new Vector3(colliderDistance2D.normal.x, colliderDistance2D.normal.y) * colliderDistance2D.distance;
            transform.position -= diff;
        }

        if (other.CompareTag("DropValid"))
        {
            movementDestination = other.transform.position;
        }
        else if (other.CompareTag("DropInvalid"))
        {
            movementDestination = LastPosition;
        }
    }
}
