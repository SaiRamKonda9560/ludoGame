using TMPro;
using UnityEngine;
using UnityEngine.UI;
[ExecuteAlways]
public class ludoBoardPawnGen : MonoBehaviour
{
    public Image Image
    {
        get
        {
            return GetComponent<Image>();
        }
    }
    public int hight;
    public int width;
    public Vector3 offset;
    public TMP_Text tMP_Text;
    public GameObject spanGam;

    private void Update()
    {
        gen();
    }
    public void gen()
    {
        destroyAllChilds(transform);
        if(tMP_Text != null && spanGam)
        {
            var rectTransform = Image.rectTransform;
            var sizeDelta = rectTransform.sizeDelta;
            Vector3[] corners=new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var bottamLeft = corners[0];
            var bottamRight = corners[3];
            var topLeft = corners[1];
            var topRight = corners[2];
            var worldHight = topLeft - bottamLeft;
            var worldWidth = bottamRight - bottamLeft;



            for (int i = 0; i < corners.Length; i++)
            {
                //spanText(corners[i],i.ToString());
            }
            var c = 0;
            for (int y = 0; y < hight; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var normalHight = ((float)y / (float)hight);
                    var normalWidth = ((float)x / (float)width);

                    var h = (normalHight * worldHight);
                    var w = (normalWidth * worldWidth);

                    var pos = h + w +bottamLeft + offset;
                    //spanText(pos, c.ToString());
                    span(pos);

                    c++;
                }
            }
        }

    }
    public void spanText(Vector3 pos,string text)
    {
        var spanedText = Instantiate(tMP_Text.gameObject,transform).GetComponent<TMP_Text>();
        spanedText.rectTransform.position = pos;
        spanedText.text = text;
    }
    public void span(Vector3 pos)
    {
        var spanedText = Instantiate(spanGam, transform);
        spanedText.transform.position = pos;
    }

    private void destroyAllChilds(Transform transform)
    {
        if (transform)
            foreach (var child in transform.GetComponentsInChildren<Transform>())
            {
                if (transform != child.transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
    }

}
