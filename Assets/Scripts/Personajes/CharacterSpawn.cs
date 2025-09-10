using UnityEngine;
using System.Collections;

public class CharacterSpawn : MonoBehaviour
{
    private GameObject[] characters;

    public Transform spawnPoint;
    public Transform destination;

    private int currentIndex = 0;
    private bool interactionFinished = false;

    private GameObject currentCharacter;

    public void AsignarPersonajesDelNivel(GameObject[] personajesDelNivel)
    {
        characters = new GameObject[personajesDelNivel.Length];
        for (int i = 0; i < personajesDelNivel.Length; i++)
        {
            characters[i] = personajesDelNivel[i];
        }
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
                if (historial == null || !historial.GetLibrosPrestados().Exists(l => l.titulo == atributos.tituloLibroDevuelto))
                {
                    Debug.Log($"[SKIP] {atributos.nombreDelCliente} no será instanciado porque el libro '{atributos.tituloLibroDevuelto}' no está en el historial.");
                    currentIndex++;
                    continue;
                }
            }

            currentCharacter = Instantiate(candidate, spawnPoint.position, Quaternion.identity);

            AudioManager.instance.sonidoCampanilla.Play();

            interactionFinished = false;
            CharacterManager.instance.ResetearAtencion();

            atributos = currentCharacter.GetComponent<CharacterAttributes>();
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
        DialogueManager dialogueManager = currentCharacter?.GetComponent<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.EmpezarDialogoResultado();
            yield return new WaitUntil(() => dialogueManager.HaTerminadoElDialogo());
        }
        else
        {
            Debug.LogError("DialogueManager no encontrado en el personaje actual.");
        }

        interactionFinished = true;
    }

    public void FinalizarInteraccion()
    {
        interactionFinished = true;
    }

    private void HabilitarDialogo()
    {
        DialogueManager dialogueManager = currentCharacter?.GetComponent<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.EnableDialogue();
        }
        else
        {
            Debug.LogError("DialogueManager no encontrado al habilitar diálogo.");
        }
    }
}