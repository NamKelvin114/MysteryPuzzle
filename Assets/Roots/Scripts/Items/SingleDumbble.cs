using System;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zlib;
using UnityEngine;

public class SingleDumbble : BaseItem
{
    // Start is called before the first frame update
    private Rigidbody2D rigid;
    public SingleDumbbleState state;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform rayCastPosLeft;
    [SerializeField] private Transform rayCastPosRight;
    private bool stop = false;
    void Start()
    {
        state = SingleDumbbleState.Idle;
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop) CheckSeeDumbbleBar();
    }

    private void CheckSeeDumbbleBar()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(rayCastPosLeft.position, Vector2.left, 3f);
        Debug.DrawRay(transform.position, Vector2.left * 3f, Color.red, 5f);
        if (hitLeft.collider != null)
            if (hitLeft.collider.gameObject.CompareTag("DumbbellBar"))
            {
                state = SingleDumbbleState.Moving;
                MoveLeft();
                return;
            }
        
        RaycastHit2D hitRight = Physics2D.Raycast(rayCastPosRight.position, Vector2.right, 3f);
        Debug.DrawRay(transform.position, Vector2.right * 3f, Color.blue, 5f);
        if (hitRight.collider != null)
            if (hitRight.collider.gameObject.CompareTag("DumbbellBar"))
            {
                state = SingleDumbbleState.Moving;
                MoveRight();
                return;
            }

    }

    private void MoveLeft()
    {
        if (state == SingleDumbbleState.Moving)
        {
            Vector2 _movement = Vector2.left * moveSpeed;
            rigid.velocity = new Vector2(_movement.x, rigid.velocity.y);
        }
        else rigid.velocity = new Vector2(0, rigid.velocity.y);
    }

    public void MoveRight()
    {
        if (state == SingleDumbbleState.Moving)
        {
            Vector2 _movement = Vector2.right * moveSpeed;
            rigid.velocity = new Vector2(_movement.x, rigid.velocity.y);
        }
        else rigid.velocity = new Vector2(0, rigid.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("DumbbellBar"))
        {
            stop = true;
            DumbbellBar _dumbbellBar = col.gameObject.GetComponent<DumbbellBar>();
            _dumbbellBar.singelDumbbleCount++;
            _dumbbellBar.AddSingleDumbleToList(this);
            if (_dumbbellBar.singelDumbbleCount == 2)
            {
                _dumbbellBar.ActiveComplete();
            }
        }
    }
}

public enum SingleDumbbleState
{
    Idle,
    Moving
}
