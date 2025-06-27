using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Diálogo")]
    [SerializeField] private Button dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button botonSiguiente;
    [SerializeField] private TMP_Text botonSiguienteTexto;
    [SerializeField] private Button botonRepetir;
    [SerializeField] private Button botonFinalizar;
    public Button GetDialogueMark() => dialogueMark;
    public GameObject GetDialoguePanel() => dialoguePanel;
    public TMP_Text GetDialogueText() => dialogueText;
    public Button GetBotonSiguiente() => botonSiguiente;
    public TMP_Text GetBotonSiguienteTexto() => botonSiguienteTexto;
    public Button GetBotonRepetir() => botonRepetir;
    public Button GetBotonFinalizar() => botonFinalizar;

    [Header("Panel Unificado")]
    [SerializeField] private GameObject panelHistorial; // Ahora contiene ambos contenidos
    [SerializeField] private Button botonCerrarHistorial;
    [SerializeField] private Button botonAbrirHistorial;

    [Header("Historial de Pedido")]
    [SerializeField] private Transform historialContent;
    [SerializeField] private GameObject prefabEntradaHistorial;

    [Header("Libros Prestados")]
    [SerializeField] private Transform librosContent;
    [SerializeField] private GameObject prefabEntradaLibro;

    public GameObject GetPanelHistorial() => panelHistorial;
    public Button GetBotonCerrarHistorial() => botonCerrarHistorial;
    public Button GetBotonAbrirHistorial() => botonAbrirHistorial;
    public Transform GetHistorialContent() => historialContent;
    public GameObject GetPrefabEntradaHistorial() => prefabEntradaHistorial;
    public Transform GetLibrosContent() => librosContent;
    public GameObject GetPrefabEntradaLibro() => prefabEntradaLibro;
}