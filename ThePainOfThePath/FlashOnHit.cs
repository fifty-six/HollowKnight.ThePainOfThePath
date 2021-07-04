using System.Collections;
using UnityEngine;

namespace ThePainOfThePath
{
    internal class FlashOnHit : MonoBehaviour
    {
        public static void FlashOnEnter(GameObject go)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.black);
            tex.Apply();

            go.GetComponent<MeshRenderer>().material.mainTexture = tex;
        }

        public static void RemoveFlash(GameObject go)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();

            go.GetComponent<MeshRenderer>().material.mainTexture = tex;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            FlashOnEnter(gameObject);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            RemoveFlash(gameObject);
        }
    }
}