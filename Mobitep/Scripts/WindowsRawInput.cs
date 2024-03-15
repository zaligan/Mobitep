using System;
using System.Runtime.InteropServices;
using UnityEngine;

//  WindowsRawInput�ɂ��Ă̏ڍ׏��͈ȉ��̃����N���Q�Ƃ��Ă��������B
//  https://learn.microsoft.com/ja-jp/windows/win32/inputdev/raw-input

namespace Mobitep.RawInput
{
    public class WindowsRawInput
    {
        /// <summary>
        /// �����H�̓��̓f�o�C�X�̏����`���܂��B
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
        /// �����H�̓��̓f�o�C�X�̏����i�[���܂��B
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinputdevicelist
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PRAWINPUTDEVICELIST
        {
            /// <summary>
            /// �����̓f�o�C�X�ւ̃n���h��
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// �f�o�C�X�̎�ށBRim_Type�񋓑̂̒l���g�p���܂��B
            /// </summary>
            public UInt32 dwType;
        }

        /// <summary>
        /// �f�o�C�X(�}�E�X)����̐��̓��͂��i�[���܂��B
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinput
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT
        {
            /// <summary>
            /// ���̓��̓f�[�^�̃w�b�_�[���
            /// </summary>
            public RAWINPUTHEADER Header;

            /// <summary>
            /// �}�E�X�̏�ԂɊւ�����
            /// </summary>
            public RAWMOUSE Mouse;
        }

        /// <summary>
        /// �����H�̓��̓f�[�^�̈ꕔ�ł���w�b�_�[�����i�[���܂��B
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawinputheader
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            /// <summary>
            /// �f�o�C�X�̎��
            /// </summary>
            public UInt32 dwType;

            /// <summary>
            /// �f�[�^�̓��̓p�P�b�g�̃T�C�Y�i�o�C�g�j
            /// </summary>
            public UInt32 dwSize;

            /// <summary>
            /// �f�o�C�X�ւ̃n���h��
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// WM_INPUT ���b�Z�[�W�� wParm �p�����[�^�ɓn�����l
            /// </summary>
            public IntPtr wParam;
        }

        /// <summary>
        /// �}�E�X�̏�ԂɊւ�������i�[���܂��B32bit����long��32bit�̂���Int32���g�p
        /// https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/ns-winuser-rawmouse
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWMOUSE
        {
            /// <summary>
            /// �}�E�X�̏��
            /// </summary>
            public ushort usFlags;

            /// <summary>
            /// �}�E�X�{�^���̏��
            /// </summary>
            public ulButtons usButton;

            /// <summary>
            /// �}�E�X�{�^���̐��̏�ԁB
            /// </summary>
            public UInt32 ulRawButtons;

            /// <summary>
            /// X�����̃��[�V�����BusFlags �̒l�ɉ����āA��Βl�܂��͑��Βl�Ƃ��ĉ��߂���܂��B
            /// </summary>
            public Int32 lLastX;

            /// <summary>
            /// Y�����̃��[�V�����BusFlags �̒l�ɉ����āA��Βl�܂��͑��Βl�Ƃ��ĉ��߂���܂��B
            /// </summary>
            public Int32 lLastY;

            /// <summary>
            /// �C�x���g�̃f�o�C�X�ŗL�̒ǉ����B
            /// </summary>
            public UInt32 ulExtraInformation;
        }

        /// <summary>
        /// �z�C�[���ƃ{�^���̏�ԂɊւ�������i�[���܂��B
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct ulButtons
        {
            /// <summary>
            /// �}�E�X�{�^���̐؂�ւ����
            /// </summary>
            public ushort usButtonFlags;

            /// <summary>
            /// �z�C�[���̈ړ���
            /// </summary>
            public ushort usButtonData;
        }

        /// <summary>
        /// �ǂݍ��񂾓��͒l���i�[���܂�
        /// </summary>
        public struct ReadInputValue
        {
            /// <summary>
            /// �l��ǂݍ��񂾃f�o�C�X�̃n���h��
            /// </summary>
            public IntPtr hDevice;

            /// <summary>
            /// x�����̈ړ���
            /// </summary>
            public int x;

            /// <summary>
            /// y�����̈ړ���
            /// </summary>
            public int y;
        }

        /// <summary>
        /// �����H�̓��͂��������ۂ̃��b�Z�[�W�ł�
        /// </summary>
        public const int WM_INPUT = 0x00FF;

        public const int RIDEV_INPUTSINK = 0x00000100;

