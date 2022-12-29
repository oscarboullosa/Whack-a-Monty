using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yoshi : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private Sprite greenYoshi;
    [SerializeField] private Sprite blueYoshi;
    [SerializeField] private Sprite heel;
    [SerializeField] private Sprite Smoke;

    [Header("GameManager")]
    [SerializeField] private GameManager gameManager;
    //Esconderlo
    private Vector2 startPosition = new Vector2(0f, -2.56f); //-2.56f Por un reajuste de los píxeles
    private Vector2 endPosition = Vector2.zero;
    //Durante cuanto tiempo enseñamos a Yoshi
    private float showDuration = 0.5f;
    private float duration = 1f;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private Vector2 boxOffset;
    private Vector2 boxSize;
    private Vector2 boxOffsetHidden;
    private Vector2 boxSizeHidden;

    private bool hittable = true;
    public enum YoshiType {Standard,blueYoshi,heel};
    private YoshiType yoshiType;
    private float blueYoshiRate = 0.25f;
    private float heelRate = 0f;
    private int lives;
    private int yoshiIndex = 0;
    private IEnumerator ShowHide(Vector2 start, Vector2 end)
    {
        //Empezamos al inicio
        transform.localPosition = start;

        //Aparece Yoshi
        float elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = end;
        boxCollider2D.offset = boxOffset;
        boxCollider2D.size = boxSize;
        yield return new WaitForSeconds(duration);

        //Desaparece Yoshi
        elapsed = 0f;
        while (elapsed < showDuration)
        {
            transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
            boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
            boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = start;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
        if (hittable)
        {
            hittable = false;
            gameManager.Missed(yoshiIndex, yoshiType != YoshiType.heel);
        }
    }
    public void Hide()
    {
        transform.localPosition = startPosition;
        boxCollider2D.offset = boxOffsetHidden;
        boxCollider2D.size = boxSizeHidden;
    }
    private IEnumerator QuickHide()
    {
        yield return new WaitForSeconds(0.25f);
        if (!hittable)
        {
            Hide();
        }

    }
    private void OnMouseDown()
    {
        if (hittable)
        {
            switch (yoshiType)
            {
                case YoshiType.Standard:
                    spriteRenderer.sprite = Smoke;
                    gameManager.AddScore(yoshiIndex);
                    StopAllCoroutines();
                    StartCoroutine(QuickHide());
                    hittable = false;
                    break;
                case YoshiType.blueYoshi:
                    if (lives == 2)
                    {
                        spriteRenderer.sprite = greenYoshi;
                        lives--;
                    }
                    else
                    {
                        spriteRenderer.sprite = Smoke;
                        gameManager.AddScore(yoshiIndex);
                        StopAllCoroutines();
                        StartCoroutine(QuickHide());
                        hittable = false;
                    }
                    break;
                case YoshiType.heel:
                    gameManager.GameOver(1);
                    break;
                default:
                    break;
            }
        }
    }
    private void CreateNext()
    {
        float random = Random.Range(0f, 1f);
        if (random < heelRate)
        {
            yoshiType = YoshiType.heel;
        }
        else
        {
            random = Random.Range(0f, 1f);
            if (random < blueYoshiRate)
            {
                yoshiType = YoshiType.blueYoshi;
                spriteRenderer.sprite = blueYoshi;
                lives = 2;
            }
            else
            {
                yoshiType = YoshiType.Standard;
                spriteRenderer.sprite = greenYoshi;
                lives = 1;
            }
        }
        hittable = true;
    }
    private void SetLevel(int level)
    {
        heelRate = Mathf.Min(level * 0.025f, 0.25f);
        blueYoshiRate = Mathf.Min(level * 0.025f, 1f);
        float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
        float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
        duration = Random.Range(durationMin, durationMax);
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxOffset = boxCollider2D.offset;
        boxSize = boxCollider2D.size;
        boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
        boxSizeHidden = new Vector2(boxSize.x, 0f);
    }
    public void Activate(int level)
    {
        SetLevel(level);
        CreateNext();
        StartCoroutine(ShowHide(startPosition, endPosition));
    }
    public void SetIndex(int index)
    {
        yoshiIndex = index;
    }
    // Start is called before the first frame update
    
    public void StopGame()
    {
        hittable = false;
        StopAllCoroutines();
    }

}
