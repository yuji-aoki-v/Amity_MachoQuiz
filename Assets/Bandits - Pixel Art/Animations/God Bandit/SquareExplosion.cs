using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareExplosion : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void RightThunder()
    {
        GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
        Destroy(effect, 1.1f);
    }
}