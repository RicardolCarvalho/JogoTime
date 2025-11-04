using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class MenuActions : MonoBehaviour
{
    [Header("Cutscene Config")]
    [SerializeField] private VideoClip cutscene1;
    [SerializeField] private int gameSceneIndex = 2;
    [SerializeField] private bool allowSkip = true;

    [Header("Menu UI")]
    [SerializeField] private Canvas menuCanvas;     // arraste seu Canvas
    [SerializeField] private Image blackout;        // arraste o Image "Blackout" (opcional, mas recomendado)

    private GameObject playerGO;
    private VideoPlayer vp;
    private AudioSource audioSrc;
    private bool playing;

    public void IniciarJogo()
    {
        if (cutscene1 == null)
        {
            SceneManager.LoadScene(gameSceneIndex);
            return;
        }
        PlayCutscene();
    }

    private void PlayCutscene()
    {
        // Mostra o blackout por cima para não aparecer o fundo da câmera
        if (blackout != null) blackout.enabled = true;

        playing = true;

        playerGO = new GameObject("CutscenePlayer");
        DontDestroyOnLoad(playerGO);

        vp = playerGO.AddComponent<VideoPlayer>();
        audioSrc = playerGO.AddComponent<AudioSource>();

        vp.playOnAwake = false;
        vp.isLooping = false;
        vp.clip = cutscene1;
        vp.waitForFirstFrame = true;

        vp.renderMode = VideoRenderMode.CameraNearPlane;
        vp.targetCamera = Camera.main ?? FindAnyObjectByType<Camera>();
        vp.aspectRatio = VideoAspectRatio.FitVertically;

        vp.audioOutputMode = VideoAudioOutputMode.AudioSource;
        vp.EnableAudioTrack(0, true);
        vp.SetTargetAudioSource(0, audioSrc);

        // Só esconda o menu quando o vídeo estiver realmente pronto para tocar
        vp.prepareCompleted += OnPrepared;
        vp.loopPointReached += OnCutsceneFinished;
        vp.Prepare();
    }

    private void OnPrepared(VideoPlayer _)
    {
        vp.prepareCompleted -= OnPrepared;

        // Agora que o primeiro frame está pronto, esconda o menu e dê Play
        if (menuCanvas) menuCanvas.enabled = false;

        vp.Play();

        // Libera o blackout no frame seguinte (já com o vídeo por baixo)
        StartCoroutine(HideBlackoutNextFrame());
    }

    private IEnumerator HideBlackoutNextFrame()
    {
        yield return null; // espera 1 frame para garantir que o vídeo começou a desenhar
        if (blackout != null) blackout.enabled = false;
    }

    private void Update()
    {
        if (!playing || !allowSkip) return;

        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) ||
            (Gamepad.current?.startButton.wasPressedThisFrame ?? false))
        {
            if (blackout != null) blackout.enabled = true; // cobre enquanto troca de cena
            vp.Stop();
            OnCutsceneFinished(vp);
        }
    }

    private IEnumerator LoadGameAsync()
    {
        var op = SceneManager.LoadSceneAsync(gameSceneIndex);
        op.allowSceneActivation = false;

        // mantém blackout ligado durante o carregamento da nova cena
        while (op.progress < 0.9f)
            yield return null;

        yield return null;           // 1 frame de folga
        op.allowSceneActivation = true;
    }

    private void OnCutsceneFinished(VideoPlayer _)
    {
        playing = false;
        vp.loopPointReached -= OnCutsceneFinished;

        // Garante blackout ligado até a nova cena entrar
        if (blackout != null) blackout.enabled = true;

        if (playerGO) Destroy(playerGO);
        if (Time.timeScale != 1f) Time.timeScale = 1f;

        StartCoroutine(LoadGameAsync());
    }

    public void Menu() => SceneManager.LoadScene(0);
}
