using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
//using System.Collections;

//using UnityEngine;

public class MenuControl : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    // Reference to canvas controller game object 
    public GameObject menuController;

    // Default text colours
    private Color mainColour = new Color(0.25f, 0.37f, 0.19f, 1);
    private Color highlightColour = new Color(0.27f, 0.18f, 0.18f, 1);
    private Color clickColour = new Color(0.27f, 0.18f, 0.18f, 1);



    // Button types
    public bool startMenu;
    public bool mainMenu;
    public bool terrainMenu;

    public bool quit;

    public bool generateTerrain;
    public bool startGame;

    //public bool newGame;
    //public bool loadButton;
    //public bool mainMenuButton;
    //public bool terrainButton;

    // Start menu button types
    //public bool terrainButton;

    ////public GameObject startCanvas;


    //// Game object for map generation
    //public GameObject mapGenerator;

    void Start()
    {
        // Set the color to mainColour and font style to normal at start
        defaultStyle();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make the text bold and change to highlightColour on mouse enter
        highlightStyle();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Return to the original font style and colour on mouse exit
        defaultStyle();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //clickStyle();

        if (startMenu)
        {
            // Go back to the start menu
            menuController.GetComponent<CanvasController>().EnableCanvas(0);
        }
        if (mainMenu)
        {
            // Activate the main menu canvas
            menuController.GetComponent<CanvasController>().EnableCanvas(1);
        }
        if (terrainMenu)
        {
            // Clear the existing terrain map
            menuController.GetComponent<MapGenerator>().ClearMap();

            // Go to terrain menu
            menuController.GetComponent<CanvasController>().EnableCanvas(2);
        }
        if (quit)
        {
            // This exits the game
            Application.Quit();
        }
        //if (newGameButton)
        //{
        //    // Go back to the terrain generator menu
        //    menuController.GetComponent<canvasController>().EnableCanvas(2);
        //}

        if (generateTerrain)
        {
            
            menuController.GetComponent<MapGenerator>().RandomiseMap();

            // Make sure the map camera is positioned correctly 
            menuController.GetComponent<CanvasController>().FitCameraToMap(menuController.GetComponent<MapGenerator>().width, menuController.GetComponent<MapGenerator>().height);

            menuController.GetComponent<MapGenerator>().GenerateMap();
            // Generate terrain and change text colour
            //gameObject.GetComponent<Text>().color = clickColour;
            //// Randomise dthv
            //mapGenerator.GetComponent<MapGenerator>().RandomiseMap();
            //mapGenerator.GetComponent<MapGenerator>().GenerateMap();
            // Go back to the start menu
            //menuController.GetComponent<canvasController>().EnableCanvas(0);
        }
        if (startGame)
        {
            // Save the terrain data
            WorldInfo saveData = new WorldInfo();
            saveData.heightOffset = menuController.GetComponent<MapGenerator>().heightOffset;
            saveData.biomeOffset = menuController.GetComponent<MapGenerator>().biomeOffset;

            // Save the initial position of the player
            saveData.playerPos = new Vector2(0f, 0f);

            // Save the data 
            DataSaver.saveData(saveData, "newWorldData");

            // Load the main game
            SceneManager.LoadScene("World");
        }
        defaultStyle();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickStyle();
    }

    // Default text colour and style
    void defaultStyle()
    {
        gameObject.GetComponent<Text>().color = mainColour;
        gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;
    }

    // Style on mouse over
    void highlightStyle()
    {
        gameObject.GetComponent<Text>().color = highlightColour;
        gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
    }

    // Text colour and style on mouse click
    void clickStyle()
    {
        gameObject.GetComponent<Text>().color = clickColour;
        gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
    }
}
