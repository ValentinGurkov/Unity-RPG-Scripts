using System.Collections.Generic;

namespace RPG.Stats
{
    /// <summary>
    /// Classes that provide modifiers to base stats implement this interface
    /// </summary>
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiers(Stat stat);
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}