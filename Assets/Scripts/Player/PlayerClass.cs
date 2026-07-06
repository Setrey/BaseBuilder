using UnityEngine;
using Unity.Netcode;

public enum CharacterProfession
{
    Builder,
    Combatant
}
public class PlayerClass : NetworkBehaviour
{
    public NetworkVariable<CharacterProfession> currentProfession 
        = new NetworkVariable<CharacterProfession>(CharacterProfession.Builder, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public override void OnNetworkSpawn()
    {
        currentProfession.OnValueChanged += OnProffesionChanged;

        if (IsServer)
        {
            Debug.Log("JEst?");
            currentProfession.Value = (Random.value > 0.5f) ? CharacterProfession.Builder : CharacterProfession.Combatant;
        }

        ApplyProffesionVisuals(currentProfession.Value);
    }
    public override void OnNetworkDespawn()
    {
        currentProfession.OnValueChanged -= OnProffesionChanged;
    }
    void OnProffesionChanged(CharacterProfession oldProf, CharacterProfession newProf)
    {
        ApplyProffesionVisuals(newProf);
        ApplyProffesionDifferences(newProf);
    }

    private void ApplyProffesionVisuals(CharacterProfession profesion)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
            renderer.material.color = (profesion == CharacterProfession.Builder) ? Color.green : Color.red;

        Debug.Log($"[Klasa] Obiektowi zosta³a przydzielona profesja: {profesion}");
    }
    private void ApplyProffesionDifferences(CharacterProfession newProfesion)
    {
        Player player = GetComponent<Player>();

        setDefaultProffesionStatistics(player);
        
        if (newProfesion == CharacterProfession.Combatant)
            player.baseFireRate = 0.3f;
       
    }
    private void setDefaultProffesionStatistics(Player player)
    {
        player.baseFireRate = 1f;
        // player.playerWallet.wallCost=25f;
    }
}
