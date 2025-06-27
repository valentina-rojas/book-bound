using UnityEngine;
using TMPro;

public class ShelfEstante : MonoBehaviour
{
    public string genero;
    public TMP_Text cartelGenero;

    private Color colorOriginal;
    private int cantidadEsperadaActual;

    private void Start()
    {
        if (cartelGenero != null)
        {
            colorOriginal = cartelGenero.color;
            if (colorOriginal.a < 0.1f)
            {
                colorOriginal.a = 1f;
                cartelGenero.color = colorOriginal;
            }
        }

        cantidadEsperadaActual = ShelfManager.instance.ObtenerLibrosEsperadosParaGenero(genero);
    }

    public void VerificarEstante()
    {
        int librosCorrectos = 0;
        bool hayLibroIncorrecto = false;

        foreach (Transform slot in transform)
        {
            if (slot.childCount == 1)
            {
                Transform libro = slot.GetChild(0);
                if (!libro.gameObject.activeInHierarchy) continue;

                BookData data = libro.GetComponent<BookData>();
                if (data != null)
                {
                    if (data.tipoLibro == genero)
                    {
                        librosCorrectos++;
                    }
                    else
                    {
                        hayLibroIncorrecto = true;
                    }
                }
            }
        }

        if (!hayLibroIncorrecto && librosCorrectos == cantidadEsperadaActual)
        {
            cartelGenero.color = new Color(1f, 0.85f, 0f);
        }
        else
        {
            cartelGenero.color = colorOriginal;
        }
    }

    public void ActualizarCantidadEsperada()
    {
        cantidadEsperadaActual = ShelfManager.instance.ObtenerLibrosEsperadosParaGenero(genero);
        VerificarEstante();
    }

    public void MarcarCartelComoCorrecto()
    {
        cartelGenero.color = new Color(1f, 0.85f, 0f); 
    }
}