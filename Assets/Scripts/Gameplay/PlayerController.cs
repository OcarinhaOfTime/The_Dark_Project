using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float timeReachMax = .5f;
    public float timeToStop = .25f;
    public float max_speed = 10;

    public Vector2 velocity;
    public Vector2 deltaVelocity;

    public LayerMask groundMask = (1 << 8);     
    
    public CollisionInfo collisionInfo = new CollisionInfo();
    public bool grounded { get; private set; }

    private float lastDir;
    private float h_input;
    private const float skin_width = .01f;
    private RaycastOrigins raycastOrigins;
    private int horizontalRayCount = 4;
    private int verticalRayCount = 4;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private const float MaxClimbSlope = 60;

    private Collider2D col;
    private Animator animator;


    private float acceleration {
        get {
            return max_speed / timeReachMax;
        }
    }

    private float desacceleration {
        get {
            return max_speed / timeToStop;
        }
    }

    public float g = 24;

    public bool facingRight {
        get {
            return transform.localScale.x > 0;
        }

        set {
            var scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (value ? 1 : -1);
            transform.localScale = scale;
        }
    }

    public float facingSign {
        get {
            return transform.localScale.x > 0 ? 1 : -1;
        }
    }

    private void Awake() {
        col = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        CalculateRaySpacing();
    }

    float Sign(float x) {
        if (x > .01f)
            return 1;

        if (x < -.01f)
            return -1;

        return 0;
    }

    public void Move(float h_input, bool jump) {
        if(h_input == 0) {
            velocity.x -= desacceleration * lastDir * Time.fixedDeltaTime;
            
            if(Sign(velocity.x) != lastDir) {
                lastDir = 0;
                velocity.x = 0;
            }
        } else {
            velocity.x += h_input * acceleration * Time.fixedDeltaTime;
            lastDir = h_input;
        }
    }

    private void FixedUpdate() {
        if (collisionInfo.below || collisionInfo.above) {
            velocity.y = 0;
        }

        Move(Input.GetAxisRaw("Horizontal"), Input.GetButtonDown("Jump"));

        velocity.y += g * Time.deltaTime;
        velocity.x = Mathf.Clamp(velocity.x, -max_speed, max_speed);

        deltaVelocity = Time.deltaTime * velocity;

        collisionInfo.Reset();
        UpdateRaycastOrigins();

        if (deltaVelocity.y < 0) {
            DescendSlope();
        }

        if (deltaVelocity.x != 0) {
            HorizontalCollisions();
        }

        if (deltaVelocity.y != 0) {
            VerticalCollisions();
        }

        transform.Translate(deltaVelocity);

        if (facingRight && velocity.x < -.01f || !facingRight && velocity.x > .01f)
            facingRight = !facingRight;

        //animator.SetFloat("VSpeed", velocity.y);
        //animator.SetBool("Run", Mathf.Abs(velocity.x) > .1f);
        //animator.SetBool("Grounded", grounded);
    }    

    void OnLand() {
    }

    void HorizontalCollisions() {
        var dir = Mathf.Sign(deltaVelocity.x);
        var rayLength = Mathf.Abs(deltaVelocity.x) + skin_width;

        for (int i = 0; i < horizontalRayCount; i++) {
            var pos = dir < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            pos += Vector2.up * horizontalRaySpacing * i;

            var hit = Physics2D.Raycast(pos, Vector2.right * dir, rayLength, groundMask);
            Debug.DrawRay(pos, Vector2.right * dir * rayLength, Color.red);

            if (hit) {
                var angle = Vector2.Angle(hit.normal, Vector2.up);

                if(i ==0 && angle <= MaxClimbSlope) {
                    var distanceToSlopeStart = 0f;
                    if(collisionInfo.slopeAngleOld != angle) {
                        distanceToSlopeStart = hit.distance - skin_width;
                        deltaVelocity.x -= distanceToSlopeStart * dir;
                    }
                    ClimbSlope(angle);
                    deltaVelocity.x += distanceToSlopeStart * dir;
                }

                if(!collisionInfo.climbingSlope || angle > MaxClimbSlope) {
                    deltaVelocity.x = (hit.distance - skin_width) * dir;
                    rayLength = hit.distance;

                    if (collisionInfo.climbingSlope) {
                        deltaVelocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(deltaVelocity.x);
                    }

                    collisionInfo.left = dir < 0;
                    collisionInfo.right = dir > 0;
                }                                
            }
        }
    }

    void ClimbSlope(float slopeAngle) {
        float moveDistance = Mathf.Abs(deltaVelocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (deltaVelocity.y <= climbVelocityY) {
            deltaVelocity.y = climbVelocityY;
            deltaVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(deltaVelocity.x);
            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope() {
        float dir = Mathf.Sign(deltaVelocity.x);
        Vector2 pos = (dir == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(pos, -Vector2.up, Mathf.Infinity, groundMask);

        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= MaxClimbSlope) {
                if (hit.normal.x * dir > 0) {
                    var moveDistance = Mathf.Abs(deltaVelocity.x);
                    if (hit.distance - skin_width <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * moveDistance) {
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        deltaVelocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * dir;
                        deltaVelocity.y -= descendVelocityY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.below = true;
                    }
                }
            }
        }
    }


    void VerticalCollisions() {
        var dir = Mathf.Sign(deltaVelocity.y);
        var rayLength = Mathf.Abs(deltaVelocity.y) + skin_width;

        grounded = false;

        for (int i=0; i< verticalRayCount; i++) {
            var pos = dir < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            pos += Vector2.right * (verticalRaySpacing * i + deltaVelocity.x);

            var hit = Physics2D.Raycast(pos, Vector2.up * dir, rayLength, groundMask);
            Debug.DrawRay(pos, Vector2.up * dir * rayLength, Color.red);

            if (hit) {
                deltaVelocity.y = (hit.distance - skin_width) * dir;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope) {
                    deltaVelocity.x = Mathf.Abs(deltaVelocity.y) / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(deltaVelocity.x);
                }

                collisionInfo.below = dir < 0;
                collisionInfo.above = dir > 0;
            }
        }

        if (collisionInfo.climbingSlope) {
            dir = Mathf.Sign(deltaVelocity.x);
            rayLength = Mathf.Abs(deltaVelocity.x) + skin_width;
            var pos = dir < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight + Vector2.up * deltaVelocity.y;
            var hit = Physics2D.Raycast(pos, Vector2.right * dir, rayLength, groundMask);

            if (hit) {
                var angle = Vector2.Angle(hit.normal, Vector2.up);
                if(angle != collisionInfo.slopeAngle) {
                    deltaVelocity.x = (hit.distance - skin_width) * dir;
                    collisionInfo.slopeAngle = angle;
                }                
            }
        }

        if (collisionInfo.below) {
            grounded = true;
            OnLand();
        }
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = col.bounds;
        bounds.Expand(skin_width * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = col.bounds;
        bounds.Expand(skin_width * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    [System.Serializable]
    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle;
        public float slopeAngleOld;

        public void Reset() {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
