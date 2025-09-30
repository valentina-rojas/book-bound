using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariables : MonoBehaviour
{
    public static class SessionData
    {
        // Nivel
        public static int level = 1;

        // LevelComplete
        public static int timeComplete;
        public static int goodClients;
        public static int badClients;

        // Dialogo
        public static int timeDialogue;
        public static bool dlgRepeat;

        // Cinematica
        public static bool cinSkipped;

        // Libros
        public static string bookId;
        public static bool bookOpened;
        public static bool selectedCorrectly;

        // TareasCompletadas
        public static int timeTasks;

        // Estante
        public static bool openedBefore;
        public static bool placedCorrect;

        // Portada
        public static bool correctCover;

        // OrdenarPaginas
        public static int timeOrder;

        // Hechizo
        public static bool spell;

        // Traduccion
        public static int timeTranslation;

        // Tienda
        public static bool shopOpened;
        public static int timeInShop;

        // TiendaCompra
        public static string itemName;
        public static int itemCount;
        public static bool purchaseFailed;

        // Quit
        public static string reason;
        public static int strikes;
        public static int charIndex;
        public static int reactionTime;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
