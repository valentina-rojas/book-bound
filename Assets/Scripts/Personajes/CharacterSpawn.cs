using UnityEngine;
using System.Collections;

public class CharacterSpawn : MonoBehaviour
{
    private GameObject[] characters;

    public Transform spawnPoint;
    public Transform destination;

    private int currentIndex = 0;
    private bool interactionFinished = false;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip sonidoAparicionPersonaje;

    public void AsignarPersonajesDelNivel(GameObject[] personajesDelNivel)
    {
        characters = personajesDelNivel;
    }

    public void ComenzarSpawn()
    {
        currentIndex = 0;
        StartCoroutine(SpawnCharacters());
    }

    IEnumerator SpawnCharacters()
    {
        while (currentIndex < characters.Length)
        {
            GameObject candidate = characters[currentIndex];
            CharacterAttributes atributos = candidate.GetComponent<CharacterAttributes>();

            if (atributos != null && atributos.tipoDePedido == CharacterAttributes.TipoDePedido.DevolverLibro)
            {
                HistorialManager historial = FindFirstObjectByType<HistorialManager>();
                if (historial == null || !historial.GetLibrosPrestados().Contains(atributos.tituloLibroDevuelto))
                {
                    Debug.Log($"[SKIP] {atributos.nombreDelCliente} no será instanciado porque el libro '{atributos.tituloLibroDevuelto}' no está en el historial.");
                    currentIndex++;
                    continue;
                }
            }

            GameObject currentCharacter = Instantiate(candidate, spawnPoint.position, Quaternion.identity);

            if (audioSource != null && sonidoAparicionPersonaje != null)
            {
                audioSource.PlayOneShot(sonidoAparicionPersonaje);
            }

            interactionFinished = false;
            CharacterManager.instance.ResetearAtencion();

            DialogueManager dialogueManager = currentCharacter.GetComponent<DialogueManager>();

            if (atributos != null)
            {
                GameManager.instance.EstablecerPersonajeActual(atributos);
                GameManager.instance.resultadoRecomendacion = GameManager.ResultadoRecomendacion.Ninguna;
            }
            else
            {
                Debug.LogError("El personaje instanciado no tiene CharacterAttributes.");
            }

            yield return StartCoroutine(MoveCharacter(currentCharacter, destination.position));

            yield return new WaitUntil(() => interactionFinished);

            yield return StartCoroutine(MoveCharacter(currentCharacter, spawnPoint.position));

            Destroy(currentCharacter);

            BookManager.instance.DeshabilitarBotonConfirmacion();

            currentIndex++;

            yield return new WaitForSeconds(2f);
        }

        Debug.Log("Todos los personajes han pasado.");
        GameManager.instance.FinDeNivel();
    }

    IEnumerator MoveCharacter(GameObject character, Vector3 targetPosition)
    {
        float duration = 2f;
        float elapsedTime = 0f;
        Vector3 startPosition = character.transform.position;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            character.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }

        character.transform.position = targetPosition;

        HabilitarDialogo();
    }

    public void EndInteraction()
    {
        if (!interactionFinished)
        {
            StartCoroutine(MostrarDialogoDeResultado());
        }
    }

    private IEnumerator MostrarDialogoDeResultado()
    {
        DialogueManager dialogueManager = FindObjectOfType<CharacterAttributes>()?.gameObject.GetComponent<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.EmpezarDialogoResultado();
            yield return new WaitUntil(() => dialogueManager.HaTerminadoElDialogo());
        }
        else
        {
            Debug.LogError("DialogueManager no encontrado.");
        }

        interactionFinished = true;
    }

    public void FinalizarInteraccion()
    {
        interactionFinished = true;
    }

    private void HabilitarDialogo()
    {
        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.EnableDialogue();
            Debug.Log("Dialogue habilitado.");
        }
        else
        {
            Debug.LogError("DialogueManager no encontrado al habilitar diálogo.");
        }
    }
}
