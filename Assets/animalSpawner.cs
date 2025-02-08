using UnityEngine;

public class animalSpawner : MonoBehaviour
{
    public bool isAnimalInThere;
    public GameObject[] animalPrefabs;

    private void Start()
    {
        gachaAnimalIfInThare();
    }

    public void CheckIfAnimalInThere()
    {
        if (!isAnimalInThere) return;
        int randomIndex = Random.Range(0, animalPrefabs.Length); // Pilih hewan secara acak
        Instantiate(animalPrefabs[randomIndex], transform.position, Quaternion.identity);
        isAnimalInThere = false;
    }

    public void gachaAnimalIfInThare()
    {
        float chance = Random.Range(0f, 1f); // Menghasilkan angka antara 0.0 dan 1.0

        if (chance <= 0.4f) // 30% kemungkinan untuk spawn hewan
        {
            isAnimalInThere = true;
        }
        else
        {
            isAnimalInThere = false;
        }
    }
}