using UnityEngine;

namespace EnemyScripts
{
    public class BeegColumnAttack : ShootBehaviour
    {
        [SerializeField] private float arcAngle;
        [SerializeField] private int numColumns;
        [SerializeField] private float maxColumnDistance;
        
        
        public override void Shoot(Transform shootPoint, Enemy enemy)
        {
            var dir = enemy.target.position - shootPoint.position;
            for (var c = 0; c < numColumns; c++)
            {
                var columnDir = (Quaternion.Euler(0f, 0f, -arcAngle / 2 + arcAngle * c / (numColumns-1)) * dir).normalized;
                var endPoint = shootPoint.position + columnDir * maxColumnDistance;
                var hit = Physics2D.Raycast(shootPoint.position, columnDir, maxColumnDistance,
                   LayerMask.GetMask("Block", "Default"));
                if (hit)
                    endPoint = hit.point;
                var columnAttack = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity).GetComponent<ColumnAttack>();
                columnAttack.destination = endPoint;
                columnAttack.speed = bulletSpeed;
                columnAttack.damage = damage;
            }
        }
    }
}