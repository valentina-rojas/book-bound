using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EncantoManager : MonoBehaviour
{
    public static EncantoManager instance;

    [Header("Sets de puntos para distintos encantos")]
    public GameObject setMalDeOjo;
    public GameObject setResfriado;
    public GameObject setHongos;
    public GameObject setVerrugas;

    [Header("Configuración de puntos (automático)")]
    public List<Transform> puntosEncanto = new List<Transform>();
    private GameObject setActivo;

    private int indiceActual = 0;
    private bool dibujando = false;
    private bool mousePresionado = false;

    [Header("Botón de finalizar")]
    public Button botonFinalizar; 
    public GameObject panelEncanto; 

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(false); 
    }

    private void Update()
    {
        if (mousePresionado && Input.GetMouseButtonUp(0))
        {
            mousePresionado = false;

            if (dibujando && indiceActual < puntosEncanto.Count)
            {
                ReiniciarEncanto();
            }
        }
    }

    public void ActivarEncanto(CharacterAttributes.TipoEncanto tipoEncanto)
    {
        gameObject.SetActive(true);
        StartCoroutine(EsperarYSeleccionarSet(tipoEncanto));
    }

    private IEnumerator EsperarYSeleccionarSet(CharacterAttributes.TipoEncanto tipoEncanto)
    {
        yield return null; 
        SeleccionarSetEncantoActual(tipoEncanto);
        IniciarEncanto();
    }

    private void SeleccionarSetEncantoActual(CharacterAttributes.TipoEncanto tipoEncanto)
    {
        if (setMalDeOjo != null) setMalDeOjo.SetActive(false);
        if (setResfriado != null) setResfriado.SetActive(false);
        if (setHongos != null) setHongos.SetActive(false);
        if (setVerrugas != null) setVerrugas.SetActive(false);

        switch (tipoEncanto)
        {
            case CharacterAttributes.TipoEncanto.MalDeOjo:
                setActivo = setMalDeOjo;
                break;
            case CharacterAttributes.TipoEncanto.Resfriado:
                setActivo = setResfriado;
                break;
            case CharacterAttributes.TipoEncanto.Hongos:
                setActivo = setHongos;
                break;
            case CharacterAttributes.TipoEncanto.Verrugas:
                setActivo = setVerrugas;
                break;
            default:
                Debug.LogWarning("El tipo de encanto no es válido.");
                setActivo = null;
                return;
        }

        if (setActivo != null)
        {
            setActivo.SetActive(true);

            puntosEncanto.Clear();
           for (int i = 0; i < setActivo.transform.childCount; i++)
            {
                if (i == 0) continue; // ignora el primer hijo
                puntosEncanto.Add(setActivo.transform.GetChild(i));
            }
            Debug.Log($"Encanto seleccionado: {tipoEncanto} - puntos cargados: {puntosEncanto.Count}");
        }
    }

    public void IniciarEncanto()
    {
        indiceActual = 0;
        dibujando = false;
        mousePresionado = false;

        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(false);

        foreach (var p in puntosEncanto)
        {
            var img = p.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }

        if (puntosEncanto.Count > 0)
            CambiarColorPunto(0, Color.red);
    }

    public void ComenzarDibujo(int indice)
    {
        if (indice == 0 && !dibujando)
        {
            dibujando = true;
            mousePresionado = true;
            CambiarColorPunto(0, Color.yellow);
            indiceActual = 1;

            if (indiceActual < puntosEncanto.Count)
                CambiarColorPunto(indiceActual, Color.red);
        }
    }

    public void PasarPorPunto(int indice)
    {
        if (!dibujando || !mousePresionado) return;

        if (indice == indiceActual)
        {
            AudioManager.instance.encanto.Play();
            CambiarColorPunto(indice, Color.yellow);
            indiceActual++;

            if (indiceActual < puntosEncanto.Count)
                CambiarColorPunto(indiceActual, Color.red);
            else
                CompletarEncanto();
        }
        else if (indice > indiceActual)
        {
            ReiniciarEncanto();
        }
    }

    private void CambiarColorPunto(int indice, Color color)
    {
        if (indice < 0 || indice >= puntosEncanto.Count) return;
        var img = puntosEncanto[indice].GetComponent<Image>();
        if (img != null) img.color = color;
    }

    private void CompletarEncanto()
    {
        dibujando = false;
        mousePresionado = false;

        GameManager.instance.CompletarEncanto();
        AudioManager.instance.sonidoLibroCorrecto.Play();

        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(true);
    }

    private void ReiniciarEncanto()
    {
        dibujando = false;
        mousePresionado = false;
        indiceActual = 0;

        foreach (var p in puntosEncanto)
        {
            var img = p.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }

        if (puntosEncanto.Count > 0)
            CambiarColorPunto(0, Color.red);
    }

    public void FinalizarEncanto()
    {
        if (panelEncanto != null)
            panelEncanto.SetActive(false);

        InventarioManager.Instance.MostrarInventarioCompleto();
        HistorialManager.Instance.MostrarBotonAbrirHistorial();
        EconomyManager.instance.MostrarContenedorDinero();
        TaskManager.instance.MostrarListaTareas();
        GameManager.instance.CompletarEncanto();
        if (FindFirstObjectByType<CharacterSpawn>() != null)
        {
            FindFirstObjectByType<CharacterSpawn>().EndInteraction();
        }

        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }
}