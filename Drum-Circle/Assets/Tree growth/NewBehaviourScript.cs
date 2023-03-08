using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public list<MeshRenderer> growTreesMesh;
    public float timegrow= 5;
    public float refreshRate= 0.05f;
    [Range(0,1)]
    public float miniGrow=0.2f;
    [Range(0,1)]
    public float maxiGrow=0.97f;



    private List<Material> growTreesMaterials = new list<Material>();
    private bool fullyGrown;

    void Start()
    {
        for(int i=0; i<growTreesMesh.Count; i++){
            for(int j=0; i<growTreesMesh[i].materials.lenght; j++){
                for(int j=0; i<growTreesMesh[i].materials.lenght; j++){
                    growTreesMesh[i].materials[j].SetFloat("grow_", miniGrow);
                    growTreesMaterials.Add(growTreesMesh[i].materials[j]);
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey){
            for(int i=0; i<growTreesMaterials.Count;i++){
                StartCoroutine(GrowTrees(growTreesMaterials[i]));
            }
        }
    }
}

IEnumerator GrowTrees (Material mat){
    float growValue= mat.GetFloat("grow_");

    if(!fullyGrown){
        while(growValue<maxiGrow){
            growValue+=1/(timegrow/refreshRate);
            mat.SetFloat("grow_", growValue);

            yield return new WaitFroSeconds (refreshRate);
        }

    }
    else{
        while(growValue>miniGrow){
            growValue-=1/(timegrow/refreshRate);
            mat.SetFloat("grow_", growValue);

            yield return new WaitFroSeconds (refreshRate);
        }
        
    }
    if (growValue>=maxiGrow){
        fullyGrown=true;
    }
    else{
        fullyGrown=false;
    }

}
