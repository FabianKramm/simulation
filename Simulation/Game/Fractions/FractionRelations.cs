using Simulation.Game.Fractions;
using Simulation.Game.Objects.Entities;
using System.Collections.Generic;

namespace Simulation.Game.Fractions
{
    public class FractionRelations
    {
        private static Dictionary<FractionType, Dictionary<FractionType, FractionRelationType>> fractionRelations = new Dictionary<FractionType, Dictionary<FractionType, FractionRelationType>>
        {
            {FractionType.NPC, new Dictionary<FractionType, FractionRelationType>()
            {
                {FractionType.PLAYER, FractionRelationType.FRIENDLY},
                {FractionType.MONSTER, FractionRelationType.HOSTILE},
                {FractionType.BANDIT, FractionRelationType.HOSTILE}
            }},
            {FractionType.MONSTER, new Dictionary<FractionType, FractionRelationType>()
            {
                {FractionType.PLAYER, FractionRelationType.HOSTILE},
                {FractionType.BANDIT, FractionRelationType.HOSTILE},
                {FractionType.NPC, FractionRelationType.HOSTILE}
            }},
            {FractionType.BANDIT, new Dictionary<FractionType, FractionRelationType>()
            {
                {FractionType.PLAYER, FractionRelationType.HOSTILE},
                {FractionType.MONSTER, FractionRelationType.HOSTILE},
                {FractionType.NPC, FractionRelationType.HOSTILE}
            }},
            {FractionType.PLAYER, new Dictionary<FractionType, FractionRelationType>()
            {
                {FractionType.NPC, FractionRelationType.FRIENDLY},
                {FractionType.MONSTER, FractionRelationType.HOSTILE},
                {FractionType.BANDIT, FractionRelationType.HOSTILE}
            }}
        };

        /*public static FractionRelationType GetFractionRelationFromAggro(int aggro)
        {
            if (aggro >= 100)
                return FractionRelationType.ALLIED;

            if (aggro >= 50)
                return FractionRelationType.FRIENDLY;

            if (aggro >= 0)
                return FractionRelationType.NEUTRAL;

            return FractionRelationType.HOSTILE;
        } */

        public static int GetAggro(LivingEntity origin, LivingEntity target)
        {
            if (origin.Fraction == target.Fraction)
                return (int)FractionRelationType.ALLIED;

            if (fractionRelations.ContainsKey(origin.Fraction) == false)
                return (int)FractionRelationType.NEUTRAL;

            if (fractionRelations[origin.Fraction].ContainsKey(target.Fraction) == false)
                return (int)FractionRelationType.NEUTRAL;

            return (int)fractionRelations[origin.Fraction][target.Fraction];
        }
    }
}
