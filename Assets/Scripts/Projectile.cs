using UnityEngine;

namespace Assets.Scripts
{
    public class Projectile : MonoBehaviour
    {
 
        [SerializeField]

        private float _speed = 10;

        [SerializeField]
        private float _damage = 1;

        [SerializeField]
        private LayerMask _collisionMask;

        private float skinWidth = 0.1f;
        private float LifeTime=3;

        public void SetSpeed( float newSpeed)
        {

            _speed = newSpeed;
        }

       void Start()
        {
            Destroy(gameObject,LifeTime);
            Collider[] initialCollision = Physics.OverlapSphere(transform.position, 0.1f, _collisionMask); 
            if (initialCollision.Length > 0)
            {
                OnHitObject(initialCollision[0], transform.position);
            }
        
        }

         void Update()
        {
            float moveDistance = _speed * Time.deltaTime;
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);
        }
        void CheckCollisions(float moveDistance)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, _collisionMask, QueryTriggerInteraction.Collide))
            {
                OnHitObject(hit.collider,hit.point);
            }
        }

        void OnHitObject(Collider c, Vector3 hitPoint)
        {
            IDamageable damageableObject = c.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeHit(_damage,hitPoint,transform.forward);
            }
            GameObject.Destroy(gameObject);
        }
    }
}