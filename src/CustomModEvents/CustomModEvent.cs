using System;

namespace MSNTools
{
    public class CustomModEvent : CustomModEventAbs<Action>
    {
        public void Invoke()
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc();
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1> : CustomModEventAbs<Action<T1>>
    {
        public void Invoke(T1 _a1)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2> : CustomModEventAbs<Action<T1, T2>>
    {
        public void Invoke(T1 _a1, T2 _a2)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3> : CustomModEventAbs<Action<T1, T2, T3>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4> : CustomModEventAbs<Action<T1, T2, T3, T4>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4, T5> : CustomModEventAbs<Action<T1, T2, T3, T4, T5>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4,T5 _a5)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4, _a5);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4, T5, T6> : CustomModEventAbs<Action<T1, T2, T3, T4, T5, T6>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4, T5 _a5, T6 _a6)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4, _a5, _a6);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4, T5, T6, T7> : CustomModEventAbs<Action<T1, T2, T3, T4, T5, T6, T7>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4, T5 _a5, T6 _a6, T7 _a7)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4, _a5, _a6, _a7);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4, T5, T6, T7, T8> : CustomModEventAbs<Action<T1, T2, T3, T4, T5, T6, T7, T8>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4, T5 _a5, T6 _a6, T7 _a7, T8 _a8)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9> : CustomModEventAbs<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4, T5 _a5, T6 _a6, T7 _a7, T8 _a8, T9 _a9)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }

    public class CustomModEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : CustomModEventAbs<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>
    {
        public void Invoke(T1 _a1, T2 _a2, T3 _a3, T4 _a4, T5 _a5, T6 _a6, T7 _a7, T8 _a8, T9 _a9, T10 _a10)
        {
            for (int index = 0; index < receivers.Count; ++index)
            {
                Receiver receiver = receivers[index];
                try
                {
                    receiver.DelegateFunc(_a1, _a2, _a3, _a4, _a5, _a6, _a7, _a8, _a9, _a10);
                }
                catch (Exception ex)
                {
                    LogError(ex, receiver);
                }
            }
        }
    }
}
