using UnityEngine;

public class Coin : MonoBehaviour
    {
        public bool Color=true;
        public bool BeforeColor=false;
        public float Rotat=0;
        public float Rotating=0;
        void FixedUpdate()
        {
            if (Color != BeforeColor)
            {
                Rotat = Color ? 0 : 180;
                if (Color) Rotating -= 6f;
                else Rotating += 6f;
                this.transform.rotation=Quaternion.Euler(Rotating,0,0);
                this.transform.position=new Vector3(transform.position.x,Mathf.Sin(Rotating*Mathf.PI/180),transform.position.z);
                if (Mathf.Abs(Rotat-Rotating)<0.1)BeforeColor = Color;
            }
        }
    }