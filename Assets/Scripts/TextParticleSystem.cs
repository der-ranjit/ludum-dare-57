using UnityEngine;

public static class TextParticleSystem
{
    public static TextParticle SpawnTextParticle(Vector3 position, string text, float lifetime = 2.0f, Color? color = null, float size = 1.0f)
    {
        return TextParticle.Create(position, text, lifetime, color, size);
    }
    
    // Shorthand method for common use cases
    public static TextParticle ShowText(Vector3 position, string text)
    {
        return SpawnTextParticle(position, text, 3, null, 0.6f);
    }
    
    // Original method that takes a string
    public static TextParticle ShowDamage(Vector3 position, string damage)
    {
        return SpawnTextParticle(position, damage, 2.5f, Color.red, 0.5f);
    }

    // Overloaded method that takes an int
    public static TextParticle ShowDamage(Vector3 position, int damage)
    {
        return ShowDamage(position, damage.ToString());
    }
    
    public static TextParticle ShowEffect(Vector3 position, string effectText)
    {
        return SpawnTextParticle(position, effectText, 2.0f, Color.yellow, 0.4f);
    }

    public static TextParticle ShowTinyEffect(Vector3 position, string effectText)
    {
        return SpawnTextParticle(position, effectText, 0.7f, Color.green, 0.35f);
    }
}