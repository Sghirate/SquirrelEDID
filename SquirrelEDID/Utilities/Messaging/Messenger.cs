using System;
using System.Collections.Generic;

namespace SquirrelEDID.Utilities.Messaging
{
    // Customized version of Rod Hyde's Unity Messenger ( http://wiki.unity3d.com/index.php?title=CSharpMessenger )
    static public class Messenger
    {
        private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        static public void AddListener(string eventType, Callback handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Create an entry for this event type if it doesn't already exist.
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, null);
                }
                // Add the handler to the event.
                eventTable[eventType] = (Callback)eventTable[eventType] + handler;
            }
        }

        static public void RemoveListener(string eventType, Callback handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Only take action if this event type exists.
                if (eventTable.ContainsKey(eventType))
                {
                    // Remove the event handler from this event.
                    eventTable[eventType] = (Callback)eventTable[eventType] - handler;

                    // If there's nothing left then remove the event type from the event table.
                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }

        static public void Invoke(string eventType)
        {
            Delegate d;
            // Invoke the delegate only if the event type is in the dictionary.
            if (eventTable.TryGetValue(eventType, out d))
            {
                // Take a local copy to prevent a race condition if another thread
                // were to unsubscribe from this event.
                Callback callback = (Callback)d;

                // Invoke the delegate if it's not null.
                if (callback != null)
                {
                    callback();
                }
            }
        }
    }


    /**
     * A messenger for events that have one parameter of type T.
     */
    static public class Messenger<T>
    {
        private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        private static object listenerLock = new object();
        private static Callback<T> listeners;

        static public void AddListener(string eventType, Callback<T> handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Create an entry for this event type if it doesn't already exist.
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, null);
                }
                // Add the handler to the event.
                eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
            }
        }

        static public void AddListener(Callback<T> handler)
        {
            lock (listenerLock)
            {
                listeners += handler;
            }
        }

        static public void RemoveListener(string eventType, Callback<T> handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Only take action if this event type exists.
                if (eventTable.ContainsKey(eventType))
                {
                    // Remove the event handler from this event.
                    eventTable[eventType] = (Callback<T>)eventTable[eventType] - handler;

                    // If there's nothing left then remove the event type from the event table.
                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }

        static public void RemoveListener(Callback<T> handler)
        {
            lock (listenerLock)
            {
                listeners -= handler;
            }
        }

        static public void Invoke(string eventType, T arg1)
        {
            Delegate d;
            // Invoke the delegate only if the event type is in the dictionary.
            if (eventTable.TryGetValue(eventType, out d))
            {
                // Take a local copy to prevent a race condition if another thread
                // were to unsubscribe from this event.
                Callback<T> callback = (Callback<T>)d;

                // Invoke the delegate if it's not null.
                if (callback != null)
                {
                    callback(arg1);
                }
            }
        }

        static public void Invoke(T arg1)
        {
            if (listeners != null)
                listeners(arg1);
        }
    }


    /**
     * A messenger for events that have two parameters of types T and U.
     */
    static public class Messenger<T, U>
    {
        private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        private static object listenerLock = new object();
        private static Callback<T, U> listeners;

        static public void AddListener(string eventType, Callback<T, U> handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Create an entry for this event type if it doesn't already exist.
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, null);
                }
                // Add the handler to the event.
                eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
            }
        }

        static public void AddListener(Callback<T,U> handler)
        {
            lock (listenerLock)
            {
                listeners += handler;
            }
        }

        static public void RemoveListener(string eventType, Callback<T, U> handler)
        {
            // Obtain a lock on the event table to keep this thread-safe.
            lock (eventTable)
            {
                // Only take action if this event type exists.
                if (eventTable.ContainsKey(eventType))
                {
                    // Remove the event handler from this event.
                    eventTable[eventType] = (Callback<T, U>)eventTable[eventType] - handler;

                    // If there's nothing left then remove the event type from the event table.
                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }

        static public void RemoveListener(Callback<T,U> handler)
        {
            lock (listenerLock)
            {
                listeners -= handler;
            }
        }

        static public void Invoke(string eventType, T arg1, U arg2)
        {
            Delegate d;
            // Invoke the delegate only if the event type is in the dictionary.
            if (eventTable.TryGetValue(eventType, out d))
            {
                // Take a local copy to prevent a race condition if another thread
                // were to unsubscribe from this event.
                Callback<T, U> callback = (Callback<T, U>)d;

                // Invoke the delegate if it's not null.
                if (callback != null)
                {
                    callback(arg1, arg2);
                }
            }
        }

        static public void Invoke(T arg1, U arg2)
        {
            if (listeners != null)
                listeners(arg1, arg2);
        }
    }
}
