using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScrollBGTest;

namespace ScrollBGTest
{
    [System.Serializable]
    /// <summary>
    //This script is used for background scrolling demo play.
    //Used in the editor and android.
    //This is a sample script and stability and optimization are not guaranteed.
    /// </summary>

    public class ScrollDemoCtrl : MonoBehaviour
    {
        public GameObject PrefabSet;
        public Animator TitleAnim;

        private bool IsStart;

        //UI display setting for testing
        public GameObject TestControlUI;
        public GameObject TestControlUIOpen, TestControlUIClose;

        //AutoPlay
        private bool IsAutoPlay;
        private bool IsAutoPlayLoop;
        public Toggle ToggleAutoPlayLoop;

        //Test (dummy)image
        private bool IsTestCharacter, IsTestUI;
        public Toggle ToggleTestCharacter, ToggleTestUI;
        public GameObject TestCharacter, TestUI;

        private float CurTime;
        private float AutoPlayChangeSpeed = 1.0f;
        public float AutoPlayShowTime;//Set how many seconds each background images should be displayed
        private int SetNum; //Background order to show

        //UI
        public GameObject BtnAutoPlay, BtnStop, BtnSet, BtnSetMobile;
        public Button BtnUp, BtnDown, BtnLeft, BtnRight;
        bool IsLeftClick, IsRightClick;
        public Text TxtBGName;

        //Set keyboard input time interval
        bool IsBGChange;
        public float BGChangeBtnSpeed;
        private float CurBGChangeBtnSpeed;

        ScrollBackgroundCtrl scrollBackgroundCtrl;
        private float ScrollSpeed;
        public float ScrollSpeedEditor, ScrollSpeedMobile;


        private void Awake()
        {
            TitleAnim.SetTrigger("Start");
            //Debug.Log("Start");
        }

        void Start()
        {
            //Auto (Scrolling) play
            ToggleAutoPlayLoop.onValueChanged.AddListener(delegate
            {
                AutoPlayLoopSet(ToggleAutoPlayLoop);
            });
            IsAutoPlayLoop = ToggleAutoPlayLoop.isOn;

            //Character image setting for testing 
            ToggleTestCharacter.onValueChanged.AddListener(delegate { TestCharacterSet(ToggleTestCharacter); });
            IsTestCharacter = ToggleTestCharacter.isOn;
            TestCharacter.SetActive(IsTestCharacter);

            //UI image setting for testing 
            ToggleTestUI.onValueChanged.AddListener(delegate { TestUISet(ToggleTestUI); });
            IsTestUI = ToggleTestUI.isOn;
            TestUI.SetActive(IsTestUI);

            //Button setting for android
            BtnUp.onClick.AddListener(TaskUp);
            BtnDown.onClick.AddListener(TaskDown);
            BtnLeft.onClick.AddListener(delegate { TaskLeft(BtnLeft); });
            BtnRight.onClick.AddListener(delegate { TaskRight(BtnRight); });

            BtnAutoPlay.SetActive(true);
            BtnStop.SetActive(false);

            //UI setting by device
#if UNITY_EDITOR
            BtnSet.SetActive(true);
            BtnSetMobile.SetActive(false);
            ScrollSpeed = ScrollSpeedEditor;
#else
        BtnSet.SetActive(false);
        BtnSetMobile.SetActive(true);
        ScrollSpeed = ScrollSpeedMobile;
#endif

            TxtBGName.text = "";

            for (int i = 0; i < PrefabSet.transform.childCount; i++)
            {
                scrollBackgroundCtrl = PrefabSet.transform.GetChild(i).gameObject.GetComponent<ScrollBackgroundCtrl>();
                scrollBackgroundCtrl.MoveValue = 0.0f;
            }

            IsAutoPlay = false;
            CurTime = 0;
            SetNum = 0;

            SetBackground();

            TestControlUISet(true);
        }


        void Update()
        {
            //Notification of start of play
            if (Input.GetMouseButtonDown(0) || Input.anyKey)
            {
                if (!IsStart)
                    IsStartSet();
            }

            //Manual scrolling and background switching
            if (!IsAutoPlay)
            {
                //Scroll left
                if (Input.GetKey(KeyCode.LeftArrow) || IsLeftClick)
                    scrollBackgroundCtrl.MoveValue -= scrollBackgroundCtrl.MoveSpeed * ScrollSpeed;

                //Scroll right
                if (Input.GetKey(KeyCode.RightArrow) || IsRightClick)
                    scrollBackgroundCtrl.MoveValue += scrollBackgroundCtrl.MoveSpeed * ScrollSpeed;

                //Switch to previus background
                if (Input.GetKey(KeyCode.DownArrow))
                    TaskDown();

                //Switch to next background
                if (Input.GetKey(KeyCode.UpArrow))
                    TaskUp();
            }

            //Automatic scrolling and background switching
            if (IsAutoPlay)
            {
                CurTime += Time.unscaledDeltaTime * AutoPlayChangeSpeed;
                scrollBackgroundCtrl.MoveValue += scrollBackgroundCtrl.MoveSpeed * Time.deltaTime;

                if (CurTime > AutoPlayShowTime)
                {
                    if (PrefabSet.transform.childCount > SetNum)
                    {
                        SetNum += 1;
                        CurTime = 0;
                        SetBackground();
                    }

                    if (PrefabSet.transform.childCount <= SetNum)
                    {
                        if (IsAutoPlay && IsAutoPlayLoop)
                        {
                            SetNum = 0;
                            CurTime = 0;
                            SetBackground();
                        }

                        if (!IsAutoPlayLoop)
                        {
                            IsAutoPlay = false;
                            AutoPlayStop();
                        }
                    }
                }
            }

            //Background switching time interval
            if (IsBGChange)
            {
                CurBGChangeBtnSpeed += BGChangeBtnSpeed * Time.deltaTime;
                if (CurBGChangeBtnSpeed > 1.0f)
                {
                    IsBGChange = false;
                }
            }
        }

