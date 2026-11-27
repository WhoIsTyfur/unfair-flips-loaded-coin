using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

[BepInPlugin("tyfur.unfairflips.autoflip", "Unfair Flips AutoFlip", "1.0.0")]
public class AutoFlip : BaseUnityPlugin
{
    const int EndingArrangement = 4;
    const int ShopArrangement = 2;

    static readonly FieldInfo FlipBlocked = typeof(CoinFlip).GetField("flipBlocked", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo TimeSinceMoved = typeof(CoinFlip).GetField("timeSinceMoved", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo IsFlipping = typeof(CoinFlip).GetField("isFlipping", BindingFlags.Instance | BindingFlags.NonPublic);

    ConfigEntry<bool> autoFlip, autoBuy, saveForTop;
    ConfigEntry<string> priority;
    ConfigEntry<KeyCode> flipKey, buyKey;

    CoinFlip coin;
    PlayerMoney money;
    PanelManager panels;
    ShopButton[] shop = Array.Empty<ShopButton>();
    float nextBuy;

    void Awake()
    {
        autoFlip = Config.Bind("General", "AutoFlip", true, "Flip the coin whenever it is ready.");
        autoBuy = Config.Bind("General", "AutoBuy", true, "Buy upgrades once the shop is open.");
        priority = Config.Bind("General", "Priority", "FlipBaseWorth,HeadsChance,FlipMultiplier,FlipTime",
            "Upgrade order. FlipBaseWorth = Coin Value, FlipMultiplier = Combo.");
        saveForTop = Config.Bind("General", "SaveForTop", false,
            "true: only buy the top unfinished upgrade, saving up for it. false: buy the highest-priority one you can afford.");
        flipKey = Config.Bind("Keys", "ToggleFlip", KeyCode.F8, "Toggle auto flip (game must be focused).");
        buyKey = Config.Bind("Keys", "ToggleBuy", KeyCode.F9, "Toggle auto buy (game must be focused).");
        Application.runInBackground = true;
    }

    void Update()
    {
        // The game can reset it on focus loss; keep it on so flipping continues while tabbed out.
        Application.runInBackground = true;

        if (Input.GetKeyDown(flipKey.Value)) Toggle(autoFlip, "Auto flip");
        if (Input.GetKeyDown(buyKey.Value)) Toggle(autoBuy, "Auto buy");

        if (coin == null)
        {
            coin = FindObjectOfType<CoinFlip>();
            money = FindObjectOfType<PlayerMoney>();
            panels = FindObjectOfType<PanelManager>();
            shop = FindObjectsOfType<ShopButton>(true);
            if (coin == null) return;
        }

        int arrangement = panels != null ? panels.GetCurrentArrangement() : 0;
        if (autoFlip.Value && arrangement != EndingArrangement) TryFlip();
        if (autoBuy.Value && arrangement == ShopArrangement && Time.unscaledTime >= nextBuy) TryBuy();
    }

    void Toggle(ConfigEntry<bool> entry, string label)
    {
        entry.Value = !entry.Value;
        Logger.LogInfo($"{label}: {(entry.Value ? "on" : "off")}");
    }

    // Same gate the game uses for the Space key, plus the flip button being enabled.
    void TryFlip()
    {
        if ((bool)IsFlipping.GetValue(coin) || (bool)FlipBlocked.GetValue(coin)) return;
        if ((float)TimeSinceMoved.GetValue(coin) <= 1f) return;
        if (coin.flipButton != null && (!coin.flipButton.enabled || !coin.flipButton.gameObject.activeInHierarchy)) return;
        coin.DoFlip();
    }

    void TryBuy()
    {
        foreach (var name in priority.Value.Split(',').Select(s => s.Trim()))
        {
            if (!Enum.TryParse(name, true, out ShopButton.UpgradeType type)) continue;
            var button = shop.FirstOrDefault(b => b != null && b.upgradeType == type);
            if (button == null) continue;

            int level = button.GetShopLevel();
            if (level >= button.costs.Length) continue;

            if (money.moneyInCents >= button.costs[level])
            {
                // onClick runs Buy() plus the upgrade effect wired in the inspector.
                button.GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
                Logger.LogInfo($"Bought {type} level {level + 1}");
                nextBuy = Time.unscaledTime + 0.25f;
                return;
            }
            if (saveForTop.Value) return;
        }
    }
}
