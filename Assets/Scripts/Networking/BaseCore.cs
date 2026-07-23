using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class BaseCore : NetworkBehaviour
{
    public NetworkVariable<float> currentHealthPoints = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Slider healthSlider;
    private GameObject gameOverPanel;
    public override void OnNetworkSpawn()
    {
        currentHealthPoints.OnValueChanged += OnHpChanged;
        
        GameObject sliderObj = GameObject.Find("BaseHealthBar");
        if (sliderObj != null) healthSlider = sliderObj.GetComponent<Slider>();

        gameOverPanel = GameObject.Find("UI")?.transform.Find("GameOverPanel")?.gameObject;

        if (healthSlider != null) healthSlider.value = currentHealthPoints.Value;
    }
    private void OnHpChanged(float oldVal, float newVal)
    {
        Debug.Log($"[Sieæ] HP Bazy zmieni³o siê z {oldVal} na: {newVal}");

        if (healthSlider != null)
        {
            healthSlider.value = newVal;
        }

        // ToDo Jakiœ kolor na czerwono screena mo¿na tu upchn¹æ

        if (newVal <= 0)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.LogError("GAME OVER! Baza zosta³a zniszczona!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (IsServer)
        {
            Time.timeScale = 0f;
        }
    }
    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        currentHealthPoints.Value -= damage;

        if (currentHealthPoints.Value <= 0f)
            Debug.Log("Baza Zniszczona!");

    }
}
