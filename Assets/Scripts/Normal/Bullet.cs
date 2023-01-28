using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] SpriteRenderer _bulletFace, _bulletFade;

    BulletData _data;
    Coroutine _disableBullet;
    int _penetrationIndex;

    public BulletData Data { get => _data; }

    public void Init(BulletData data, Vector3 direction)
    {
        _data = data;

        _penetrationIndex = _data.Penetration;

        _rb.velocity = direction * _data.Speed;

        transform.localScale = Vector3.one;
        _bulletFade.DOFade(1, 0);
        _bulletFace.DOFade(1, 0);

        _disableBullet = StartCoroutine(DisableBullet());
    }

    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(ScaleBulletAndDisable());
    }

    IEnumerator ScaleBulletAndDisable()
    {
        _rb.velocity = Vector2.zero;

        transform.DOScale(new Vector3(2, 2, 2), .3f);
        _bulletFace.DOFade(0, .3f);
        _bulletFade.DOFade(0, .3f);

        _rb.simulated = false;

        yield return new WaitForSeconds(.3f);

        gameObject.SetActive(false);
    }

    public void Collision()
    {
        _penetrationIndex--;

        if (_penetrationIndex <= 0)
        {
            StartCoroutine(ScaleBulletAndDisable());
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 20)
        {
            EnemyController tempEnemy = collision.gameObject.GetComponent<EnemyController>();

            tempEnemy.Rb.AddForce(_rb.velocity);

            tempEnemy.TakeDamage(_data.Damage);

            Collision();
        }
    }
}
