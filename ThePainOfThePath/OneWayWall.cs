using System.Collections;
using UnityEngine;

namespace ThePainOfThePath
{
    internal class OneWayWall : MonoBehaviour
    {
        private IEnumerator OnCollisionEnter2D(Collision2D other)
        {
            Modding.Logger.Log("reee " + other.relativeVelocity.x);

            if (other.relativeVelocity.x <= 0)
            {
                FlashOnHit.FlashOnEnter(gameObject);
                GameCameras.instance.cameraShakeFSM.SendEvent("BigShake");
                yield break;
            }
            
            Physics2D.IgnoreCollision(other.collider, gameObject.GetComponent<BoxCollider2D>());

            Destroy(GameObject.Find("KillMe"));
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            FlashOnHit.RemoveFlash(gameObject);
        }
    }
}