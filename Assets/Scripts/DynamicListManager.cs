using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Logic for dynamic UI lists.
/// </summary>
/// <seealso cref="GameController"/>
/// <seealso cref="UIEventHandler"/>
/// <seealso cref="DatabaseManager"/>
/// <seealso cref="DynamicListType"/>
/// <seealso cref="ListHeaderProperty"/>
/// <seealso cref="SaveGameInfo"/>
/// <seealso cref="WinInfo"/>
public class DynamicListManager : MonoBehaviour
{
    /// <summary>The level's GameController</summary>
    [SerializeField] private GameController gameController;
    /// <summary>The level's UIEventHandler</summary>
    [SerializeField] private UIEventHandler uiEventHandler;
    /// <summary>The level's DatabaseManager</summary>
    [SerializeField] private DatabaseManager databaseManager;
    /// <summary>The button prefab the list will use</summary>
    [SerializeField] private GameObject buttonPrefab;
    /// <summary>Number of buttons, debug option</summary>
    [SerializeField] private int btnAmount;
    /// <summary>Type of the list</summary>
    [SerializeField] private DynamicListType listType;
    /// <summary>List member height</summary>
    [SerializeField] private float listMemberHeight;
    /// <summary>Reference to the list header</summary>
    [SerializeField] private GameObject header;
    /// <summary>Header font size</summary>
    [SerializeField] private int headerFontSize;
    /// <summary>List member font size</summary>
    [SerializeField] private int listFontSize;
    /// <summary>Reference to the confirmation panel</summary>
    [SerializeField] private GameObject confirmationPanel;
    /// <summary>References to the confirmation text fields</summary>
    [SerializeField] private Text[] confirmationTexts = new Text[2];
    /// <summary>Reference to the confirmation panel's button</summary>
    [SerializeField] private Button confirmPanelButton;
    /// <summary>Reference to the feedback panel</summary>
    [SerializeField] private GameObject feedbackPanel;
    /// <summary>References to the feedback text fields</summary>
    [SerializeField] private Text[] feedbackTexts = new Text[2];
    /// <summary>Reference to the feedback panel's button</summary>
    [SerializeField] private Button feedbackPanelButton;
    /// <summary>Reference to the new save panel</summary>
    [SerializeField] private GameObject newSavePanel;
    /// <summary>Reference to the new save panel's input field</summary>
    [SerializeField] private InputField newSaveInput;
    /// <summary>Reference to the new save panel's feedback rectangle</summary>
    [SerializeField] private Image saveFeedbackRect;
    /// <summary>Reference to the new save panel's feedback text field</summary>
    private Text saveFeedbackText;
    /// <summary>Reference to the choose button</summary>
    [SerializeField] private GameObject chooseButton;
    /// <summary>Reference to the delete button</summary>
    [SerializeField] private GameObject deleteButton;
    /// <summary>Colors that will be assigned to the selected list member button</summary>
    [SerializeField] private ColorBlock selectedColorBlock;
    /// <summary>Colors that will be assigned to the normal list member button</summary>
    private ColorBlock normalColorBlock;
    /// <summary>Number of functional buttons (fe.: New save button, Add quiz button) before the buttons that have the data on them</summary>
    private int extraButtonCount;
    /// <summary>Color of the list's text fields</summary>
    private Color textColor;
    /// <summary>The header's field information</summary>
    private ListHeaderProperty headerProperty;
    /// <summary>The generated buttons</summary>
    private List<GameObject> listMemberButtons = new List<GameObject>();
    /// <summary>The SaveGameInfos that belong to the list members</summary>
    private List<object> listMemberDatas;
    /// <summary>The currently selected button</summary>
    private GameObject selectedButton;
    /// <summary>Time of the last click on a list list member</summary>
    private float lastClickTime;
    /// <summary>Header element buttons</summary>
    private List<GameObject> headerButtons = new List<GameObject>();
    /// <summary>The currently selected column</summary>
    private int selectedHeader;
    /// <summary>Sorting mode</summary>
    private bool sortingMode;
    /// <summary>The UI element the list is confined to</summary>
    private GameObject content;
    /// <summary>The list's ContentSizeFitter</summary>
    private ContentSizeFitter contentFitter;



