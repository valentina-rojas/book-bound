using UnityEngine;

public class PersonasSentadas : MonoBehaviour
{
    [Header("Personas en sala de lectura")]
    [SerializeField] private GameObject[] personasSentadas;


    //aleatorizar en un futuro para que no sean siempre las mismas
    public void ActivarPersonasSentadas()
    {
        foreach (GameObject obj in personasSentadas)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    public void DesactivarPersonasSentadas()
    {
        foreach (GameObject obj in personasSentadas)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
