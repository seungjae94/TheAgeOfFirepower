using System;
using Unity.Behavior;

namespace Mathlife.ProjectL.Gameplay.Play
{
	[BlackboardEnum]
	public enum AttackTargetingStrategy
	{
		LowestHpFirst,
		HighestDamageFirst,
		NearestFirst
	}
}