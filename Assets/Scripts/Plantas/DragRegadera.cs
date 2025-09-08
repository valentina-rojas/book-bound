using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DragRegadera : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector3 posicionInicial;
    private Image image;
    private Transform parentOriginal;

    [Header("Configuración")]
    public float rotationWhenWatering = 45f;
    public float wateringDistance = 100f;

    private PlantWithRegadera plantaActual;
    private bool sobrePlanta = false;
    private bool estaArrastrando = false;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        posicionInicial = rectTransform.localPosition;
        parentOriginal = rectTransform.parent;
    }

    private void Update()
    {
        if (estaArrastrando && sobrePlanta && plantaActual != null)
        {
            plantaActual.RegarTick(Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        rectTransform.SetParent(canvas.transform, true);
        estaArrastrando = true;
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
            DetectarPlantas(eventData.position);
            rectTransform.rotation = Quaternion.Euler(0, 0, sobrePlanta ? rotationWhenWatering : 0f);
        }
    }

    private void DetectarPlantas(Vector2 screenPosition)
    {
        List<PlantWithRegadera> plantasActivas = ObtenerPlantasActivas();
        
        bool encontroPlanta = false;
        PlantWithRegadera nuevaPlanta = null;

        foreach (var planta in plantasActivas)
        {
            if (planta.IsFullyWatered) continue;

            if (EstaSobrePlanta(planta, screenPosition))
            {
                encontroPlanta = true;
                nuevaPlanta = planta;
                break;
            }
        }

        if (encontroPlanta)
        {
            if (nuevaPlanta != plantaActual)
            {
                if (plantaActual != null)
                    plantaActual.DetenerSonidoRegadera();
                
                plantaActual = nuevaPlanta;
            }
        }
        else
        {
            if (plantaActual != null)
            {
                plantaActual.DetenerSonidoRegadera();
                plantaActual = null;
            }
        }

        sobrePlanta = encontroPlanta;
    }

    private List<PlantWithRegadera> ObtenerPlantasActivas()
    {
        List<PlantWithRegadera> activas = new List<PlantWithRegadera>();
        
        if (PlantManager.instance != null)
        {
            foreach (var planta in FindObjectsOfType<PlantWithRegadera>())
            {
                if (planta.activaHoy && !planta.IsFullyWatered && planta.gameObject.activeInHierarchy)
                {
                    activas.Add(planta);
                }
            }
        }
        
        return activas;
    }

    private bool EstaSobrePlanta(PlantWithRegadera planta, Vector2 screenPosition)
    {
        foreach (Camera cam in Camera.allCameras)
        {
            if (!cam.gameObject.activeInHierarchy || !cam.enabled) continue;

            Vector3 plantaScreenPos = cam.WorldToScreenPoint(planta.transform.position);
            
            float distancia = Vector2.Distance(screenPosition, plantaScreenPos);
            float umbralDeteccion = 50f; 

            if (distancia <= umbralDeteccion)
            {
                return true;
            }
        }

        return false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        estaArrastrando = false;
        
        if (plantaActual != null)
        {
            plantaActual.DetenerSonidoRegadera();
            plantaActual = null;
        }

        rectTransform.SetParent(parentOriginal, true);
        rectTransform.localPosition = posicionInicial;
        rectTransform.rotation = Quaternion.identity;
        image.raycastTarget = true;
        
        sobrePlanta = false;
    }
}