using UnityEngine;

namespace Assets.Scripts
{
    public class GunSys : MonoBehaviour
    {

        public Transform WeaponHold;

        public Gun[] AllGuns;

        Gun _equipedGun;

        void Start(){
             
        }

        public void EquipGun(Gun gun)
        {
            if (_equipedGun != null) {
                Destroy(_equipedGun.gameObject);
            }

            _equipedGun = Instantiate(
                original: gun,
                position: WeaponHold.position,
                rotation: WeaponHold.rotation,
                parent: WeaponHold);

        }

        public void EquipGun(int gunIndex)
        {
            EquipGun(AllGuns[gunIndex]);

        }

        public void OnTriggerHold( )
        {
            if (_equipedGun != null){

                _equipedGun.OnTriggerHold();

            }
        }

        public void OnTriggerRelease()
        {
            if (_equipedGun != null)
            {

                _equipedGun.OnTriggerRelease();

            }
        }

        public void Aim(Vector3 aimPoint)
        {

            if (_equipedGun != null)
            {

                _equipedGun.Aim(aimPoint);

            }
        }

        public void Reload() 
        {

            if (_equipedGun != null)
            {

                _equipedGun.Reload();

            }
        }

        public float GetHeight => WeaponHold.position.y;
    }

}
