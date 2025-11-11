using UnityEngine;
using TMPro;

public class MolhoController : MonoBehaviour
{
    public static MolhoController instance;

    [Header("Pontuação e Erros")]
    public int points = 0;
    public int errors = 0;
    public int maxPoints = 5;
    public int maxErrors = 3;

    [Header("Referências")]
    public TomatoSpawner spawner;
    public GameObject screenFlash; // arraste o painel vermelho do Canvas

    private bool gameEnded = false;
    private float flashTimer;
    private bool flashing;

    void Awake() => instance = this;

    void Update()
    {
        if (flashing)
        {
            flashTimer += Time.deltaTime;
            float alpha = Mathf.PingPong(flashTimer * 2f, 1f);
            var img = screenFlash.GetComponent<UnityEngine.UI.Image>();
            var c = img.color;
            c.a = alpha * 0.4f;
            img.color = c;
        }
    }

    public void AddPoint()
    {
        if (gameEnded) return;
        points++;

        if (points >= maxPoints)
        {
            Victory();
        }
    }

    public void AddError()
    {
        if (gameEnded) return;
        errors++;

        if (errors >= maxErrors)
        {
            Explosion();
        }
    }

    private void Victory()
    {
        gameEnded = true;
        spawner.canSpawn = false;
        Debug.Log("✅ Vitória! Você salvou o jantar!");
    }

    private void Explosion()
    {
        gameEnded = true;
        spawner.canSpawn = false;
        flashing = true;
        Debug.Log("💥 Explosão de Molho!");
    }
}
