using UnityEngine;
using UnityEngine.UI;
public class HeartController : MonoBehaviour
{
    PlayerControl player;

    private GameObject[] heartContainers;
    private Image[] heartFills;
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
   
    void Start()
    {
        player = PlayerControl.Instance;
        heartContainers = new GameObject[PlayerControl.Instance.maxHealth];
        heartFills = new Image[PlayerControl.Instance.maxHealth];


        PlayerControl.Instance.onHealthChangedCallback += UpdateHeartsHUD;
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetHeartContainers()
    {
        for(int i = 0; i < heartContainers.Length; i++)
        {
            if(i < PlayerControl.Instance.maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }
    void SetFilledHearts()
    {
        for(int i = 0; i < heartFills.Length; i++)
        {
            if(i < PlayerControl.Instance.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
    }
    void InstantiateHeartContainers()
    {
        for(int i = 0; i < PlayerControl.Instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
