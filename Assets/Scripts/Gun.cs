using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class Gun : MonoBehaviour
    {
        public enum FireMode
        {
            Auto,
            Burst,
            Single
        };

        public FireMode fireMode;

        public Transform[] Muzzle;
        public Projectile Projectile;
        public float MsBtweenShots = 100;
        public float MuzzlVelocity = 35;
        float _nexShootMePlsTime = 0;
        public int burstCount;
        public int projectlesPerMag;
        public float reloadTime = .3f;

        [Header("Recoil")]
        public Vector2 kickMinMax = new Vector2(0.5f,.2f);
        public Vector2 recoilAngleMinMax;
        public float recoilMoveSettleTime;
        public float recoilRotationSettleTime;

        [Header("Effects")]
        public Transform Shell;
        public Transform ShellEjection;
        public AudioClip shootAudio;
        public AudioClip reloadAudio;

        int projectilesRemainingInMag;
        bool isReloading;

        int shotsRemainingInBurst;

        Muzzleflash muzzle;

        private bool triggerReleaseSLS;

        Vector3 recoilSmoothDampVelocity;
        float recoilRotSmoothDampVelocity;
        float recoilAngle;
        

        private void Start()
        {
            muzzle = GetComponent<Muzzleflash>();
            shotsRemainingInBurst = burstCount;
            projectilesRemainingInMag = projectlesPerMag;
        }

        void LateUpdate()
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, .1f);
            recoilAngle = Mathf.SmoothDampAngle(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
            transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

            if (!isReloading && projectilesRemainingInMag == 0)
            {
                Reload();
            }
        }

        void Shoot()
        {
            if (!isReloading && Time.time > _nexShootMePlsTime && projectilesRemainingInMag > 0)
            {
                if (fireMode == FireMode.Burst)
                {
                    if (shotsRemainingInBurst == 0)
                    {
                        return;
                    }
                    shotsRemainingInBurst--;
                }
                else if (fireMode == FireMode.Single)
                {
                    if (!triggerReleaseSLS)
                    {
                        return;
                    }
                }


                for (int i = 0; i < Muzzle.Length; i++)
                {
                    if (projectilesRemainingInMag == 0)
                    {
                        return;
                    }
                    projectilesRemainingInMag--;
                    Projectile newBullet = Instantiate(
                        original: Projectile,
                        position: Muzzle[i].position,
                        rotation: Muzzle[i].rotation);

                    newBullet.SetSpeed(MuzzlVelocity);

                }

                Instantiate(
                    original: Shell,
                    position: ShellEjection.position,
                    rotation: ShellEjection.rotation);


                muzzle.Activate();
                transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
                recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
                recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
                AudioManager.instance.PlaySound(shootAudio,transform.position);
                _nexShootMePlsTime = Time.time+ MsBtweenShots/1000 ;
                Debug.Log("We shot");
            }
        }

        public void Reload()
        {
            if (!isReloading && projectilesRemainingInMag != projectlesPerMag)
            {
                StartCoroutine(AnimateReload());
                AudioManager.instance.PlaySound(reloadAudio, transform.position);
            }
        }

        public IEnumerator AnimateReload()
        {
            isReloading = true;
            yield return new WaitForSeconds(.2f);

            float reloadSpeed = 1f / reloadTime;
            float percent = 0;
            Vector3 initialRot = transform.localEulerAngles;
            float maxReloidAngle = 30;

            while (percent < 1)
            {
                percent += Time.deltaTime * reloadSpeed;
                float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                float reloadAngle = Mathf.Lerp(0, maxReloidAngle, interpolation);


                yield return null;
            }

            isReloading = false;

            projectilesRemainingInMag = projectlesPerMag;
        }

        public void Aim(Vector3 aimPoint)
        {
            if (!isReloading)
            {
                transform.LookAt(aimPoint);
            }
            
        }

        public void OnTriggerHold()
        {

                Shoot();
                triggerReleaseSLS = false;
            
        }

        public void OnTriggerRelease()
        {
            triggerReleaseSLS = true;
            shotsRemainingInBurst = burstCount;
        }


    }
    
}
