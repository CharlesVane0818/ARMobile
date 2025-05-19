using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

   public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance => _instance;

        private readonly List<GameObject> _currentUIs = new();
        private GameObject _topLayerUI;
        public Dictionary<string, GameObject> UIDictionary { get; } = new();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
                // Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                InitializeUIDictionary();
            }
        }

        private void InitializeUIDictionary()
        {
            GameObject uiCanvas = GameObject.Find("UICanvas");
            if (uiCanvas != null)
            {
                foreach (Transform child in uiCanvas.transform)
                {
                    if (child.gameObject.GetComponent<Canvas>() != null)
                    {
                        UIDictionary[child.name] = child.gameObject;
                        // child.gameObject.SetActive(false); // 初始时隐藏所有UI
                    }
                }
            }
        }

        /// <summary>
        /// 显示UI界面
        /// </summary>
        /// <param name="uiName">要显示的UI名称</param>
        public void ShowUI(string uiName)
        {
            if (UIDictionary.ContainsKey(uiName))
            {
                UIDictionary[uiName].SetActive(true);
            }
        }

        /// <summary>
        /// 隐藏UI界面
        /// </summary>
        /// <param name="uiName">要隐藏的UI名称</param>
        public void HideUI(string uiName)
        {
            if (UIDictionary.ContainsKey(uiName))
            {
                UIDictionary[uiName].SetActive(false);
            }
        }   


        /// <summary>
        /// 隐藏所有UI界面
        /// </summary>
        public void HideAllUI()
        {
            foreach (var ui in UIDictionary.Values)
            {
                ui.SetActive(false);
            }
        }   

        


        /// <summary>
        /// <summary>
        /// 切换UI界面
        /// </summary>
        /// <param name="uiName">要显示的UI名称</param>
        public void SwitchUI(string uiName, bool hideCurrent = true)
        {
            if (hideCurrent && _currentUIs.Count > 0)
            {
                foreach (var ui in _currentUIs)
                {
                    ui.SetActive(false);
                }
                _currentUIs.Clear();
            }

            if (UIDictionary.ContainsKey(uiName))
            {
                var newUI = UIDictionary[uiName];
                newUI.SetActive(true);
                _currentUIs.Add(newUI);
                newUI.transform.SetAsLastSibling();
                
                if (_topLayerUI != null)
                {
                    _topLayerUI.transform.SetAsLastSibling();
                }
               
            }
        }
        
        public void SetTopLayerUI(string uiName)
        {
            if (UIDictionary.ContainsKey(uiName))
            {
                _topLayerUI = UIDictionary[uiName];
                _topLayerUI.SetActive(true);
                _topLayerUI.transform.SetAsLastSibling();
            }
        }

        public void DemoFunction()
        {
            UIManager.Instance.ShowUI("UI1");//显示UI 不记录当前
            UIManager.Instance.HideUI("UI2");//隐藏UI 不记录当前
            UIManager.Instance.SwitchUI("UI3");//切换UI
            UIManager.Instance.SwitchUI("UI3",false);//切换UI 不隐藏当前UI 叠加显示
            UIManager.Instance.HideAllUI();//隐藏所有UI
            UIManager.Instance.SetTopLayerUI("UI4");//创建一个永久前置UI

        }
    }