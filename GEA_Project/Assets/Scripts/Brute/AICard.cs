using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UI;

public class AICard : MonoBehaviour
{
    Coroutine runningRoutine;

    [SerializeField] Text text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickButton()
    {
        if (runningRoutine != null)
        {
            Debug.Log("[Brute] 이미 실행중입니다.");
            return;
        }
        runningRoutine = StartCoroutine(BruteForceRoutine());
    }

    IEnumerator BruteForceRoutine()
    {
        int quickShot = 2;
        int heavyShot = 2;
        int multiShot = 1;
        int tripleShot = 1;

        int quickCard = 0;
        int heavyCard = 0;
        int multiCard = 0;
        int tripleCard = 0;

        int bestDamage = 0;
        int bestCost = 15;

        for (int q = 0; q <= quickShot; q++)
        {
            for (int h = 0; h <= heavyShot; h++)
            {
                for (int m = 0; m <= multiShot; m++)
                {
                    for (int t = 0; t <= tripleShot; t++)
                    {
                        int sumCost = q * 2 + h * 3 + m * 5 + t * 7;
                        if (sumCost > bestCost)
                            continue;

                        int sumDamage = q * 6 + h * 8 + m * 16 + t * 24;

                        if (sumDamage > bestDamage && sumCost <= bestCost)
                        {
                            bestDamage = sumDamage;
                            quickCard = q;
                            heavyCard = h;
                            multiCard = m;
                            tripleCard = t;
                        }
                    }
                }
            }

            string result = ($"최대 데미지 {bestDamage}, 사용 코스트 {bestCost}" +
                $"\n사용한 카드\n퀵:{quickCard}장\n헤비:{heavyCard}장\n멀티:{multiCard}장\n트리플:{tripleCard}장");

            text.text = result.ToString();

            runningRoutine = null;
            yield break;
        }
    }
}
