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
    /// <summary>Color of the list's text fields</summary>
    private Color textColor;
    /// <summary>The header's field information</summary>
    private ListHeaderProperty headerProperty;
    /// <summary>The generated buttons</summary>
    private List<GameObject> listMemberButtons = new List<GameObject>();
    /// <summary>The SaveGameInfos that belong to the list members</summary>
    private List<SaveGameInfo> saveGameInfos;
    /// <summary>The currently selected button</summary>
    private GameObject selectedButton;
    /// <summary>Time of the last click on a list list member</summary>
    private float lastClickTime;
    /// <summary>The currently selected header column</summary>
    public GameObject selectedHeader;
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
        saveGameInfos = new List<SaveGameInfo>();
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

        //Initialise a ListHeaderProperty corresponding to the current DynamicListType
        headerProperty = new ListHeaderProperty(listType);
        //Create header members according to the ListHeaderProperty
        for (int i = 0; i < headerProperty.textFields.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
            button.name = i.ToString();
            Button bComp = button.GetComponent<Button>();
            bComp.onClick.AddListener(delegate { HeaderElementPressed(button); });
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
        }
    }

    /// <summary>
    /// Sets up the list members.
    /// </summary>
    private void LoadButtons()
    {
        //Destroy all existing list members
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }



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

                //Get the user's saves from the database
                saveGameInfos = databaseManager.GetSavedGames();

                //Set up a button for each SaveGameInfo
                for (int i = 0; i < saveGameInfos.Count; i++)
                {
                    GameObject button = CreateListMember();

                    Button bComponent = button.GetComponent<Button>();
                    bComponent.onClick.AddListener(delegate { ListElementPressed(button); });
                    button.name = i.ToString();

                    //Set up the text fields of the button
                    CreateTextFields(button, i);
                    listMemberButtons.Add(button);
                }
                break;
            case DynamicListType.LoadList:

                //Set up delete button
                deleteButton.SetActive(true);
                deleteButtonComp.interactable = false;

                //Set up choose button
                chooseButton.GetComponentInChildren<Text>().text = "Betöltés";
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { ChooseButtonClicked(); });

                //Get the user's saves from the database
                saveGameInfos = databaseManager.GetSavedGames();

                //Set up a button for each SaveGameInfo
                for (int i = 0; i < saveGameInfos.Count; i++)
                {
                    GameObject button = CreateListMember();

                    Button bComponent = button.GetComponent<Button>();
                    bComponent.onClick.AddListener(delegate { ListElementPressed(button); });
                    button.name = i.ToString();
                    //Set up the text fields of the button
                    CreateTextFields(button, i);
                    listMemberButtons.Add(button);
                }
                break;
            default:
                deleteButton.SetActive(false);
                break;
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
        GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(content.transform);
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
    public void CreateTextFields(GameObject button, int index)
    {
        //Destroy the text field the prefab already has
        Destroy(button.transform.GetChild(0).gameObject);

        ListMemberText listMemberText;
        switch (listType)
        {
            case DynamicListType.SaveList:

                //Get the displayable text from the SaveGameInfo specified by the given index
                listMemberText = new ListMemberText(saveGameInfos[index]);

                //Create text fields acoording to the new ListMemberText
                for (int i = 0; i < headerProperty.textFields.Length; i++)
                {
                    GameObject textObject = new GameObject($"ButtonTextField{i}");
                    Text textComponent = textObject.AddComponent<Text>();
                    RectTransform rectTransform = textObject.GetComponent<RectTransform>();
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
                break;
            case DynamicListType.LoadList:

                //Get the displayable text from the SaveGameInfo specified by the given index
                listMemberText = new ListMemberText(saveGameInfos[index]);

                //Create text fields acoording to the new ListMemberText
                for (int i = 0; i < headerProperty.textFields.Length; i++)
                {
                    GameObject textObject = new GameObject($"ButtonTextField{i}");
                    Text textComponent = textObject.AddComponent<Text>();
                    RectTransform rectTransform = textObject.GetComponent<RectTransform>();
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
                break;
            default:
                break;
        }



    }



    private void HeaderElementPressed(GameObject button)
    {
        LoadButtons();
    }

    private void ListElementPressed(GameObject clickedButton)
    {

        switch (listType)
        {
            case DynamicListType.SaveList:
                chooseButton.GetComponent<Button>().interactable = true;
                deleteButton.GetComponent<Button>().interactable = true;

                if (clickedButton == selectedButton)
                {
                    if (Time.time - lastClickTime <= 0.5f)
                    {
                        ChooseButtonClicked();
                        //Debug.Log("DoubleClick");
                    }

                }
                else
                {


                    Button clickedButtonComp = clickedButton.GetComponent<Button>();

                    if (selectedButton != null)
                    {
                        Button selectedButtonComp = selectedButton.GetComponent<Button>();
                        selectedButtonComp.colors = normalColorBlock;
                    }

                    clickedButtonComp.colors = selectedColorBlock;

                    selectedButton = clickedButton;
                }
                break;
            case DynamicListType.LoadList:
                chooseButton.GetComponent<Button>().interactable = true;
                deleteButton.GetComponent<Button>().interactable = true;

                if (clickedButton == selectedButton)
                {
                    if (Time.time - lastClickTime <= 0.5f)
                    {
                        ChooseButtonClicked();
                        //Debug.Log("DoubleClick");
                    }

                }
                else
                {


                    Button clickedButtonComp = clickedButton.GetComponent<Button>();

                    if (selectedButton != null)
                    {
                        Button selectedButtonComp = selectedButton.GetComponent<Button>();
                        selectedButtonComp.colors = normalColorBlock;
                    }

                    clickedButtonComp.colors = selectedColorBlock;

                    selectedButton = clickedButton;
                }
                break;
            default:
                break;
        }


        lastClickTime = Time.time;
    }

    public void NewSaveSlotPressed()
    {
        ClearListSelection();
        newSavePanel.SetActive(true);
    }

    public void NewSave()
    {
        Regex rgx = new Regex("[^A-Za-z0-9]");
        bool ready = true;

        if (newSaveInput.text.Equals("") || newSaveInput.text == null)
        {
            ready = false;
            saveFeedbackRect.gameObject.SetActive(true);
            saveFeedbackRect.color = Color.red;
            saveFeedbackText.text = "Ezt a mezõt kötelezõ kitölteni";

        }
        else if (rgx.IsMatch(newSaveInput.text))
        {
            ready = false;
            saveFeedbackRect.gameObject.SetActive(true);
            saveFeedbackRect.color = Color.red;
            saveFeedbackText.text = "Nem tartalmazhat speciális karaktert";

        }
        else
        {
            saveFeedbackRect.gameObject.SetActive(false);


            if (newSaveInput.text.Length < 3)
            {
                ready = false;
                saveFeedbackRect.gameObject.SetActive(true);
                saveFeedbackRect.color = Color.red;
                saveFeedbackText.text = "Minimum 3 karakter";
            }
        }

        if (ready)
        {


            if (databaseManager.CheckIfSaveNameTaken(newSaveInput.text))
            {
                saveFeedbackRect.gameObject.SetActive(true);
                saveFeedbackRect.color = Color.red;
                saveFeedbackText.text = "Már van egy ilyen nevû mentésed";
            }
            else
            {
                saveFeedbackRect.gameObject.SetActive(false);


                string saveName = newSaveInput.text;
                if (gameController.Save(saveName, saveName, false))
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
                PlaySession.saveInfo = saveGameInfos[int.Parse(selectedButton.name)];
                uiEventHandler.SwitchScene(PlaySession.saveInfo.levelName);
                break;
            default:
                break;
        }
    }

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

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);

    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    private void OverwriteSelectedSave()
    {

        SaveGameInfo saveInfo = saveGameInfos[int.Parse(selectedButton.name)];

        if (gameController.Save(saveInfo.saveTitle, saveInfo.fileName, true))
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
            default:
                break;
        }
    }

    private void DeleteSelectedSave()
    {
        confirmationPanel.SetActive(false);

        SaveGameInfo saveInfo = saveGameInfos[int.Parse(selectedButton.name)];

        if (FileManager.DeleteFile(FileManager.saveDirectory, saveInfo.fileName, ".save"))
        {

            databaseManager.DeleteSave(saveInfo.saveTitle);
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


    // Update is called once per frame
    void Update()
    {

    }

    public void Close()
    {
        uiEventHandler.CloseMenu();
    }

    public struct ListMemberText
    {
        public string[] textContents;

        public ListMemberText(SaveGameInfo saveGameInfo)
        {
            TimeSpan time = TimeSpan.FromSeconds(saveGameInfo.elapsedTime);

            textContents = new string[] { saveGameInfo.saveTitle, saveGameInfo.levelName, saveGameInfo.difficulty.ToString(), saveGameInfo.moves.ToString(), time.ToString("hh':'mm':'ss"), saveGameInfo.saveTime.ToString("yyyy-MM-dd HH:mm:ss") };
        }

    }

    public struct ListHeaderProperty
    {
        public string[] colHeaderNames;
        public string[] colDatabaseFields;
        public bool[] defaultSortingMode; //1: ASC 0: DSC
        public float[] textRectAnchors;
        public Text[] textFields;
        public TextAnchor[] textAnchors;

        public ListHeaderProperty(DynamicListType listType)
        {
            switch (listType)
            {
                case DynamicListType.SaveList:
                    colHeaderNames = new string[] { "Mentésnév", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Mentés ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, true, true, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter };
                    break;
                case DynamicListType.LoadList:
                    colHeaderNames = new string[] { "Mentésnév", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Mentés ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, true, true, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter };
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

    public enum DynamicListType
    {
        SaveList, LoadList
    }
}

public struct SaveGameInfo
{
    public string saveTitle;
    public int difficulty;
    public string levelName;
    public string fileName;
    public int moves;
    public DateTime saveTime;
    public float elapsedTime;

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
