using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintInput : MonoBehaviour
{
    private MouseInputReader m_mouseInputReader;

    /// <summary>
    /// EditorWindow�ŉE�ɐݒ肵���f�o�C�X�̓��͒l
    /// </summary>
    private Vector2 m_rightInputValueVec;

    /// <summary>
    /// EditorWindow�ō��ɐݒ肵���f�o�C�X�̓��͒l
    /// </summary>
    private Vector2 m_leftInputValueVec;

    // Start is called before the first frame update
    void Start()
    {
        // ����GameObject�ɃA�^�b�`����Ă���MouseInputReader���擾
        m_mouseInputReader = GetComponent<MouseInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        // �v�f���� 0 ���傫���ꍇ�̂ݓ��͒l���擾
        if (0 < m_mouseInputReader.GetRightInputValueListCount())
        {
            m_rightInputValueVec = m_mouseInputReader.GetRightInputValueList();
        }

        if (0 < m_mouseInputReader.GetLeftInputValueListCount())
        {
            m_leftInputValueVec = m_mouseInputReader.GetLeftInputValueList();
        }

        Debug.Log("Right: " + m_rightInputValueVec);
        Debug.Log("Left: " + m_leftInputValueVec);
    }
}
