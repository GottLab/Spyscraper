
using System;
using System.Collections.Generic;

public class LockManager
{
    private readonly HashSet<string> locks = new();

    public event Action OnUnlock;
    public event Action OnLock;


    public void SetLock(string reason, bool locked)
    {
        if (locked)
        {
            Lock(reason);
        }
        else
        {
            Unlock(reason);
        }
    }

    public void Unlock(string reason)
    {
        this.locks.Remove(reason);
        this.OnUnlock?.Invoke();
    }

    public void Lock(string reason)
    {
        this.locks.Add(reason);
        this.OnLock?.Invoke();
    }

    public bool Locked
    {
        get => this.locks.Count > 0;
    }
}