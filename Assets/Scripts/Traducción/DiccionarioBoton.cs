using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DiccionarioBoton : MonoBehaviour
{
    public string letra; 

    private void Start()
    {
        letra = (letra ?? "").ToUpperInvariant();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (CriptogramaManager.instance != null)
                CriptogramaManager.instance.ColocarLetra(letra);
        });
    }
}

