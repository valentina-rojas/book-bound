using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterAttributes))]
public class CharacterAttributesEditor : Editor
{
    #region Propiedades Serialized
    SerializedProperty tipoDePedido;
    SerializedProperty dialogueLinesInicio;
    SerializedProperty dialogueLinesBuena;
    SerializedProperty dialogueLinesMala;
    SerializedProperty spriteRespuestaBuena;
    SerializedProperty spriteRespuestaMala;
    SerializedProperty libroDeseadoID;
    SerializedProperty tipoPreferido;
    SerializedProperty tituloLibroPrestado;
    SerializedProperty pistasReparacion;
    SerializedProperty categoriaLibroReparar;
    SerializedProperty stickersRequeridos;
    SerializedProperty tituloLibroPortada;
    SerializedProperty setStickersDeseado; 
    SerializedProperty pistasPortada;
    SerializedProperty hechizoSolicitado;
    SerializedProperty pistasHechizo;
    SerializedProperty libroDonadoID;
    SerializedProperty libroDevueltoID;       
    SerializedProperty tituloLibroDevuelto;      
    SerializedProperty nombreDelCliente;
    SerializedProperty descripcionPedido;
    SerializedProperty claveMensajeCriptograma;
    SerializedProperty tipoEncantoSolicitado;
    #endregion

    #region OnEnable
    void OnEnable()
    {
        tipoDePedido = serializedObject.FindProperty("tipoDePedido");
        dialogueLinesInicio = serializedObject.FindProperty("dialogueLinesInicio");
        dialogueLinesBuena = serializedObject.FindProperty("dialogueLinesBuena");
        dialogueLinesMala = serializedObject.FindProperty("dialogueLinesMala");
        spriteRespuestaBuena = serializedObject.FindProperty("spriteRespuestaBuena");
        spriteRespuestaMala = serializedObject.FindProperty("spriteRespuestaMala");

        libroDeseadoID = serializedObject.FindProperty("libroDeseadoID");
        tipoPreferido = serializedObject.FindProperty("tipoPreferido");
        tituloLibroPrestado = serializedObject.FindProperty("tituloLibroPrestado");

        pistasReparacion = serializedObject.FindProperty("pistasReparacion");
        categoriaLibroReparar = serializedObject.FindProperty("categoriaLibroReparar");

        stickersRequeridos = serializedObject.FindProperty("stickersRequeridos");
        tituloLibroPortada = serializedObject.FindProperty("tituloLibroPortada");
        setStickersDeseado = serializedObject.FindProperty("setStickersDeseado"); 
        pistasPortada = serializedObject.FindProperty("pistasPortada");

        hechizoSolicitado = serializedObject.FindProperty("hechizoSolicitado");
        pistasHechizo = serializedObject.FindProperty("pistasHechizo");

        libroDonadoID = serializedObject.FindProperty("libroDonadoID");
        libroDevueltoID = serializedObject.FindProperty("libroDevueltoID");           
        tituloLibroDevuelto = serializedObject.FindProperty("tituloLibroDevuelto");   

        nombreDelCliente = serializedObject.FindProperty("nombreDelCliente");
        descripcionPedido = serializedObject.FindProperty("descripcionPedido");
        claveMensajeCriptograma = serializedObject.FindProperty("claveMensajeCriptograma");

        tipoEncantoSolicitado = serializedObject.FindProperty("tipoEncantoSolicitado");
    }
    #endregion

    #region OnInspectorGUI
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTipoYDialogos();
        DrawSpritesRespuesta();
        DrawDatosCliente();
        DrawSeccionPorTipo();

        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #region Secciones Inspector
    private void DrawTipoYDialogos()
    {
        EditorGUILayout.PropertyField(tipoDePedido);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Diálogos", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dialogueLinesInicio);
        EditorGUILayout.PropertyField(dialogueLinesBuena);
        EditorGUILayout.PropertyField(dialogueLinesMala);
    }

    private void DrawSpritesRespuesta()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sprites de Respuesta", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spriteRespuestaBuena);
        EditorGUILayout.PropertyField(spriteRespuestaMala);
    }

    private void DrawDatosCliente()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Datos del Cliente", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(nombreDelCliente);
        EditorGUILayout.PropertyField(descripcionPedido);
    }

    private void DrawSeccionPorTipo()
    {
        CharacterAttributes.TipoDePedido tipo = (CharacterAttributes.TipoDePedido)tipoDePedido.enumValueIndex;
        EditorGUILayout.Space();

        switch (tipo)
        {
            case CharacterAttributes.TipoDePedido.BuscarLibro:
                EditorGUILayout.LabelField("Preferencias del Libro", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(libroDeseadoID);
                EditorGUILayout.PropertyField(tipoPreferido);
                EditorGUILayout.PropertyField(tituloLibroPrestado);
                break;

            case CharacterAttributes.TipoDePedido.RepararLibro:
                EditorGUILayout.LabelField("Reparación", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(categoriaLibroReparar); 
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Pistas para Reparación", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(pistasReparacion, true);
                break;

            case CharacterAttributes.TipoDePedido.HacerPortada:
                EditorGUILayout.LabelField("Portada", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(stickersRequeridos);
                EditorGUILayout.PropertyField(tituloLibroPortada);
                EditorGUILayout.PropertyField(setStickersDeseado); 
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Pistas para la Portada", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(pistasPortada, true); 
                break;

            case CharacterAttributes.TipoDePedido.HechizarLibro:
                EditorGUILayout.LabelField("Hechizo", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(hechizoSolicitado);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Pistas para el Hechizo", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(pistasHechizo, true); 
                break;

            case CharacterAttributes.TipoDePedido.JuegoTrivia:
                EditorGUILayout.LabelField("Trivia", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_preguntasTrivia"), true);
                break;

            case CharacterAttributes.TipoDePedido.DonarLibro:
                EditorGUILayout.LabelField("Donación", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(libroDonadoID);
                break;

            case CharacterAttributes.TipoDePedido.DevolverLibro:
                EditorGUILayout.LabelField("Devolución", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(libroDevueltoID);
                EditorGUILayout.PropertyField(tituloLibroDevuelto);
                break;

            case CharacterAttributes.TipoDePedido.Traduccion:
                EditorGUILayout.LabelField("Traducción", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(claveMensajeCriptograma, new GUIContent("Clave Mensaje Criptograma"));
                break;

            case CharacterAttributes.TipoDePedido.EncantarLibro:
                EditorGUILayout.LabelField("Encanto", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(tipoEncantoSolicitado, new GUIContent("Tipo de Encanto"));
                break;
        }
    }
    #endregion
}