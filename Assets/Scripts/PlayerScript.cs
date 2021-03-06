﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {
    private float     xPos;
    public float      xspeed = .03f;
    private float     yPos = -4f;
    public float      yspeed = .03f;
    public float      leftWall, rightWall, topWall, bottomWall, pineappleCount;
    private Rigidbody2D rb;
    public GameObject Projectile;
    public Image healthbar;
    public KeyCode fireKey;
    public float health = 1f;
    public Animator animator;

    public List<GameObject> powerUps;

    public GameObject explosion, gameOver;


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("isDead", false);
    }

    // Update is called once per frame
    private bool isMoving = false;

    void Update() {
        if (health < 0) {
            GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>().isGameOver = true;
        }

        if (!GameObject.FindGameObjectWithTag("gamemanager").GetComponent<GameManagerScript>().isGameOver) {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                isMoving = true;
                if (xPos > leftWall) {
                    xPos -= xspeed;
                }
            }

            if (Input.GetKey(KeyCode.RightArrow)) {
                isMoving = true;
                if (xPos < rightWall) {
                    xPos += xspeed;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow)) {
                isMoving = true;
                if (yPos < topWall) {
                    yPos += yspeed;
                }
            }

            if (Input.GetKey(KeyCode.DownArrow)) {
                isMoving = true;
                if (yPos > bottomWall) {
                    yPos -= yspeed;
                }
            }

            if(!Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) ) {
                isMoving = false;
            }

            if (Input.GetKeyDown(fireKey)) {
                Instantiate(Projectile, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);

                if (pineappleCount > 0){
                    for (int i = 1; i <= pineappleCount; i++) {
                        if(i%2 == 0){
                            Instantiate(Projectile, new Vector2(transform.position.x - (-0.1f * (i - 0.5f)), transform.position.y + 0.5f), Quaternion.identity);
                        }else{
                            Instantiate(Projectile, new Vector2(transform.position.x - (0.1f * (i + 0.5f)), transform.position.y + 0.5f), Quaternion.identity);
                        }
                    }
                }
            }
            
            transform.localPosition = new Vector3(xPos, yPos, 0);
            animator.SetBool("isMoving", isMoving);
        } else {
            animator.SetBool("isDead", true);
            gameOver.SetActive(true);

            if (Input.GetKeyDown(fireKey)) {
                gameOver.SetActive(false);
                SceneManager.LoadScene("Main");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "enemyprojectile")
        {
            Destroy(other.gameObject);
            health -= .1f;
            healthbar.fillAmount = health;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "powerup")
        {
            if (other.gameObject.GetComponent<PowerScript>().isFollowing == false) {
                //Debug.Log("Powerup Hit");
                other.gameObject.GetComponent<PowerScript>().isFollowing = true;
                other.gameObject.GetComponent<PowerScript>().position = powerUps.Count + 1;
                other.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);

                if (powerUps.Count == 0) {
                    other.gameObject.GetComponent<PowerScript>().followObject = GameObject.FindWithTag("Player");
                } else {
                    other.gameObject.GetComponent<PowerScript>().followObject = powerUps[powerUps.Count - 1];
                }

                if (other.gameObject.name == "Pineapple(Clone)") {
                    pineappleCount++;
                }

                powerUps.Add(other.gameObject);
            }
        }

        if (other.gameObject.tag == "enemy") {
            health -= .3f;
            healthbar.fillAmount = health;
            Instantiate(explosion, other.gameObject.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
    }
}

