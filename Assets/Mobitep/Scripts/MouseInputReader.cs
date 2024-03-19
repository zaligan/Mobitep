using Mobitep.RawInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Handle.txtに保存されたマウスから未加工の入力を読み込みます。
/// </summary>
public class MouseInputReader : MonoBehaviour
{
    /// <summary>
    /// メインウィンドウのハンドル
    /// </summary>
    private HandleRef m_hMainWindow;

    /// <summary>
    /// ウィンドウプロシージャのアドレス
    /// </summary>
    private IntPtr m_oldWndProcPtr;
    private IntPtr m_newWndProcPtr;

    /// <summary>
    /// ウィンドウプロシージャのデリゲート
    /// </summary>
    private WindowsRawInput.WndProcDelegate m_newWndProc;

    /// <summary>
    /// RAWINPUT構造体のヘッダーのサイズ
    /// </summary>
    private UInt32 m_rawInputHeaderSize = (UInt32)Marshal.SizeOf<WindowsRawInput.RAWINPUTHEADER>();

    /// <summary>
    /// 左のマウスのハンドル
    /// </summary>
    private IntPtr m_lMouce;

    /// <summary>
    /// 右のマウスのハンドル
    /// </summary>
    private IntPtr m_rMouce;

    /// <summary>
    /// 右側のマウスの入力値を格納するリスト
    /// </summary>
    private List<Vector2> m_rightInputValueList = new List<Vector2>();

    /// <summary>
    /// 左側のマウスの入力値を格納するリスト
    /// </summary>
    private List<Vector2> m_leftInputValueList = new List<Vector2>();

    /// <summary>
    /// 右側のマウスの入力値リストの要素数を取得
    /// </summary>
    /// <returns> リストの要素数 </returns>
    public Int32 GetRightInputValueListCount()
    {
        return m_rightInputValueList.Count;
    }

    /// <summary>
    /// 左側のマウスの入力値リストの要素数を取得
    /// </summary>
    /// <returns> リストの要素数 </returns>
    public Int32 GetLeftInputValueListCount()
    {
        return m_leftInputValueList.Count;
    }

    /// <summary>
    /// 右側のマウスの入力値リストから先頭の要素を取り出す。
    /// </summary>
    /// <returns> 右側のマウスの入力値 </returns>
    /// <exception cref="IndexOutOfRangeException"> 要素数が 0 の時は使用できません </exception>
    public Vector2 PopRightInputValueList()
    {
        if (0 < m_rightInputValueList.Count)
        {
            //読み込んだ要素を削除
            Vector2 result = m_rightInputValueList[0];
            m_rightInputValueList.RemoveAt(0);

            return result;
        }
        else
        {
            throw new IndexOutOfRangeException("Access Error: Failed to read the element. The PopRightInputValueList currently contains no elements.\r\n");
        }
    }

    /// <summary>
    /// 左側のマウスの入力値リストから先頭の要素を取り出す
    /// </summary>
    /// <returns> 左側のマウスの入力値 </returns>
    /// <exception cref="IndexOutOfRangeException"> 要素数が 0 の時は使用できません </exception>
    public Vector2 PopLeftInputValueList()
    {
        if (0 < m_leftInputValueList.Count)
        {
            //読み込んだ要素を削除
            Vector2 result = m_leftInputValueList[0];
            m_leftInputValueList.RemoveAt(0);

            return result;
        }
        else
        {
            throw new IndexOutOfRangeException("Access Error: Failed to read the element. The PopLeftInputValueList currently contains no elements.\r\n");
        }
    }

    private void Start()
    {
        ReadHandlePtr();

        IntPtr windPtr = WindowsRawInput.GetActiveWindow();
        RegisterMouseDevice(windPtr);

        m_hMainWindow = new HandleRef(null, windPtr);
        m_newWndProc = new WindowsRawInput.WndProcDelegate(WndProc);
        m_newWndProcPtr = Marshal.GetFunctionPointerForDelegate(m_newWndProc);

        //ウィンドウプロシージャの置き換え
        m_oldWndProcPtr = WindowsRawInput.SetWindowLongPtr(m_hMainWindow, WindowsRawInput.GWLP_WNDPROC, m_newWndProcPtr);
    }

    private void OnDestroy()
    {
        Term();
    }

