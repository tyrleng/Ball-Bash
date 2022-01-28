using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 naturalTilt;
    private GameManager _gameManager;
    private Rigidbody selfRb;

    public GameObject powerupRepulsionIndicator;
    private bool isRepulsionPowerUpActive;
    private int powerupBounceRemaining = 0;

    public AudioClip thudAudioClip;
    public AudioClip pickUpPowerUpRepulsionClip;
    public AudioClip pickUpPowerUpBounceClip;
    public AudioClip bounceClip;
    public AudioClip repulsionClip;
    private AudioSource audioPlayer;

    private void Start()
    {
        // default naturalTilt value. Was empirically determined.
        naturalTilt = new Vector3(0, -0.8f, -0.7f);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        selfRb = GetComponent<Rigidbody>();
        audioPlayer = GetComponent<AudioSource>();
    }

    public void CalibrateNaturalTilt()
    {
        naturalTilt = Input.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.isGameActive)
        {
            if (transform.position.y < -10)
            {
                _gameManager.GameOver();
            }

            Vector3 rawAcceleration = Input.acceleration;
            // device tilted forwards beyond flatness. This is indicated by the y value becoming positive.
            if (rawAcceleration.y > 0)
            {
                // in such a case, the magnitude of z actually decreases instead of increasing like what we'd expect.
                // we'll need to then adjust the z value to increase instead of decreaasing.
                float compensatedZ = -(2 + rawAcceleration.z);
                rawAcceleration = new Vector3(rawAcceleration.x, 0, compensatedZ);
            }

            // compensating for the natural tilt
            Vector3 compensatedAcceleration = rawAcceleration - naturalTilt;
            Vector3 finalAcceleration = new Vector3(compensatedAcceleration.x, 0, -compensatedAcceleration.z);
            selfRb.AddForce(finalAcceleration * 50, ForceMode.Force);
            selfRb.velocity = Vector3.ClampMagnitude(selfRb.velocity, 8);

            powerupRepulsionIndicator.transform.position = transform.position + new Vector3(0, -0.2f, 0);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            KnockBackAll(other);
            audioPlayer.PlayOneShot(thudAudioClip, 1.0f);
            if (isRepulsionPowerUpActive)
                KnockBackEnemyWithRepulsionPowerUp(other);
        }
    }

    void KnockBackAll(Collision other)
    {
        Vector3 midPos = (transform.position + other.transform.position) / 2;
        selfRb.AddExplosionForce(20, midPos, 5, 0, ForceMode.Impulse);
        other.rigidbody.AddExplosionForce(70, midPos, 5, 0, ForceMode.Impulse);
    }

    private void KnockBackEnemyWithRepulsionPowerUp(Collision other)
    {
        Rigidbody enemyRb = other.rigidbody;
        Vector3 knockBackVectorRaw = enemyRb.position - transform.position;
        Vector3 knockBackVectorNormalised = knockBackVectorRaw.normalized;
        enemyRb.AddForce(knockBackVectorNormalised * 150, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerupRepulsion"))
        {
            if (isRepulsionPowerUpActive)
            {
                // don't consume the powerup
                return;
            }

            Destroy(other.gameObject);
            StartCoroutine(PowerupRepulsionCountDown());
            isRepulsionPowerUpActive = true;
            powerupRepulsionIndicator.SetActive(true);
            audioPlayer.PlayOneShot(pickUpPowerUpRepulsionClip);
        }

        if (other.CompareTag("PowerupBounce"))
        {
            powerupBounceRemaining += 1;
            _gameManager.CollectedBouncePowerUp(powerupBounceRemaining);
            audioPlayer.PlayOneShot(pickUpPowerUpBounceClip);
            Destroy(other.gameObject);
        }
    }

    private IEnumerator PowerupRepulsionCountDown()
    {
        audioPlayer.PlayOneShot(repulsionClip, 0.2f);
        yield return new WaitForSeconds(8);
        isRepulsionPowerUpActive = false;
        powerupRepulsionIndicator.SetActive(false);
    }

    public void Bounce()
    {
        if (powerupBounceRemaining != 0)
        {
            selfRb.AddForce(Vector3.up * 40, ForceMode.Impulse);
            audioPlayer.PlayOneShot(bounceClip);
            powerupBounceRemaining -= 1;
            _gameManager.UsedBouncePowerUp(powerupBounceRemaining);
        }
    }

}
