using System;
using System.Collections.Generic;

namespace InexperiencedDeveloper.Dialogue.Samples
{
    public class ClearableEvent
    {
        protected List<Action> m_eventListeners = new List<Action>();

        public void Subscribe(bool shouldSubscribe, Action action)
        {
            if(!shouldSubscribe)
            {
                m_eventListeners.Remove(action);
                return;
            }

            m_eventListeners.Add(action);
        }

        public void Invoke()
        {
            foreach(Action listener in m_eventListeners)
            {
                listener?.Invoke();
            }
        }

        public void ClearAllListeners()
        {
            m_eventListeners.Clear();
        }
    }

    public class ClearableEventOneParam<T>
    {
        protected List<Action<T>> m_eventListeners = new List<Action<T>>();

        public void Subscribe(bool shouldSubscribe, Action<T> action)
        {
            if (!shouldSubscribe)
            {
                m_eventListeners.Remove(action);
                return;
            }

            m_eventListeners.Add(action);
        }

        public void Invoke(T param)
        {
            foreach (Action<T> listener in m_eventListeners)
            {
                listener?.Invoke(param);
            }
        }

        public void ClearAllListeners()
        {
            m_eventListeners.Clear();
        }
    }
}
