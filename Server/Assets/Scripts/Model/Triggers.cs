/*
 * Triggers from the B4 project, Author: Boris Musarais
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum triggerAction
{ 
    doNothing, triggerEvent, grantReward, makeNpcSayMessage, spawnNpcs, killNpc, moveToPoint,
}

public class Triggers
{
    public Entity parentEntity;
   
    public Triggers[] triggersToEnable = new Triggers[0];
    public Triggers[] requiredTriggersOn = new Triggers[0];
    public Triggers[] requiredTriggersOff = new Triggers[0];

    public string[] requiredKeys = new string[0];

    public bool activated = false;
    public bool locked = false;
    public int autoTrigger = 0; //if >0 call activate every X runs
    public int autoTriggerCounter = 0;

    public triggerAction onActivation = triggerAction.doNothing;

    public Triggers(Entity _parentEntity)
    {
        parentEntity = _parentEntity;
    }

    public void activate()
    {
        if (requiredTriggersOn.Length > 0)
        {
            foreach (Triggers t in requiredTriggersOn)
            {
                if (!t.activated)
                    return;
            }
        }

        if (requiredTriggersOff.Length > 0)
        {
            foreach (Triggers t in requiredTriggersOff)
            {
                if (t.activated)
                    return;
            }
        }

        if (activated)
        {
            activated = false;
        }
        else
        {
            activated = true;
        }
        sendTriggerStatus();
    }

    public void activate(Entity author)
    {

        if (requiredTriggersOn.Length > 0)
        {
            foreach (Triggers t in requiredTriggersOn)
            {
                if (!t.activated)
                    return;
            }
        }

        if (requiredTriggersOff.Length > 0)
        {
            foreach (Triggers t in requiredTriggersOff)
            {
                if (t.activated)
                    return;
            }
        }

       
        if (locked)
            return;

        if (requiredKeys.Length > 0)
        {
            foreach (string s in requiredKeys)
            {
                //if i have the key proceed, otherwise return.
            }
        }
       
        foreach (Triggers t in triggersToEnable)
        {
            t.activate();
        }
        

        if (activated)
        {
            activated = false;
        }
        else
        {
            activated = true;
        }
		
		sendTriggerStatus();
     }
	
	void sendTriggerStatus()
	{
		
	}
}