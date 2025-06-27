using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public static ShelfManager instance;
    public AudioSource audioLibroCorrecto;
    private int contadorDesorden = 2;
    private bool librosDesorganizados = false;

    public Dictionary<string, int> librosEsperadosPorGenero = new Dictionary<string, int>()
    {
        { "fantasia", 3 },
        { "misterio", 3 },
        { "pociones", 0 },
        { "herbologia", 3 },
        { "recetas", 0 },
        { "historia", 0 },
        { "terror", 3 },
    };

    private void Awake()
    {
        instance = this;
    }

    public int ObtenerLibrosEsperadosParaGenero(string genero)
    {
        if (librosEsperadosPorGenero.TryGetValue(genero, out int cantidad))
            return cantidad;
        return 0;
    }

    public void RestarLibroEsperadoPorGenero(string genero)
    {
        if (librosEsperadosPorGenero.ContainsKey(genero))
        {
            librosEsperadosPorGenero[genero] = Mathf.Max(0, librosEsperadosPorGenero[genero] - 1);

            ShelfEstante[] estantes = FindObjectsOfType<ShelfEstante>();
            foreach (var estante in estantes)
            {
                if (estante.genero == genero)
                {
                    estante.ActualizarCantidadEsperada();
                }
            }
        }
    }

    public void SumarLibroEsperadoPorGenero(string genero)
    {
        if (librosEsperadosPorGenero.ContainsKey(genero))
        {
            librosEsperadosPorGenero[genero] += 1;

            ShelfEstante[] estantes = FindObjectsOfType<ShelfEstante>();
            foreach (var estante in estantes)
            {
                if (estante.genero == genero)
                {
                    estante.ActualizarCantidadEsperada();
                }
            }
        }
    }

    public void RevisarOrganizacionConDelay()
    {
        Invoke(nameof(RevisarOrganizacion), 0.1f);
    }

    public void RevisarOrganizacion()
    {
        ShelfEstante[] estantes = FindObjectsOfType<ShelfEstante>();
        bool todosCorrectos = true;

        foreach (ShelfEstante estante in estantes)
        {
            int librosCorrectos = 0;
            bool hayLibroIncorrecto = false;

            foreach (Transform slot in estante.transform)
            {
                if (slot.childCount == 1)
                {
                    Transform libro = slot.GetChild(0);
                    if (libro == null || !libro.gameObject.activeInHierarchy)
                        continue;

                    BookData book = libro.GetComponent<BookData>();
                    if (book != null)
                    {
                        if (book.tipoLibro == estante.genero)
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

            if (librosCorrectos != ObtenerLibrosEsperadosParaGenero(estante.genero) || hayLibroIncorrecto)
            {
                todosCorrectos = false;
            }

            estante.VerificarEstante();
        }

        if (todosCorrectos)
        {
            Debug.Log("🎉 Todos los libros visibles están correctamente organizados.");
            TaskManager.instance.CompletarTareaPorID(1);
            MarcarTodosLosCartelesComoCorrectos();
        }
    }

    public void MarcarTodosLosCartelesComoCorrectos()
    {
        ShelfEstante[] estantes = FindObjectsOfType<ShelfEstante>();
        foreach (var estante in estantes)
        {
            estante.MarcarCartelComoCorrecto();
        }
    }

    public void AvanzarContadorDesorden()
    {
        contadorDesorden++;
        if (contadorDesorden > 5)
            contadorDesorden = 1;
    }

    public void DesorganizarLibros()
    {
        List<Transform> librosActivos = new List<Transform>();
        ShelfEstante[] estantes = FindObjectsOfType<ShelfEstante>();
        List<Transform> slotsDisponibles = new List<Transform>();

        foreach (ShelfEstante estante in estantes)
        {
            foreach (Transform slot in estante.transform)
            {
                if (slot.childCount > 0)
                {
                    Transform libro = slot.GetChild(0);
                    if (libro.gameObject.activeSelf)
                    {
                        librosActivos.Add(libro);
                        libro.SetParent(null);
                    }
                }
                slotsDisponibles.Add(slot);
            }
        }

        Shuffle(slotsDisponibles);

        for (int i = 0; i < librosActivos.Count && i < slotsDisponibles.Count; i++)
        {
            Transform libro = librosActivos[i];
            Transform nuevoSlot = slotsDisponibles[i];

            libro.SetParent(nuevoSlot);
            libro.localPosition = Vector3.zero;
        }

        foreach (ShelfEstante estante in estantes)
        {
            estante.VerificarEstante();
        }

        Debug.Log($"🔀 Libros reorganizados: {librosActivos.Count}");
    }

    public void IntentarDesorganizarLibros()
    {
        RevisarOrganizacion();
        if (contadorDesorden == 1 && !librosDesorganizados)
        {
            DesorganizarLibros();
            librosDesorganizados = true;
        }
    }

    public void ReiniciarEstado()
    {
        librosDesorganizados = false;
    }

    private void Shuffle<T>(List<T> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            int randomIndex = Random.Range(i, lista.Count);
            T temp = lista[i];
            lista[i] = lista[randomIndex];
            lista[randomIndex] = temp;
        }
    }
}