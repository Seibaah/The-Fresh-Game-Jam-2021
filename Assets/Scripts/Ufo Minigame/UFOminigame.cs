using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * @desc gameplay class for the ufo targetting minigame
 * @author Denis
 * Code implementation from Greg Dev Stuff at https://www.youtube.com/watch?v=ZBuGdGQiPj0
 */
public class UFOminigame : MonoBehaviour
{
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;
    [SerializeField] Transform ufo;

    float ufoPos;
    float ufoDest;
    float ufoTimer;
    [SerializeField] float timerMult = 3f;

    float ufoSpeed;
    [SerializeField] float smoothMotion = 1f;

    [SerializeField] Transform target;
    float targetPos;
    [SerializeField] float targetSize = 0.1f;
    [SerializeField] float targetPower = 5f;
    float targetProgress;
    float targetPullVelocity;
    [SerializeField] float targetPullPower = 0.01f;
    [SerializeField] float targetGravityPower = 0.005f;
    [SerializeField] float targetProgressDegradationPower = 1f;

    [SerializeField] SpriteRenderer targetSpriteRenderer;

    [SerializeField] Transform progressBarContainer;

    [SerializeField] float failTimer = 5f;

    private void Start()
    {
        Resize();
    }

    void Resize()
    {
        Bounds b = targetSpriteRenderer.bounds;
        float ySize = b.size.y;
        Vector3 ls = target.localScale;
        float distance = Vector3.Distance(topPivot.position, bottomPivot.position);
        ls.y = (distance / ySize * targetSize);
        target.localScale = ls;
    }

    void Update()
    {
        Ufo();
        Target();
        ProgressCheck();
    }

    void ProgressCheck()
    {
        Vector3 ls = progressBarContainer.localScale;
        ls.y = targetProgress;
        progressBarContainer.localScale = ls;

        float min = targetPos - targetSize / 2;
        float max = targetPos + targetSize / 2;

        if (min <= ufoPos && max >= ufoPos)
        {
            targetProgress += targetPower * Time.deltaTime;
        }
        else
        {
            targetProgress -= targetProgressDegradationPower * Time.deltaTime;

            failTimer -= Time.deltaTime;
            if (failTimer < 0f)
            {
                Debug.Log("You lost");
            }
        }

        if (targetProgress >= 1f)
        {
            Debug.Log("You won");
        }

        targetProgress = Mathf.Clamp(targetProgress, 0f, 1f);
    }

    void Target()
    {
        if (Input.GetMouseButton(0))
        {
            targetPullVelocity += targetPullPower * Time.deltaTime;
        }
        targetPullVelocity -= targetGravityPower * Time.deltaTime;

        targetPos += targetPullVelocity;

        if (targetPos - targetSize / 2 <= 0.0f && targetPullVelocity < 0f)
        {
            targetPullVelocity = 0f;
        }
        if (targetPos + targetSize / 2 >= 1f && targetPullVelocity > 0f)
        {
            targetPullVelocity = 0f;
        }

        //Debug.Log("Pos" + targetPos);
        //Debug.Log("Vel" + targetPullVelocity);

        targetPos = Mathf.Clamp(targetPos, targetSize/2, 1 - targetSize/2);
        target.position = Vector3.Lerp(bottomPivot.position, topPivot.position, targetPos);
    }

    void Ufo()
    {
        ufoTimer -= Time.deltaTime;
        if (ufoTimer < 0f)
        {
            ufoTimer = UnityEngine.Random.value * timerMult;

            ufoDest = UnityEngine.Random.value;
        }

        ufoPos = Mathf.SmoothDamp(ufoPos, ufoDest, ref ufoSpeed, smoothMotion);
        ufo.position = Vector3.Lerp(bottomPivot.position, topPivot.position, ufoPos);
    }
}
