using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringEnemy : BasicEnemy {
    // moving
    [Header("Moving")]
    public float stoppingDistance;
    public float maxDistance;
    float targetHeight;
    public float speed;
    public float hoverDst;
    public float hoverSpeed;
    float hoverTimerX = 0f;
    float hoverTimerY = 0f;
    Vector3 anchorPos;
    bool hovering = false;

    protected override void EnemyStart() {
        MoveInward(Planet.radius + stoppingDistance);
    }

    void MoveInward(float _targetHeight) {
        targetHeight = _targetHeight;
        state = State.moving;
    }

    void StartHover() {
        hoverTimerX = 0f;
        hoverTimerY = 0f;
        anchorPos = transform.position;
        hovering = true;
    }

    void EndHover() {
        hovering = false;
        //transform.position = anchorPos;
    }


    protected override void EnemyUpdate() {
        if (state == State.moving) {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
            if (transform.position.magnitude <= targetHeight) {
                // end movement
                StartShooting();
                StartHover();
            }
        }
        if (hovering) {
            hoverTimerX += Time.deltaTime;
            hoverTimerY += Time.deltaTime / 2f;
            float xOffset = Mathf.Sin(hoverTimerX * Mathf.PI * hoverSpeed) * hoverDst;
            float yOffset = Mathf.Sin(hoverTimerY * Mathf.PI * hoverSpeed) * hoverDst;
            transform.position = anchorPos + (transform.right * xOffset) + (transform.up * yOffset);
        }
    }

    public override void PlanetRadiusUpdated(float newRadius) {
        if (newRadius + maxDistance < transform.position.magnitude) {
            EndHover();
            MoveInward(newRadius + stoppingDistance);
        }
    }
}
