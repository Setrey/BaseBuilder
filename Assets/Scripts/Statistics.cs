using UnityEngine;

public class Statistics
{
    // --- Statystyki Podstawowe (Attributes) ---
    // Czêsto okreœlane jako "Core Six"

    public int strength { get; set; }      // Si³a fizyczna i udŸwig
    public int dexterity { get; set; }     // Zrêcznoœæ, refleks i celnoœæ
    public int constitution { get; set; }  // Wytrzyma³oœæ i punkty ¿ycia
    public int intelligence { get; set; }  // Logika, pamiêæ i moc czarów
    public int wisdom { get; set; }        // Intuicja, percepcja i silna wola
    public int charisma { get; set; }      // Si³a osobowoœci i perswazja

    // --- Statystyki Pochodne (Derived Stats) ---
    // Wyliczane zazwyczaj na podstawie statystyk podstawowych

    public int maxHealth => constitution * 10 + 50;
    public int maxMana => intelligence * 8 + 20;
    public float armorClass => 10 + CalculateModifier(dexterity);
    public float criticalHitChance => dexterity * 0.5f;
    public float speed => 10f + CalculateModifier(dexterity);

    // --- Zasoby (Resources) ---

    public int currentHealth { get; set; }
    public int currentMana { get; set; }
    public int experiencePoints { get; set; }
    public int level { get; set; }

    // --- Konstruktor ---
    public Statistics(int str, int dex, int con, int intel, int wis, int cha)
    {
        strength = str;
        dexterity = dex;
        constitution = con;
        intelligence = intel;
        wisdom = wis;
        charisma = cha;

        // Inicjalizacja zasobów do pe³na
        level = 1;
        currentHealth = maxHealth;
        currentMana = maxMana;
    }
    public Statistics()
    {
        strength = 10;
        dexterity = 10;
        constitution = 10;
        intelligence = 10;
        wisdom = 10;
        charisma = 10;

        level = 1;
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    /// <summary>
    /// Pomocnicza metoda do obliczania modyfikatora (klasyczny system d20).
    /// Przyk³ad: 10 (+0), 12 (+1), 14 (+2) itd.
    /// </summary>
    public float CalculateModifier(float attributeValue)
    {
        return (float)Mathf.Floor((attributeValue - 10f) / 2.0f);
    }
}
