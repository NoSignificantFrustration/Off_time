using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DynamicListManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private UIEventHandler uiEventHandler;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private int btnAmount;
    [SerializeField] private DynamicListType listType;
    [SerializeField] private float listMemberHeight;
    [SerializeField] private GameObject header;
    [SerializeField] private int headerFontSize;
    [SerializeField] private int listFontSize;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Text[] confirmationTexts = new Text[2];
    [SerializeField] private Button confirmPanelButton;
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private Text[] feedbackTexts = new Text[2];
    [SerializeField] private Button feedbackPanelButton;
    [SerializeField] private GameObject newSavePanel;
    [SerializeField] private InputField newSaveInput;
    [SerializeField] private Image saveFeedbackRect;
    private Text saveFeedbackText;
    [SerializeField] private GameObject chooseButton;
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private ColorBlock selectedColorBlock;
    private ColorBlock normalColorBlock;
    private ListHeaderProperty headerProperty;
    private List<GameObject> listMemberButtons = new List<GameObject>();
    private List<SaveGameInfo> saveGameInfos;
    private GameObject selectedButton;
    private float lastClickTime;
    public GameObject selectedHeader;
    private GameObject content;
    private ContentSizeFitter contentFitter;


    private void Awake()
    {
        content = gameObject.GetComponent<ScrollRect>().content.gameObject;
        contentFitter = content.GetComponent<ContentSizeFitter>();
        normalColorBlock = buttonPrefab.GetComponent<Button>().colors;
        saveGameInfos = new List<SaveGameInfo>();
        saveFeedbackText = saveFeedbackRect.GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lastClickTime = Time.time;

        //SetupHeader();
        //LoadButtons();

        //Canvas.ForceUpdateCanvases();
    }

    public void ReloadList(DynamicListType listType)
    {
        this.listType = listType;
        SetupHeader();
        LoadButtons();

    }


    private void SetupHeader()
    {

        foreach (Transform child in header.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        headerProperty = new ListHeaderProperty(listType);
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

    private void LoadButtons()
    {
        foreach (Transform child in content.transform)
        {
            GameObject.Destroy(child.gameObject);
        }



        Button chooseButtonComp = chooseButton.GetComponent<Button>();
        Button deleteButtonComp = deleteButton.GetComponent<Button>();

        switch (listType)
        {
            case DynamicListType.SaveList:

                deleteButton.SetActive(true);
                deleteButtonComp.interactable = false;

                chooseButton.GetComponentInChildren<Text>().text = "Ment�s";
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { ChooseButtonClicked(); });

                GameObject newSaveButton = CreateListMember();
                Button newSaveButtonComponent = newSaveButton.GetComponent<Button>();
                newSaveButtonComponent.onClick.AddListener(delegate { NewSaveSlotPressed(); });
                Text textComponent = newSaveButtonComponent.GetComponentInChildren<Text>();
                textComponent.text = "�j ment�s";
                textComponent.fontSize = (int)listMemberHeight - 10;
                listMemberButtons.Add(newSaveButton);

                saveGameInfos = databaseManager.GetSavedGames();
                for (int i = 0; i < saveGameInfos.Count; i++)
                {
                    GameObject button = CreateListMember();

                    Button bComponent = button.GetComponent<Button>();
                    bComponent.onClick.AddListener(delegate { ListElementPressed(button); });
                    button.name = i.ToString();
                    CreateTextFields(button, i);
                    listMemberButtons.Add(button);
                }
                break;
            case DynamicListType.LoadList:

                deleteButton.SetActive(true);
                deleteButtonComp.interactable = false;

                chooseButton.GetComponentInChildren<Text>().text = "Bet�lt�s";
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { ChooseButtonClicked(); });

                saveGameInfos = databaseManager.GetSavedGames();
                for (int i = 0; i < saveGameInfos.Count; i++)
                {
                    GameObject button = CreateListMember();

                    Button bComponent = button.GetComponent<Button>();
                    bComponent.onClick.AddListener(delegate { ListElementPressed(button); });
                    button.name = i.ToString();
                    CreateTextFields(button, i);
                    listMemberButtons.Add(button);
                }
                break;
            default:
                deleteButton.SetActive(false);
                break;
        }




        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public GameObject CreateListMember()
    {
        GameObject button = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(content.transform);
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
        rectTransform.localPosition = new Vector3(0f, listMemberButtons.Count * -listMemberHeight, 0f);
        rectTransform.sizeDelta = new Vector2(0f, listMemberHeight);
        Color imageColor = new Color(1f, 1f, 1f, 0.470f);
        button.GetComponent<Image>().color = imageColor;
        return button;
    }

    public void CreateTextFields(GameObject button, int index)
    {

        Destroy(button.transform.GetChild(0).gameObject);

        ListMemberText listMemberText;
        switch (listType)
        {
            case DynamicListType.SaveList:

                listMemberText = new ListMemberText(saveGameInfos[index]);

                for (int i = 0; i < headerProperty.textFields.Length; i++)
                {
                    GameObject textObject = new GameObject($"ButtonTextField{i}");
                    Text textComponent = textObject.AddComponent<Text>();
                    RectTransform rectTransform = textObject.GetComponent<RectTransform>();
                    textComponent.text = listMemberText.textContents[i];
                    textComponent.fontSize = listFontSize;
                    textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    textComponent.alignment = headerProperty.textAnchors[i];
                    rectTransform.SetParent(button.transform);
                    rectTransform.anchorMin = new Vector2(headerProperty.textRectAnchors[i], 0f);
                    rectTransform.anchorMax = new Vector2(headerProperty.textRectAnchors[i + 1], 1f);
                    rectTransform.pivot = new Vector2(0f, 1f);
                    rectTransform.localScale = new Vector3(1f, 1f, 1f);
                    rectTransform.localPosition = Vector3.zero;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                    rectTransform.offsetMin = Vector2.zero;
                }
                break;
            case DynamicListType.LoadList:

                listMemberText = new ListMemberText(saveGameInfos[index]);

                for (int i = 0; i < headerProperty.textFields.Length; i++)
                {
                    GameObject textObject = new GameObject($"ButtonTextField{i}");
                    Text textComponent = textObject.AddComponent<Text>();
                    RectTransform rectTransform = textObject.GetComponent<RectTransform>();
                    textComponent.text = listMemberText.textContents[i];
                    textComponent.fontSize = listFontSize;
                    textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    textComponent.alignment = headerProperty.textAnchors[i];
                    rectTransform.SetParent(button.transform);
                    rectTransform.anchorMin = new Vector2(headerProperty.textRectAnchors[i], 0f);
                    rectTransform.anchorMax = new Vector2(headerProperty.textRectAnchors[i + 1], 1f);
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
            saveFeedbackText.text = "Ezt a mez�t k�telez� kit�lteni";

        }
        else if (rgx.IsMatch(newSaveInput.text))
        {
            ready = false;
            saveFeedbackRect.gameObject.SetActive(true);
            saveFeedbackRect.color = Color.red;
            saveFeedbackText.text = "Nem tartalmazhat speci�lis karaktert";

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
                saveFeedbackText.text = "M�r van egy ilyen nev� ment�sed";
            }
            else
            {
                saveFeedbackRect.gameObject.SetActive(false);


                string saveName = newSaveInput.text;
                if (gameController.Save(saveName, saveName, false))
                {
                    newSaveInput.text = "";
                    newSavePanel.SetActive(false);

                    feedbackTexts[0].text = "Ment�s";
                    feedbackTexts[1].text = "Sikeres ment�s";
                    feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
                    feedbackPanelButton.onClick.RemoveAllListeners();
                    feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
                    feedbackPanelButton.onClick.AddListener(delegate { uiEventHandler.CloseMenu(); });
                    feedbackPanel.SetActive(true);
                }
                else
                {
                    feedbackTexts[0].text = "Hiba";
                    feedbackTexts[1].text = "A ment�s sikertelen";
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
                    confirmationTexts[0].text = "Ment�s";
                    confirmationTexts[1].text = "Biztosan fel�l�rod a kiv�lasztott ment�st?\n(Ez a m�velet nem ford�that� vissza)";
                    confirmPanelButton.GetComponentInChildren<Text>().text = "Fel�l�r�s";
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
            feedbackTexts[0].text = "Ment�s";
            feedbackTexts[1].text = "Sikeres ment�s";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanelButton.onClick.AddListener(delegate { uiEventHandler.CloseMenu(); });
            feedbackPanel.SetActive(true);
        }
        else
        {
            feedbackTexts[0].text = "Hiba";
            feedbackTexts[1].text = "A ment�s sikertelen";
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
                    confirmationTexts[0].text = "T�rl�s";
                    confirmationTexts[1].text = "Biztosan t�rl�d a kiv�lasztott ment�st?\n(Ez a m�velet nem ford�that� vissza)";
                    confirmPanelButton.GetComponentInChildren<Text>().text = "T�rl�s";
                    confirmPanelButton.onClick.RemoveAllListeners();
                    confirmPanelButton.onClick.AddListener(delegate { DeleteSelectedSave(); });
                    confirmationPanel.SetActive(true);
                }
                break;
            case DynamicListType.LoadList:
                if (selectedButton != null)
                {
                    confirmationTexts[0].text = "T�rl�s";
                    confirmationTexts[1].text = "Biztosan t�rl�d a kiv�lasztott ment�st?\n(Ez a m�velet nem ford�that� vissza)";
                    confirmPanelButton.GetComponentInChildren<Text>().text = "T�rl�s";
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
            feedbackTexts[0].text = "T�rl�s";
            feedbackTexts[1].text = "A ment�s sikeresen t�r�lve";
            feedbackPanelButton.GetComponentInChildren<Text>().text = "Ok";
            feedbackPanelButton.onClick.RemoveAllListeners();
            feedbackPanelButton.onClick.AddListener(delegate { feedbackPanel.SetActive(false); });
            feedbackPanel.SetActive(true);
        }
        else
        {
            feedbackTexts[0].text = "Hiba";
            feedbackTexts[1].text = "A t�rl�s sikertelen";
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
                    colHeaderNames = new string[] { "Ment�sn�v", "P�lyan�v", "Neh�zs�g", "L�p�sek", "J�t�kid�", "Ment�s ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, true, true, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter };
                    break;
                case DynamicListType.LoadList:
                    colHeaderNames = new string[] { "Ment�sn�v", "P�lyan�v", "Neh�zs�g", "L�p�sek", "J�t�kid�", "Ment�s ideje" };
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
