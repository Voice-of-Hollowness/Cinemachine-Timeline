using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        Rigidbody _myRigid;
        Vector3 _velocity;

        void Start()
        {
            _myRigid = GetComponent<Rigidbody>();
        }

        public void LookAtFckingPoint(Vector3 point)
        {

            Vector3 corectPoint = new Vector3(point.x,transform.position.y,point.z) ;
            transform.LookAt(corectPoint);
        }

        void FixedUpdate()
        {

            _myRigid.MovePosition(_myRigid.position+_velocity*Time.fixedDeltaTime);
        }

        public void Move(Vector3 velocity)
        {
            this._velocity = velocity;
        }
    }
}
