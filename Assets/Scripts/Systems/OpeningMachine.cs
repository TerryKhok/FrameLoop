using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningMachine : MonoBehaviour
{
    [SerializeField]
    private GameObject _getMachineParticle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            var pos = transform.position;

            pos += new Vector3(1.0f, 0.7f);

            var instance = Instantiate(_getMachineParticle, pos, _getMachineParticle.transform.rotation);
            Destroy(instance, 2.0f);

            var playerOpening = collision.GetComponent<PlayerOpening>();
            playerOpening.GetFrameMachine();
            Destroy(gameObject);
        }
    }
}