    /// <summary>
    /// Gets references when the script is loaded.
    /// </summary>
    private void Awake()
    {
        content = gameObject.GetComponent<ScrollRect>().content.gameObject;
        contentFitter = content.GetComponent<ContentSizeFitter>();
        normalColorBlock = buttonPrefab.GetComponent<Button>().colors;
        listMemberDatas = new List<object>();
        saveFeedbackText = saveFeedbackRect.GetComponentInChildren<Text>();
        textColor = buttonPrefab.GetComponentInChildren<Text>().color;
    }

    /// <summary>
    /// Initialises lastClickTime when the game starts.
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        lastClickTime = Time.time;

        //SetupHeader();
        //LoadButtons();

        //Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Reinitialises the list with as the provided DynamicListType.
    /// </summary>
    /// <param name="listType">New DynamicListType</param>
    public void ReloadList(DynamicListType listType)
    {
        this.listType = listType;
        SetupHeader();
        LoadButtons();

    }

    /// <summary>
    /// Sets up the header.
    /// </summary>
    private void SetupHeader()
    {

        //Destroy all existing header elements
        foreach (Transform child in header.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        headerButtons = new List<GameObject>();
        selectedHeader = new int();


        //Initialise a ListHeaderProperty corresponding to the current DynamicListType
        headerProperty = new ListHeaderProperty(listType);
        //Create header members according to the ListHeaderProperty
        for (int i = 0; i < headerProperty.textFields.Length; i++)
        {
            //Create a new header button 
            GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
            button.name = i.ToString();
            Button bComp = button.GetComponent<Button>();
            int temp = i;
            bComp.onClick.AddListener(delegate { HeaderElementPressed(temp); }); //Add listener
            //Store reference to the new text field in the header properties
            headerProperty.textFields[i] = button.GetComponentInChildren<Text>();
            headerProperty.textFields[i].text = headerProperty.colHeaderNames[i];
            headerProperty.textFields[i].fontSize = headerFontSize;
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(header.transform);
            //Set up the anchors
            rectTransform.anchorMin = new Vector2(headerProperty.textRectAnchors[i], 0f);
            rectTransform.anchorMax = new Vector2(headerProperty.textRectAnchors[i + 1], 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            headerButtons.Add(button);
        }

        //Set default sorting column and mode
        switch (listType)
        {
            case DynamicListType.SaveList:
                HeaderElementPressed(headerButtons.Count - 1);
                break;
            case DynamicListType.LoadList:
                HeaderElementPressed(headerButtons.Count - 1);
                break;
            case DynamicListType.LeaderboardList:
                HeaderElementPressed(headerButtons.Count - 1);
                break;
            case DynamicListType.QuizList:
                HeaderElementPressed(0);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sets up the list members.
    /// </summary>
    public void LoadButtons()
    {
        //Destroy all existing list members
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //Reset some variables
        listMemberButtons = new List<GameObject>();
        extraButtonCount = 0;

        Button chooseButtonComp = chooseButton.GetComponent<Button>();
        Button deleteButtonComp = deleteButton.GetComponent<Button>();

        //Set up list members according to the current DynamicListType
        switch (listType)
        {
            case DynamicListType.SaveList:

                //Set up delete button
                deleteButton.SetActive(true);
                deleteButtonComp.interactable = false;

                //Set up choose button
                chooseButton.SetActive(true);
                chooseButton.GetComponentInChildren<Text>().text = "Mentés";
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { ChooseButtonClicked(); });

                //Add new save button
                GameObject newSaveButton = CreateListMember();
                Button newSaveButtonComponent = newSaveButton.GetComponent<Button>();
                newSaveButtonComponent.onClick.AddListener(delegate { NewSaveSlotPressed(); });
                Text textComponent = newSaveButtonComponent.GetComponentInChildren<Text>();
                textComponent.text = "Új mentés";
                textComponent.fontSize = (int)listMemberHeight - 10;
                listMemberButtons.Add(newSaveButton);
                extraButtonCount++;



                //Get the user's saves from the database
                listMemberDatas = databaseManager.GetSavedGames(headerProperty.colDatabaseFields[selectedHeader], sortingMode);

                break;
            case DynamicListType.LoadList:

                //Set up delete button
                deleteButton.SetActive(true);
                deleteButtonComp.interactable = false;

                //Set up choose button
                chooseButton.SetActive(true);
                chooseButton.GetComponentInChildren<Text>().text = "Betöltés";
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { ChooseButtonClicked(); });

                //Get the user's saves from the database
                listMemberDatas = databaseManager.GetSavedGames(headerProperty.colDatabaseFields[selectedHeader], sortingMode);

                //Set list member height
                listMemberHeight = 80;
                break;
            case DynamicListType.LeaderboardList:

                //Disable choose and delete buttons
                chooseButton.SetActive(false);
                deleteButton.SetActive(false);

                //Get the leaderboard from database
                listMemberDatas = databaseManager.GetLeaderboard(headerProperty.colDatabaseFields[selectedHeader], sortingMode);

                //Set list member height
                listMemberHeight = 80;
                break;
            case DynamicListType.QuizList:

                //Set up delete button
                deleteButton.SetActive(true);
                deleteButtonComp.interactable = false;

                //Set up choose button
                chooseButton.SetActive(true);
                chooseButton.GetComponentInChildren<Text>().text = "Módosítás";
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { ChooseButtonClicked(); });

                //Add new quiz button
                GameObject newQuizButton = CreateListMember();
                Button newQuizButtonComponent = newQuizButton.GetComponent<Button>();
                newQuizButtonComponent.onClick.AddListener(delegate {
                    uiEventHandler.GetRecordAdder().Refresh();
                    uiEventHandler.OpenMenu(uiEventHandler.GetRecordAdder().gameObject);
                });
                Text textComp = newQuizButtonComponent.GetComponentInChildren<Text>();
                textComp.text = "Új kvíz hozzáadása";
                textComp.fontSize = 100;
                listMemberButtons.Add(newQuizButton);
                extraButtonCount++;

                //Get quizes from database
                listMemberDatas = databaseManager.GetUserQuizes(headerProperty.colDatabaseFields[selectedHeader], sortingMode);

                //Set list member height
                listMemberHeight = 180;
                break;
            default:
                deleteButton.SetActive(false);
                chooseButton.SetActive(false);
                break;
        }

        //Set up a button for each listMemberData
        for (int i = 0; i < listMemberDatas.Count; i++)
        {
            GameObject button = CreateListMember();

            Button bComponent = button.GetComponent<Button>();
            int temp = i;
            bComponent.onClick.AddListener(delegate { ListElementPressed(temp + extraButtonCount); });
            button.name = i.ToString();
            //Set up the text fields of the button
            CreateTextFields(button, i);
            listMemberButtons.Add(button);
        }

        //Force an update on the ContentSizeFitter to fit all list members in the scroll box
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    /// <summary>
    /// Creates a list member button.
    /// </summary>
    /// <returns>The created button.</returns>
    public GameObject CreateListMember()
    {
        //Instantiate a new button 
        GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(content.transform);
        //Set up the anchors
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        rectTransform.localPosition = new Vector3(0f, listMemberButtons.Count * -listMemberHeight, 0f); //Set the position of the button
        rectTransform.sizeDelta = new Vector2(0f, listMemberHeight); //Set the height of the button
        Color imageColor = new Color(1f, 1f, 1f, 0.470f);
        button.GetComponent<Image>().color = imageColor;
        return button;
    }

    /// <summary>
    /// Creates text fields in the specified button.
    /// </summary>
    /// <param name="button">Button</param>
    /// <param name="index">Data index</param>
    private void CreateTextFields(GameObject button, int index)
    {
        //Destroy the text field the prefab already has
        Destroy(button.transform.GetChild(0).gameObject);

        ListMemberText listMemberText;
        switch (listType)
        {
            case DynamicListType.SaveList:

                //Get the displayable text from the SaveGameInfo specified by the given index
                listMemberText = new ListMemberText((SaveGameInfo)listMemberDatas[index]);

                break;
            case DynamicListType.LoadList:

                //Get the displayable text from the SaveGameInfo specified by the given index
                listMemberText = new ListMemberText((SaveGameInfo)listMemberDatas[index]);

                break;
            case DynamicListType.LeaderboardList:

                //Get the displayable text from the SaveGameInfo specified by the given index
                listMemberText = new ListMemberText((WinInfo)listMemberDatas[index]);

                break;
            case DynamicListType.QuizList:

                //Get the displayable text from the SaveGameInfo specified by the given index
                listMemberText = new ListMemberText((QuizHandler.QuizData)listMemberDatas[index]);

                break;
            default:
                //This will fail, make sure to cover all list types above
                listMemberText = new ListMemberText((SaveGameInfo)listMemberDatas[index]);
                break;
        }

        //Create text fields acording to the new ListMemberText
        for (int i = 0; i < headerProperty.textFields.Length; i++)
        {
            GameObject textObject = new GameObject($"ButtonTextField{i}"); //Create the GameObject
            Text textComponent = textObject.AddComponent<Text>();
            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            //Set up the text
            textComponent.text = listMemberText.textContents[i];
            textComponent.fontSize = listFontSize;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.alignment = headerProperty.textAnchors[i];
            textComponent.color = textColor;
            rectTransform.SetParent(button.transform);
            //Set up the anchors
            rectTransform.anchorMin = new Vector2(headerProperty.textRectAnchors[i], 0f);
            rectTransform.anchorMax = new Vector2(headerProperty.textRectAnchors[i + 1], 1f);
            //Set up the transform
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }

    }


    /// <summary>
    /// Sorts the list according to the pressed button's column.
    /// </summary>
    /// <param name="index">Pressed button index</param>
    public void HeaderElementPressed(int index)
    {
        if (index == selectedHeader)
        {
            sortingMode = !sortingMode;
        }
        else
        {
            //Get the newly clicked button
            Button clickedButtonComp = headerButtons[index].GetComponent<Button>();
            sortingMode = headerProperty.defaultSortingMode[index]; //Set the new sorting mode

            //Set the previous button's colors back to normal
            Button selectedButtonComp = headerButtons[selectedHeader].GetComponent<Button>();
            selectedButtonComp.colors = normalColorBlock;




            //Set the clicked buttons's color to the selection color
            clickedButtonComp.colors = selectedColorBlock;

            selectedHeader = index;
        }
        LoadButtons(); //Reload the list
    }

    /// <summary>
    /// Selects the clicked button, also detects double clicks and calls ChoseButtonClicked().
    /// </summary>
    /// <param name="index">Pressed button index</param>
    private void ListElementPressed(int index)
    {

        switch (listType)
        {
            case DynamicListType.SaveList:
                chooseButton.GetComponent<Button>().interactable = true;
                deleteButton.GetComponent<Button>().interactable = true;

                if (listMemberButtons[index] == selectedButton)
                {
                    //Double click detection
                    if (Time.time - lastClickTime <= 0.5f)
                    {
                        ChooseButtonClicked();
                        //Debug.Log("DoubleClick");
                    }

                }
                else
                {
                    

                    Button clickedButtonComp = listMemberButtons[index].GetComponent<Button>();

                    //Set the previous button's colors back to normal
                    if (selectedButton != null)
                    {
                        Button selectedButtonComp = selectedButton.GetComponent<Button>();
                        selectedButtonComp.colors = normalColorBlock;
                    }

                    //Set the clicked buttons's color to the selection color
                    clickedButtonComp.colors = selectedColorBlock;

                    selectedButton = listMemberButtons[index];
                }
                break;
            case DynamicListType.LoadList:
                chooseButton.GetComponent<Button>().interactable = true;
                deleteButton.GetComponent<Button>().interactable = true;

                if (listMemberButtons[index] == selectedButton)
                {
                    //Double click detection
                    if (Time.time - lastClickTime <= 0.5f)
                    {
                        ChooseButtonClicked();
                        //Debug.Log("DoubleClick");
                    }

                }
                else
                {


                    Button clickedButtonComp = listMemberButtons[index].GetComponent<Button>();

                    //Set the previous button's colors back to normal
                    if (selectedButton != null)
                    {
                        Button selectedButtonComp = selectedButton.GetComponent<Button>();
                        selectedButtonComp.colors = normalColorBlock;
                    }

                    //Set the clicked buttons's color to the selection color
                    clickedButtonComp.colors = selectedColorBlock;

                    selectedButton = listMemberButtons[index];
                }
                break;
            case DynamicListType.QuizList:
                chooseButton.GetComponent<Button>().interactable = true;
                deleteButton.GetComponent<Button>().interactable = true;

                if (listMemberButtons[index] == selectedButton)
                {
                    //Double click detection
                    if (Time.time - lastClickTime <= 0.5f)
                    {
                        ChooseButtonClicked();
                        //Debug.Log("DoubleClick");
                    }

                }
                else
                {


                    Button clickedButtonComp = listMemberButtons[index].GetComponent<Button>();

                    //Set the previous button's colors back to normal
                    if (selectedButton != null)
                    {
                        Button selectedButtonComp = selectedButton.GetComponent<Button>();
                        selectedButtonComp.colors = normalColorBlock;
                    }

                    //Set the clicked buttons's color to the selection color
                    clickedButtonComp.colors = selectedColorBlock;

                    selectedButton = listMemberButtons[index];
                }
                break;
            default:
                break;
        }

        //Record the click's time
        lastClickTime = Time.time;
    }

    /// <summary>
    /// Clears the list selection and opens the new save panel.
    /// </summary>
    public void NewSaveSlotPressed()
    {
        ClearListSelection();
        newSavePanel.SetActive(true);
    }

    /// <summary>
    /// Makes a new save.
    /// </summary>
    public void NewSave()
    {
        Regex rgx = new Regex("[^A-Za-z0-9]");
        bool ready = true;


        if (newSaveInput.text.Equals("") || newSaveInput.text == null) //Check if the field is empty
        {
            ready = false;
            saveFeedbackRect.gameObject.SetActive(true);
            saveFeedbackRect.color = Color.red;
            saveFeedbackText.text = "Ezt a mezõt kötelezõ kitölteni";

        }
        else if (rgx.IsMatch(newSaveInput.text)) //Check for special characters
        {
            ready = false;
            saveFeedbackRect.gameObject.SetActive(true);
            saveFeedbackRect.color = Color.red;
            saveFeedbackText.text = "Nem tartalmazhat speciális karaktert";

        }
        else
        {
            saveFeedbackRect.gameObject.SetActive(false);


            if (newSaveInput.text.Length < 3) //Check for minimum length
            {
                ready = false;
                saveFeedbackRect.gameObject.SetActive(true);
                saveFeedbackRect.color = Color.red;
                saveFeedbackText.text = "Minimum 3 karakter";
            }
        }

        if (ready)
        {


            if (databaseManager.CheckIfSaveNameTaken(newSaveInput.text)) //Check if the name is taken
            {
                saveFeedbackRect.gameObject.SetActive(true);
                saveFeedbackRect.color = Color.red;
                saveFeedbackText.text = "Már van egy ilyen nevû mentésed";
            }
            else
            {
                saveFeedbackRect.gameObject.SetActive(false);


                string saveName = newSaveInput.text;
                if (gameController.Save(saveName, saveName, false)) //Try to make a non-overwriting save
                {
                    newSaveInput.text = "";
                    newSavePanel.SetActive(false);

                    feedbackTexts[0].text = "Mentés";
                    feedbackTexts[1].text = "Sikeres mentés";
                    feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
                    feedbackPanelButton.onClick.RemoveAllListeners();
                    feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
                    feedbackPanelButton.onClick.AddListener(delegate { uiEventHandler.CloseMenu(); });
                    feedbackPanel.SetActive(true);
                }
                else
                {
                    feedbackTexts[0].text = "Hiba";
                    feedbackTexts[1].text = "A mentés sikertelen";
                    feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
                    feedbackPanelButton.onClick.RemoveAllListeners();
                    feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
                    feedbackPanel.SetActive(true);
                }

            }

        }
    }


    /// <summary>
    /// Called by the choose button, does actions according to the current DynamicListType.
    /// </summary>
    public void ChooseButtonClicked()
    {
        switch (listType)
        {
            case DynamicListType.SaveList:
                if (selectedButton != null)
                {
                    confirmationTexts[0].text = "Mentés";
                    confirmationTexts[1].text = "Biztosan felülírod a kiválasztott mentést?\n(Ez a mûvelet nem fordítható vissza)";
                    confirmPanelButton.GetComponentInChildren<Text>().text = "Felülírás";
                    confirmPanelButton.onClick.RemoveAllListeners();
                    confirmPanelButton.onClick.AddListener(delegate { OverwriteSelectedSave(); });
                    confirmationPanel.SetActive(true);
                }
                break;
            case DynamicListType.LoadList:
                PlaySession.saveInfo = (SaveGameInfo)listMemberDatas[int.Parse(selectedButton.name)];
                uiEventHandler.SwitchScene(PlaySession.saveInfo.levelName);
                break;
            case DynamicListType.QuizList:
                uiEventHandler.GetRecordAdder().Refresh((QuizHandler.QuizData)listMemberDatas[int.Parse(selectedButton.name)]);
                uiEventHandler.OpenMenu(uiEventHandler.GetRecordAdder().gameObject);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Clears the selected item.
    /// </summary>
    public void ClearListSelection()
    {
        if (selectedButton != null)
        {
            Button selectedButtonComp = selectedButton.GetComponent<Button>();
            selectedButtonComp.colors = normalColorBlock;
            selectedButton = null;
        }
        deleteButton.GetComponent<Button>().interactable = false;
        chooseButton.GetComponent<Button>().interactable = false;

    }

    /// <summary>
    /// Opens the specified panel.
    /// </summary>
    /// <param name="panel">Panel to open</param>
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);

    }

    /// <summary>
    /// Closes the specified panel.
    /// </summary>
    /// <param name="panel">Panel to close</param>
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// Overwrites the selected save.
    /// </summary>
    private void OverwriteSelectedSave()
    {
        //Get the SaveGameInfo that corresponds to the clicked button
        SaveGameInfo saveInfo = (SaveGameInfo)listMemberDatas[int.Parse(selectedButton.name)];

        if (gameController.Save(saveInfo.saveTitle, saveInfo.fileName, true)) //Try to do an overwriting save
        {
            confirmationPanel.SetActive(false);
            feedbackTexts[0].text = "Mentés";
            feedbackTexts[1].text = "Sikeres mentés";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanelButton.onClick.AddListener(delegate { uiEventHandler.CloseMenu(); });
            feedbackPanel.SetActive(true);
        }
        else
        {
            feedbackTexts[0].text = "Hiba";
            feedbackTexts[1].text = "A mentés sikertelen";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Deletes the selected item from the list.
    /// </summary>
    public void DeleteButtonClicked()
    {
        switch (listType)
        {
            case DynamicListType.SaveList:
                if (selectedButton != null)
                {
                    confirmationTexts[0].text = "Törlés";
                    confirmationTexts[1].text = "Biztosan törlöd a kiválasztott mentést?\n(Ez a mûvelet nem fordítható vissza)";
                    confirmPanelButton.GetComponentInChildren<Text>().text = "Törlés";
                    confirmPanelButton.onClick.RemoveAllListeners();
                    confirmPanelButton.onClick.AddListener(delegate { DeleteSelectedSave(); });
                    confirmationPanel.SetActive(true);
                }
                break;
            case DynamicListType.LoadList:
                if (selectedButton != null)
                {
                    confirmationTexts[0].text = "Törlés";
                    confirmationTexts[1].text = "Biztosan törlöd a kiválasztott mentést?\n(Ez a mûvelet nem fordítható vissza)";
                    confirmPanelButton.GetComponentInChildren<Text>().text = "Törlés";
                    confirmPanelButton.onClick.RemoveAllListeners();
                    confirmPanelButton.onClick.AddListener(delegate { DeleteSelectedSave(); });
                    confirmationPanel.SetActive(true);
                }
                break;
            case DynamicListType.QuizList:
                confirmationTexts[0].text = "Törlés";
                confirmationTexts[1].text = "Biztosan törlöd a kiválasztott kvízt?\n(Ez a mûvelet nem fordítható vissza)";
                confirmPanelButton.GetComponentInChildren<Text>().text = "Törlés";
                confirmPanelButton.onClick.RemoveAllListeners();
                confirmPanelButton.onClick.AddListener(delegate { DeleteSelectedQuiz(); });
                confirmationPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Deletes the selected save.
    /// </summary>
    private void DeleteSelectedSave()
    {
        confirmationPanel.SetActive(false);

        //Get the SaveGameInfo that corresponds to the clicked button
        SaveGameInfo saveInfo = (SaveGameInfo)listMemberDatas[int.Parse(selectedButton.name)];

        if (FileManager.DeleteFile(FileManager.saveDirectory, saveInfo.fileName, ".save")) //Try to delete the file
        {

            //Delete save record from database
            databaseManager.DeleteSave(saveInfo.saveTitle);
            //Reload the list
            LoadButtons();
            if (saveInfo.saveTitle == PlaySession.saveInfo.saveTitle)
            {
                PlaySession.saveInfo.saveTitle = null;
                PlaySession.saveInfo.fileName = null;
            }
            feedbackTexts[0].text = "Törlés";
            feedbackTexts[1].text = "A mentés sikeresen törölve";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanel.SetActive(true);
        }
        else
        {
            feedbackTexts[0].text = "Hiba";
            feedbackTexts[1].text = "A törlés sikertelen";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanel.SetActive(true);
        }

    }

    private void DeleteSelectedQuiz()
    {
        confirmationPanel.SetActive(false);

        //Get the QuizHandler.QuizData that corresponds to the clicked button
        QuizHandler.QuizData quizData = (QuizHandler.QuizData)listMemberDatas[int.Parse(selectedButton.name)];

        //Delete quiz record from database
        databaseManager.DeleteQuiz(quizData.id);

        //Reload the list
        LoadButtons();

        feedbackTexts[0].text = "Törlés";
        feedbackTexts[1].text = "A kvíz sikeresen törölve";
        feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
        feedbackPanelButton.onClick.RemoveAllListeners();
        feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
        feedbackPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the current menu.
    /// </summary>
    public void Close()
    {
        uiEventHandler.CloseMenu();
    }

    /// <summary>
    /// Struct that turns other data types into flat string arrays.
    /// </summary>
    public struct ListMemberText
    {
        

        /// <summary>
        /// The resulting string array.
        /// </summary>
        public string[] textContents;

        /// <summary>
        /// Turns the provided SaveGameInfo into a string array.
        /// </summary>
        /// <param name="saveGameInfo">Source SaveGameInfo</param>
        public ListMemberText(SaveGameInfo saveGameInfo)
        {
            string[] difficultyNames = new string[] { "Könnyû", "Közepes", "Nehéz" };

            TimeSpan time = TimeSpan.FromSeconds(saveGameInfo.elapsedTime);

            textContents = new string[] { saveGameInfo.saveTitle, saveGameInfo.levelName, difficultyNames[saveGameInfo.difficulty], saveGameInfo.moves.ToString(), time.ToString("hh':'mm':'ss"), saveGameInfo.saveTime.ToString("yyyy-MM-dd HH:mm:ss") };
        }

        /// <summary>
        /// Turns the provided WinInfo into a string array.
        /// </summary>
        /// <param name="winInfo">Source WinInfo</param>
        public ListMemberText(WinInfo winInfo)
        {
            string[] difficultyNames = new string[] { "Könnyû", "Közepes", "Nehéz" };

            TimeSpan time = TimeSpan.FromSeconds(winInfo.elapsedTime);

            textContents = new string[] { winInfo.username, winInfo.levelName, difficultyNames[winInfo.difficulty], winInfo.moves.ToString(), time.ToString("hh':'mm':'ss"), winInfo.saveTime.ToString("yyyy-MM-dd HH:mm:ss") };
        }

        /// <summary>
        /// Turns the provided QuizHandler.QuizData into a string array.
        /// </summary>
        /// <param name="winInfo">Source QuizHandler.QuizData</param>
        public ListMemberText(QuizHandler.QuizData quizData)
        {
            string[] difficultyNames = new string[] { "Könnyû", "Közepes", "Nehéz" };

            textContents = new string[] { difficultyNames[quizData.difficulty], quizData.question, quizData.good_answer, quizData.bad_answers[0], quizData.bad_answers[1], quizData.bad_answers[2] };
        }

    }

    /// <summary>
    /// Struct containing information about how the header and how columns should be contructed.
    /// </summary>
    public struct ListHeaderProperty
    {
        /// <summary>Column header names</summary>
        public string[] colHeaderNames;
        /// <summary>Database field names</summary>
        public string[] colDatabaseFields;
        /// <summary>Default sorting mode</summary>
        public bool[] defaultSortingMode; //1: ASC 0: DSC
        /// <summary>Column anchors</summary>
        public float[] textRectAnchors;
        /// <summary>Text fields</summary>
        public Text[] textFields;
        /// <summary>Text anchors</summary>
        public TextAnchor[] textAnchors;

        /// <summary>
        /// Sets up column properties according to the provided DynamicListType.
        /// </summary>
        /// <param name="listType">List type</param>
        public ListHeaderProperty(DynamicListType listType)
        {
            switch (listType)
            {
                case DynamicListType.SaveList:
                    colHeaderNames = new string[] { "Mentésnév", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Mentés ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, false, false, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter };
                    break;
                case DynamicListType.LoadList:
                    colHeaderNames = new string[] { "Mentésnév", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Mentés ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, false, false, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter };
                    break;
                case DynamicListType.LeaderboardList:
                    colHeaderNames = new string[] { "Felhasználó", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Teljesítés ideje" };
                    colDatabaseFields = new string[] { "userID", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, false, false, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter };
                    break;
                case DynamicListType.QuizList:
                    colHeaderNames = new string[] { "Nehézség", "Kérdés", "Jó válasz", "Rossz válasz 1", "Rossz válasz 2", "Rossz válasz 3" };
                    colDatabaseFields = new string[] { "difficulty", "question", "good_answer", "bad_answer1", "bad_answer2", "bad_answer3" };
                    defaultSortingMode = new bool[] { true, true, true, true, true, true };
                    textRectAnchors = new float[] { 0f, 0.0842f, 0.485f, 0.6125f, 0.74f, 0.871f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleLeft };
                    break;
                default:
                    colHeaderNames = new string[] { };
                    colDatabaseFields = new string[] { };
                    defaultSortingMode = new bool[] { };
                    textRectAnchors = new float[] { };
                    textFields = new Text[] { };
                    textAnchors = new TextAnchor[] { };
                    break;
            }



        }
    }

    /// <summary>
    /// List type enum.
    /// </summary>
    public enum DynamicListType
    {
        SaveList, LoadList, LeaderboardList, QuizList
    }
}

/// <summary>
/// Struct containing basic information about a saved game.
/// </summary>
public struct SaveGameInfo
{
    /// <summary>Title</summary>
    public string saveTitle;
    /// <summary>Difficulty</summary>
    public int difficulty;
    /// <summary>Level name</summary>
    public string levelName;
    /// <summary>File name</summary>
    public string fileName;
    /// <summary>Number of moves made</summary>
    public int moves;
    /// <summary>Time the save was made</summary>
    public DateTime saveTime;
    /// <summary>Time spent in game</summary>
    public float elapsedTime;

    /// <summary>
    /// Initialises a new SaveGameInfo with the provided level name.
    /// </summary>
    /// <param name="levelName">Level name</param>
    public SaveGameInfo(string levelName)
    {
        saveTitle = null;
        difficulty = 0;
        this.levelName = levelName;
        fileName = null;
        moves = 0;
        saveTime = new DateTime();
        elapsedTime = 0f;
    }
}

/// <summary>
/// Struct containing basic information about a level completion.
/// </summary>
public struct WinInfo
{
    /// <summary>Username</summary>
    public string username;
    /// <summary>Level name</summary>
    public string levelName;
    /// <summary>Difficulty</summary>
    public int difficulty;
    /// <summary>Moves made</summary>
    public int moves;
    /// <summary>Time taken</summary>
    public float elapsedTime;
    /// <summary>When it was completed</summary>
    public DateTime saveTime;
}