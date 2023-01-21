using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hook : MonoBehaviour
{
    //delegete nedir? Metot adý ve parametresi metot imzasý olarak adlandýrýlýr.
    //Bellekte bulunan metotlara eriþebilmek için metot imzasýnýn bilinmesi gerekir.
    //Bu metot imzasý C# içerisinde yer alan delegate ile tanýmlanýr.
    //pointer gibi düþünebiliriz
    public Transform hookedTransfrom;

    private Camera camera;
    private Collider2D coll;

    private int length;
    private int strength;
    private int fishCount;
    private bool canMove;

    private List<Fish> hookedFishes;


    private Tweener cameraTween;

    //List<fish>

    private void Awake()
    {
        camera = Camera.main;
        coll = GetComponent<Collider2D>();
        hookedFishes = new List<Fish>();
    }
  
    void Update()
    {
        if(canMove && Input.GetMouseButton(0))
        {
            Vector3 vector = camera.ScreenToWorldPoint(Input.mousePosition);//mouse ile nereye týklarsak oraya hareket edecek 
            Vector3 position = transform.position;// pozisyon oluþturduk ve týklanan yere göre dönüþtür dedik
            position.x = vector.x;// hareketi x ekseninde yapýyoruz
            transform.position = position;
        }
        
    }

    public void StartFishing()
    {
        length = -50;
        strength = 3;
        fishCount = 0;
        float time = (-length) * 0.1f;//eksi çünkü olta aþaðý dogru gitmeli

        cameraTween = camera.transform.DOMoveY(length, 1 + time * 0.25f, false).OnUpdate(delegate
              {
                  if (camera.transform.position.y <= -11)
                  {
                      transform.SetParent(camera.transform);
                  }

              }
        ).OnComplete(delegate
        {
            coll.enabled = true;// balýklarý tutabilmek için
            cameraTween = camera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            { 
               if(camera.transform.position.y>=-25f)
                {
                    StopFishing();
                }
            });
        });

        coll.enabled = false;
        canMove = true;
        hookedFishes.Clear();

    }

    void StopFishing()
    {
        canMove = false;
        cameraTween.Kill(false);
        cameraTween = camera.transform.DOMoveY(0, 2, false).OnUpdate(delegate
            {
                if (camera.transform.position.y >= -11)
                {
                    transform.SetParent(null);
                    transform.position = new Vector2(transform.position.x, -6);
                }
            }).OnComplete(delegate
            {
                transform.position = Vector2.down * 6;
                coll.enabled = true;
                int num = 0;
                for(int i=0;i<hookedFishes.Count;i++)
                {
                    hookedFishes[i].transform.SetParent(null);
                    hookedFishes[i].ResetFish();
                    num += hookedFishes[i].Type.price;
                }
            });
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Fish") && fishCount != strength)
        {
            fishCount++;
            Fish component = target.GetComponent<Fish>();
            component.Hooked();
            hookedFishes.Add(component);
            target.transform.SetParent(transform);
            target.transform.position = hookedTransfrom.position;
            target.transform.rotation = hookedTransfrom.rotation;
            target.transform.localScale = Vector3.one;

            target.transform.DOShakeRotation(5, Vector3.forward * 45, 10, 90, false).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {
                target.transform.rotation = Quaternion.identity;
            });
            if (fishCount == strength)
                StopFishing();
        }
    }
}
