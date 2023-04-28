using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float NORMAL_SPEED = 10;
    [SerializeField] float currentMoveSpeed = 0;
    [SerializeField] float turnSpeed = 0;
    Vector3 inputVector;

    [Header("Dashing")]
    [SerializeField] float dashTime = 1;
    [SerializeField] float coolDownTime = 2;
    [SerializeField] float DASH_SPEED = 20;
    enum DashStates {available, dashing, cooldown};
    DashStates currentDashState = DashStates.available;
    float dashTicks = 0;

    [Header("Collision Algorithm")]
    [SerializeField] float capsuleHeight = 1;
    [SerializeField] float capsuleRadius = 1;
    [SerializeField] float maxDistance = 0.5f;

    [Header("Shooting")]
    [SerializeField] GameObject bulletPrefab;
    enum ShootStates {shoot, waiting, disabled}
    ShootStates currentShootingState = ShootStates.disabled;
    float shootTicks = 0;
    float shootWaitTime = 0.1f;
    [SerializeField] Transform gunEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        currentMoveSpeed = NORMAL_SPEED;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();

        LookAtMouse();

        if(Input.GetMouseButtonDown(0)){
            StartShooting();
        }

        if(Input.GetMouseButtonUp(0)){
            StopShooting();
        }

        if(Input.GetMouseButtonDown(1)){
            //start dash mechanic
            StartPlayerDash();
        }

        switch (currentShootingState)
        {
            case ShootStates.shoot:
                //spawn bullet
                GameObject bullet = Instantiate(bulletPrefab, gunEndPoint.position, gunEndPoint.rotation);
                //start waiting for next bullet
                shootTicks = 0;
                currentShootingState = ShootStates.waiting;
                break;
            case ShootStates.waiting:
                //wait for next 
                shootTicks += Time.deltaTime;
                shootTicks = shootTicks > shootWaitTime ? shootWaitTime : shootTicks;
                if(shootTicks == shootWaitTime){
                    shootTicks = 0;
                    currentShootingState = ShootStates.shoot;
                }
                break;
            case ShootStates.disabled:
                break;
            default:
                break;
        }

        switch (currentDashState)
        {
            case DashStates.available:
            break;
            case DashStates.dashing:
            dashTicks += Time.deltaTime;
            dashTicks = dashTicks > dashTime ? dashTime : dashTicks; //ternary expression
            if(dashTicks == dashTime){
                //end of state
                dashTicks = 0;
                currentDashState = DashStates.cooldown;
                currentMoveSpeed = NORMAL_SPEED;
            }
            break;
            case DashStates.cooldown:
            dashTicks += Time.deltaTime;
            dashTicks = dashTicks > coolDownTime ? coolDownTime : dashTicks; //ternary expression
            if(dashTicks == coolDownTime){
                //end of state
                dashTicks = 0;
                currentDashState = DashStates.available;
            }
            break;
        }
    }

    private void MovePlayer(){
        inputVector.z = Input.GetAxisRaw("Vertical");
        inputVector.x = Input.GetAxisRaw("Horizontal");

        bool isMovable = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*capsuleHeight, capsuleRadius, inputVector, maxDistance, 1<<6);

        if(!isMovable)
        {
            Vector3 movX = new Vector3(inputVector.x, 0, 0);
            isMovable = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*capsuleHeight, capsuleRadius, movX, maxDistance, 1<<6);

            if(isMovable){
                inputVector = movX;
            }
            else{
                Vector3 movZ = new Vector3(0,0,inputVector.z);
                isMovable = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*capsuleHeight, capsuleRadius, movZ, maxDistance, 1<<6);

                if(isMovable){
                    inputVector = movZ;
                }
            }

        }

        if(isMovable)transform.position += inputVector.normalized * currentMoveSpeed * Time.deltaTime;
    }

    private void LookAtMouse(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction*10, Color.green);
        if(Physics.Raycast(ray, out var hit, Mathf.Infinity, 1<<6)){
            worldPos = hit.point;
            worldPos.y = 0;
            transform.forward = Vector3.Slerp(transform.forward, (worldPos - transform.position).normalized , turnSpeed * Time.deltaTime);
        }
    }

    private void StartPlayerDash(){
        if(currentDashState == DashStates.dashing || currentDashState == DashStates.cooldown) return;
        dashTicks = 0;
        currentDashState = DashStates.dashing;
        currentMoveSpeed = DASH_SPEED;
    }

    private void StartShooting(){
        shootTicks = 0;
        currentShootingState = ShootStates.shoot;
    }

    private void StopShooting(){
        currentShootingState = ShootStates.disabled;
    }
}
