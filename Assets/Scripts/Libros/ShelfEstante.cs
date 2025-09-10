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
        VerificarEstante();
    }

    public void VerificarEstante()
    {
        int librosCorrectos = 0;
        int librosTotales = 0;
        bool hayLibroIncorrecto = false;

        foreach (Transform slot in transform)
        {
            if (slot.childCount == 1)
            {
                Transform libro = slot.GetChild(0);
                if (!libro.gameObject.activeInHierarchy) continue;

                librosTotales++;
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

        Debug.Log($"Estante {genero}: Correctos={librosCorrectos}, Esperados={cantidadEsperadaActual}, Incorrectos={hayLibroIncorrecto}");

        if (hayLibroIncorrecto || librosCorrectos != cantidadEsperadaActual)
        {
            cartelGenero.color = colorOriginal;
            Debug.Log($"Estante {genero} NO está correcto");
        }
        else
        {
            cartelGenero.color = new Color(1f, 0.85f, 0f);
            Debug.Log($"Estante {genero} está correcto");
        }
    }

    public void ActualizarCantidadEsperada()
    {
        cantidadEsperadaActual = ShelfManager.instance.ObtenerLibrosEsperadosParaGenero(genero);
        Debug.Log($"Estante {genero}: Nueva cantidad esperada = {cantidadEsperadaActual}");
        VerificarEstante();
    }

    public void MarcarCartelComoCorrecto()
    {
        cartelGenero.color = new Color(1f, 0.85f, 0f);
    }
}