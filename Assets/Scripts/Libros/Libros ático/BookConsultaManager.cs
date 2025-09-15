using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BookConsultaManager : MonoBehaviour
{
    public static BookConsultaManager instance;

    [Header("UI")]
    public GameObject panelLectura;
    public TMP_Text textoPagina;
    public TMP_Text contadorPaginas;
    public TMP_Text tituloUI;
    public Button botonAnterior;
    public Button botonSiguiente;
    public Button botonCerrar;

    private BookConsulta libroActual;
    private int paginaActual = 0;

    private void Awake()
    {
        instance = this;
        panelLectura.SetActive(false);

        botonAnterior.onClick.AddListener(PaginaAnterior);
        botonSiguiente.onClick.AddListener(PaginaSiguiente);
        botonCerrar.onClick.AddListener(CerrarLibro);
    }

    public void AbrirLibro(BookConsulta libro)
    {
        libroActual = libro;
        paginaActual = 0;

        panelLectura.SetActive(true);
        tituloUI.text = libro.titulo;
        ActualizarPagina();
    }

    private void ActualizarPagina()
    {
        if (libroActual == null) return;

        textoPagina.text = libroActual.paginas[paginaActual];
        contadorPaginas.text = $"{paginaActual + 1} / {libroActual.paginas.Length}";

        botonAnterior.interactable = paginaActual > 0;
        botonSiguiente.interactable = paginaActual < libroActual.paginas.Length - 1;
    }

    public void PaginaSiguiente()
    {
        if (libroActual != null && paginaActual < libroActual.paginas.Length - 1)
        {
            paginaActual++;
            ActualizarPagina();
        }
    }

    public void PaginaAnterior()
    {
        if (libroActual != null && paginaActual > 0)
        {
            paginaActual--;
            ActualizarPagina();
        }
    }

    public void CerrarLibro()
    {
        panelLectura.SetActive(false);
        libroActual = null;
    }
}
