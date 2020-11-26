using com.rvkm.unitygames.YouTube;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestT : MonoBehaviour
{
    public ChannelVideoDescription desc;
    
    // Start is called before the first frame update
    void Start()
    {
        desc = Resources.Load<ChannelVideoDescription>("data");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
