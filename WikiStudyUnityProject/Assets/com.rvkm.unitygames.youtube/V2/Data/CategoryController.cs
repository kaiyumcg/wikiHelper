using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.rvkm.unitygames.YouTubeV2
{
    public class CategoryController : MonoBehaviour
    {
        [SerializeField] VideoInfo videoInfo;
        [SerializeField] CategorizedDataDesc catData;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CategoryIt(videoInfo, ref catData.allCategories);
            }
        }

        void CategoryIt(VideoInfo vInfo, ref List<Category> catList)
        {
            List<UrlData> processedList = new List<UrlData>();
            if (catList != null && catList.Count > 0)
            {
                Category uncat = null;
                uncat = catList.Find((data) => { return data.categoryName == "uncategorized"; });
                if (uncat != null) { catList.Remove(uncat); }

                if (catList != null && catList.Count > 0)
                {
                    foreach (var c in catList)
                    {
                        if (c == null) { continue; }
                        c.videos = new List<UrlData>();
                        if (vInfo != null && vInfo.allVideoInfo != null && vInfo.allVideoInfo.Length > 0)
                        {
                            for (int i = 0; i < vInfo.allVideoInfo.Length; i++)
                            {
                                var vdoInfo = vInfo.allVideoInfo[i];
                                if (vdoInfo == null) { continue; }
                                var exist = c.videos.Exists((pred) =>
                                { return string.Equals(pred.url, vdoInfo.url, System.StringComparison.OrdinalIgnoreCase); });

                                if (exist == false)
                                {
                                    if (vdoInfo.linkName.ContainsAnyOf(c.whitelistedWords)
                                        && !vdoInfo.linkName.ContainsAnyOf(c.blacklistedWords))
                                    {
                                        c.videos.Add(new UrlData(vdoInfo));
                                        processedList.Add(vInfo.allVideoInfo[i]);
                                    }
                                }
                            }
                        }

                    }
                }
            }

            List<UrlData> uncatList = new List<UrlData>();
            if (vInfo != null && vInfo.allVideoInfo != null && vInfo.allVideoInfo.Length > 0)
            {
                foreach (var v in vInfo.allVideoInfo)
                {
                    if (v == null) { continue; }
                    if (!processedList.Contains(v))
                    {
                        uncatList.Add(v);
                    }
                }
            }

            var uncatV2 = new Category() { blacklistedWords = null, categoryName = "uncategorized", whitelistedWords = null, videos = uncatList};
            catList.Add(uncatV2);
        }
    }
}