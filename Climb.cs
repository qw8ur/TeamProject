
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Climb : MonoBehaviour
{
    public Animator animator;
    float charaterHeight = 2f;
    float characterRadius = 0.3f;

    public LayerMask tMask;//�a�O
    public float climbSpeed = 1f; 
    public LayerMask ClimbableMask;//�i�k���ϼh
    float maxDetectDistance = 1.5f;//�̤j�˴��Z��(�t����b�|)
    float offsetFromWall = 0.15f;//���������q
    Collider collider;
    Rigidbody body;
    private Transform wall;
    RaycastHit bottom, top;
    private float step;
    //private Vector3 previousPosition;
    //private Vector3 currentPosition;
    //private Vector3 direction;


    public ClimbStatus climbStatus;
    public enum ClimbStatus
    { 
        notAtClimbing,
        isOnTheWall,

    }

    void Start()
    {

        body = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        //previousPosition = transform.position;


    }

    private void Awake()
    {
        if (wall== null) 
        {
            wall = new GameObject("wall").transform;
        }
    }


    float v;
    float h;
    float q;

    void Update()

    {
        //currentPosition = transform.position;
        //direction = currentPosition - previousPosition;
        //previousPosition = currentPosition;
        //Debug.Log("Direction: " + direction);

        RaycastHit hit;
        var origin = transform.position + (transform.up * characterRadius);
        Physics.Raycast(origin, transform.forward, out hit, 2f, ClimbableMask);
        Debug.DrawRay(origin, transform.forward * 2f, Color.blue);
      



        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
       


       
        DectectWallOnDepends();

        //�b��W��
        if (climbStatus == ClimbStatus.isOnTheWall)
            pClimb();


            //�h�X���𪬺A
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
            ExitClimbWall();
            }
            


    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.05f);
    }

  
    private Quaternion targetRotation;
    private Vector3 targetPos;
    

    void pClimb()
    {

        //�W�U����
     
        transform.position = Vector3.MoveTowards(transform.position, targetPos, v * Time.deltaTime);
        transform.Translate(Vector3.up * v * Time.deltaTime* climbSpeed);
        //����
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
        //���k����
        transform.Translate(Vector3.right * h * Time.fixedDeltaTime);
       

    }



    bool DectectWallOnDepends()
    {
        if (climbStatus == ClimbStatus.notAtClimbing && Time.frameCount % 60 != 0 && v > 0)
        {
            if (DectectWallRightNow( out top, out bottom))
            {
                EnterClimbWall();
                return true;
            }
        
        }

        if (climbStatus == ClimbStatus.isOnTheWall)
        {
            
            Ray ray = new(transform.position, -transform.up);
            if (Physics.Raycast(ray, out RaycastHit hit, 2f, tMask))
            {
               
                var playerPos = transform.position;
                playerPos -= Vector3.up * 1.5F;
                ExitClimbWall();
                animator.CrossFade("idleBreath", 0.5F);
                animator.SetFloat("Move", 0F);
                animator.SetBool("IsClimbUp", false);
            }

           
            var topOrigin = transform.position + Vector3.up*1.5f;
            var bottomOrigin = transform.position - Vector3.up*0.8f;
            Physics.Raycast(topOrigin, transform.forward, out top, 5f, ClimbableMask);
            Physics.Raycast(bottomOrigin, transform.forward, out bottom, 5f, ClimbableMask);

            Debug.DrawRay(topOrigin, transform.forward * 5f, Color.black);
            Debug.DrawRay(bottomOrigin, transform.forward * 5f, Color.black);

            if (IsGrounded() && Time.frameCount % 60 != 0 && v < 0)
            {
                Debug.Log("�ۦa");
                ExitClimbWall();
            }
            if (DectectWallRightNow( out top,out bottom))
            {
                UpdateClimbWall();
                return true;
            }
            else
            {
                if (top.collider == null )
                {
                    Debug.Log("top.collider == null");
                 
                    animator.SetBool("IsClimbUp", false);
                    animator.CrossFade("HangingIdle", 0F);
                    UpdateClimbWall();
                }
                if (bottom.collider == null)
                {
                   ExitClimbWall();
                }
               

                    return false;
            }
           
            
        }
        return false;
    }

    void EnterClimbWall()
    {
        
        climbStatus = ClimbStatus.isOnTheWall;
        Debug.Log("�i�J���𪬺A");
        GetComponent<Rigidbody>().isKinematic=true;
        step = Vector3.Distance(transform.position, targetPos) / 0.05f;
        SetCharacterPosision(bottom, true);
        collider = bottom.collider;




        
        
    
    }

    void UpdateClimbWall()
    {
        transform.position += transform.up * v * Time.deltaTime;
        animator.SetBool("IsExitClimbWall", false);


        if (v > 0 || h > 0||h<0)
        {
            animator.SetBool("IsClimbUp", true);
            if (h > 0)
            animator.SetBool("IsClimbUp", true);

        }
       
        else if (v == 0)
        {
            Debug.Log("V>0������");
            if (h > 0 || h < 0)
            animator.SetBool("IsClimbUp", true);

            animator.SetBool("IsClimbUp", false);
            animator.CrossFade("HangingIdle", 0F);
            

        }

        if (v == 0 && h == 0)
        {
            if (h > 0 || h < 0)
            {
                Debug.Log("H>0������");
                animator.SetBool("IsClimbUp", true);
            }
            Debug.Log("�S��J��");
            animator.SetBool("IsClimbUp", false);
            animator.CrossFade("HangingIdle", 0F);

        }
         
            if (v == 0 && Physics.Raycast(transform.position, -transform.up, 2f, tMask))
        {
            Debug.Log("Enter�������U�ɺ����k�����ժ�����");

            animator.CrossFade("idleBreath", 0F);
        }
        

        collider =bottom.collider;


    }

     void ExitClimbWall()
    {
        climbStatus = ClimbStatus.notAtClimbing;
        GetComponent<Rigidbody>().isKinematic=false;

        if (v > 0)
        {
            animator.CrossFade("ClimbUp", 1F);
        }
        if (v == 0 && Physics.Raycast(transform.position, -transform.up, 2f, ClimbableMask))
        {
            Debug.Log("���W���ݮɪ�����");

            animator.CrossFade("idleBreath", 0F);
        }

        Debug.Log("�h�X���𪬺A");
            animator.SetBool("IsExitClimbWall", true);
        
    }

    
    public Collider[] dectetedCollers;
    private Vector3 playerBodyCenter;
    private Vector3 closestPoint;
    
    bool DectectWallRightNow(out RaycastHit hitfromTop ,out RaycastHit hitFromBottom)//�q�����M�����o�g�g�u
    {
        if (wall == null)
        {
            Debug.LogError("Wall object is not assigned. Please assign a valid object to 'wall'.");
            
        }
        //�����˴��I:������I+���⪺����
        Vector3 topDectectPoint = transform.position + Vector3.up * charaterHeight; ;
        Vector3 bottonDectectPoint = transform.position + new Vector3(0, 0.2F, 0);
        Vector3 detectDirection = Vector3.forward;

        //������
        Debug.DrawRay(topDectectPoint, transform.forward*100F, Color.blue);
        Debug.DrawRay(bottonDectectPoint, transform.forward * 100F, Color.red);
        Debug.DrawLine(topDectectPoint, bottonDectectPoint, Color.magenta);

        bool isTopDetected;
        bool isBottomDetected;
        bool isRightDetected;
        bool isLeftDectected;

        if (climbStatus == ClimbStatus.notAtClimbing)
        {
            
            
            
            playerBodyCenter = transform.position + Vector3.up * (charaterHeight - characterRadius);
            //����y��d�򤺪��I����

            dectetedCollers = Physics.OverlapSphere(playerBodyCenter, characterRadius * 10, ClimbableMask, QueryTriggerInteraction.Collide);

            if (dectetedCollers.Length>0)//������F��
            {
                Debug.Log("123 " + dectetedCollers[0]);
                playerBodyCenter = transform.position + Vector3.up * (charaterHeight- characterRadius);

                //�b�����I���I����@�ӶZ�����餤�̪߳񪺤@���I
                closestPoint = dectetedCollers[0].ClosestPoint(playerBodyCenter);
                //��@�ӳ̪��k���I
                foreach (Collider coll in dectetedCollers)
                {
                    //���@�Ӱ��קC���餤�ߪ��I����
                    if (coll.transform.position.y > playerBodyCenter.y)
                        continue;
                    Vector3 point=coll.ClosestPoint(playerBodyCenter);
                    //�p�G���餤�߻P�I����̪��I���Z�� �p�� ���餤�߻PclosestPoint���Z��,�N�אּpoint
                    if(Vector3.Distance(playerBodyCenter,point)<Vector3.Distance(playerBodyCenter,closestPoint))
                        closestPoint = point;

                }

                closestPoint.y = playerBodyCenter.y;
                //�˴���V:�q���餤�ߥX�o��̪��I
                detectDirection = closestPoint - playerBodyCenter;

                //�o������,�����D������O100��
                float angle = Mathf.Abs(Vector3.SignedAngle(detectDirection, transform.forward, Vector3.up));
                if (angle >100)
                    detectDirection=transform.forward;


            }
        
        
        }

        isTopDetected = Physics.Raycast(topDectectPoint, detectDirection, out hitfromTop, maxDetectDistance * 2, ClimbableMask);
        isBottomDetected = Physics.Raycast(bottonDectectPoint, detectDirection, out hitFromBottom, maxDetectDistance, ClimbableMask);

        //ø�s�g�u
        Debug.DrawRay(topDectectPoint, detectDirection * maxDetectDistance, Color.cyan);
        Debug.DrawRay(bottonDectectPoint, detectDirection * maxDetectDistance, Color.cyan);

        //�˴��O�_��F�����t
        if (isTopDetected && isBottomDetected)
        {
            wall.position = hitFromBottom.point;
            wall.forward = hitFromBottom.normal;

            //�q�����˴��I���k��.�����V�W�o�g�g�u
            RaycastHit right, left;
            isRightDetected = Physics.Raycast(bottonDectectPoint + wall.right * 0.5F, detectDirection, out right, Mathf.Infinity, ClimbableMask);
            isLeftDectected = Physics.Raycast(bottonDectectPoint - wall.right * 0.5F, detectDirection, out left, Mathf.Infinity, ClimbableMask);
        }

        return (isTopDetected && isBottomDetected);
    }

    //�W�U����
    private void SetCharacterPosision(RaycastHit bottomhit,bool setToTarget=false)
    {
        //�ؼЦ�m:�����I���I+�����I���I���k�V�q*�����q
        targetPos=bottomhit.point+bottomhit.normal*offsetFromWall;
        targetPos.y= transform.position.y;
        targetRotation = GetRotationFromDirection(-bottomhit.normal);
        if (setToTarget) 
        {
            float t = body.velocity.magnitude * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPos, t);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
        }
        

    }

    private float yaw;

    
    Quaternion GetRotationFromDirection(Vector3 dir)
    {
        var x = dir.x;
        var z = dir.z;
       yaw = Mathf.Atan2(x, z);
        return Quaternion.Euler(0, yaw*Mathf.Rad2Deg, 0);

    }


    public  void DectectTopOfTheWall()
    {
       
        var a = transform.forward.normalized.magnitude;
        var b=transform.up.normalized.magnitude;
        Debug.Log("����");
        //�⨤������
        animator.CrossFade("ClimbTopJump", 0.5f);
        Vector3 targetpoint = new Vector3(transform.position.x, transform.position.y + b * 3f, transform.position.z + a * 2f);
        transform.position = Vector3.Lerp(transform.position, targetpoint, 0.6F);
        animator.CrossFade("walkforward", 0.5f);

    }

}

    





