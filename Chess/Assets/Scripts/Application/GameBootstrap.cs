using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public IEventBus EventBus { get; private set; }

    public void Init()
    {
        EventBus = new EventBus(new EventBusOptions
        {
            EnableLogging = true,
            ThrowOnNoSubscribers = false,
            UseEventQueue = true
        });
    }
}