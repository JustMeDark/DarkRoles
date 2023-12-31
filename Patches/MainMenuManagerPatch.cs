using TMPro;
using System;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using static DarkRoles.Translator;
using static DarkRoles.CredentialsPatch;

namespace DarkRoles
{
    [HarmonyPatch(typeof(MainMenuManager))]
    public static class MainMenuManagerPatch
    {
        private static PassiveButton template;
        private static PassiveButton gitHubButton;
        private static PassiveButton discordButton;
        private static PassiveButton websiteButton;

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate)), HarmonyPostfix]
        public static void Postfix(MainMenuManager __instance)
        {
            if (__instance == null) return;
            __instance.playButton.transform.gameObject.SetActive(Options.IsLoaded);
        }

        [HarmonyPatch(nameof(MainMenuManager.Start)), HarmonyPostfix, HarmonyPriority(Priority.Normal)]
        public static void StartPostfix(MainMenuManager __instance)
        {
            if (template == null) template = __instance.quitButton;

            // FPS
            //Application.targetFrameRate = Main.unk.Value ? 165 : 60;

            __instance.screenTint.gameObject.transform.localPosition += new Vector3(1000f, 0f);
            __instance.screenTint.enabled = false;
            __instance.rightPanelMask.SetActive(true);
            // The background texture (large sprite asset)
            __instance.mainMenuUI.FindChild<SpriteRenderer>("BackgroundTexture").transform.gameObject.SetActive(false);
            // The glint on the Among Us Menu
            __instance.mainMenuUI.FindChild<SpriteRenderer>("WindowShine").transform.gameObject.SetActive(false);
            __instance.mainMenuUI.FindChild<Transform>("ScreenCover").gameObject.SetActive(false);

            GameObject leftPanel = __instance.mainMenuUI.FindChild<Transform>("LeftPanel").gameObject;
            GameObject rightPanel = __instance.mainMenuUI.FindChild<Transform>("RightPanel").gameObject;
            rightPanel.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            GameObject maskedBlackScreen = rightPanel.FindChild<Transform>("MaskedBlackScreen").gameObject;
            maskedBlackScreen.GetComponent<SpriteRenderer>().enabled = false;
            //maskedBlackScreen.transform.localPosition = new Vector3(-3.345f, -2.05f); //= new Vector3(0f, 0f);
            maskedBlackScreen.transform.localScale = new Vector3(7.35f, 4.5f, 4f);

            __instance.mainMenuUI.gameObject.transform.position += new Vector3(-0.2f, 0f);

            leftPanel.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            leftPanel.gameObject.FindChild<SpriteRenderer>("Divider").enabled = false;
            leftPanel.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r.name == "Shine").ForEach(r => r.enabled = false);

            __instance.playButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.playButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorPlayButton = __instance.playButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorPlayButton2 = __instance.playButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.playButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorPlayButton * 0.5f;
            __instance.playButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorPlayButton2 * 0.5f;
            __instance.playButton.activeTextColor = Color.white;
            __instance.playButton.inactiveTextColor = Color.white;

            __instance.inventoryButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.inventoryButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorInventoryButton = __instance.inventoryButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorInventoryButton2 = __instance.inventoryButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.inventoryButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorInventoryButton * 0.5f;
            __instance.inventoryButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorInventoryButton2 * 0.5f;
            __instance.inventoryButton.activeTextColor = Color.white;
            __instance.inventoryButton.inactiveTextColor = Color.white;

            __instance.shopButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.shopButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorShopButton = __instance.shopButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorShopButton2 = __instance.shopButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.shopButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorShopButton * 0.5f;
            __instance.shopButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorShopButton2 * 0.5f;
            __instance.shopButton.activeTextColor = Color.white;
            __instance.shopButton.inactiveTextColor = Color.white;

            __instance.newsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorNewsButton = __instance.newsButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorNewsButton2 = __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.newsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorNewsButton * 0.5f;
            __instance.newsButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorNewsButton2 * 0.5f;
            __instance.newsButton.activeTextColor = Color.white;
            __instance.newsButton.inactiveTextColor = Color.white;

            __instance.myAccountButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.myAccountButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorMyAccount = __instance.myAccountButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorMyAccount2 = __instance.myAccountButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.myAccountButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorMyAccount * 0.5f;
            __instance.myAccountButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorMyAccount2 * 0.5f;
            __instance.myAccountButton.activeTextColor = Color.white;
            __instance.myAccountButton.inactiveTextColor = Color.white;
            __instance.accountButtons.transform.position += new Vector3(0f, 0f, -1f);

            __instance.settingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.settingsButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorSettingsButton = __instance.settingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorSettingsButton2 = __instance.settingsButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.settingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorSettingsButton * 0.5f;
            __instance.settingsButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorSettingsButton2 * 0.5f;
            __instance.settingsButton.activeTextColor = Color.white;
            __instance.settingsButton.inactiveTextColor = Color.white;

            __instance.quitButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorQuitButton = __instance.quitButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorQuitButton2 = __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.quitButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorQuitButton * 0.5f;
            __instance.quitButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorQuitButton2 * 0.5f;
            __instance.quitButton.activeTextColor = Color.white;
            __instance.quitButton.inactiveTextColor = Color.white;

            __instance.creditsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new Color(.7f, .1f, .7f);
            __instance.creditsButton.activeSprites.GetComponent<SpriteRenderer>().color = new Color(.5f, .1f, .5f);
            Color originalColorCreditsButton = __instance.creditsButton.inactiveSprites.GetComponent<SpriteRenderer>().color;
            Color originalColorCreditsButton2 = __instance.creditsButton.activeSprites.GetComponent<SpriteRenderer>().color;
            __instance.creditsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = originalColorCreditsButton * 0.5f;
            __instance.creditsButton.activeSprites.GetComponent<SpriteRenderer>().color = originalColorCreditsButton2 * 0.5f;
            __instance.creditsButton.activeTextColor = Color.white;
            __instance.creditsButton.inactiveTextColor = Color.white;

            if (template == null) return;


            // GitHub Button
            if (gitHubButton == null)
            {
                gitHubButton = CreateButton(
                    "GitHubButton",
                    new(-3.18f, -2.5f, 1f),
                    new Color(.7f, .1f, .7f, .5f),
                    new Color(.5f, .1f, .5f, .5f),
                    () => Application.OpenURL("https://github.com/sleepyfor/DarkRoles"),
                    "GitHub", new Vector2(2.0f, 0.55f)); //"GitHub"
            }
            gitHubButton.gameObject.SetActive(true);

            // Discord Button
            if (discordButton == null)
            {
                discordButton = CreateButton(
                    "DiscordButton",
                    new(-3.18f, -2.97f, 1f),
                    new Color(.7f, .1f, .7f, .5f),
                    new Color(.5f, .1f, .5f, .5f),
                    () => Application.OpenURL(Main.DiscordInviteUrl),
                    "Join our discord!", new Vector2(2.0f, 0.46f)); //"Discord"
            }
            discordButton.gameObject.SetActive(Main.ShowDiscordButton);

            var howToPlayButton = __instance.howToPlayButton;
            var freeplayButton = howToPlayButton.transform.parent.Find("FreePlayButton");

            if (freeplayButton != null) freeplayButton.gameObject.SetActive(false);

            howToPlayButton.transform.SetLocalX(0);

        }

        private static PassiveButton CreateButton(string name, Vector3 localPosition, Color32 normalColor, Color32 hoverColor, Action action, string label, Vector2? scale = null)
        {
            var button = Object.Instantiate(template, CredentialsPatch.TohLogo.transform);
            button.name = name;
            Object.Destroy(button.GetComponent<AspectPosition>());
            button.transform.localPosition = localPosition;

            button.OnClick = new();
            button.OnClick.AddListener(action);

            var buttonText = button.transform.Find("FontPlacer/Text_TMP").GetComponent<TMP_Text>();
            buttonText.DestroyTranslator();
            buttonText.fontSize = buttonText.fontSizeMax = buttonText.fontSizeMin = 3.5f;
            buttonText.enableWordWrapping = false;
            buttonText.text = label;
            var normalSprite = button.inactiveSprites.GetComponent<SpriteRenderer>();
            var hoverSprite = button.activeSprites.GetComponent<SpriteRenderer>();
            normalSprite.color = normalColor;
            hoverSprite.color = hoverColor;

            var container = buttonText.transform.parent;
            Object.Destroy(container.GetComponent<AspectPosition>());
            Object.Destroy(buttonText.GetComponent<AspectPosition>());
            container.SetLocalX(0f);
            buttonText.transform.SetLocalX(0f);
            buttonText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            var buttonCollider = button.GetComponent<BoxCollider2D>();
            if (scale.HasValue)
            {
                normalSprite.size = hoverSprite.size = buttonCollider.size = scale.Value;
            }

            buttonCollider.offset = new(0f, 0f);

            return button;
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            IEnumerator<TSource> enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action(enumerator.Current);
            }

            enumerator.Dispose();
        }

        public static T FindChild<T>(this MonoBehaviour obj, string name) where T : Object
        {
            string name2 = name;
            return obj.GetComponentsInChildren<T>().First((T c) => c.name == name2);
        }
        public static T FindChild<T>(this GameObject obj, string name) where T : Object
        {
            string name2 = name;
            return obj.GetComponentsInChildren<T>().First((T c) => c.name == name2);
        }
    }
}
