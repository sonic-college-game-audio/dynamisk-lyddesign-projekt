using System;
using FMODUnity;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public static event Action OnArrivedAtGate;
    
    public Collider trigger;
    public Vector3 offsetToPlayer;
    public AnimationCurve animationCurve;

    [Header("Audio")]
    public EventReference pickupEvent;
    public EventReference startsFlyingEvent;
    public StudioEventEmitter loopEmitter;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        trigger.enabled = false;
        RuntimeManager.PlayOneShot(pickupEvent);
        loopEmitter.Stop();
        PickupAsync().Run();
    }

    private async Awaitable PickupAsync()
    {
        await AnimatePickupAsync();
        await KeepInFrontOfPlayerAsync();
        await FlyTowardsEndGateAsync();
        
        OnArrivedAtGate?.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// Moves the pickup from its position in the world to be in front of the player
    /// </summary>
    private async Awaitable AnimatePickupAsync()
    {
        Transform playerTransform = Game.currentLevel.playerTransform;
        Vector3 startOffset = playerTransform.InverseTransformPoint(transform.position);
        Vector3 startForward = transform.forward;
        
        float t = 0;
        while (t < 1)
        {
            float curveT = animationCurve.Evaluate(t);
            Vector3 offset = Vector3.Lerp(startOffset, offsetToPlayer, animationCurve.Evaluate(curveT));
            transform.position = playerTransform.TransformPoint(offset);
            transform.forward = Vector3.Slerp(startForward, transform.forward, animationCurve.Evaluate(curveT));
            
            await Awaitable.NextFrameAsync();

            t += Time.deltaTime;
        }
    }
    
    /// <summary>
    /// Keeps the pickup in front of the player while the player is outside of the safe path
    /// </summary>
    private async Awaitable KeepInFrontOfPlayerAsync()
    {
        Transform playerTransform = Game.currentLevel.playerTransform;
        DistanceToPath distanceToPath = FindFirstObjectByType<DistanceToPath>();
        
        while (distanceToPath.NormalizedTimeOutsideSafeArea > 0)
        {
            transform.position = playerTransform.TransformPoint(offsetToPlayer);
            transform.forward = playerTransform.forward;

            try
            {
                await Awaitable.NextFrameAsync(destroyCancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }
    
    /// <summary>
    /// Moves the pickup towards to the end gate
    /// </summary>
    private async Awaitable FlyTowardsEndGateAsync()
    {
        Transform target = FindFirstObjectByType<EndGate>().transform;
        Vector3 to = target.position + Vector3.up * 3;
        CharacterController characterController = FindFirstObjectByType<CharacterController>();
        
        Vector3 velocity = characterController.velocity;
        float maxVelocity = 50f;
        float acceleration = 20;
        float maxRadiansDelta = 0;
        Vector3 position = transform.position;
        
        RuntimeManager.PlayOneShot(startsFlyingEvent, position);
        
        while (Vector3.Distance(position, to) > 10)
        {
            if (velocity.magnitude < maxVelocity)
            {
                velocity += transform.forward * acceleration * Time.unscaledDeltaTime;
            }

            float velocityMagnitude = velocity.magnitude;
            Vector3 targetDirection = (to - position).normalized;
            Vector3 targetVelocity = targetDirection * velocityMagnitude;
            
            // We slowly rotate the velocity to get a nice curved motion
            velocity = Vector3.RotateTowards(velocity, targetVelocity, maxRadiansDelta, 0);
            velocity.y = 0;
            position += velocity * Time.unscaledDeltaTime;
            
            if (Physics.Raycast(transform.position + Vector3.down * 0.5f, Vector3.down, 3))
            {
                // There is something right under the pickup. Move the pickup up a little bit.  
                position.y = Mathf.MoveTowards(position.y, position.y + 1, Time.unscaledDeltaTime);
            }
            else
            {
                // There is nothing right under the pickup. Move the pickup down a little bit.
                position.y = Mathf.MoveTowards(position.y, position.y - 1, Time.unscaledDeltaTime / 10);
            }

            transform.position = position;
            transform.forward = velocity.normalized;
            
            // Increase how quickly the velocity can change every frame to make it snappier over time
            maxRadiansDelta += Time.unscaledDeltaTime / 60;
            
            await Awaitable.NextFrameAsync();
        }
    }
}
