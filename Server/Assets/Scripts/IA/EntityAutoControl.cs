using UnityEngine;
using System.Collections;

/// <summary>
/// IA behaviour types, they define the way the system has to control an Entity when it is not controlled by a player.
/// </summary>
public enum IABehaviourType
{
	none, //the entity will do nothing
	defensive, //the entity will focus anyone attacking it 
	agressive, //the entity will attack anyone in attack range
}

/// <summary>
/// Entity base auto control system, handles the focus system, allowing the Entity to walk at a focused ennemy and attack it.
/// Uses the ViewTile entitiy awareness system to check for ennemies
/// </summary>
public class EntityAutoControl
{
	/// <summary>
	/// The _entity.
	/// </summary>
	Entity _entity;
	
	/// <summary>
	/// The focused target.
	/// </summary>
	Entity focusedTarget = null;
	
	IABehaviourType _behaviour;

	public IABehaviourType Behaviour 
	{
		get 
		{
			return this._behaviour;
		}
		set 
		{
			_behaviour = value;
		}
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="EntityAutoControl"/> class.
	/// </summary>
	/// <param name='entity'>
	/// Entity.
	/// </param>
	public EntityAutoControl(Entity entity, IABehaviourType behaviour)
	{
		_entity = entity;
		_behaviour = behaviour;
	}
	
	/// <summary>
	/// Processes the IA decisions for basic behaviours such as auto focus and auto attack.
	/// </summary>
	public void processThinking()
	{
		if(_entity.Infos.Hp<=0)
		{
			if(focusedTarget!=null)
				cancelFocus();
			
			return;
		}
		
		if(focusedTarget==null)//if i am not focusing anyone
		{
			//if I am not controlled by a player, I will behave using one of the decision model
			if(_behaviour==IABehaviourType.agressive)
				checkForPotentialFocusTargets();
		}
		else
		{
			if(checkIfTargetInNotAttackRange())
			{
				if(_entity.getFrameSpeed()>0)
					walkToFocusedUnitUntilItIsInRange();
			}
			else
			{
				if(_entity.status==EntityStatus.walking)
				{
					_entity.resetPaths();
				}
				
				if(focusedTarget.Infos.Hp>0)
				{
					_entity.tryToLaunchBasicAttack(focusedTarget);
				}
				else
				{
					cancelFocus();
				}
			}
		}
	}
	
	/// <summary>
	/// Focuses the entity if I am not doing anything else.
	/// </summary>
	/// <param name='potentialTarget'>
	/// Potential target.
	/// </param>
	public void focusEntityIfIAmNotDoingAnythingElse(Entity potentialTarget)
	{
		if(focusedTarget==null)
			focusedTarget = potentialTarget;
	}
	
	public void forceEntityToFocusSpecifiedTarget(Entity target)
	{
		if(target.Infos.Hp>0)
		{
			if(focusedTarget!=target)
			{
				cancelFocus();
				focusedTarget = target;
			}
		}
		else
		{
			_entity.findPath(new Vector2(target.position.x, target.position.z));
		}
	}
	
	/// <summary>
	/// Checks for potential focus targets.
	/// </summary>
	void checkForPotentialFocusTargets()
	{
		if(focusedTarget == null)
		{
			Hashtable visibleEnemiesCloned = new Hashtable(_entity.visibleEnemies);
            foreach (string s in visibleEnemiesCloned.Keys)
            {
                try
                {
					Entity ennemy = _entity.myGame.getEntity(int.Parse(s));
                    
					if (ennemy.getDistance(_entity) < _entity.getViewRange())
                    {
                        try
                        {
							focusedTarget = ennemy;
                            break;
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {
                    _entity.visibleEnemies.Remove(s);
                }
            }
		}
	}
	
	/// <summary>
	/// The last focused unit position.
	/// </summary>
	B4.Vector3 lastFocusedUnitPosition=null;
	
	/// <summary>
	/// Walks to focused unit until it is in range, if it is out of range or dead, stop focusing it.
	/// </summary>
	void walkToFocusedUnitUntilItIsInRange()
	{
		try
		{
			if(checkIfFocusedTargetIsNotTooFar() && focusedTarget.Infos.Hp>0)
			{
				if(checkIfTargetInNotAttackRange())
				{
					bool hasToRecalculatePath = false;
					
					if(lastFocusedUnitPosition==null)
					{
						hasToRecalculatePath = true;
						lastFocusedUnitPosition = focusedTarget.position.getNewInstance();
					}
					else
					{
						if(lastFocusedUnitPosition.Substract(focusedTarget.position).SqrMagnitude()>_entity.myGame.baseStep)
						{
							hasToRecalculatePath = true;
							lastFocusedUnitPosition = focusedTarget.position.getNewInstance();
						}
					}
					
					if(hasToRecalculatePath)
					{
						_entity.findPath(new Vector2(focusedTarget.position.x, focusedTarget.position.z));
					}
				}
				else
				{
					//if the target is in range and i am walking, stop.
					if(_entity.status==EntityStatus.walking)
					{
						_entity.resetPaths();
					}
				}
			}
			else
			{
				cancelFocus();
			}
		}
		catch //if there is a null reference, the target has been destroyed, we stop focusing it
		{
			cancelFocus();
		}
	}
	
	/// <summary>
	/// Cancels the focus.
	/// </summary>
	public void cancelFocus()
	{
		focusedTarget = null;
		lastFocusedUnitPosition = null;
		_entity.resetPaths();
	}
	
	/// <summary>
	/// Checks if focusedTarget is not too far. 
	/// </summary>
	public bool checkIfFocusedTargetIsNotTooFar()
	{
		if(_entity.getDistance(focusedTarget)<_entity.getViewRange())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	/// <summary>
	/// Checks if target in attack range.
	/// </summary>
	/// <returns>
	/// The bool defining if target in attack range.
	/// </returns>
	public bool checkIfTargetInNotAttackRange()
	{
		return _entity.position.Substract(focusedTarget.position).SqrMagnitude()>_entity.getAttackRange();
	}
	
	/// <summary>
	/// Gets the level difference between two entities.
	/// </summary>
	/// <returns>
	/// The level difference between two entities.
	/// </returns>
	/// <param name='requester'>
	/// Requester.
	/// </param>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='factor'>
	/// Factor.
	/// </param>
	/// <param name='minDistance'>
	/// Minimum distance.
	/// </param>
	/// <param name='maxDistance'>
	/// Max distance.
	/// </param>
	public float getLevelDifferenceBetweenTwoEntities(Entity requester, Entity target, float factor, float minDistance, float maxDistance)
	{
		int maxLevel = target.Infos.Level;
        if (target.Infos.Level > maxLevel)
            maxLevel = target.Infos.Level;

        float calculatedMinDistance = (requester.Infos.Level - maxLevel) * factor;

        if (calculatedMinDistance > maxDistance)
            calculatedMinDistance = maxDistance;

        if (calculatedMinDistance < minDistance)
            calculatedMinDistance = minDistance;
		
		return calculatedMinDistance;
	}
	
	public bool hasFocus()
	{
		return focusedTarget!=null;
	}
}
