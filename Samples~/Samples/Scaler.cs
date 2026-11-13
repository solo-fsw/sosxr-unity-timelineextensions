using UnityEngine;

public class Scaler : MonoBehaviour
{
    public void ScaleTransform(float scale)
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
    }
}
