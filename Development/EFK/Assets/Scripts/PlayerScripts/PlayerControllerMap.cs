using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerMap : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    private Vector3 _movement;
    private bool _hasChange = false;
    private bool _isDead = false;
    private bool _isPanelActive;
    
    private AudioManager audioManager;
    private Animator animator;
    private GameManager gameManager;
    private GameObject diaryPanel;
    private GameObject inventoryPanel;
    [SerializeField] private Text diaryTextGUI;
    [SerializeField] private TimerText timerGUI;
    [SerializeField] private List<ItemSlot> slots;
    [SerializeField] private RuntimeAnimatorController[] runtimeanimators;

    [SerializeField] private GameObject lostGamePrefab;
    [SerializeField] private GameObject wonGamePrefab;
    private AudioSource[] footstepSounds;


    public RuntimeAnimatorController[] RuntimeAnimators => runtimeanimators;

    private RoomAbstract myRoom;

    private bool networkMine;
    
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    
    public bool IsPanelActive
    {
        get => _isPanelActive;
        set => _isPanelActive = value;
    }

    public void Awake() {
        diaryPanel = transform.Find("Canvas").Find("Diary-Panel").gameObject;
        diaryPanel.SetActive(false);
        inventoryPanel = transform.Find("Canvas").GetComponentInChildren<GridLayoutGroup>().gameObject;
        audioManager = FindObjectOfType<AudioManager>();
        footstepSounds = transform.Find("FootSounds").GetComponentsInChildren<AudioSource>();
        
    }

    public void SetLocation(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void Move(Vector3 movement)
    {
        _movement = movement;
        _hasChange = true;
    }

    private void FixedUpdate()
    {
        if (_hasChange)
        {
            transform.position += _movement.normalized * Time.fixedDeltaTime * _speed;
            _hasChange = false;
        }
    }

    public Vector3 Movement => _movement.normalized;

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    public void SetRoom(RoomAbstract room)
    {
        ResetDiaryImages();
        myRoom = room;
        SetText(myRoom.DiaryText);
        BuildDiaryImages();
    }

    private void ResetDiaryImages() {
        foreach (Transform child in diaryPanel.transform){
            if (child.CompareTag("DiaryImage")) Destroy(child.gameObject);
        }
    }

    public void ResetInventory()
    {
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            inventoryPanel.transform.GetChild(i).GetComponent<ItemSlot>().RemoveImage();
        }
    }

    public void BuildDiaryImages()
    {
        List<Image> imageList = myRoom.DiaryImageList;
        foreach (Image image in imageList)
        {
            if(image!=null) Instantiate(image, diaryPanel.transform);
        }
    }

    private void SetText(Text newText)
    {
        diaryTextGUI.text = newText.text;
    }
    
    public ItemSlot GetFirstFreeSlot()
    {
        return slots.Find(s => s.GetIsFree());
    }

    public bool IsDead
    {
        get => _isDead;
    }
    
    public void SetPlayerIsDead() {
        if (PhotonNetwork.IsConnected)
        {
            GetComponent<PhotonView>().RPC("SetIsDead", RpcTarget.All);
        }
        else
        {
            _isDead = true;
            EventManager.TriggerEvent(EventType.FinishGame);
        }
    }
    
    [PunRPC]
    public void SetIsDead()
    {
        _isDead = true;
        EventManager.TriggerEvent(EventType.FinishGame);
    }

    public void FinishGame(bool victory)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (victory) {
            Instantiate(wonGamePrefab);
        }
        else {
            gameObject.SetActive(false);
            FindObjectOfType<AudioManager>().Play("LoseTheme");
            Instantiate(lostGamePrefab);
        }
    }

    public void Explode() {
        animator = GetComponent<Animator>();
        GetComponent<PlayerInput>().CanMove = false;
        animator.SetTrigger("Exploding");
        audioManager.Play("Explosion");
    }

    public void SetTimer(int time, bool trigger = true) {
        timerGUI.SetTime(time, trigger);
    }

    public void TriggerHalveTimePenalization() {
        timerGUI.HalveTime();
    }

    public void IncrementTimer(int value) {
        timerGUI.IncrementTime(value);
    }
    
    [PunRPC]
    public void ReloadMain()
    {
        try
        {
            Destroy(FindObjectOfType<PhotonVoiceNetwork>().gameObject);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
        PhotonNetwork.LoadLevel("Main");
    }

    public void Footsep()
    {
        AudioSource footstep = footstepSounds[UnityEngine.Random.Range(0,footstepSounds.Length)];
        footstep.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        footstep.Play();
    }
}
