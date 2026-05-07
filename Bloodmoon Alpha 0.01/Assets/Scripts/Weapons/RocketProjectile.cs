using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 5f;

    [Header("Explosion")]
    public float explosionRadius = 5f;
    public float explosionForce = 10f;

    private Rigidbody rb;

    private float damage;
    private float knockback;

    private Vector3 lastPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 direction, float dmg, float kb)
    {
        damage = dmg;
        knockback = kb;

        rb.useGravity = false; // 🚀 no drop
        rb.linearVelocity = direction * speed;
    }

    private void Update()
    {
        lastPosition = transform.position;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
            transform.forward = rb.linearVelocity.normalized;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode(collision.contacts[0].point, collision.collider);
    }

    void Explode(Vector3 position, Collider directHit)
    {
        // 🔴 DEBUG: draw explosion sphere in-game (for 2 seconds)
        DrawDebugSphere(position, explosionRadius, 2f, Color.red);

        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);

        foreach (Collider hit in hits)
        {
            IDamageable dmgTarget = hit.GetComponentInParent<IDamageable>();
            if (dmgTarget == null) continue;

            float distance = Vector3.Distance(position, hit.transform.position);

            float finalDamage;

            // 🎯 Direct hit = full damage
            if (hit == directHit)
            {
                finalDamage = damage;
            }
            else
            {
                // 🌊 AOE falloff (max 60%)
                float falloff = 1f - (distance / explosionRadius);
                finalDamage = damage * 0.6f * Mathf.Clamp01(falloff);
            }

            Vector3 dir = (hit.transform.position - position).normalized;
            Vector3 knock = dir * knockback;

            dmgTarget.TakeDamage(finalDamage, knock);
        }

        // 💥 Physics explosion force
        foreach (Collider hit in hits)
        {
            Rigidbody r = hit.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(explosionForce, position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    // 🧠 Draw runtime debug sphere using lines
    void DrawDebugSphere(Vector3 center, float radius, float duration, Color color)
    {
        int segments = 20;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * Mathf.PI * 2;
            float angle2 = ((i + 1) / (float)segments) * Mathf.PI * 2;

            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;

            Vector3 p3 = center + new Vector3(0, Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;
            Vector3 p4 = center + new Vector3(0, Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;

            Vector3 p5 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 p6 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Debug.DrawLine(p1, p2, color, duration);
            Debug.DrawLine(p3, p4, color, duration);
            Debug.DrawLine(p5, p6, color, duration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastPosition, explosionRadius);
    }
}