        /// <summary>
        /// GetRawInputDeviceInfo �Ńf�o�C�X�����擾���܂�
        /// </summary>
        public const int RIDI_DEVICENAME = 0x20000007;

        /// <summary>
        /// �E�B���h�E�v���V�[�W���̃A�h���X��ݒ肵�܂�
        /// </summary>
        public const int GWLP_WNDPROC = -4;

        /// <summary>
        /// RAWINPUT ����擾����f�[�^�̃R�}���h�t���O
        /// </summary>
        public enum RID_Type
        {
            /// <summary>
            /// �w�b�_�[�����擾���܂��B
            /// </summary>
            RID_HEADER = 0x10000005,

            /// <summary>
            /// �����H�̓��̓f�[�^���擾���܂��B
            /// </summary>
            RID_INPUT = 0x10000003
        }

        /// <summary>
        /// RAWINPUT�����̃f�o�C�X����̓��͂��������܂��B(RAWINPUTHEADER.dwType)
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

        //TODO: RAWINPUT �ɃL�[�{�[�h�Ƃ��̑��̃f�o�C�X�̏���ǉ�����
        private UInt32 m_rawInputHeaderSize = (UInt32)Marshal.SizeOf<RAWINPUTHEADER>();


        /// <summary>
        /// �E�B���h�E�v���V�[�W���̃A�h���X��ݒ肵�܂�
        /// </summary>
        /// <param name="hWnd">�E�B���h�E�ւ̃n���h��</param>
        /// <param name="msg">���b�Z�[�W</param>
        /// <param name="wParam">�ǉ��̃��b�Z�[�W���</param>
        /// <param name="lParam">�ǉ��̃��b�Z�[�W���</param>
        /// <returns>���b�Z�[�W�����̌��ʁB���b�Z�[�W�ɂ���ĈقȂ�܂�</returns>
        public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// �w�肵���E�B���h�E�̑�����ύX���܂��B
        /// </summary>
        /// <param name="hWnd">�E�B���h�E�ւ̃n���h���ƃE�B���h�E��������N���X���ԐړI�Ɏw�肵�܂��B</param>
        /// <param name="nIndex">�ݒ肷��l�ւ̃I�t�Z�b�g</param>
        /// <param name="dwNewLong">�u���l</param>
        /// <returns>���s�����ꍇ 0 ��Ԃ��܂�</returns>
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
        /// �Ăяo�����X���b�h�̃��b�Z�[�W�L���[�ɃA�^�b�`���ꂽ�E�B���h�E�̃n���h�����擾���܂��B
        /// </summary>
        /// <returns>
        /// �E�B���h�E�̃n���h��
        /// </returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern System.IntPtr GetActiveWindow();

        /// <summary>
        /// �w�肳�ꂽ�N���X���ƃE�B���h�E���Ɉ�v����g�b�v���x���E�B���h�E�̃n���h�����擾���܂��B
        /// </summary>
        /// <param name="lpszClass"></param>
        /// <param name="lpszTitle"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszTitle);

