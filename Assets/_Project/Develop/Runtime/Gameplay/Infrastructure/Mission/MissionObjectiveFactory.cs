using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Gameplay.Features.Main.Characters.EnemyCharacters.Core;
using _Project.Develop.Runtime.Gameplay.Features.Main.Interactables;
using _Project.Develop.Runtime.Meta.Features.Missions;

namespace _Project.Develop.Runtime.Gameplay.Infrastructure.Mission
{
    public class MissionObjectiveFactory
    {
        private readonly EnemyAIService _enemyAIService;
        private readonly StealItemService _stealItemService;

        public MissionObjectiveFactory(EnemyAIService enemyAIService, StealItemService stealItemService)
        {
            _enemyAIService = enemyAIService;
            _stealItemService = stealItemService;
        }

        public IMissionObjective Create(MissionObjectiveDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            switch (definition.Type)
            {
                case MissionObjectiveType.ClearAllEnemies:
                    return new ClearAllEnemiesObjective(_enemyAIService);

                case MissionObjectiveType.StealItem:
                    return new StealItemObjective(definition.TargetId, _stealItemService);

                case MissionObjectiveType.ReachTrigger:
                    throw new NotSupportedException(
                        $"Objective type '{definition.Type}' is not implemented yet (id: '{definition.Id}').");

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(definition),
                        definition.Type,
                        $"Unknown objective type '{definition.Type}'.");
            }
        }

        public SequenceObjective CreatePrimarySequence(IReadOnlyList<MissionObjectiveDefinition> steps)
        {
            if (steps == null || steps.Count == 0)
                throw new InvalidOperationException("Mission has no primary steps configured.");

            var children = new List<IMissionObjective>(steps.Count);
            for (int i = 0; i < steps.Count; i++)
                children.Add(Create(steps[i]));

            return new SequenceObjective(children);
        }
    }
}
