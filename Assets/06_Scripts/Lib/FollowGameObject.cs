using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    public GameObject Target { get; set; }
    public float Speed { get => m_speed; set => m_speed = value; }

    float m_speed;


    [SerializeField]
    float m_randomDirection = 5;


    [SerializeField]
    float m_scaleRandom = 10;

    float m_id;

    // Start is called before the first frame update
    void Start()
    {
        m_id = Utils.RandomFloat(0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsPause)
        {
            return;
        }

        if (Target != null)
        {
            Vector3 direction = Quaternion.AngleAxis(m_randomDirection * (Mathf.PerlinNoise(m_id, Time.time * m_scaleRandom) - 0.5f) * 2, Vector3.forward) * (Target.transform.position - transform.position).normalized;
            transform.position += direction * Speed * Time.deltaTime;
            transform.up = direction;
        }
    }
}
