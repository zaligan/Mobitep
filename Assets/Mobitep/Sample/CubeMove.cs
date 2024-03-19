using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    /// <summary>
    /// �E�ɐݒ肵���}�E�X�œ������I�u�W�F�N�g
    /// </summary>
    [SerializeField] GameObject R;

    /// <summary>
    /// ���ɐݒ肵���}�E�X�œ������I�u�W�F�N�g
    /// </summary>
    [SerializeField] GameObject L;

    /// <summary>
    /// �I�u�W�F�N�g R �� L �̈ړ����x
    /// </summary>
    public float MoveSpeed = 0.1f;

    /// <summary>
    /// �����H�̓��͂�ǂݍ��ރN���X
    /// </summary>
    private MouseInputReader m_mouseInputReader;

    /// <summary>
    /// �E�̃}�E�X�̈ړ��x�N�g��
    /// </summary>
    private Vector2 m_rightInputValueVec;

    /// <summary>
    /// ���̃}�E�X�̈ړ��x�N�g��
    /// </summary>
    private Vector2 m_leftInputValueVec;

    // Start is called before the first frame update
    void Start()
    {
        m_mouseInputReader = GetComponent<MouseInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        // ���͂�����Γǂݍ���
        if (0 < m_mouseInputReader.GetRightInputValueListCount())
        {
            m_rightInputValueVec = m_mouseInputReader.PopRightInputValueList() * MoveSpeed;
        }
        else
        {
            m_rightInputValueVec = Vector2.zero;
        }

        if (0 < m_mouseInputReader.GetLeftInputValueListCount())
        {
            m_leftInputValueVec = m_mouseInputReader.PopLeftInputValueList() * MoveSpeed;
        }
        else
        {
            m_leftInputValueVec = Vector2.zero;
        }

        // �I�u�W�F�N�g���ړ�������
        R.transform.position += new Vector3(m_rightInputValueVec.x, 0, -m_rightInputValueVec.y);
        L.transform.position += new Vector3(m_leftInputValueVec.x, 0, -m_leftInputValueVec.y);

        Debug.Log("Right: " + m_rightInputValueVec);
        Debug.Log("Left: " + m_leftInputValueVec);
    }
}
