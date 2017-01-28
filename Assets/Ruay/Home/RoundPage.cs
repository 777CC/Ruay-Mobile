using UnityEngine;
using System.Collections;

public class RoundPage : MonoBehaviour {
    [SerializeField]
    protected CanvasGroup canvasGroup;
    protected Round round;
    public virtual void SetRound(Round r)
    {
        Debug.Log("SetRound");
        round = r;
        LeanTween.alphaCanvas(canvasGroup, 1, 0.3f);
    }
    public virtual void Back()
    {
        LeanTween.alphaCanvas(canvasGroup, 0, 0.3f).setOnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    
}
