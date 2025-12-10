using System.Text;
using UnityEngine;
using UnityEngine.UI;

enum Strategy
{
    BruteForce,
    WasteMin,
    BestEfficiency,
    BigExpFirst
}

public class GreedySample : MonoBehaviour
{
    [Header("강화 레벨")]
    public int currentLevel = 1;
    public int targetLevel = 2;

    [Header("UI 참조 (없으면 비워둬도 됨)")]
    public Text levelText;
    public Text requiredExpText;
    public Slider expSlider;
    public Text resultText;
    public Image image;
    public Sprite[] images;

    Strategy currentStrategy = Strategy.BruteForce;

    bool isMaxLevel = false;

    [System.Serializable]
    public class UpgradeStone
    {
        public string name;
        public int exp;
        public int price;

        public UpgradeStone(string name, int exp, int price)
        {
            this.name = name;
            this.exp = exp;
            this.price = price;
        }
    }

    public class PurchaseResult
    {
        public int[] counts;
        public int totalExp;
        public int totalPrice;

        public PurchaseResult(int stoneCount)
        {
            counts = new int[stoneCount];
        }
    }

    UpgradeStone[] stones;

    void Awake()
    {
        // 재료 4개 세팅
        stones = new UpgradeStone[4];
        stones[0] = new UpgradeStone("강화석 소", 3, 8);
        stones[1] = new UpgradeStone("강화석 중", 5, 12);
        stones[2] = new UpgradeStone("강화석 대", 12, 30);
        stones[3] = new UpgradeStone("강화석 특대", 20, 45);

        UpdateUI();
    }

    int RequiredExpForLevel(int level)
    {
        return 8 * level * level;
    }

    int GetRequiredExp()
    {
        return RequiredExpForLevel(targetLevel);
    }

    void UpdateUI()
    {
        int required = GetRequiredExp();

        if (currentLevel == images.Length)
        {
            levelText.text = $"최대레벨입니다! ({currentLevel})";
            isMaxLevel = true;
        }
        else
        {
            levelText.text = $"+{currentLevel} -> +{targetLevel}";
            image.sprite = images[currentLevel - 1];
            requiredExpText.text = $"필요 경험치 {required}/{required}";
        }

        if (expSlider != null)
        {
            expSlider.minValue = 0;
            expSlider.maxValue = required;
            expSlider.value = required;
        }
    }

