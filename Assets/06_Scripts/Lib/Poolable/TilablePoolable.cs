using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilablePoolable : CameraDependantPoolable
{
    bool m_hasSpawned;

    [SerializeField]
    bool m_canSpawnRigth;

    [SerializeField]
    bool m_canSpawnLeft;

    float m_percentScale = 1.0f;

    public bool CurrentCanSpawnLeft { protected get; set; }
    public bool CurrentCanSpawnRigth { protected get; set; }
    public int[] Position { get => m_position; set => m_position = value; }
    public int Id { get => m_id; set => m_id = value; }
    protected List<TilablePoolable> NextObjectPrefabs { get; set; }
    protected List<TilablePoolable> LeftRightObjectPrefabs { get; set; }
    public float PercentScale { protected get => m_percentScale; set => m_percentScale = value; }

    int m_id;

    int[] m_position;

    SpriteRenderer m_spriteRenderer;

    Vector3 m_min;
    Vector3 m_max;

    // Start is called before the first frame update
    protected void Start()
    {
        CurrentCanSpawnLeft = m_canSpawnLeft;
        CurrentCanSpawnRigth = m_canSpawnRigth;
        StartCoroutine(Loop());
    }


    public void Init(List<TilablePoolable> a_nextObjectPrefabs, List<TilablePoolable> a_leftRightObjectPrefabs)
    {
        SetCameraOffsetDestroy();
        IsDestroyed = false;
        TileManager.Instance.Spawn(Id, Position[0], Position[1]);
        NextObjectPrefabs = a_nextObjectPrefabs;
        LeftRightObjectPrefabs = a_leftRightObjectPrefabs;

        transform.localScale = m_percentScale * m_initScale;




        if (m_spriteRenderer == null)
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }

        m_min = m_spriteRenderer.bounds.min;
        m_max = m_spriteRenderer.bounds.max;
    }


    public void SetAsFirst(List<TilablePoolable> a_nextObjectPrefabs, List<TilablePoolable> a_leftRightObjectPrefabs)
    {
        Id = TileManager.Instance.GetNextFreeId();
        Position = new int[2] { 0, 0 };
        Init(a_nextObjectPrefabs, a_leftRightObjectPrefabs);
    }


    IEnumerator Loop()
    {
        while (true)
        {
            CheckSpawn();
            yield return UtilsYield.GetWaitForSeconds(0.5f);
        }
    }

    void CheckSpawn()
    {
        if (!IsDestroyed)
        {
            //spawn under
            if (!m_hasSpawned && TileManager.Instance.CanSpawn(Id, Position[0], Position[1] - 1)
                && CameraManager.Instance.IsYBetween(GetMin(), GetMax()) && ((!CurrentCanSpawnLeft && !CurrentCanSpawnRigth) || (CameraManager.Instance.IsXBetween(GetMin(), GetMax()))))
            {
                m_hasSpawned = true;
                TilablePoolable spawned = PoolerManager.Instance.InstantiatePoolable(NextObjectPrefabs[Utils.RandomInt(0, NextObjectPrefabs.Count)]);
                spawned.transform.position = new Vector3(transform.position.x, GetMin().y, transform.position.z);
                spawned.Position = new int[2] { Position[0], Position[1] - 1 };
                InitSpawned(spawned, false);
            }

            if (CameraManager.Instance.IsXBetween(GetMin(), GetMax()))
            {
                //spawn left
                if (CurrentCanSpawnLeft && TileManager.Instance.CanSpawn(Id, Position[0] - 1, Position[1]))
                {
                    TilablePoolable spawned = PoolerManager.Instance.InstantiatePoolable(LeftRightObjectPrefabs[Utils.RandomInt(0, LeftRightObjectPrefabs.Count)]);
                    spawned.transform.position = new Vector3(transform.position.x - (GetMax().x - GetMin().x), transform.position.y, transform.position.z);
                    spawned.CurrentCanSpawnRigth = false;
                    spawned.Position = new int[2] { Position[0] - 1, Position[1] };
                    CurrentCanSpawnLeft = false;
                    InitSpawned(spawned, true);
                }

                //spawn right
                if (CurrentCanSpawnRigth && TileManager.Instance.CanSpawn(Id, Position[0] + 1, Position[1]))
                {
                    TilablePoolable spawned = PoolerManager.Instance.InstantiatePoolable(LeftRightObjectPrefabs[Utils.RandomInt(0, LeftRightObjectPrefabs.Count)]);
                    spawned.transform.position = new Vector3(transform.position.x + (GetMax().x - GetMin().x), transform.position.y, transform.position.z);
                    spawned.CurrentCanSpawnLeft = false;
                    spawned.Position = new int[2] { Position[0] + 1, Position[1] };
                    CurrentCanSpawnRigth = false;
                    InitSpawned(spawned, true);
                }
            }
        }
    }

    void InitSpawned(TilablePoolable a_spawned, bool a_keepRightLeftPrefab)
    {
        a_spawned.Id = Id;
        a_spawned.PercentScale = PercentScale;
        a_spawned.Init(NextObjectPrefabs, a_keepRightLeftPrefab ? LeftRightObjectPrefabs : NextObjectPrefabs);
    }

    Vector3 GetMin()
    {
        return m_min;// m_spriteRenderer.bounds.min;
    }

    Vector3 GetMax()
    {
        return m_max;// m_spriteRenderer.bounds.max;
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        CheckSpawn();
        CurrentCanSpawnLeft = m_canSpawnLeft;
        CurrentCanSpawnRigth = m_canSpawnRigth;
        m_hasSpawned = false;
        TileManager.Instance.Free(Id, Position[0], Position[1]);
    }

    protected override Vector3 GetPos()
    {
        return GetMin();
    }

}