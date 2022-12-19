using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorObject : OnActionObject
{
    [Header("Door Settings")]
    public bool locked = false;
    public bool isOpen = false;
    public bool automatic = true;
    public float timeOpen = 2f;

    [Header("Animations")]
    public float animationSpeed = 0.5f;
    public float numFrames = 30f;
    
    [Header("Door Objects")]
    public GameObject doorAsset;
    public GameObject colliderA;
    public GameObject colliderB;
    public GameObject lightObj;

    [Header("Lights")]
    public Material unlockedLight;
    public Material lockedLight;

    [HideInInspector]
    public bool active = false;
    [HideInInspector]
    public Collider doorCollider; 
    [HideInInspector]
    public UnityEvent openEvent = new UnityEvent();
    private List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();
    [HideInInspector]
    public bool fightingFires = false;
    [HideInInspector]
    public ChildCollider[] childColliders = new ChildCollider[2];

    void Start(){
        //populate renderers
        int numChildren = doorAsset.transform.childCount;
        for (int i = 0; i < numChildren; i++){
            SkinnedMeshRenderer temp = doorAsset.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
            if (temp == null) continue;
            renderers.Add(temp);
        }
        //get collider for door (to disable/enable)
        doorCollider = doorAsset.GetComponent<Collider>();
        //get child colliders
        childColliders[0] = colliderA.GetComponent<ChildCollider>();
        childColliders[1] = colliderB.GetComponent<ChildCollider>();

        //make sure door is not open if locked
        if (locked) {
            isOpen = false;
            //turn light red
            lightObj.GetComponent<Renderer>().material = lockedLight;
        }
        else {
            //turn light green
            lightObj.GetComponent<Renderer>().material = unlockedLight;
        }
       

        //open door if open is checked
        if (isOpen && !locked && !automatic){
            foreach (SkinnedMeshRenderer renderer in renderers){
                renderer.SetBlendShapeWeight(0, 100f);
            }
        }     
    }

    //unlocks the door because the action has occurred
    public override void OnAction(){
        if (fightingFires) return; //no door stuff when fighting fires!!!
        if (locked){
            //unlock
            locked = false; 
            //turn light green
            lightObj.GetComponent<Renderer>().material = unlockedLight;
        } 
        else return;
        if (!automatic) StartCoroutine(OpenDoor());
    }

    public void ChildCollision(){
        if (fightingFires) return; //no door stuff when fighting fires!!!
        if (!active && automatic && !isOpen && !locked){
            StartCoroutine(AutomaticDoor());
        }
    }

    public IEnumerator AutomaticDoor(){
        yield return StartCoroutine(OpenDoor());
        yield return new WaitForSeconds(timeOpen);
        yield return StartCoroutine(CloseDoor());
    }

    public IEnumerator OpenDoor(){
        if (fightingFires) yield return null; //no door stuff when fighting fires!!!
        //verify not already open
        if (isOpen) yield return null;
        //make sounds!
        GameManager.instance.AudioEvent(AudioEvents.instance.StartDoorOpen);

        active = true;
        //set step speed and blendshape values
        float stepSpeed = animationSpeed/numFrames;
        float blendSteps = 100f/numFrames;

        //temp blendshape value holder
        float bsValue = 0f;
        //while the door is closed, update blendshapes over time
        while(!isOpen){
            foreach (SkinnedMeshRenderer renderer in renderers){
                bsValue = renderer.GetBlendShapeWeight(0);
                bsValue += blendSteps;
                if (bsValue >= 100){
                    bsValue = 100f;
                    isOpen = true;
                }
                renderer.SetBlendShapeWeight(0, bsValue);
            }

            yield return new WaitForSeconds(stepSpeed);
        }  
        
        active = false;
        //disable collider bc door is open
        doorCollider.enabled = false;
        //notify anyone listening that the door opened
        openEvent.Invoke();

        //make sounds!
        GameManager.instance.AudioEvent(AudioEvents.instance.FinishDoorOpen);
        yield return null;
    }

    public IEnumerator CloseDoor(){
        //verify not already closed
        if (!isOpen) yield return null;
        //make sure the character is out of range
        while (childColliders[0].playerCollision || childColliders[1].playerCollision){
            yield return new WaitForSeconds(timeOpen);
        }

        //make sounds!
        GameManager.instance.AudioEvent(AudioEvents.instance.StartDoorClose);

        //enable collider bc door is closed
        doorCollider.enabled = true;
        active = true;
        //set step speed and blendshape values
        float stepSpeed = animationSpeed/numFrames;
        float blendSteps = 100f/numFrames;

        //temp blendshape value holder
        float bsValue = 0f;
        //while the door is closed, update blendshapes over time
        while(isOpen){
            foreach (SkinnedMeshRenderer renderer in renderers){
                bsValue = renderer.GetBlendShapeWeight(0);
                bsValue -= blendSteps;
                if (bsValue <= 0){
                    bsValue = 0f;
                    isOpen = false;
                }
                renderer.SetBlendShapeWeight(0, bsValue);
            }

            yield return new WaitForSeconds(stepSpeed);
        }  

        active = false;
        //make sounds!
        GameManager.instance.AudioEvent(AudioEvents.instance.FinishDoorClose);
        yield return null;
    }

    //this is just for integration with the firefighter task
    public void FirefighterTask(bool taskStart){
        if (taskStart){
            fightingFires = true;
            if (locked) return;
            //turn light red
            lightObj.GetComponent<Renderer>().material = lockedLight;
            //close door
            StartCoroutine(CloseDoor());
        }
        else {
            fightingFires = false;
            if (locked) return;
            //turn light green
            lightObj.GetComponent<Renderer>().material = unlockedLight;
            //open door
            if (!automatic) StartCoroutine(OpenDoor());
        }
    }

}
