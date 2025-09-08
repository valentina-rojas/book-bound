using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioManager : MonoBehaviour
{
    public static InventarioManager Instance;

    [Header("UI Inventario")]
    [SerializeField] private GameObject panelInventario;
    [SerializeField] public GameObject prefabSlotItem;

    [Header("Contenedores por pestaña")]
    [SerializeField] private Transform contenedorHerramientas;
    [SerializeField] private Transform contenedorParedes;
    [SerializeField] private Transform contenedorPisos;
    [SerializeField] private Transform contenedorCuadros;
    // puedes añadir más contenedores según categorías

    [Header("Botones de pestañas")]
    [SerializeField] private Button botonPestañaHerramientas;
    [SerializeField] private Button botonPestañaParedes;
    [SerializeField] private Button botonPestañaPisos;
    [SerializeField] private Button botonPestañaCuadros;

    [Header("Botón abrir/cerrar inventario")]
    [SerializeField] private Button botonAbrirInventario;
    [SerializeField] private Sprite spriteBotonCerrado;
    [SerializeField] private Sprite spriteBotonAbierto;

    private List<Item> items = new List<Item>();
    private List<UISlotItem> slotsPermanentes = new List<UISlotItem>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelInventario.SetActive(false);

        // Botón abrir/cerrar
        if (botonAbrirInventario != null)
            botonAbrirInventario.onClick.AddListener(ToggleInventario);

        // Botones de pestañas
        if (botonPestañaHerramientas != null)
            botonPestañaHerramientas.onClick.AddListener(() => MostrarPestaña(CategoriaItem.Herramientas));
        if (botonPestañaParedes != null)
            botonPestañaParedes.onClick.AddListener(() => MostrarPestaña(CategoriaItem.Paredes));
        if (botonPestañaPisos != null)
            botonPestañaPisos.onClick.AddListener(() => MostrarPestaña(CategoriaItem.Pisos));
        if (botonPestañaCuadros != null)
            botonPestañaCuadros.onClick.AddListener(() => MostrarPestaña(CategoriaItem.Cuadros));

        InicializarHerramientas();
        MostrarPestaña(CategoriaItem.Herramientas); 
    }

    private void InicializarHerramientas()
    {
        foreach (Transform child in contenedorHerramientas)
        {
            UISlotItem slot = child.GetComponent<UISlotItem>();
            if (slot != null && slot.currentItem != null)
                slotsPermanentes.Add(slot);
        }
    }

    // Activa solo el contenedor correspondiente a la categoría
    private void MostrarPestaña(CategoriaItem categoria)
    {
        contenedorHerramientas.gameObject.SetActive(categoria == CategoriaItem.Herramientas);
        contenedorParedes.gameObject.SetActive(categoria == CategoriaItem.Paredes);
        contenedorPisos.gameObject.SetActive(categoria == CategoriaItem.Pisos);
        contenedorCuadros.gameObject.SetActive(categoria == CategoriaItem.Cuadros);
    }

    public void AgregarItem(Item item, ItemMundo itemMundo)
    {
        if (!items.Contains(item))
            items.Add(item);

        ActualizarUI(item.categoria);
        AbrirInventario();
    }

    public void AgregarItemSinAbrir(Item item)
    {
        if (!items.Contains(item))
            items.Add(item);

        ActualizarUI(item.categoria);
    }

    private void ActualizarUI(CategoriaItem categoria)
    {
        Transform contenedor = ObtenerContenedor(categoria);
        if (contenedor == null) return;

        // Limpiar solo el contenedor de la categoría (excepto herramientas permanentes)
        if (categoria != CategoriaItem.Herramientas)
        {
            foreach (Transform child in contenedor)
                Destroy(child.gameObject);
        }

        foreach (Item i in items)
        {
            if (i.categoria != categoria) continue;

            if (i.categoria == CategoriaItem.Herramientas) continue; // ya permanentes

            GameObject nuevoSlot = Instantiate(prefabSlotItem, contenedor);
            UISlotItem slotItem = nuevoSlot.GetComponent<UISlotItem>();
            if (slotItem != null)
                slotItem.Configurar(i, UsarItem);
        }
    }

    private Transform ObtenerContenedor(CategoriaItem categoria)
    {
        switch (categoria)
        {
            case CategoriaItem.Herramientas: return contenedorHerramientas;
            case CategoriaItem.Paredes: return contenedorParedes;
            case CategoriaItem.Pisos: return contenedorPisos;
            case CategoriaItem.Cuadros: return contenedorCuadros;
            default: return null;
        }
    }

    public void UsarItem(Item item)
    {
        if (item.categoria == CategoriaItem.Paredes || item.categoria == CategoriaItem.Pisos || item.categoria == CategoriaItem.Cuadros)
        {
            SlotLugar slot = EncontrarSlot(item.categoria);
            if (slot != null)
            {
                slot.ColocarItem(item);

                if (item.categoria != CategoriaItem.Herramientas)
                    items.Remove(item);

                ActualizarUI(item.categoria);
            }
        }
    }

    public void EliminarItem(Item item)
    {
        if (items.Contains(item) && item.categoria != CategoriaItem.Herramientas)
        {
            items.Remove(item);
            ActualizarUI(item.categoria);
        }
    }

    private SlotLugar EncontrarSlot(CategoriaItem categoria)
    {
        SlotLugar[] slots = FindObjectsByType<SlotLugar>(FindObjectsSortMode.None);
        foreach (var s in slots)
            if (s.categoriaSlot == categoria)
                return s;

        return null;
    }

    public void ToggleInventario()
    {
        if (panelInventario.activeSelf)
            CerrarInventario();
        else
            AbrirInventario();
    }

    public void AbrirInventario()
    {
        panelInventario.SetActive(true);
        MostrarPestaña(CategoriaItem.Herramientas); 
        if (botonAbrirInventario != null && spriteBotonAbierto != null)
            botonAbrirInventario.image.sprite = spriteBotonAbierto;
    }

    public void CerrarInventario()
    {
        panelInventario.SetActive(false);
        if (botonAbrirInventario != null && spriteBotonCerrado != null)
            botonAbrirInventario.image.sprite = spriteBotonCerrado;
    }

    public void CerrarInventarioDesdeBoton() => CerrarInventario();

    public List<Item> ObtenerItems() => new List<Item>(items);

    public void OcultarBotonAbrirInventario() => botonAbrirInventario?.gameObject.SetActive(false);
    public void MostrarBotonAbrirInventario() => botonAbrirInventario?.gameObject.SetActive(true);

    public void MostrarInventarioCompleto()
    {
        panelInventario.SetActive(true);
        MostrarBotonAbrirInventario();
        MostrarPestaña(CategoriaItem.Herramientas); 
    }

    public void OcultarInventarioCompleto()
    {
        panelInventario.SetActive(false);
        OcultarBotonAbrirInventario();
    }
}