        //Notification of start of play
        public void IsStartSet()
        {
            //Check start demo play
            TitleAnim.SetTrigger("Play");
            //Debug.Log("Play");
            IsStart = true;
        }

        //Automatic scrolling and background switching
        public void AutoPlayGo()
        {
            CurTime = 0;
            CurBGChangeBtnSpeed = 0;

            if (PrefabSet.transform.childCount <= SetNum)
                SetNum = -1;

            BtnAutoPlay.SetActive(false);
            BtnStop.SetActive(true);

            //UI setting by device
            BtnSet.SetActive(false);
            BtnSetMobile.SetActive(false);

            if (!IsAutoPlay)
                IsAutoPlay = true;
        }


        //whether to use autoplay
        public void AutoPlayLoopSet(Toggle _IsLoop)
        {
            IsAutoPlayLoop = ToggleAutoPlayLoop.isOn;
        }

        //Whether to use CharacterImage for testing
        public void TestCharacterSet(Toggle _IsTestCharacter)
        {
            IsTestCharacter = ToggleTestCharacter.isOn;
            TestCharacter.SetActive(ToggleTestCharacter.isOn);
        }

        //Whether to use UIImage for testing
        public void TestUISet(Toggle _IsTestUI)
        {
            IsTestUI = ToggleTestUI.isOn;
            TestUI.SetActive(ToggleTestUI.isOn);
        }

        //Stop autoPlay
        public void AutoPlayStop()
        {
            TitleAnim.SetTrigger("Idle");
            IsStart = false;
            IsAutoPlay = false;

            BtnAutoPlay.SetActive(true);
            BtnStop.SetActive(false);

            //UI setting by device
#if UNITY_EDITOR
            BtnSet.SetActive(true);
            BtnSetMobile.SetActive(false);
#else
        BtnSet.SetActive(false);
         BtnSetMobile.SetActive(true);
#endif
        }

        //Background setting
        public void SetBackground()
        {
            //Debug.Log("SetBackground/SetNum=" + SetNum.ToString());
            if (PrefabSet.transform.childCount > SetNum)
            {
                for (int i = 0; i < PrefabSet.transform.childCount; i++)
                {
                    GameObject BG = PrefabSet.transform.GetChild(i).gameObject;
                    BG.SetActive(false);
                }
                scrollBackgroundCtrl = PrefabSet.transform.GetChild(SetNum).gameObject.GetComponent<ScrollBackgroundCtrl>();

                GameObject SetBG = PrefabSet.transform.GetChild(SetNum).gameObject;
                SetBG.SetActive(true);
                TxtBGName.text = SetBG.name;
            }
        }

        //Switch to previus background
        public void TaskUp()
        {
            if (!IsBGChange)
            {
                SetNum -= 1;
                if (SetNum < 0)
                    SetNum = PrefabSet.transform.childCount - 1;

                CurBGChangeBtnSpeed = 0;
                IsBGChange = true;
                SetBackground();

                //Debug.Log("PrefabSet.transform.childCount=" + PrefabSet.transform.childCount.ToString()
                //+ "/SetNum=" + SetNum);
            }
        }

        //Switch to next background
        public void TaskDown()
        {
            if (!IsBGChange)
            {
                SetNum += 1;
                if (PrefabSet.transform.childCount <= SetNum)
                    SetNum = 0;

                CurBGChangeBtnSpeed = 0;
                IsBGChange = true;
                SetBackground();

                //Debug.Log("PrefabSet.transform.childCount=" + PrefabSet.transform.childCount.ToString()
                //+ "/SetNum=" + SetNum);
            }
        }

        //Scroll left
        public void TaskLeft(bool _isClick)
        {
            IsLeftClick = _isClick;
        }

        public void TestControlUISet(bool _IsActive)
        {
            TestControlUI.SetActive(_IsActive);
            TestControlUIOpen.SetActive(!_IsActive);
            TestControlUIClose.SetActive(_IsActive);
        }

        //Scroll right
        public void TaskRight(bool _isClick)
        {
            IsRightClick = _isClick;
        }

    }
}
