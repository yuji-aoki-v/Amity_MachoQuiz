using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftTunderExplosion : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // スペースキーが押されたら、explosionPrefabが表示され、Destoroyされる
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        //    Destroy(this.gameObject);
        //}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject effect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
            Destroy(effect, 1.1f);
        }
    }
}
