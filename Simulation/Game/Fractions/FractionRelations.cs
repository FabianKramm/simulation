using Simulation.Game.Enums;
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
            }}
        };

        public static int GetAggroFromFractionRelation(FractionRelationType fractionRelation)
        {
            if (fractionRelation == FractionRelationType.ALLIED)
                return 125;

            if (fractionRelation == FractionRelationType.FRIENDLY)
                return 75;

            if (fractionRelation == FractionRelationType.NEUTRAL)
                return 25;

            return -25;
        }

        public static FractionRelationType GetFractionRelationFromAggro(int aggro)
        {
            if (aggro >= 100)
                return FractionRelationType.ALLIED;

            if (aggro >= 50)
                return FractionRelationType.FRIENDLY;

            if (aggro >= 0)
                return FractionRelationType.NEUTRAL;

            return FractionRelationType.HOSTILE;
        }

        public static FractionRelationType GetFractionRelation(LivingEntity origin, LivingEntity target)
        {
            if (origin.Fraction == target.Fraction)
                return FractionRelationType.ALLIED;

            if (fractionRelations.ContainsKey(origin.Fraction) == false)
                return FractionRelationType.NEUTRAL;

            if (fractionRelations[origin.Fraction].ContainsKey(target.Fraction) == false)
                return FractionRelationType.NEUTRAL;

            return fractionRelations[origin.Fraction][target.Fraction];
        }
    }
}