    void ShowResult(string title, PurchaseResult r)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"[{title}]");

        for (int i = 0; i < stones.Length; i++)
        {
            if (r.counts[i] > 0)
            {
                sb.AppendLine(
                    $"{stones[i].name} (exp{stones[i].exp}) x {r.counts[i]}"
                );
            }
        }

        sb.AppendLine($"총 가격   : {r.totalPrice} gold");

        if (resultText != null)
            resultText.text = sb.ToString();
    }

    public void UpgradeBtn()
    {
        if (isMaxLevel) return;

        int required = GetRequiredExp();
        PurchaseResult result = null;
        string title = "";

        switch (currentStrategy)
        {
            case Strategy.BruteForce:
                result = CalcBruteForce(required);
                title = "Brute Force";
                break;

            case Strategy.WasteMin:
                result = CalcGreedyWasteMin(required);
                title = "경험치 낭비 최소";
                break;

            case Strategy.BestEfficiency:
                result = CalcGreedyBestEfficiency(required);
                title = "골드 효율 최대";
                break;

            case Strategy.BigExpFirst:
                result = CalcGreedyBigExpFirst(required);
                title = "exp 큰 거부터";
                break;
        }

        if (result != null)
            ShowResult(title, result);

        currentLevel = targetLevel;
        targetLevel++;
        UpdateUI();
    }

    public void OnClickSelectBruteForce() => currentStrategy = Strategy.BruteForce;

    public void OnClickSelectGreedyWasteMin() => currentStrategy = Strategy.WasteMin;

    public void OnClickSelectGreedyBestEfficiency() => currentStrategy = Strategy.BestEfficiency;

    public void OnClickSelectGreedyBigExpFirst() => currentStrategy = Strategy.BigExpFirst;

    PurchaseResult CalcBruteForce(int requiredExp)
    {
        PurchaseResult best = new PurchaseResult(stones.Length);
        int bestPrice = int.MaxValue;
        int bestExp = int.MaxValue;

        int max0 = requiredExp / stones[0].exp + 2;
        int max1 = requiredExp / stones[1].exp + 2;
        int max2 = requiredExp / stones[2].exp + 2;
        int max3 = requiredExp / stones[3].exp + 2;

        for (int a = 0; a <= max0; a++)
            for (int b = 0; b <= max1; b++)
                for (int c = 0; c <= max2; c++)
                    for (int d = 0; d <= max3; d++)
                    {
                        int totalExp =
                            a * stones[0].exp +
                            b * stones[1].exp +
                            c * stones[2].exp +
                            d * stones[3].exp;

                        if (totalExp < requiredExp) continue;

                        int totalPrice =
                            a * stones[0].price +
                            b * stones[1].price +
                            c * stones[2].price +
                            d * stones[3].price;

                        bool better =
                            (totalPrice < bestPrice) ||
                            (totalPrice == bestPrice && totalExp < bestExp);

                        if (better)
                        {
                            bestPrice = totalPrice;
                            bestExp = totalExp;

                            best.counts[0] = a;
                            best.counts[1] = b;
                            best.counts[2] = c;
                            best.counts[3] = d;
                            best.totalExp = totalExp;
                            best.totalPrice = totalPrice;
                        }
                    }

        return best;
    }

    PurchaseResult CalcGreedyWasteMin(int requiredExp)
    {
        PurchaseResult result = new PurchaseResult(stones.Length);
        int curExp = 0;
        int curPrice = 0;

        while (curExp < requiredExp)
        {
            int bestIndex = -1;
            int bestRemain = int.MaxValue;
            int bestOver = int.MaxValue;

            for (int i = 0; i < stones.Length; i++)
            {
                int afterExp = curExp + stones[i].exp;
                int remain = requiredExp - afterExp;

                if (remain >= 0)
                {
                    if (remain < bestRemain)
                    {
                        bestRemain = remain;
                        bestIndex = i;
                    }
                }
                else
                {
                    int over = -remain;
                    if (bestRemain == int.MaxValue && over < bestOver)
                    {
                        bestOver = over;
                        bestIndex = i;
                    }
                }
            }

            if (bestIndex == -1) break;

            curExp += stones[bestIndex].exp;
            curPrice += stones[bestIndex].price;
            result.counts[bestIndex]++;
        }

        result.totalExp = curExp;
        result.totalPrice = curPrice;
        return result;
    }

    PurchaseResult CalcGreedyBestEfficiency(int requiredExp)
    {
        PurchaseResult result = new PurchaseResult(stones.Length);

        int bestIndex = 0;
        double bestRatio = 0.0;

        for (int i = 0; i < stones.Length; i++)
        {
            double ratio = (double)stones[i].exp / stones[i].price;
            if (ratio > bestRatio)
            {
                bestRatio = ratio;
                bestIndex = i;
            }
        }

        int curExp = 0;
        int curPrice = 0;
        while (curExp < requiredExp)
        {
            curExp += stones[bestIndex].exp;
            curPrice += stones[bestIndex].price;
            result.counts[bestIndex]++;
        }

        result.totalExp = curExp;
        result.totalPrice = curPrice;
        return result;
    }

    PurchaseResult CalcGreedyBigExpFirst(int requiredExp)
    {
        PurchaseResult result = new PurchaseResult(stones.Length);

        int bestIndex = 0;
        int bestExp = 0;
        for (int i = 0; i < stones.Length; i++)
        {
            if (stones[i].exp > bestExp)
            {
                bestExp = stones[i].exp;
                bestIndex = i;
            }
        }

        int curExp = 0;
        int curPrice = 0;
        while (curExp < requiredExp)
        {
            curExp += stones[bestIndex].exp;
            curPrice += stones[bestIndex].price;
            result.counts[bestIndex]++;
        }

        result.totalExp = curExp;
        result.totalPrice = curPrice;
        return result;
    }
}
