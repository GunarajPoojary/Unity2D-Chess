using System;
using System.Collections.Generic;

public interface IGameEvent { }

public class EventBusOptions
{
    public bool EnableLogging { get; set; } = false;
    public bool ThrowOnNoSubscribers { get; set; } = false;
    public bool UseEventQueue { get; set; } = false;
}

public interface IEventBus
{
    void Subscribe<T>(Action<T> listener) where T : IGameEvent;
    void Unsubscribe<T>(Action<T> listener) where T : IGameEvent;
    void Publish<T>(T gameEvent) where T : IGameEvent;

    void ProcessQueue();

    void Clear();
}

public class EventBus : IEventBus
{
    private readonly Dictionary<Type, Delegate> _events = new();
    private readonly Queue<IGameEvent> _queue = new();
    private readonly object _lock = new();

    private readonly EventBusOptions _options;

    public EventBus(EventBusOptions options = null)
    {
        _options = options ?? new EventBusOptions();
    }

    public void Subscribe<T>(Action<T> listener) where T : IGameEvent
    {
        var type = typeof(T);

        lock (_lock)
        {
            if (_events.TryGetValue(type, out var existing))
                _events[type] = Delegate.Combine(existing, listener);
            else
                _events[type] = listener;
        }

        if (_options.EnableLogging)
            Console.WriteLine($"[EventBus] Subscribed: {type.Name}");
    }

    public void Unsubscribe<T>(Action<T> listener) where T : IGameEvent
    {
        var type = typeof(T);

        lock (_lock)
        {
            if (_events.TryGetValue(type, out var existing))
            {
                Delegate current = Delegate.Remove(existing, listener);

                if (current == null)
                    _events.Remove(type);
                else
                    _events[type] = current;
            }
        }

        if (_options.EnableLogging)
            Console.WriteLine($"[EventBus] Unsubscribed: {type.Name}");
    }

    public void Publish<T>(T gameEvent) where T : IGameEvent
    {
        if (_options.UseEventQueue)
        {
            lock (_lock)
            {
                _queue.Enqueue(gameEvent);
            }
            return;
        }

        Dispatch(gameEvent);
    }

    public void ProcessQueue()
    {
        if (!_options.UseEventQueue)
            return;

        while (true)
        {
            IGameEvent evt;

            lock (_lock)
            {
                if (_queue.Count == 0)
                    break;

                evt = _queue.Dequeue();
            }

            Dispatch(evt);
        }
    }

    private void Dispatch(IGameEvent gameEvent)
    {
        var type = gameEvent.GetType();
        Delegate callback = null;

        lock (_lock)
        {
            _events.TryGetValue(type, out callback);
        }

        if (callback != null)
        {
            callback.DynamicInvoke(gameEvent);
        }
        else if (_options.ThrowOnNoSubscribers)
        {
            throw new Exception($"No subscribers for event {type.Name}");
        }

        if (_options.EnableLogging)
            Console.WriteLine($"[EventBus] Published: {type.Name}");
    }

    public void Clear()
    {
        lock (_lock)
        {
            _events.Clear();
            _queue.Clear();
        }

        if (_options.EnableLogging)
            Console.WriteLine("[EventBus] Cleared");
    }
}