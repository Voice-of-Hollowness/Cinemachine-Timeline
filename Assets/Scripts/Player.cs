using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(GunSys))]
    [RequireComponent(typeof(PlayerController))]
    public class Player : LivingEntetity
    {
        public float moveSpeed = 5;

        public Crosshair crosshair;

        Camera _view;
        GunSys _gunSys;
        PlayerController _control;

        protected void  Start()
        {
            base.Start();
        }
        private void Awake()
        {
            _gunSys = GetComponent<GunSys>();
            _control = GetComponent<PlayerController>();
            _view = Camera.main;
            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        }

        void OnNewWave(int waveNum)
        {
            var howManyGuns = _gunSys.AllGuns.Length; 
            var gunIndex = waveNum%howManyGuns;
            health = startHealth;
            _gunSys.EquipGun(gunIndex);
        }

        void Update()

        {
          //todo : Refactroing smaller Methods

            Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));

            Vector3 moveVelocity = moveInput.normalized * moveSpeed;

            _control.Move(moveVelocity);

            //look here and there

            Ray ray = _view.ScreenPointToRay(Input.mousePosition);

            Plane ground = new Plane(Vector3.up,Vector3.up*_gunSys.GetHeight);

            if (ground.Raycast(ray, out var rayDistance))
            {
                Vector3 bulletPointWtf = ray.GetPoint(rayDistance);

               // Debug.DrawLine(ray.origin, bulletPointWtf, Color.red);
                _control.LookAtFckingPoint(bulletPointWtf);
                crosshair.transform.position = bulletPointWtf;
                crosshair.DetectTargets(ray);
                if ((new Vector2(bulletPointWtf.x, bulletPointWtf.z) -
                     new Vector2(transform.position.x, transform.position.z))
                    .sqrMagnitude > 1)
                {
                    _gunSys.Aim(bulletPointWtf);
                };
                
                
            }

            //weapon input
            if (Input.GetMouseButton(0)) {
                _gunSys.OnTriggerHold();
            }
            if (Input.GetMouseButtonUp(0))
            {
                _gunSys.OnTriggerRelease();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _gunSys.Reload();
            }
        }


        public override void Die()
        {
            AudioManager.instance.PlaySound("Player Death", Vector3.zero);
            base.Die();

        }
    }
}
