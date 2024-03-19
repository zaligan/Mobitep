using Mobitep.RawInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Handle.txt�ɕۑ����ꂽ�}�E�X���疢���H�̓��͂�ǂݍ��݂܂��B
/// </summary>
public class MouseInputReader : MonoBehaviour
{
    /// <summary>
    /// ���C���E�B���h�E�̃n���h��
    /// </summary>
    private HandleRef m_hMainWindow;

    /// <summary>
    /// �E�B���h�E�v���V�[�W���̃A�h���X
    /// </summary>
    private IntPtr m_oldWndProcPtr;
    private IntPtr m_newWndProcPtr;

    /// <summary>
    /// �E�B���h�E�v���V�[�W���̃f���Q�[�g
    /// </summary>
    private WindowsRawInput.WndProcDelegate m_newWndProc;

    /// <summary>
    /// RAWINPUT�\���̂̃w�b�_�[�̃T�C�Y
    /// </summary>
    private UInt32 m_rawInputHeaderSize = (UInt32)Marshal.SizeOf<WindowsRawInput.RAWINPUTHEADER>();

    /// <summary>
    /// ���̃}�E�X�̃n���h��
    /// </summary>
    private IntPtr m_lMouce;

    /// <summary>
    /// �E�̃}�E�X�̃n���h��
    /// </summary>
    private IntPtr m_rMouce;

    /// <summary>
    /// �E���̃}�E�X�̓��͒l���i�[���郊�X�g
    /// </summary>
    private List<Vector2> m_rightInputValueList = new List<Vector2>();

    /// <summary>
    /// �����̃}�E�X�̓��͒l���i�[���郊�X�g
    /// </summary>
    private List<Vector2> m_leftInputValueList = new List<Vector2>();

    /// <summary>
    /// �E���̃}�E�X�̓��͒l���X�g�̗v�f�����擾
    /// </summary>
    /// <returns> ���X�g�̗v�f�� </returns>
    public Int32 GetRightInputValueListCount()
    {
        return m_rightInputValueList.Count;
    }

    /// <summary>
    /// �����̃}�E�X�̓��͒l���X�g�̗v�f�����擾
    /// </summary>
    /// <returns> ���X�g�̗v�f�� </returns>
    public Int32 GetLeftInputValueListCount()
    {
        return m_leftInputValueList.Count;
    }

    /// <summary>
    /// �E���̃}�E�X�̓��͒l���X�g����擪�̗v�f�����o���B
    /// </summary>
    /// <returns> �E���̃}�E�X�̓��͒l </returns>
    /// <exception cref="IndexOutOfRangeException"> �v�f���� 0 �̎��͎g�p�ł��܂��� </exception>
    public Vector2 PopRightInputValueList()
    {
        if (0 < m_rightInputValueList.Count)
        {
            //�ǂݍ��񂾗v�f���폜
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
    /// �����̃}�E�X�̓��͒l���X�g����擪�̗v�f�����o��
    /// </summary>
    /// <returns> �����̃}�E�X�̓��͒l </returns>
    /// <exception cref="IndexOutOfRangeException"> �v�f���� 0 �̎��͎g�p�ł��܂��� </exception>
    public Vector2 PopLeftInputValueList()
    {
        if (0 < m_leftInputValueList.Count)
        {
            //�ǂݍ��񂾗v�f���폜
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

        //�E�B���h�E�v���V�[�W���̒u������
        m_oldWndProcPtr = WindowsRawInput.SetWindowLongPtr(m_hMainWindow, WindowsRawInput.GWLP_WNDPROC, m_newWndProcPtr);
    }

    private void OnDestroy()
    {
        Term();
    }

    /// <summary>
    /// RawData���󂯎�邽�߃}�E�X���f�o�C�X�o�^����
    /// </summary>
    /// <param name="hwnd">���͂��Ď�����E�B���h�E�̃n���h��</param>
    /// <exception cref="ApplicationException">�f�o�C�X�o�^�Ɏ��s�����瓊���܂�</exception>
    public void RegisterMouseDevice(IntPtr hwnd)
    {
        WindowsRawInput.RAWINPUTDEVICE[] rid = new WindowsRawInput.RAWINPUTDEVICE[1];
        rid[0].usUsagePage = Convert.ToUInt16(1);//�}�E�X��\��ID
        rid[0].usUsage = Convert.ToUInt16(2);//�}�E�X��\��ID
        rid[0].dwFlags = WindowsRawInput.RIDEV_INPUTSINK;//���[�h�I��
        rid[0].hwndTarget = hwnd;//�E�B���h�E�̃n���h��

        if (!WindowsRawInput.RegisterRawInputDevices(rid, Convert.ToUInt32(rid.Length), Convert.ToUInt32(Marshal.SizeOf(rid[0]))))
        {
            throw new ApplicationException("Failed to register raw input device(s). " +
                "Error code: " + Marshal.GetLastWin32Error());
        }
    }

    /// <summary>
    /// �E�B���h�E���b�Z�[�W��������Ă΂��E�B���h�E�v���V�[�W���ł�
    /// </summary>
    /// <param name="hWnd"> �E�B���h�E�̃n���h�� </param>
    /// <param name="msg"> �E�B���h�E���b�Z�[�W </param>
    /// <param name="wParam"> �ǉ��̃��b�Z�[�W��� </param>
    /// <param name="lParam"> �ǉ��̃��b�Z�[�W��� </param>
    /// <returns> ���b�Z�[�W�Ɉˑ����܂� </returns>
    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            //RawInputData�������Ă����Ƃ�
            case WindowsRawInput.WM_INPUT:
                UInt32 dwSize = 0;
                if (WindowsRawInput.GetRawInputData(lParam, (uint)WindowsRawInput.RID_Type.RID_INPUT, IntPtr.Zero, ref dwSize, m_rawInputHeaderSize) != 0)
                {
                    Debug.LogError("����GRID���s");
                }

                IntPtr rawInputPointer = IntPtr.Zero;
                rawInputPointer = Marshal.AllocHGlobal((int)dwSize);
                if (WindowsRawInput.GetRawInputData(lParam, (uint)WindowsRawInput.RID_Type.RID_INPUT, rawInputPointer, ref dwSize, m_rawInputHeaderSize) != dwSize)
                {
                    Debug.LogError("GetRawInputData does not return correct size !");
                }

                WindowsRawInput.RAWINPUT rawInput = Marshal.PtrToStructure<WindowsRawInput.RAWINPUT>(rawInputPointer);
                
                //�}�E�X�̈ړ��ʂ��擾�����E�Ή����郊�X�g�Ɋi�[
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

        //100�ȏ�̃f�[�^������Ƃ��͌Â��f�[�^���폜
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
    /// �������̊J�������܂�
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
    /// Handler.txt����n���h����ǂݍ���
    /// </summary>
    private void ReadHandlePtr()
    {
        // Assets�t�H���_�ւ̃p�X���擾
        string assetsPath = Application.dataPath;

        // Handler.txt�ւ̃p�X���\�z
        string filePath = Path.Combine(assetsPath, "Mobitep/Handler.txt");

        // �t�@�C������e�L�X�g��ǂݍ���
        string content = File.ReadAllText(filePath);

        string[] handleArr = content.Split(new string[] { "\n" }, System.StringSplitOptions.None);

        m_lMouce = new IntPtr(Convert.ToInt32(handleArr[0]));
        m_rMouce = new IntPtr(Convert.ToInt32(handleArr[1]));
    }

}
