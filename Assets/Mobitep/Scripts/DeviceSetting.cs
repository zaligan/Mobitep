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
    /// DeviceSetting��EditorWindow���쐬���܂��B
    /// </summary>
    public class DeviceSetting : EditorWindow
    {
        /// <summary>
        /// �E�̃}�E�X�̃n���h��
        /// </summary>
        private static IntPtr m_rHandle;

        /// <summary>
        /// ���̃}�E�X�̃n���h��
        /// </summary>
        private static IntPtr m_lHandle;

        /// <summary>
        /// �ڑ�����Ă���}�E�X�̏����i�[���郊�X�g
        /// </summary>
        private static List<WindowsRawInput.PRAWINPUTDEVICELIST> m_mouceList = new List<WindowsRawInput.PRAWINPUTDEVICELIST>();

        /// <summary>
        /// �ڑ�����Ă���L�[�{�[�h�̏����i�[���郊�X�g
        /// </summary>
        private static List<WindowsRawInput.PRAWINPUTDEVICELIST> m_keybordList = new List<WindowsRawInput.PRAWINPUTDEVICELIST>();

        /// <summary>
        /// �ڑ�����Ă��邻�̑��̃f�o�C�X�̏����i�[���郊�X�g
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

                GUILayout.Label("�}�E�X" + i);
                GUILayout.Label("�f�o�C�X�n���h���F" + m_mouceList[i].hDevice);

                if (GUILayout.Button("���ɓo�^"))
                {
                    m_lHandle = m_mouceList[i].hDevice;
                }

                if (GUILayout.Button("�E�ɓo�^"))
                {
                    m_rHandle = m_mouceList[i].hDevice;
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("���F" + m_lHandle);
            GUILayout.Label("�E�F" + m_rHandle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("�K�p"))
            {
                //�n���h�����e�L�X�g�t�@�C���ɕۑ�
                SaveHandleText();
            }

            if (GUILayout.Button("����"))
            {
                this.Close();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Windows�ɐڑ����ꂽ�f�o�C�X�̏�����ނ��Ƃɕ����z��֊i�[
        /// </summary>
        private static void GetDeviceList()
        {
            m_mouceList.Clear();
            m_keybordList.Clear();
            m_elseDeviceList.Clear();

            // ���ݐڑ�����Ă�����̓f�o�C�X�̐����擾
            UInt32 deviceNum = 0;
            WindowsRawInput.GetRawInputDeviceList(null, ref deviceNum, Convert.ToUInt32(Marshal.SizeOf(typeof(WindowsRawInput.PRAWINPUTDEVICELIST))));

            //deviceList�ցA�V�X�e���ɐڑ����ꂽ�f�o�C�X�ARawInputDeviceList�\���̂̔z��������
            WindowsRawInput.PRAWINPUTDEVICELIST[] deviceList = new WindowsRawInput.PRAWINPUTDEVICELIST[deviceNum];
            if (WindowsRawInput.GetRawInputDeviceList(deviceList, ref deviceNum, Convert.ToUInt32(Marshal.SizeOf(typeof(WindowsRawInput.PRAWINPUTDEVICELIST)))) != deviceNum)
            {
                Debug.LogError("We couldn't retrieve the DeviceList correctly.");
            }

            //�f�o�C�X��ނ��Ƃɕ�����
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
        /// �n���h�����e�L�X�g�t�@�C���ɕۑ�
        /// </summary>
        private void SaveHandleText()
        {
            string text = m_lHandle.ToString() + "\n" + m_rHandle.ToString();

            // Assets�t�H���_�ւ̃p�X���擾
            string assetsPath = Application.dataPath;

            // Handler.txt�ւ̃p�X���\�z
            string filePath = Path.Combine(assetsPath, "Mobitep/Handler.txt");
            Debug.Log("SaveHandleText" + filePath);
            // �e�L�X�g���t�@�C���ɏ�������
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// �e�L�X�g�t�@�C������n���h����ǂݍ���
        /// </summary>
        private static void ReadHandleText()
        {
            // Assets�t�H���_�ւ̃p�X���擾
            string assetsPath = Application.dataPath;

            // Handler.txt�ւ̃p�X���\�z
            string filePath = Path.Combine(assetsPath, "Mobitep/Handler.txt");

            // �t�@�C������e�L�X�g��ǂݍ���
            string content = File.ReadAllText(filePath);

            string[] handleArr = content.Split(new string[] { "\n" }, System.StringSplitOptions.None);

            m_lHandle = new IntPtr(Convert.ToInt32(handleArr[0]));
            m_rHandle = new IntPtr(Convert.ToInt32(handleArr[1]));
        }
    }
}
