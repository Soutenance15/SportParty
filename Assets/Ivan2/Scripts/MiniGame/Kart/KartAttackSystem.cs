using UnityEngine;
using UnityEngine.Video;

public class KartAttackSystem : MonoBehaviour
{
    public Rigidbody2D rb;

    // Shoot Bomb
    public GameObject bombPrefab; // drag & drop la prefab de bombe dans l'inspecteur
    public Transform firePoint; // point de spawn devant le kart

    int currentAmmoBomb = 0;
    int maxBomb = 3;

    public void InitFireBomb(Transform firepoint)
    {
        this.firePoint = firepoint;
    }

    public void InitRb(Rigidbody2D rb)
    {
        this.rb = rb;
    }

    void OnEnable()
    {
        AmmoBomb.OnAmmoEnter += TakeBomb;
    }

    void OnDisable()
    {
        AmmoBomb.OnAmmoEnter -= TakeBomb;
    }

    void Update()
    {
        // Lancer bombe avec espace (ou touche au choix)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBomb();
        }
        // Simule attrape bombe
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     TakeBomb();
        // }
    }

    void FireBomb()
    {
        if (ShootLoaded())
        {
            if (null != bombPrefab && null != firePoint)
            {
                Debug.Log("FireBomb");
                // Instancie la bombe devant le kart, dans sa rotation actuelle
                GameObject bombObject = Instantiate(
                    bombPrefab,
                    firePoint.position,
                    transform.rotation
                );
                Bomb bomb = bombObject.GetComponent<Bomb>();
                if (null != bomb && rb != null)
                {
                    bomb.state = Bomb.State.Move;
                    bomb.speed += rb.linearVelocity.magnitude;
                    currentAmmoBomb -= 1;
                    Debug.Log("NB Bomb : + " + currentAmmoBomb);
                }
            }
        }
    }

    bool ShootLoaded()
    {
        return currentAmmoBomb > 0;
    }

    // Attention La dessous il faut certainement un fichier manager
    // il recupere les evenements et appelle la bonne attaque pour le bon objet
    // pour aller plus vite je fais directement ici
    // Mais c'est pas bien
    // Plus tard
    // -> Attack System
    // -> Input System
    // -> Ammo System
    // -> KartSystem qui fait le lien avec les differents kart, à créer
    void TakeBomb(KartAttackSystem kartAttack)
    {
        if (kartAttack.currentAmmoBomb < kartAttack.maxBomb - 1)
        {
            kartAttack.currentAmmoBomb += 1;
        }
        Debug.Log("NB Bomb : + " + kartAttack.currentAmmoBomb);
    }
}
