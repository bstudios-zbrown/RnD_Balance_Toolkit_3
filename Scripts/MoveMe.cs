using UnityEngine;
public class MoveMe : MonoBehaviour
{
   public float velocity = 10;
   void Update ()
   {
        transform.position += transform.forward * velocity * Time.deltaTime;
   }
} 