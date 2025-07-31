using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedStrikethrough : MonoBehaviour
{
    public bool aplicarTachado = false;

    private TMP_Text textoTMP;
    private LocalizeStringEvent localizeStringEvent;

    private void Awake()
    {
        textoTMP = GetComponent<TMP_Text>();
        localizeStringEvent = GetComponent<LocalizeStringEvent>();

        if (localizeStringEvent != null)
        {
            localizeStringEvent.OnUpdateString.AddListener(OnStringChanged);
        }
        else
        {
            Debug.LogWarning($"[LocalizedStrikethrough] No se encontró LocalizeStringEvent en {gameObject.name}. El texto no podrá actualizarse automáticamente.");
        }
    }

    private void OnDestroy()
    {
        if (localizeStringEvent != null)
        {
            localizeStringEvent.OnUpdateString.RemoveListener(OnStringChanged);
        }
    }

    private void OnStringChanged(string nuevoTexto)
    {
        if (textoTMP == null) return;

        if (aplicarTachado)
            textoTMP.text = $"<s>{nuevoTexto}</s>";
        else
            textoTMP.text = nuevoTexto;
    }

    public void ActivarTachado()
    {
        aplicarTachado = true;
        if (localizeStringEvent != null)
            localizeStringEvent.RefreshString();
    }

    public void Reiniciar()
    {
        aplicarTachado = false;
        if (localizeStringEvent != null)
            localizeStringEvent.RefreshString();
    }
}
