using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DragPlumero : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DragPlumero instance;

    [Header("Configuración de limpieza")]
    public float cleaningDistance = 100f;
    public float cleaningSpeed = 1f;
    public float rotationWhenCleaning = -25f;
    public float shakeSpeed = 10f;         
    public float shakeAmplitude = 10f;     

    private Canvas canvas;
    private RectTransform rectTransform;
    private Image image;
    private Vector3 startPosition;
    private Transform originalParent;

    private bool isDragging = false;
    private bool sobreTelaraña = false;
    private CobwebCleaning telarañaActual;
    private float tiempoSacudida = 0f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        startPosition = rectTransform.localPosition;
        originalParent = rectTransform.parent;
    }

    private void Update()
    {
        if (!isDragging) return;

        ActualizarRotacion();

        if (sobreTelaraña && telarañaActual != null)
        {
            telarañaActual.LimpiarTick(Time.deltaTime * cleaningSpeed);
        }
    }

    private void ActualizarRotacion()
    {
        if (sobreTelaraña)
        {
            tiempoSacudida += Time.deltaTime * shakeSpeed;
            float oscilacion = Mathf.Sin(tiempoSacudida) * shakeAmplitude;
            float angulo = Mathf.Lerp(0f, rotationWhenCleaning, 0.5f + oscilacion * 0.5f);
            rectTransform.rotation = Quaternion.Euler(0, 0, angulo);
        }
        else
        {
            tiempoSacudida = 0f;
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, Quaternion.identity, Time.deltaTime * 10f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        rectTransform.SetParent(canvas.transform, true);
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            rectTransform.localPosition = localPoint;
            DetectarTelarañas(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        rectTransform.SetParent(originalParent, true);
        rectTransform.localPosition = startPosition;
        rectTransform.rotation = Quaternion.identity;
        image.raycastTarget = true;

        sobreTelaraña = false;
        telarañaActual = null;
    }

    private void DetectarTelarañas(Vector2 screenPosition)
    {
        if (CobwebManager.instance == null) return;

        List<CobwebCleaning> activas = CobwebManager.instance.ObtenerTelarañasActivas();
        bool encontro = false;
        CobwebCleaning nueva = null;

        foreach (var t in activas)
        {
            if (!t.puedeInteractuar || !t.gameObject.activeInHierarchy) continue;

            foreach (Camera cam in Camera.allCameras)
            {
                if (!cam.enabled || !cam.gameObject.activeInHierarchy) continue;

                Vector3 posPantalla = cam.WorldToScreenPoint(t.transform.position);
                float dist = Vector2.Distance(screenPosition, posPantalla);

                if (dist <= cleaningDistance)
                {
                    encontro = true;
                    nueva = t;
                    break;
                }
            }

            if (encontro) break;
        }

        sobreTelaraña = encontro;
        telarañaActual = nueva;
    }

    public void ActualizarVisibilidadPlumero()
    {
        if (TaskManager.instance == null) return;
        gameObject.SetActive(TaskManager.instance.EsTareaActiva(0));
    }
}