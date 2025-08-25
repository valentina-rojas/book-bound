using UnityEngine;
using UnityEngine.UI;

public class PlantWithRegadera : MonoBehaviour
{
    public Sprite[] growthStages; 
    public SpriteRenderer plantRenderer;

    [Header("Referencias UI")]
    public RectTransform regaderaUI;
    public RectTransform areaPlantaUI;
    public Slider barraRiegoUI;

    [Header("Parámetros")]
    public float tiempoNecesarioRiego = 2f;

    [Header("Configuración de aparición")]
    public int nivelMinimo = 1;  

    private float tiempoSobrePlanta = 0f;
    private bool isFullyWatered = false;
    private bool regaderaSonando = false;

    [HideInInspector] public bool activaHoy = false; 

    private void Start()
    {
        ReiniciarPlanta();

        if (PlantManager.instance != null)
            PlantManager.instance.RegisterPlant(this);
    }

    private void Update()
    {
        if (activaHoy) 
            VerificarRiegoConArrastre();
    }

    private void VerificarRiegoConArrastre()
    {
        if (isFullyWatered)
            return;

        Vector2 posicionRegadera = RectTransformUtility.WorldToScreenPoint(Camera.main, regaderaUI.position);

        if (RectTransformUtility.RectangleContainsScreenPoint(areaPlantaUI, posicionRegadera, Camera.main))
        {
            if (!barraRiegoUI.gameObject.activeSelf)
                barraRiegoUI.gameObject.SetActive(true);

            tiempoSobrePlanta += Time.deltaTime;
            barraRiegoUI.value = tiempoSobrePlanta / tiempoNecesarioRiego;

            UpdatePlantAppearance();

            if (!regaderaSonando)
            {
                AudioManager.instance.sonidoRegadera.Play();
                regaderaSonando = true;
            }

            if (tiempoSobrePlanta >= tiempoNecesarioRiego)
                FinalizarRiego();
        }
        else
        {
            tiempoSobrePlanta = 0f;
            barraRiegoUI.value = 0f;
            barraRiegoUI.gameObject.SetActive(false);

            UpdatePlantAppearance();

            if (regaderaSonando)
            {
                AudioManager.instance.sonidoRegadera.Stop();
                regaderaSonando = false;
            }
        }
    }

    private void FinalizarRiego()
    {
        isFullyWatered = true;
        tiempoSobrePlanta = tiempoNecesarioRiego;

        barraRiegoUI.value = 1f;
        barraRiegoUI.gameObject.SetActive(false);

        UpdatePlantAppearance();

        if (regaderaSonando)
        {
            AudioManager.instance.sonidoRegadera.Stop();
            regaderaSonando = false;
        }

        if (PlantManager.instance != null)
            PlantManager.instance.NotifyPlantFullyWatered();
    }

    private void UpdatePlantAppearance()
    {
        if (!activaHoy)
        {
            plantRenderer.sprite = growthStages[growthStages.Length - 1];
            return;
        }

        int stage = Mathf.Clamp(
            (int)(tiempoSobrePlanta / tiempoNecesarioRiego * (growthStages.Length - 1)),
            0,
            growthStages.Length - 1
        );

        plantRenderer.sprite = growthStages[stage];
    }

    public void ReiniciarPlanta()
    {
        isFullyWatered = false;
        tiempoSobrePlanta = 0f;

        if (barraRiegoUI != null)
        {
            barraRiegoUI.value = 0f;
            barraRiegoUI.gameObject.SetActive(false);
        }

        activaHoy = false;

        if (growthStages != null && growthStages.Length > 0)
            plantRenderer.sprite = growthStages[growthStages.Length - 1];
    }

    public void ActivarHoy()
    {
        ReiniciarPlanta();
        activaHoy = true;

        if (growthStages != null && growthStages.Length > 0)
            plantRenderer.sprite = growthStages[0];
    }
}