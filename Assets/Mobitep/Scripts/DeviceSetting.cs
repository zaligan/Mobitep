using Mobitep.RawInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Mobitep.Editor
{
    /// <summary>
    /// DeviceSettingのEditorWindowを作成します。
    /// </summary>
    public class DeviceSetting : EditorWindow
    {
        /// <summary>
        /// 右のマウスのハンドル
        /// </summary>
        private static IntPtr m_rHandle;

        /// <summary>
        /// 左のマウスのハンドル
        /// </summary>
        private static IntPtr m_lHandle;

        /// <summary>
        /// 接続されているマウスの情報を格納するリスト
        /// </summary>
        private static List<WindowsRawInput.PRAWINPUTDEVICELIST> m_mouceList = new List<WindowsRawInput.PRAWINPUTDEVICELIST>();

        /// <summary>
        /// 接続されているキーボードの情報を格納するリスト
        /// </summary>
        private static List<WindowsRawInput.PRAWINPUTDEVICELIST> m_keybordList = new List<WindowsRawInput.PRAWINPUTDEVICELIST>();

        /// <summary>
        /// 接続されているその他のデバイスの情報を格納するリスト
        /// </summary>
        private static List<WindowsRawInput.PRAWINPUTDEVICELIST> m_elseDeviceList = new List<WindowsRawInput.PRAWINPUTDEVICELIST>();


        [MenuItem("Mobitep/DeviceSetting")]

        public static void ShowWindow()
        {
            GetDeviceList();
            ReadHandleText();
            EditorWindow.GetWindow(typeof(DeviceSetting));
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            for (int i = 0; i < m_mouceList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("マウス" + i);
                GUILayout.Label("デバイスハンドル：" + m_mouceList[i].hDevice);

                if (GUILayout.Button("左に登録"))
                {
                    m_lHandle = m_mouceList[i].hDevice;
                }

                if (GUILayout.Button("右に登録"))
                {
                    m_rHandle = m_mouceList[i].hDevice;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("左：" + m_lHandle);
            GUILayout.Label("右：" + m_rHandle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("適用"))
            {
                //ハンドルをテキストファイルに保存
                SaveHandleText();
            }

            if (GUILayout.Button("閉じる"))
            {
                this.Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Windowsに接続されたデバイスの情報を種類ごとに分け配列へ格納
        /// </summary>
        private static void GetDeviceList()
        {
            m_mouceList.Clear();
            m_keybordList.Clear();
            m_elseDeviceList.Clear();

            // 現在接続されている入力デバイスの数を取得
            UInt32 deviceNum = 0;
            WindowsRawInput.GetRawInputDeviceList(null, ref deviceNum, Convert.ToUInt32(Marshal.SizeOf(typeof(WindowsRawInput.PRAWINPUTDEVICELIST))));

            //deviceListへ、システムに接続されたデバイス、RawInputDeviceList構造体の配列をいれる
            WindowsRawInput.PRAWINPUTDEVICELIST[] deviceList = new WindowsRawInput.PRAWINPUTDEVICELIST[deviceNum];
            if (WindowsRawInput.GetRawInputDeviceList(deviceList, ref deviceNum, Convert.ToUInt32(Marshal.SizeOf(typeof(WindowsRawInput.PRAWINPUTDEVICELIST)))) != deviceNum)
            {
                Debug.LogError("We couldn't retrieve the DeviceList correctly.");
            }

            //デバイス種類ごとに分ける
            foreach (WindowsRawInput.PRAWINPUTDEVICELIST pRAWINPUTDEVICELIST in deviceList)
            {
                switch (pRAWINPUTDEVICELIST.dwType)
                {
                    case 0://mouse
                        m_mouceList.Add(pRAWINPUTDEVICELIST);
                        break;

                    case 1://keyboard
                        m_keybordList.Add(pRAWINPUTDEVICELIST);
                        break;

                    case 2://else
                        m_elseDeviceList.Add(pRAWINPUTDEVICELIST);
                        break;

                    default:
                        Debug.LogError("The 'default' case was called in the 'GetDeviceList' switch statement.");
                        break;
                }
            }
        }

        /// <summary>
        /// ハンドルをテキストファイルに保存
        /// </summary>
        private void SaveHandleText()
        {
            string text = m_lHandle.ToString() + "\n" + m_rHandle.ToString();

            // Assetsフォルダへのパスを取得
            string assetsPath = Application.dataPath;

            // Handler.txtへのパスを構築
            string filePath = Path.Combine(assetsPath, "Mobitep/Handler.txt");
            Debug.Log("SaveHandleText" + filePath);
            // テキストをファイルに書き込む
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// テキストファイルからハンドルを読み込む
        /// </summary>
        private static void ReadHandleText()
        {
            // Assetsフォルダへのパスを取得
            string assetsPath = Application.dataPath;

            // Handler.txtへのパスを構築
            string filePath = Path.Combine(assetsPath, "Mobitep/Handler.txt");

            // ファイルからテキストを読み込む
            string content = File.ReadAllText(filePath);

            string[] handleArr = content.Split(new string[] { "\n" }, System.StringSplitOptions.None);

            m_lHandle = new IntPtr(Convert.ToInt32(handleArr[0]));
            m_rHandle = new IntPtr(Convert.ToInt32(handleArr[1]));
        }
    }
}
