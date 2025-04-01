using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    public RectTransform[] items;
    public float animationTime;

    private Vector3[] startPos;
    private Quaternion[] startRot;
    private Rigidbody2D[] itemRBs;
    private float timer;


    void Start()
    {
        startPos = new Vector3[items.Length];
        startRot = new Quaternion[items.Length];
        itemRBs = new Rigidbody2D[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            startPos[i] = items[i].position;
            startRot[i] = items[i].rotation;
            itemRBs[i] = items[i].GetComponent<Rigidbody2D>();
        }

        timer = animationTime;
        gameObject.SetActive(false);
    }
    

    void Update()
    {
        if(timer <= 0)
        {
            ResetAnimations();
            timer = animationTime;
        }

        timer -= Time.deltaTime;
    }

    public void ResetAnimations()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].position = startPos[i];
            items[i].rotation = startRot[i];
            itemRBs[i].velocity = Vector2.zero;
            itemRBs[i].angularVelocity = 0f;
        }
    }
}
