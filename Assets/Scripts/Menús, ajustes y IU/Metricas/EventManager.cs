using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnalyticsEvent = Unity.Services.Analytics.Event;

public class EventManager : MonoBehaviour
{
    public class CinematicaEvent : AnalyticsEvent
    {
        public CinematicaEvent() : base("Cinematica") { }
        public bool skip { set { SetParameter("skip", value); } }
    }

    public class DialogoEvent : AnalyticsEvent
    {
        public DialogoEvent() : base("Dialogo") { }
        public bool repeat { set { SetParameter("repeat", value); } }
        public int timeDialogue { set { SetParameter("timeDialogue", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class LibrosEvent : AnalyticsEvent
    {
        public LibrosEvent() : base("Libros") { }
        public string bookId { set { SetParameter("bookId", value); } }
        public bool opened { set { SetParameter("opened", value); } }
        public bool selectedCorrectly { set { SetParameter("selectedCorrectly", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class TareasCompletadasEvent : AnalyticsEvent
    {
        public TareasCompletadasEvent() : base("TareasCompletadas") { }
        public int timeTasks { set { SetParameter("timeTasks", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class EstanteEvent : AnalyticsEvent
    {
        public EstanteEvent() : base("Estante") { }
        public string bookId { set { SetParameter("bookId", value); } }
        public bool openedBefore { set { SetParameter("openedBefore", value); } }
        public bool placedCorrect { set { SetParameter("placedCorrect", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class PortadaEvent : AnalyticsEvent
    {
        public PortadaEvent() : base("Portada") { }
        public bool correctCover { set { SetParameter("correctCover", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class OrdenarPaginasEvent : AnalyticsEvent
    {
        public OrdenarPaginasEvent() : base("OrdenarPaginas") { }
        public int timeOrder { set { SetParameter("timeOrder", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class HechizoEvent : AnalyticsEvent
    {
        public HechizoEvent() : base("Hechizo") { }
        public bool spell { set { SetParameter("spell", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class TraduccionEvent : AnalyticsEvent
    {
        public TraduccionEvent() : base("Traduccion") { }
        public int timeTranslation { set { SetParameter("timeTranslation", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class TiendaEvent : AnalyticsEvent
    {
        public TiendaEvent() : base("Tienda") { }
        public bool opened { set { SetParameter("opened", value); } }
        public int timeInShop { set { SetParameter("timeInShop", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class TiendaCompraEvent : AnalyticsEvent
    {
        public TiendaCompraEvent() : base("TiendaCompra") { }
        public string itemName { set { SetParameter("itemName", value); } }
        public int cant { set { SetParameter("cant", value); } }
        public bool failed { set { SetParameter("failed", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class InventarioUsoEvent : AnalyticsEvent
    {
        public InventarioUsoEvent() : base("InventarioUso") { }
        public string itemName { set { SetParameter("itemName", value); } }
        public int cant { set { SetParameter("cant", value); } }
        public int level { set { SetParameter("level", value); } }
    }

    public class LevelStartEvent : AnalyticsEvent
    {
        public LevelStartEvent() : base("LevelStart") { }
        public int level { set { SetParameter("level", value); } }
    }

    public class LevelCompleteEvent : AnalyticsEvent
    {
        public LevelCompleteEvent() : base("LevelComplete") { }
        public int level { set { SetParameter("level", value); } }
        public int timeComplete { set { SetParameter("timeComplete", value); } }
        public int goodClients { set { SetParameter("goodClients", value); } }
        public int badClients { set { SetParameter("badClients", value); } }
    }

    public class QuitEvent : AnalyticsEvent
    {
        public QuitEvent() : base("Quit") { }
        public int level { set { SetParameter("level", value); } }
    }
}