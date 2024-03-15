using System;
using System.Runtime.InteropServices;
using UnityEngine;

//  WindowsRawInputについての詳細情報は以下のリンクを参照してください。
//  https://learn.microsoft.com/ja-jp/windows/win32/inputdev/raw-input

namespace Mobitep.RawInput
{
    public class WindowsRawInput
    {
        /// <summary>
        /// 未加工の入力デバイスの情報を定義します。
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinputdevice
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
            public IntPtr hwndTarget;
        }

        /// <summary>
        /// 未加工の入力デバイスの情報を格納します。
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinputdevicelist
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PRAWINPUTDEVICELIST
        {
            /// <summary>
            /// 生入力デバイスへのハンドル
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// デバイスの種類。Rim_Type列挙体の値を使用します。
            /// </summary>
            public UInt32 dwType;
        }

        /// <summary>
        /// デバイス(マウス)からの生の入力を格納します。
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinput
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT
        {
            /// <summary>
            /// 生の入力データのヘッダー情報
            /// </summary>
            public RAWINPUTHEADER Header;

            /// <summary>
            /// マウスの状態に関する情報
            /// </summary>
            public RAWMOUSE Mouse;
        }

        /// <summary>
        /// 未加工の入力データの一部であるヘッダー情報を格納します。
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinputheader
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            /// <summary>
            /// デバイスの種類
            /// </summary>
            public UInt32 dwType;

            /// <summary>
            /// データの入力パケットのサイズ（バイト）
            /// </summary>
            public UInt32 dwSize;

