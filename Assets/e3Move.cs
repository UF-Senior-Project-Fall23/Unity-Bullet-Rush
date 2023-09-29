using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e3Move : MonoBehaviour
{
    public GameObject target;
    public float speed = 2.0f;
    public bool straight = true;
    void Start()
    {

    }
    private void Update()
    {
        var distVal = 50.0f;
        var dis = Vector3.Distance(transform.position, target.transform.position);
        if (dis <= distVal)
        {
            if (straight)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
            }
            else
            {
                transform.position = Vector3.Slerp(transform.position, target.transform.position, speed);
            }
        }
        
    }

}
