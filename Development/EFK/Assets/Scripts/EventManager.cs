using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class EventManager:MonoBehaviour
{
    private static readonly object padlock = new object();  
    private static EventManager instance = null;
    static Dictionary<EventType, UnityEvent> _eventDictionary = new Dictionary<EventType, UnityEvent>();

    EventManager() 
    {
    }  
    
    public static void StartListening (EventType eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (_eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.AddListener (listener);
        } 
        else
        {
            thisEvent = new UnityEvent ();
            thisEvent.AddListener (listener);
            _eventDictionary.Add (eventName, thisEvent);
        }
    }
    
    public static void StopListening (EventType eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (_eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.RemoveListener (listener);
        }
    }

    public static void TriggerEvent (EventType eventName)
    {
        UnityEvent thisEvent = null;
        if (_eventDictionary.TryGetValue (eventName, out thisEvent))
        {
            thisEvent.Invoke ();
        }
    }
    
    
    
    public static EventManager Instance  
    {  
        get  
        {  
            lock (padlock)  
            {  
                if (instance == null)  
                {  
                    instance = new EventManager();  
                }  
                return instance;  
            }  
        }  
    }
    
    

}