        /// <summary>
        /// �w�肵���E�B���h�E�̑�����ύX���܂��B
        /// </summary>
        /// <param name="hWnd">�E�B���h�E�ւ̃n���h���ƃE�B���h�E��������N���X���ԐړI�Ɏw�肵�܂��B</param>
        /// <param name="nIndex">�ݒ肷��l�ւ̃I�t�Z�b�g</param>
        /// <param name="dwNewLong">�u���l</param>
        /// <returns>�֐������������ꍇ�A�w�肳�ꂽ�I�t�Z�b�g�̑O�̒l�B���s�����ꍇ 0 ��Ԃ��܂��B</returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// �w�肵���E�B���h�E�̑�����ύX���܂��B�w�肵���I�t�Z�b�g��32�r�b�g�l��ǉ��̃E�B���h�E�������ɐݒ肵�܂��B
        /// </summary>
        /// <param name="hWnd">�E�B���h�E�ւ̃n���h���ƃE�B���h�E��������N���X���ԐړI�Ɏw�肵�܂��B</param>
        /// <param name="nIndex">�ݒ肷��l�ւ̃I�t�Z�b�g</param>
        /// <param name="dwNewLong">�u���l</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// ����̃E�B���h�E�v���V�[�W�����Ăяo���āA�A�v���P�[�V�������������Ȃ��E�B���h�E���b�Z�[�W�̊���̏�����񋟂��܂��B
        /// </summary>
        /// <param name="hWnd">���b�Z�[�W����M�����E�B���h�E�v���V�[�W���ւ̃n���h��</param>
        /// <param name="wMsg">���b�Z�[�W</param>
        /// <param name="wParam">�ǉ��̃��b�Z�[�W���</param>
        /// <param name="lParam">�ǉ��̃��b�Z�[�W���</param>
        /// <returns>���b�Z�[�W�����̌��ʂł���A���b�Z�[�W�Ɉˑ����܂��B</returns>
        [DllImport("user32.dll", EntryPoint = "DefWindowProcA")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// �����H�̓��̓f�[�^��񋟂���f�o�C�X��o�^���܂��B
        /// </summary>
        /// <param name="pRawInputDevice">�o�^���� RAWINPUTDEVICE �\���̂̔z��</param>
        /// <param name="uiNumDevices"> pRawInputDevice ���w���\���̗̂v�f��</param>
        /// <param name="cbSize"> RAWINPUTDEVICE �\���̂̃T�C�Y(�o�C�g)</param>
        /// <returns>���������� TRUE .����ȊO�� FALSE </returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, UInt32 uiNumDevices, UInt32 cbSize);

        /// <summary>
        /// �w�肵���f�o�C�X���疢���H�̓��̓f�[�^���擾���܂��BpData �� NULL �̏ꍇ�A�o�b�t�@�[�̕K�v�ȃT�C�Y�� *pcbSize �ŕԂ���܂��B
        /// </summary>
        /// <param name="hRawInput"> RAWINPUT �\���̂ւ̃n���h���BWM_INPUT �� lParam </param>
        /// <param name="uiComand"> RAWINPUT �\���̂���擾����f�[�^�̃R�}���h�t���O</param>
        /// <param name="pData"> RAWINPUT �\���̂���擾�����f�[�^�ւ̃|�C���^�BNULL �̏ꍇ�A�o�b�t�@�[�̕K�v�ȃT�C�Y�� *pcbSize �ŕԂ���܂��B</param>
        /// <param name="pcbSize"> pData ���̃f�[�^�̃T�C�Y(�o�C�g�P��)</param>
        /// <param name="cbSizeHeader"> RAWINPUTHEADER �\���̂̃T�C�Y(�o�C�g�P��)</param>
        /// <returns> pData �� NULL �ŁA�֐������������ꍇ 0 ��Ԃ��BpData �� NULL �ł͂Ȃ��A�֐������������ꍇ�ApData �ɃR�s�[���ꂽ�o�C�g����Ԃ��B</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern UInt32 GetRawInputData(IntPtr hRawInput, UInt32 uiComand, IntPtr pData, ref UInt32 pcbSize, UInt32 cbSizeHeader);

        /// <summary>
        /// �V�X�e���ɐڑ�����Ă�����̓f�o�C�X���擾���܂��B
        /// </summary>
        /// <param name="pRawInputDeviceList">�V�X�e���ɐڑ�����Ă���f�o�C�X�� RAWINPUTDEVICELIST �\���̂̔z��BNULL �̏ꍇ�A�f�o�C�X�̐��� *puiNumDevices �ŕԂ���܂��B</param>
        /// <param name="puiNumDevices"> pRawInputDeviceList ���w���o�b�t�@�[�Ɋ܂߂邱�Ƃ��ł��� RAWINPUTDEVICELIST �\���̂̐�</param>
        /// <param name="cbSize"> RAWINPUTDEVICELIST �\���̂̃T�C�Y(�o�C�g�P��)</param>
        /// <returns> pRawInputDeviceList ���w���o�b�t�@�[�Ɋi�[����Ă���f�o�C�X�̐�</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern UInt32 GetRawInputDeviceList(PRAWINPUTDEVICELIST[] pRawInputDeviceList, ref UInt32 puiNumDevices, UInt32 cbSize);

        /// <summary>
        /// ���̓f�o�C�X�Ɋւ�������擾���܂��B
        /// </summary>
        /// <param name="hDevice">�f�o�C�X�ւ̃n���h��</param>
        /// <param name="uiCommand"> pData �ŕԂ����f�[�^���w�肵�܂��B</param>
        /// <param name="pData"> uiCommand �Ŏw�肳�ꂽ���ւ̃|�C���^</param>
        /// <param name="pcbSize"> pData ���̃f�[�^�̃T�C�Y</param>
        /// <returns> ���������� pData �ɃR�s�[���ꂽ�o�C�g�����������ȊO�̐��l</returns>
        [DllImport("User32.dll", SetLastError = true)]
        public static extern UInt32 GetRawInputDeviceInfoA(IntPtr hDevice, UInt32 uiCommand, IntPtr pData, ref UInt32 pcbSize);
    }
}
