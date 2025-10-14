using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EncantoManager : MonoBehaviour
{
    public static EncantoManager instance;

    [Header("Configuración de puntos")]
    public List<Transform> puntosEncanto;
    private int indiceActual = 0;
    private bool dibujando = false;
    private bool mousePresionado = false;

    [Header("Botón de finalizar")]
    public Button botonFinalizar; 
    public GameObject panelEncanto; 

    private void Awake()
    {
        instance = this;
        if (botonFinalizar != null)
            botonFinalizar.gameObject.SetActive(false); 
    }

    private void OnEnable()
    {
        IniciarEncanto();
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

        if (TendCat.instance != null)
            TendCat.instance.puedeAcariciar = true;
    }
}