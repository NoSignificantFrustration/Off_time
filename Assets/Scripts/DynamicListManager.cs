using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        SetupHeader();
        LoadButtons();

        //saveGameInfos.Sort((s1, s2) => s1.saveID.CompareTo(s2.saveID));
        //Canvas.ForceUpdateCanvases();
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

        

        for (int i = 0; i < btnAmount; i++)
        {
            GameObject button = CreateListMember();
            
            Button bComponent = button.GetComponent<Button>();
            bComponent.onClick.AddListener(delegate { ListElementPressed(button); });
            button.name = i.ToString();
            listMemberButtons.Add(button);
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



    private void HeaderElementPressed(GameObject button)
    {
        LoadButtons();
    }

    private void ListElementPressed(GameObject clickedButton)
    {
        if (clickedButton == selectedButton)
        {
            if (Time.time - lastClickTime <= 0.5f)
            {
                Debug.Log("DoubleClick");
            }
            else
            {
                Debug.Log("Click");
                //Debug.Log(Time.time - lastClickTime);
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

    



    // Update is called once per frame
    void Update()
    {

    }


    public struct ListMemberText
    {
        public string[] textContents;

        public ListMemberText(SaveGameInfo saveGameInfo)
        {
            textContents = new string[] { saveGameInfo.saveTitle, saveGameInfo.difficulty.ToString(), saveGameInfo.moves.ToString(), saveGameInfo.levelName, saveGameInfo.elapsedTime.ToString(), saveGameInfo.saveTime.ToString()};
        }

    }

    public struct ListHeaderProperty
    {
        public string[] colHeaderNames;
        public string[] colDatabaseFields;
        public bool[] defaultSortingMode; //1: ASC 0: DSC
        public float[] textRectAnchors;
        public Text[] textFields;

        public ListHeaderProperty(DynamicListType listType)
        {
            switch (listType)
            {
                case DynamicListType.SaveList:
                    colHeaderNames = new string[] { "Mentésnév", "Pályanév", "Nehézség", "Lépések", "Játékidõ", "Mentés ideje" };
                    colDatabaseFields = new string[] { "title", "levelName", "difficulty", "moves", "elapsedTime", "saveTime" };
                    defaultSortingMode = new bool[] { true, true, true, true, false, false};
                    textRectAnchors = new float[] { 0f, 0.16f, 0.32f, 0.48f, 0.64f, 0.8f, 1f };
                    textFields = new Text[6];
                    break;
                default:
                    colHeaderNames = new string[] { };
                    colDatabaseFields = new string[] { };
                    defaultSortingMode = new bool[] { };
                    textRectAnchors = new float[] { };
                    textFields = new Text[] { };
                    break;
            }

            
            
        }
    }

    public enum DynamicListType
    {
        SaveList
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
