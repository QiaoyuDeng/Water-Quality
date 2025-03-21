using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBoard : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Main Camera")?.transform;
            if (player == null) return;
        }

        transform.LookAt(player);
        transform.Rotate(0, 180, 0); // 反转朝向，确保 UI 正确显示
    }


}
