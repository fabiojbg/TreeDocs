using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps.Sdk.Helpers
{
    public delegate void EventDispatcherDelegate(object evtData);

    public static class InternalEventDispatcher
    {
        static Dictionary<string, List<EventDispatcherDelegate>> _listeners = new Dictionary<string, List<EventDispatcherDelegate>>();
        static object listlock = new object();
        public static void AddEventHandler(string eventName, EventDispatcherDelegate callback)
        {
            lock (listlock)
            {
                List<EventDispatcherDelegate> evtHandlers = null;
                if (!_listeners.TryGetValue(eventName, out evtHandlers))
                {
                    evtHandlers = new List<EventDispatcherDelegate>();
                    _listeners.Add(eventName, evtHandlers);
                }

                if (!evtHandlers.Contains(callback))
                    evtHandlers.Add(callback);
            }
        }

        public static void RemoveEventHandler(string eventName, EventDispatcherDelegate callback)
        {
            lock (listlock)
            {
                List<EventDispatcherDelegate> evtHandlers = null;
                if (_listeners.TryGetValue(eventName, out evtHandlers))
                {
                    if (evtHandlers.Contains(callback))
                    {
                        evtHandlers.Remove(callback);
                    }
                }
            }
        }

        public static void RaiseEvent(string eventName, object evt)
        {
            List<EventDispatcherDelegate> evtHandlers = null, handlers = null;
            if (_listeners.TryGetValue(eventName, out evtHandlers))
            {
                lock (listlock)
                {
                    handlers = evtHandlers.Select(x => x).ToList(); // ATENÇÃO>>>cria lista de handlers porquê a lista não pode ser alterada durante o loop.
                }
                for (int i = 0; i < handlers.Count; i++)
                {
                    try
                    {
                        evtHandlers[i](evt);
                    }
                    catch { }
                }
            }
        }
    }
}
