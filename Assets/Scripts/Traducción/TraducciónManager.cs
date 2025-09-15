using UnityEngine;

public class TraduccionManager : MonoBehaviour
{
    public static TraduccionManager instance;
    private CharacterSpawn characterSpawn;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        characterSpawn = FindFirstObjectByType<CharacterSpawn>();
        if (characterSpawn == null)
        {
            Debug.LogError("CharacterSpawn no encontrado por TraduccionManager.");
        }
    }

    public void EntregarTraduccion()
    {
        GameManager.instance.CompletarTraduccion();
        CameraManager.instance.DesctivarPanelTraduccion();

        if (characterSpawn != null)
            characterSpawn.EndInteraction();
    }
}
