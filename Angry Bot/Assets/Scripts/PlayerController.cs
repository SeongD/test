using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Walk,
    MoveLeft,
    MoveRight,
    Run,
    Attack,
    Dead,
}

public class PlayerController : MonoBehaviour
{
    public PlayerState playerState;
    public Vector3 lookDirection;
    public float speed;
    public float walkSpeed;
    public float runSpeed;
    bool moveSide = false;

    private Animation anim;
    public AnimationClip idleAni;
    public AnimationClip walkAni;
    public AnimationClip moveLeftAni;
    public AnimationClip moveRightAni;
    public AnimationClip runAni;

    private AudioSource audioSrc;
    public AudioClip shotSound;
    public GameObject bullet;
    public Transform shotPoint;
    public GameObject shotFx;

    public Slider LifeBar;
    public float maxHp;
    public float hp;   

    private void Start()
    {
        anim = GetComponent<Animation>();
        audioSrc = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerState != PlayerState.Dead)
        {
            KeyboardInput();
            if (!moveSide)
                LookUpdate(false);
        }

        AnimationUpdate();
    }

    void KeyboardInput()
    {
        float xx = Input.GetAxis("Horizontal");
        float zz = Input.GetAxis("Vertical");

        if (playerState != PlayerState.Attack)
        {
            if (xx != 0 || zz != 0)
            {
                lookDirection = (xx * Vector3.right) + (zz * Vector3.forward);
                moveSide = false;

                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    speed = runSpeed;
                    anim[runAni.name].speed = 2f;
                    playerState = PlayerState.Run;

                }
                else
                {
                    speed = walkSpeed;
                    anim[runAni.name].speed = 1f;
                    playerState = PlayerState.Walk;
                }
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                moveSide = true;
                speed = runSpeed;
                playerState = PlayerState.MoveLeft;
                GoLeft();
            }
            else if (Input.GetKey(KeyCode.E)) 
            {
                moveSide = true;
                speed = runSpeed;
                playerState = PlayerState.MoveRight;
                GoRight();
            }
            else if (playerState != PlayerState.Idle)
            {
                playerState = PlayerState.Idle;
                speed = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerState != PlayerState.Dead)
            StartCoroutine(nameof(Shot));   // StartCoroutine(Shot());
    }

    private void GoLeft() 
    {  
        transform.Translate(Vector3.left * speed * Time.deltaTime);    
    }
    private void GoRight() 
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void LookUpdate(bool rightNow)
    {
        Quaternion r = Quaternion.LookRotation(lookDirection);
        if (rightNow)
            transform.rotation = r;
        else
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, r, 600f * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void AnimationUpdate()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                anim.CrossFade(idleAni.name, 0.2f);
                break;
            case PlayerState.Walk:
                anim.CrossFade(walkAni.name, 0.2f);
                break;
            case PlayerState.MoveLeft:
                anim.CrossFade(moveLeftAni.name, 0.2f);
                break;
            case PlayerState.MoveRight:
                anim.CrossFade(moveRightAni.name, 0.2f);
                break;
            case PlayerState.Run:
                anim.CrossFade(runAni.name, 0.2f);
                break;
            case PlayerState.Attack:
                anim.CrossFade(idleAni.name, 0.2f);
                break;
            case PlayerState.Dead:
                anim.CrossFade(idleAni.name, 0.2f);
                break;
        }
    }

    public IEnumerator Shot()
    {
        GameObject bulletObj = Instantiate(
            bullet,
            shotPoint.position,
            Quaternion.LookRotation(shotPoint.forward));

        Physics.IgnoreCollision(
            bulletObj.GetComponent<Collider>(),
            GetComponent<Collider>());

        audioSrc.clip = shotSound;
        audioSrc.Play();

        shotFx.SetActive(true);

        playerState = PlayerState.Attack;
        speed = 0;

        yield return new WaitForSeconds(0.15f);
        shotFx.SetActive(false);

        yield return new WaitForSeconds(0.15f);
        playerState = PlayerState.Idle;
    }

    public void Hurt(float damage) 
    {
        if (hp > 0) 
        {
            hp -= damage;
            LifeBar.value = hp / maxHp;
        }

        if (hp <= 0)
        {
            speed = 0;
            playerState = PlayerState.Dead;

            PlayManager pm = GameObject.Find("PlayManager").GetComponent<PlayManager>();
            pm.GameOver();            
        }
    }
}
