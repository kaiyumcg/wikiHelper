using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Balsal : MonoBehaviour
{
    [SerializeField][Multiline] string testStr, outputStr;

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            outputStr = com.rvkm.unitygames.YouTubeSearch.Utility.GetWWWResponse(testStr);
        }
    }
}
