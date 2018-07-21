using UnityEngine;

public class SpawnParticlesController : MonoBehaviour
{
    // Chunks seemed to occupy too much screen space so I 
    // removed them (uncomment deprecated code and deactive childObject
    // to bring it back)

    // Scaling [Particles] object scales the burst partices 
    // (chunks scaling acts weird cause with tail tracing)

    // To use just have an instance of the particles somewhere in the scene
    // and then call one of the functions below (by design there would 
    // only exist one "burst" at the time tho instanciating [Particles] gameobject
    // would bypass that


    public ParticleSystem burst;
    //public ParticleSystem[] chunks;

    public void PlayParticles(Vector3 position, float scale)
    {
        // If any issues with position replace 
        // localPosition with position (world pos)

        this.transform.localPosition = position;
        this.transform.localScale = Vector3.one * scale;
        burst.Play();
    }

    public void StopParticles()
    {
        burst.Stop();
        burst.Clear();
    }

    #region Deprecated
    //public ParticleSystem burst;
    ////public ParticleSystem[] chunks;

    //public void PlayParticles(Vector3 position, float scale)
    //{
    //    // If any issues with position replace 
    //    // localPosition with position (world pos)
    //    this.transform.localPosition = position;
    //    this.transform.localScale = Vector3.one * scale;
    //    //burst.startSize = scale;

    //    burst.Play();
    //    //for (int i = 0; i < chunks.Length; i++)
    //    //{
    //    //    chunks[i].startSize = scale;
    //    //    chunks[i].Play();
    //    //}
    //}

    //public void StopParticles()
    //{
    //    burst.Stop();
    //    burst.Clear();

    //    //for (int i = 0; i < chunks.Length; i++)
    //    //{
    //    //    chunks[i].Stop();
    //    //    chunks[i].Clear();
    //    //}
    //}
    #endregion
}

