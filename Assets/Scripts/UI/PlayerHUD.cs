using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerHUD : NetworkBehaviour
{
    private TextMeshProUGUI goldText;
    private TextMeshProUGUI classText;

    private PlayerWallet wallet;
    private PlayerClass playerClass;


    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer) return;

        GameObject goldObj = GameObject.Find("GoldText");
        GameObject classObj = GameObject.Find("ClassText");

        if (goldObj != null) goldText = goldObj.GetComponent<TextMeshProUGUI>();
        if (classObj != null) classText = classObj.GetComponent<TextMeshProUGUI>();

        wallet = GetComponent<PlayerWallet>();
        playerClass = GetComponent<PlayerClass>();

        if (wallet != null)
        {
            wallet.gold.OnValueChanged += UpdateGoldUI;
            UpdateGoldUI(0, wallet.gold.Value); // Pierwsze, startowe odœwie¿enie
        }

        if (playerClass != null)
        {
            playerClass.currentProfession.OnValueChanged += UpdateClassUI;
            UpdateClassUI(CharacterProfession.Builder, playerClass.currentProfession.Value); // Startowe odœwie¿enie
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsLocalPlayer) return;

        // Sprz¹tamy po sobie przy wyjœciu z gry
        if (wallet != null) wallet.gold.OnValueChanged -= UpdateGoldUI;
        if (playerClass != null) playerClass.currentProfession.OnValueChanged -= UpdateClassUI;
    }

    private void UpdateGoldUI(float oldVal, float newVal)
    {
        if (goldText != null)
        {
            goldText.text = $"Z³oto: {newVal}";
        }
    }

    private void UpdateClassUI(CharacterProfession oldVal, CharacterProfession newVal)
    {
        if (classText != null)
        {
            classText.text = $"Klasa: {newVal}";

            classText.color = (newVal == CharacterProfession.Builder) ? Color.green : Color.blue;
        }
    }
}
