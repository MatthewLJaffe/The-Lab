using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyScripts.Agent
{
    public class TakeCoverState : BaseState
    {
        /// <summary>
        /// state where agent npc takes cover from player, not in use
        /// </summary>
        [SerializeField] private Vector2 scanCooldownRange;
        [SerializeField] private float hideCooldown;
        [SerializeField] private int scanDirs;
        [SerializeField] private float scanDistance;
        [SerializeField] private BaseState[] overridableStates;
        [SerializeField] private float pokeOutDistance;
        [SerializeField] private CircleCollider2D collider;
        [SerializeField] private float walkSpeed;
        [SerializeField] private ShootController shoot;
        [SerializeField] private Animator animator;
        [SerializeField] private HumanAnimator humanAnimator;
        [SerializeField] private Vector2 waitInCoverRange;
        [SerializeField] private float disengageTime;
        [SerializeField] private AnimationCurve walkCurve;
        private Rigidbody2D _rb;
        private string _coverEndAnimation;
        private string _coverBeginAnimation;
        private BaseState _previousState;
        private Coroutine _walkRoutine;
        private Coroutine _disengageRoutine;
        private Enemy _enemy;
        private bool _inScanCooldown;
        private bool _inHideCooldown;
        private bool _inCover;
        private bool _atEndpoint;
        private Vector2 _coverBegin;
        private Vector2 _coverCenter;
        private Vector2 _coverEnd;
        private Vector2 _shootPoint;

        protected override void Awake()
        {
            _enemy = GetComponentInParent<Enemy>();
            _rb = GetComponentInParent<Rigidbody2D>();
            base.Awake();
        }

        public override Type Tick()
        {
            //check for need to exit cover state
            if (!IsCover()) {
                ExitCoverState();
                return _previousState.GetType();
            }
            if (_walkRoutine != null)
                return null;
            _shootPoint = FindShootPoint();
            //if there is no line of sight start disengage
            if (_shootPoint == Vector2.zero && _disengageRoutine == null)
            {
                _disengageRoutine = StartCoroutine(Disengage());
                _shootPoint = Vector2.zero;
                _walkRoutine = StartCoroutine(WalkToPoint(_coverCenter));
            }
            return null;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_inCover || _inHideCooldown || other.gameObject.layer != LayerMask.NameToLayer("Bullet"))
                return;
            if (_walkRoutine != null)
                StopCoroutine(_walkRoutine);
            _shootPoint = Vector2.zero;
            shoot.CanShoot = false;
            _walkRoutine = StartCoroutine(WalkToPoint(_coverCenter));
            StartCoroutine(HideCooldown());
        }

        protected override void SwitchState(BaseState state)
        {
            if (state == this)
            {
                _inCover = true;
                foreach (var o in overridableStates)
                    o.OverrideState = null;
                _steeringController.enabled = false;
            }
            else
                _previousState = state;
            base.SwitchState(state);
        }

        private void FixedUpdate()
        {
            if (_inScanCooldown || _inCover) return;
            ScanForCover();
            StartCoroutine(ScanCooldown());
        }

        private IEnumerator ScanCooldown()
        {
            _inScanCooldown = true;
            yield return new WaitForSeconds(Random.Range(scanCooldownRange.x, scanCooldownRange.y));
            _inScanCooldown = false;
        }

        private IEnumerator HideCooldown()
        {
            _inHideCooldown = true;
            yield return new WaitForSeconds(hideCooldown);
            _inHideCooldown = false;
        }
        
        private void ScanForCover()
        {
            for (var c = 0; c < scanDirs; c++)
            {
                var theta = c * 360f / scanDirs;
                var dir =  Quaternion.Euler(0, 0, theta) * Vector2.up;
                var hit = Physics2D.Raycast(transform.position, dir, scanDistance, LayerMask.GetMask("Block"));
                if (hit && CheckForCover(hit)) {
                    foreach (var state in overridableStates) {
                        state.OverrideState = this;
                    }
                }
            }
        }

        private bool CheckForCover(RaycastHit2D hit)
        {
            var boxCollider = hit.transform.gameObject.GetComponent<BoxCollider2D>();
            if (!boxCollider) return false;
            var boxCenter = (Vector2)hit.transform.position + boxCollider.offset;
            var hitNormal = hit.normal.normalized;
            PickCoverAnimations(hitNormal);
            var size = boxCollider.size;
            _coverCenter = boxCenter + new Vector2((size.x/2 + collider.radius) * hitNormal.x, (size.y/2 + collider.radius) * hitNormal.y);
            _coverCenter -= collider.offset;
            var perp = new Vector2(hitNormal.y, hitNormal.x);
            _coverBegin = new Vector2(_coverCenter.x + perp.x * (size.x/2 + pokeOutDistance),
                _coverCenter.y + perp.y * (size.y/2 + pokeOutDistance));
            _coverEnd = new Vector2(_coverCenter.x - perp.x * (size.x/2 + pokeOutDistance),
                _coverCenter.y - perp.y * (size.y/2 + pokeOutDistance));
            //Check if there is a line of sight to player from cover endpoints
            var beginHit = Physics2D.Raycast(_coverBegin + collider.offset, (Vector2)_enemy.target.position - (_coverBegin + collider.offset), 
                Vector2.Distance(_enemy.target.position, _coverBegin + collider.offset) + 1f, 
                Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")) | LayerMask.GetMask("Invincible"));
            
            var endHit = Physics2D.Raycast(_coverEnd + collider.offset, (Vector2)_enemy.target.position - (_coverEnd + collider.offset), 
                Vector2.Distance(_enemy.target.position, _coverEnd - collider.offset) + 1f, 
                Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")) | LayerMask.GetMask("Invincible"));
            if ((beginHit && beginHit.transform == _enemy.target) || (endHit && endHit.transform == _enemy.target))
                return IsCover();
            return false;

        }

        private void PickCoverAnimations(Vector2 hitNormal)
        {
            if (hitNormal.x < -.99f)
            {
                _coverBeginAnimation = "GunRightHand";
                _coverEndAnimation = "GunRightHand";
            }
            else if (hitNormal.x > .99f)
            {
                _coverBeginAnimation = "GunLeftHand";
                _coverEndAnimation = "GunLeftHand";
            }
            else if (hitNormal.y > .99f)
            {
                _coverBeginAnimation = "GunRightHand";
                _coverEndAnimation = "GunLeftHand";
            }
            else
            {
                _coverBeginAnimation = "GunLeftHand";
                _coverEndAnimation = "GunRightHand";
            }
        }

        private bool IsCover()
        {
            var hit =  Physics2D.Raycast(_coverCenter + collider.offset, (Vector2)_enemy.target.position - (_coverCenter + collider.offset), 
                Vector2.Distance(_enemy.target.position, _coverCenter), LayerMask.GetMask("Block"));
            return hit;
        }

        //select shoot point and if needed walk there
        private Vector2 FindShootPoint()
        {
            var beginHit = Physics2D.Raycast(_coverBegin + collider.offset, (Vector2)_enemy.target.position - (_coverBegin + collider.offset), 
                Vector2.Distance(_enemy.target.position, _coverBegin + collider.offset) + 1f, 
                Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")) | LayerMask.GetMask("Invincible"));
            
            var endHit = Physics2D.Raycast(_coverEnd + collider.offset, (Vector2)_enemy.target.position - (_coverEnd + collider.offset), 
                Vector2.Distance(_enemy.target.position, _coverEnd - collider.offset) + 1f, 
                Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Enemy Bullet")) | LayerMask.GetMask("Invincible"));
            Debug.DrawRay(_coverEnd + collider.offset, (Vector2)_enemy.target.position - (_coverEnd + collider.offset), Color.blue, 1f);
            Debug.DrawRay(_coverBegin + collider.offset, (Vector2)_enemy.target.position - (_coverBegin + collider.offset), Color.blue, 1f);
            var hitFromBegin = beginHit && beginHit.transform == _enemy.target;
            var hitFromEnd = endHit && endHit.transform == _enemy.target;
            if (_shootPoint == _coverBegin && hitFromBegin)
                return _coverBegin;
            if (_shootPoint == _coverEnd && hitFromEnd)
                return _coverEnd;
            
            if ((hitFromBegin || hitFromEnd) && _walkRoutine != null)
                StopCoroutine(_walkRoutine);
            if (hitFromBegin)
            {
                _walkRoutine = StartCoroutine(WalkToPoint(_coverBegin));
                return _coverBegin;
            }
            if (hitFromEnd)
            {
                _walkRoutine = StartCoroutine(WalkToPoint(_coverEnd));
                return _coverEnd;
            }
            return Vector2.zero;
        }

        private IEnumerator WalkToPoint(Vector2 point)
        {
            
            var distance = Vector2.Distance(_enemy.transform.position, point);
            var walkTime = distance / (walkSpeed/2);
            var walkDir = (point - (Vector2)_enemy.transform.position).normalized * walkSpeed;
            _rb.velocity = walkDir;
            var walkComplete = false;
            for (var t = 0f; t < walkTime * 3; t += Time.deltaTime)
            {
                var curDistance = Vector2.Distance(_enemy.transform.position, point);
                var localSpeed = walkSpeed * Mathf.Clamp(walkCurve.Evaluate(1f - curDistance/distance), 0f, 1f);
                _rb.velocity = (point - (Vector2)_enemy.transform.position).normalized * localSpeed;
                if (curDistance < .15f)
                {
                    walkComplete = true;
                    break;
                }
                humanAnimator.UpdateAnimationNoAim();
                yield return new WaitForEndOfFrame();
            }
            if (!walkComplete)
            {
                OverrideState = _previousState;
                ExitCoverState();
                yield break;
            }
            _rb.velocity = Vector2.zero;
            _enemy.transform.position = point;
            if (point == _coverBegin)
            {
                shoot.CanShoot = true;
                animator.Play(_coverBeginAnimation);
                _enemy.transform.rotation = Quaternion.identity;
            }
            else if (point == _coverEnd)
            {
                shoot.CanShoot = true;
                animator.Play(_coverEndAnimation);
                _enemy.transform.rotation = Quaternion.identity;
            }
            else if (point == _coverCenter)
            {
                yield return new WaitForSeconds(Random.Range(waitInCoverRange.x, waitInCoverRange.y));
                FindShootPoint();
            }
            _walkRoutine = null;
        }

        private IEnumerator Disengage()
        {
            shoot.CanShoot = false;
            for (var c = 0; c < 8; c++)
            {
                _shootPoint = FindShootPoint();
                if (_shootPoint != Vector2.zero)
                {
                    _walkRoutine = StartCoroutine(WalkToPoint(_shootPoint));
                    _disengageRoutine = null;
                    yield break;
                }
                yield return new WaitForSeconds(disengageTime / 8);
            }
            OverrideState = _previousState;
            _disengageRoutine = null;
            ExitCoverState();
        }

        private void ExitCoverState()
        {
            shoot.CanShoot = true;
            _inCover = false;
            if (_walkRoutine != null)
            {
                StopCoroutine(_walkRoutine);
                _walkRoutine = null;
            }
            if (_disengageRoutine != null)
            {
                StopCoroutine(_disengageRoutine);
                _disengageRoutine = null;
            }
            _steeringController.enabled = true;
        }
    }
    
}