            /// <summary>
            /// デバイスへのハンドル
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// WM_INPUT メッセージの wParm パラメータに渡される値
            /// </summary>
            public IntPtr wParam;
        }

        /// <summary>
        /// マウスの状態に関する情報を格納します。32bit環境でlongは32bitのためInt32を使用
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawmouse
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWMOUSE
        {
            /// <summary>
            /// マウスの状態
            /// </summary>
            public ushort usFlags;

            /// <summary>
            /// マウスボタンの状態
            /// </summary>
            public ulButtons usButton;

            /// <summary>
            /// マウスボタンの生の状態。
            /// </summary>
            public UInt32 ulRawButtons;

            /// <summary>
            /// X方向のモーション。usFlags の値に応じて、絶対値または相対値として解釈されます。
            /// </summary>
            public Int32 lLastX;

            /// <summary>
            /// Y方向のモーション。usFlags の値に応じて、絶対値または相対値として解釈されます。
            /// </summary>
            public Int32 lLastY;

            /// <summary>
            /// イベントのデバイス固有の追加情報。
            /// </summary>
            public UInt32 ulExtraInformation;
        }

        /// <summary>
        /// ホイールとボタンの状態に関する情報を格納します。
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ulButtons
        {
            /// <summary>
            /// マウスボタンの切り替え状態
            /// </summary>
            public ushort usButtonFlags;

            /// <summary>
            /// ホイールの移動量
            /// </summary>
            public ushort usButtonData;
        }

        /// <summary>
        /// 読み込んだ入力値を格納します
        /// </summary>
        public struct ReadInputValue
        {
            /// <summary>
            /// 値を読み込んだデバイスのハンドル
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// x方向の移動量
            /// </summary>
            public int x;

            /// <summary>
            /// y方向の移動量
            /// </summary>
            public int y;
        }

        /// <summary>
        /// 未加工の入力があった際のメッセージです
        /// </summary>
        public const int WM_INPUT = 0x00FF;

        public const int RIDEV_INPUTSINK = 0x00000100;

        /// <summary>
        /// GetRawInputDeviceInfo でデバイス名を取得します
        /// </summary>
        public const int RIDI_DEVICENAME = 0x20000007;

        /// <summary>
        /// ウィンドウプロシージャのアドレスを設定します
        /// </summary>
        public const int GWLP_WNDPROC = -4;

        /// <summary>
        /// RAWINPUT から取得するデータのコマンドフラグ
        /// </summary>
        public enum RID_Type
        {
            /// <summary>
            /// ヘッダー情報を取得します。
            /// </summary>
            RID_HEADER = 0x10000005,

            /// <summary>
            /// 未加工の入力データを取得します。
            /// </summary>
            RID_INPUT = 0x10000003
        }

        /// <summary>
        /// RAWINPUTが何のデバイスからの入力かを示します。(RAWINPUTHEADER.dwType)
        /// </summary>
        public enum RIM_Type
        {
            Mouse = 0,
            Keyboard = 1,
            HID = 2
        }

        
        private IntPtr m_oldWndProcPtr;
        private IntPtr m_newWndProcPtr;
        private WndProcDelegate m_newWndProc;

        //TODO: RAWINPUT にキーボードとその他のデバイスの情報を追加する
        private UInt32 m_rawInputHeaderSize = (UInt32)Marshal.SizeOf<RAWINPUTHEADER>();


        /// <summary>
        /// ウィンドウプロシージャのアドレスを設定します
        /// </summary>
        /// <param name="hWnd">ウィンドウへのハンドル</param>
        /// <param name="msg">メッセージ</param>
        /// <param name="wParam">追加のメッセージ情報</param>
        /// <param name="lParam">追加のメッセージ情報</param>
        /// <returns>メッセージ処理の結果。メッセージによって異なります</returns>
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 指定したウィンドウの属性を変更します。
        /// </summary>
        /// <param name="hWnd">ウィンドウへのハンドルとウィンドウが属するクラスを間接的に指定します。</param>
        /// <param name="nIndex">設定する値へのオフセット</param>
        /// <param name="dwNewLong">置換値</param>
        /// <returns>失敗した場合 0 を返します</returns>
        public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                Debug.Log("IntPtr.Size != 8");
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }

        /// <summary>
        /// 呼び出し元スレッドのメッセージキューにアタッチされたウィンドウのハンドルを取得します。
        /// </summary>
        /// <returns>
        /// ウィンドウのハンドル
        /// </returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern System.IntPtr GetActiveWindow();

        /// <summary>
        /// 指定されたクラス名とウィンドウ名に一致するトップレベルウィンドウのハンドルを取得します。
        /// </summary>
        /// <param name="lpszClass"></param>
        /// <param name="lpszTitle"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszTitle);

        /// <summary>
        /// 指定したウィンドウの属性を変更します。
        /// </summary>
        /// <param name="hWnd">ウィンドウへのハンドルとウィンドウが属するクラスを間接的に指定します。</param>
        /// <param name="nIndex">設定する値へのオフセット</param>
        /// <param name="dwNewLong">置換値</param>
        /// <returns>関数が成功した場合、指定されたオフセットの前の値。失敗した場合 0 を返します。</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 指定したウィンドウの属性を変更します。指定したオフセットの32ビット値を追加のウィンドウメモリに設定します。
        /// </summary>
        /// <param name="hWnd">ウィンドウへのハンドルとウィンドウが属するクラスを間接的に指定します。</param>
        /// <param name="nIndex">設定する値へのオフセット</param>
        /// <param name="dwNewLong">置換値</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// 既定のウィンドウプロシージャを呼び出して、アプリケーションが処理しないウィンドウメッセージの既定の処理を提供します。
        /// </summary>
        /// <param name="hWnd">メッセージを受信したウィンドウプロシージャへのハンドル</param>
        /// <param name="wMsg">メッセージ</param>
        /// <param name="wParam">追加のメッセージ情報</param>
        /// <param name="lParam">追加のメッセージ情報</param>
        /// <returns>メッセージ処理の結果であり、メッセージに依存します。</returns>
        [DllImport("user32.dll", EntryPoint = "DefWindowProcA")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 未加工の入力データを提供するデバイスを登録します。
        /// </summary>
        /// <param name="pRawInputDevice">登録する RAWINPUTDEVICE 構造体の配列</param>
        /// <param name="uiNumDevices"> pRawInputDevice が指す構造体の要素数</param>
        /// <param name="cbSize"> RAWINPUTDEVICE 構造体のサイズ(バイト)</param>
        /// <returns>成功したら TRUE .それ以外は FALSE </returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, UInt32 uiNumDevices, UInt32 cbSize);

        /// <summary>
        /// 指定したデバイスから未加工の入力データを取得します。pData が NULL の場合、バッファーの必要なサイズは *pcbSize で返されます。
        /// </summary>
        /// <param name="hRawInput"> RAWINPUT 構造体へのハンドル。WM_INPUT の lParam </param>
        /// <param name="uiComand"> RAWINPUT 構造体から取得するデータのコマンドフラグ</param>
        /// <param name="pData"> RAWINPUT 構造体から取得されるデータへのポインタ。NULL の場合、バッファーの必要なサイズは *pcbSize で返されます。</param>
        /// <param name="pcbSize"> pData 内のデータのサイズ(バイト単位)</param>
        /// <param name="cbSizeHeader"> RAWINPUTHEADER 構造体のサイズ(バイト単位)</param>
        /// <returns> pData が NULL で、関数が成功した場合 0 を返す。pData が NULL ではなく、関数が成功した場合、pData にコピーされたバイト数を返す。</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern UInt32 GetRawInputData(IntPtr hRawInput, UInt32 uiComand, IntPtr pData, ref UInt32 pcbSize, UInt32 cbSizeHeader);

        /// <summary>
        /// システムに接続されている入力デバイスを取得します。
        /// </summary>
        /// <param name="pRawInputDeviceList">システムに接続されているデバイスの RAWINPUTDEVICELIST 構造体の配列。NULL の場合、デバイスの数が *puiNumDevices で返されます。</param>
        /// <param name="puiNumDevices"> pRawInputDeviceList が指すバッファーに含めることができる RAWINPUTDEVICELIST 構造体の数</param>
        /// <param name="cbSize"> RAWINPUTDEVICELIST 構造体のサイズ(バイト単位)</param>
        /// <returns> pRawInputDeviceList が指すバッファーに格納されているデバイスの数</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern UInt32 GetRawInputDeviceList(PRAWINPUTDEVICELIST[] pRawInputDeviceList, ref UInt32 puiNumDevices, UInt32 cbSize);

        /// <summary>
        /// 入力デバイスに関する情報を取得します。
        /// </summary>
        /// <param name="hDevice">デバイスへのハンドル</param>
        /// <param name="uiCommand"> pData で返されるデータを指定します。</param>
        /// <param name="pData"> uiCommand で指定された情報へのポインタ</param>
        /// <param name="pcbSize"> pData 内のデータのサイズ</param>
        /// <returns> 成功したら pData にコピーされたバイト数を示す負以外の数値</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern UInt32 GetRawInputDeviceInfoA(IntPtr hDevice, UInt32 uiCommand, IntPtr pData, ref UInt32 pcbSize);
    }
}
