using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    // Register a service (only one instance per type allowed)
    public static void Register<T>(T service)
    {
        Type type = typeof(T);

        if (_services.ContainsKey(type))
        {
            Debug.LogWarning($"Service of type {type.Name} is already registered. Replacing with new instance.");
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }
    }

    // Retrieve a service
    public static T Get<T>()
    {
        Type type = typeof(T);

        if (_services.TryGetValue(type, out object service))
        {
            return (T)service;
        }

        throw new Exception($"Service of type {type.Name} is not registered in ServiceLocator.");
    }

    // Check if a service exists
    public static bool IsRegistered<T>()
    {
        return _services.ContainsKey(typeof(T));
    }

    // Unregister a service (optional cleanup)
    public static void Unregister<T>()
    {
        Type type = typeof(T);

        if (_services.ContainsKey(type))
        {
            _services.Remove(type);
        }
    }

    // Clear everything (useful for scene changes / resets)
    public static void Clear() => _services.Clear();
}