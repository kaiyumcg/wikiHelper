using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.extensions
{
    public class UrlFormattingTest : MonoBehaviour
    {
        [Multiline]
        [SerializeField] string url;
        [SerializeField] bool isStarts;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isStarts)
                {
                    url = url.TrimUrlSlashesFromStart();
                }
                else
                {
                    url = url.TrimUrlSlashesFromEnd();
                }
                GameDebug.LogMagenta(" formatted url: " + url);
            }
        }
    }
}