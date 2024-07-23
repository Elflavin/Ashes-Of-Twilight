using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;
	private float speedY = 0f;

	float horizontalMove = 0f;
	bool jump = false;
	bool dash = false;

	void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));	

		if (gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 && !animator.GetBool("IsInFloor")) 
		{
			animator.SetBool("JumpUp", false);
			speedY = gameObject.GetComponent<Rigidbody2D>().velocity.y;
		} 
		else
		{
			speedY = 0f;
		}

		// Si pulsa espacio salta
		if (Input.GetKeyDown(KeyCode.Space))
		{
			jump = true;
            animator.SetBool("IsInFloor", false);
        }

		// Si pulsa c hace un dahs
		if (Input.GetKeyDown(KeyCode.C))
		{
			dash = true;
		}
	}

	public void OnFall()
	{
        animator.SetFloat("SpeedY", speedY);
    }

	public void OnLanding()
	{
        animator.SetBool("IsInFloor", true);
    }

	void FixedUpdate ()
	{
		// Mover al jugador
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
		jump = false;
		dash = false;
	}
}
