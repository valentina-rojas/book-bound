using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StickerData))]
public class StickerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StickerData data = (StickerData)target;

        data.stickerSet = (StickerSet)EditorGUILayout.EnumPopup("Sticker Set", data.stickerSet);

        StickerID[] validIDs = GetValidIDs(data.stickerSet);
        int currentIndex = System.Array.IndexOf(validIDs, data.stickerID);
        if (currentIndex < 0) currentIndex = 0;

        currentIndex = EditorGUILayout.Popup("Sticker ID", currentIndex, System.Array.ConvertAll(validIDs, s => s.ToString()));
        data.stickerID = validIDs[currentIndex];

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data);
        }
    }

    private StickerID[] GetValidIDs(StickerSet set)
    {
        switch (set)
        {
            case StickerSet.Aventura:
                return new StickerID[]
                {
                    StickerID.Avion_Aventura,
                    StickerID.Barril,
                    StickerID.Cofre,
                    StickerID.Espada,
                    StickerID.Estrella_uno,
                    StickerID.Estrella_dos,
                    StickerID.Estrella_tres,
                    StickerID.Faro,
                    StickerID.Fogata,
                    StickerID.Hoja_uno,
                    StickerID.Hoja_dos,
                    StickerID.Isla,
                    StickerID.Mapa,
                    StickerID.Pajaro,
                    StickerID.Sol_Aventura,
                    StickerID.Sombrero,
                    StickerID.Tiburon,
                    StickerID.Velero
                };
            case StickerSet.Astronomico:
                return new StickerID[]
                {
                    StickerID.Agujero_Negro,
                    StickerID.Asteroide,
                    StickerID.Cohete,
                    StickerID.Cometa,
                    StickerID.Constelacion_uno,
                    StickerID.Constelacion_dos,
                    StickerID.Constelacion_tres,
                    StickerID.Constelacion_cuatro,
                    StickerID.Estrella_Fugas,
                    StickerID.Estrella_Astronomia_uno,
                    StickerID.Estrella_Astronomia_dos,
                    StickerID.Estrella_Astronomia_tres,
                    StickerID.Estrella_Astronomia_cuatro,
                    StickerID.Galaxia_uno,
                    StickerID.Galaxia_dos,
                    StickerID.Nave,
                    StickerID.Planeta_uno,
                    StickerID.Planeta_dos,
                    StickerID.Planeta_tres,
                    StickerID.Planeta_cuatro,
                    StickerID.Planeta_cinco,
                    StickerID.Satelite,
                    StickerID.Sol_Astronomia,
                    StickerID.Telescopio
                };
            case StickerSet.Plantas:
                return new StickerID[]
                {
                    StickerID.Lirio,
                    StickerID.Flor_pomposa,
                    StickerID.Hongo_solo,
                    StickerID.Planta_espinosa,
                    StickerID.Cerezas,
                    StickerID.Nenufar,
                    StickerID.Hoja_corazon,
                    StickerID.Bellota,
                    StickerID.Nabo,
                    StickerID.Carnivora_grande,
                    StickerID.Hoja_plantas,
                    StickerID.Trebol,
                    StickerID.cactus,
                    StickerID.Carnivora_chica,
                    StickerID.Hoja_ondulada,
                    StickerID.Hongos_duo,
                    StickerID.Caracol,
                    StickerID.Sapo,
                    StickerID.Arbol
                };
            default: 
                return new StickerID[]
                {
                    StickerID.Flor_Azul,
                    StickerID.Flor_Morada,
                    StickerID.Flor_Rosa,
                    StickerID.Avion_Default,
                    StickerID.Dragon,
                    StickerID.Escudo,
                    StickerID.Montana,
                    StickerID.Papiro,
                    StickerID.Sol_Default,
                    StickerID.Hongo,
                    StickerID.Velas,
                    StickerID.Mariposa,
                    StickerID.Pluma,
                    StickerID.Luna,
                    StickerID.Brujula,
                    StickerID.Estrella_Morada,
                    StickerID.Estrella_Rosa,
                    StickerID.Estrella_Verde,
                    StickerID.Hoja_Default
                };
        }
    }
}