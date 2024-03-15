using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{
    [SerializeField] GameObject R;

    [SerializeField] GameObject L;

    /// <summary>
    /// �I�u�W�F�N�g R �� L �̈ړ����x
    /// </summary>
    public float MoveSpeed = 0.1f;

    private MouseInputReader m_mouseInputReader;

    private Vector2 m_rightInputValueVec;

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
            m_rightInputValueVec = m_mouseInputReader.GetRightInputValueList() * MoveSpeed;
        }
        else
        {
            m_rightInputValueVec = Vector2.zero;
        }

        if (0 < m_mouseInputReader.GetLeftInputValueListCount())
        {
            m_leftInputValueVec = m_mouseInputReader.GetLeftInputValueList() * MoveSpeed;
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
