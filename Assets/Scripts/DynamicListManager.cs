using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicListManager : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private int btnAmount;
    [SerializeField] private DynamicListType listType;
    [SerializeField] private float listMemberHeight;
    [SerializeField] private GameObject header;
    [SerializeField] private int headerFontSize;
    [SerializeField] private int listFontSize;
    [SerializeField] private GameObject newSavePanel;
    [SerializeField] private GameObject overwriteSavePanel;
    [SerializeField] private GameObject chooseButton;
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
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }


        switch (listType)
        {
            case DynamicListType.SaveList:

                chooseButton.GetComponentInChildren<Text>().text = "Mentés";
                Button chooseButtonComp = chooseButton.GetComponent<Button>();
                chooseButtonComp.onClick.RemoveAllListeners();
                chooseButtonComp.interactable = false;
                chooseButtonComp.onClick.AddListener(delegate { OverwriteSelectedSave(); });

                GameObject newSaveButton = CreateListMember();
                Button newSaveButtonComponent = newSaveButton.GetComponent<Button>();
                newSaveButtonComponent.onClick.AddListener(delegate { NewSaveSlotPressed(); });
                Text textComponent = newSaveButtonComponent.GetComponentInChildren<Text>();
                textComponent.text = "Új mentés";
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

                chooseButton.GetComponentInChildren<Text>().text = "Betöltés";

                for (int i = 0; i < btnAmount; i++)
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
                break;
        }

        if (listType == DynamicListType.SaveList)
        {

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

                listMemberText = new ListMemberText(saveGameInfos[listMemberButtons.Count]);

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
        chooseButton.GetComponent<Button>().interactable = true;

        if (clickedButton == selectedButton)
        {
            if (Time.time - lastClickTime <= 0.5f)
            {
                OverwriteSelectedSave();
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
        lastClickTime = Time.time;
    }

    public void NewSaveSlotPressed()
    {
        ClearListSelection();
        newSavePanel.SetActive(true);
    }

    public void ChooseButtonClicked()
    {
        switch (listType)
        {
            case DynamicListType.SaveList:
                if (selectedButton != null)
                {
                    OverwriteSelectedSave();
                }
                break;
            case DynamicListType.LoadList:
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
        overwriteSavePanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Screen.height / listMemberHeight);
    }


    public struct ListMemberText
    {
        public string[] textContents;

        public ListMemberText(SaveGameInfo saveGameInfo)
        {
            textContents = new string[] { saveGameInfo.saveTitle, saveGameInfo.levelName, saveGameInfo.difficulty.ToString(), saveGameInfo.moves.ToString(), saveGameInfo.elapsedTime.ToString(), saveGameInfo.saveTime.ToString() };
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
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleRight, TextAnchor.MiddleCenter };
                    break;
                case DynamicListType.LoadList:
                    colHeaderNames = new string[] { "Mentésnév", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Mentés ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "savetime" };
                    defaultSortingMode = new bool[] { true, true, true, true, false, false };
                    textRectAnchors = new float[] { 0f, 0.3765f, 0.5527f, 0.6645f, 0.74f, 0.8434f, 1f };
                    textFields = new Text[6];
                    textAnchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleRight, TextAnchor.MiddleCenter };
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
    public int indexInList;
    public int saveID;
    public string saveTitle;
    public int difficulty;
    public string levelName;
    public string fileName;
    public int moves;
    public DateTime saveTime;
    public float elapsedTime;

}