    /// <summary>
    /// RawDataを受け取るためマウスをデバイス登録する
    /// </summary>
    /// <param name="hwnd">入力を監視するウィンドウのハンドル</param>
    /// <exception cref="ApplicationException">デバイス登録に失敗したら投げます</exception>
    public void RegisterMouseDevice(IntPtr hwnd)
    {
        WindowsRawInput.RAWINPUTDEVICE[] rid = new WindowsRawInput.RAWINPUTDEVICE[1];
        rid[0].usUsagePage = Convert.ToUInt16(1);//マウスを表すID
        rid[0].usUsage = Convert.ToUInt16(2);//マウスを表すID
        rid[0].dwFlags = WindowsRawInput.RIDEV_INPUTSINK;//モード選択
        rid[0].hwndTarget = hwnd;//ウィンドウのハンドル

        if (!WindowsRawInput.RegisterRawInputDevices(rid, Convert.ToUInt32(rid.Length), Convert.ToUInt32(Marshal.SizeOf(rid[0]))))
        {
            throw new ApplicationException("Failed to register raw input device(s). " +
                "Error code: " + Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    /// ウィンドウメッセージが来たら呼ばれるウィンドウプロシージャです
    /// </summary>
    /// <param name="hWnd"> ウィンドウのハンドル </param>
    /// <param name="msg"> ウィンドウメッセージ </param>
    /// <param name="wParam"> 追加のメッセージ情報 </param>
    /// <param name="lParam"> 追加のメッセージ情報 </param>
    /// <returns> メッセージに依存します </returns>
    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            //RawInputDataが送られてきたとき
            case WindowsRawInput.WM_INPUT:
                UInt32 dwSize = 0;
                if (WindowsRawInput.GetRawInputData(lParam, (uint)WindowsRawInput.RID_Type.RID_INPUT, IntPtr.Zero, ref dwSize, m_rawInputHeaderSize) != 0)
                {
                    Debug.LogError("初回GRID失敗");
                }

                IntPtr rawInputPointer = IntPtr.Zero;
                rawInputPointer = Marshal.AllocHGlobal((int)dwSize);
                if (WindowsRawInput.GetRawInputData(lParam, (uint)WindowsRawInput.RID_Type.RID_INPUT, rawInputPointer, ref dwSize, m_rawInputHeaderSize) != dwSize)
                {
                    Debug.LogError("GetRawInputData does not return correct size !");
                }

                WindowsRawInput.RAWINPUT rawInput = Marshal.PtrToStructure<WindowsRawInput.RAWINPUT>(rawInputPointer);
                
                //マウスの移動量を取得し左右対応するリストに格納
                if (rawInput.Header.dwType == (uint)WindowsRawInput.RIM_Type.Mouse)
                {
                    if (rawInput.Header.hDevice == m_lMouce)
                    {
                        m_leftInputValueList.Add(new Vector2(rawInput.Mouse.lLastX, rawInput.Mouse.lLastY));
                    }
                    else if (rawInput.Header.hDevice == m_rMouce)
                    {
                        m_rightInputValueList.Add(new Vector2(rawInput.Mouse.lLastX, rawInput.Mouse.lLastY));
                    }
                }
                break;

            default:
                break;
        }

        //100個以上のデータがあるときは古いデータを削除
        if (m_rightInputValueList.Count > 100)
        {
            m_rightInputValueList.RemoveAt(0);
            Debug.Log("RightInputValueList is Max");
        }
        if (m_leftInputValueList.Count > 100)
        {
            m_leftInputValueList.RemoveAt(0);
            Debug.Log("LeftInputValueList is Max");
        }

        return WindowsRawInput.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    /// <summary>
    /// メモリの開放をします
    /// </summary>
    private void Term()
    {
        WindowsRawInput.SetWindowLongPtr(m_hMainWindow, WindowsRawInput.GWLP_WNDPROC, m_oldWndProcPtr);
        m_hMainWindow = new HandleRef(null, IntPtr.Zero);
        m_oldWndProcPtr = IntPtr.Zero;
        m_newWndProcPtr = IntPtr.Zero;
        m_newWndProc = null;
    }

    /// <summary>
    /// Handler.txtからハンドルを読み込む
    /// </summary>
    private void ReadHandlePtr()
    {
        // Assetsフォルダへのパスを取得
        string assetsPath = Application.dataPath;

        // Handler.txtへのパスを構築
        string filePath = Path.Combine(assetsPath, "Mobitep/Handler.txt");

        // ファイルからテキストを読み込む
        string content = File.ReadAllText(filePath);

        string[] handleArr = content.Split(new string[] { "\n" }, System.StringSplitOptions.None);

        m_lMouce = new IntPtr(Convert.ToInt32(handleArr[0]));
        m_rMouce = new IntPtr(Convert.ToInt32(handleArr[1]));
    }

}
