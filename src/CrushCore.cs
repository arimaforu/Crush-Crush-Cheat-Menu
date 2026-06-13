using System; 
using System.Reflection; 
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using System.Linq;
using MelonLoader.Preferences;
using Localization;

[assembly: MelonInfo(typeof(CrushCore), "Crush Crush Cheat Menu", BuildInfo.Version, "You")]
[assembly: MelonGame(null, "CrushCrush")]

public static class BuildInfo
{
    public const string Version = "1.0";
}

public enum MenuLanguage
{
    English,Russian
}

public class CrushCore : MelonMod
{
    private bool showMenu = false;
    private bool autoPhoneSkip = false;
    private Rect windowRect = new Rect(220, 120, 680, 400);
    private string diamondsInput = "10000";
    private string timeScaleInput = "1";
    private string resetBoostInput = "1";
    private float nextPhoneSkip;
    private float deltaTime;
    private int selectedTab = 0;
    private string[] tabs;
    private GUIStyle lineStyle;
    private GUIStyle contentStyle;
    private GUIStyle panelStyle;
    private GUIStyle sideButtonStyle;
    private GUIStyle activeSideButtonStyle;
    private GUIStyle titleStyle;
    private GUIStyle sectionStyle;
    private GUIStyle buttonStyle;
    private GUIStyle closeButtonStyle;
    private GUIStyle fieldStyle;
    private GUIStyle labelStyle;
    private GUIStyle smallLabelStyle;
    private GUIStyle sectionTitleStyle;
    private bool uiReady;
    private MelonPreferences_Category configCategory;
    private MelonPreferences_Entry<float> posX;
    private MelonPreferences_Entry<float> posY;
    private MelonPreferences_Entry<bool> savedAutoPhoneSkip;
    private MelonPreferences_Entry<int> savedTab;
    private string notificationText = "";
    private float notificationTimer = 0f;
    private bool autoPhoneSkipErrorLogged;

public override void OnUpdate()
{
    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    if (Input.GetKeyDown(KeyCode.F1))
    {
        showMenu = !showMenu;
    }
    if (Input.GetKeyDown(KeyCode.F6))
    {
    if (RuntimeInstances.cellphoneInstance != null)
        {
            HarmonyLib.Traverse.Create(RuntimeInstances.cellphoneInstance).Method("Debug_SkipMessage").GetValue();
            ShowNotification( T( "Skipped phone timer", "Телефон пропущен" ));
        }
    }
    if (autoPhoneSkip)
    {
        try
        {
            if (RuntimeInstances.cellphoneInstance != null&& Time.time >= nextPhoneSkip)
            {
                nextPhoneSkip = Time.time + 0.25f;
                HarmonyLib.Traverse.Create(RuntimeInstances.cellphoneInstance).Method("Debug_SkipMessage").GetValue();
            }
        }
        catch (Exception ex)
        {
            if (!autoPhoneSkipErrorLogged)
            {
                LogWarning("Auto phone skip failed", ex);
                autoPhoneSkipErrorLogged = true;
            }
        }
    }
}

private string T(string english, string russian)
{
    try
    {
        return Translations.CurrentLanguage.Value == 8
            ? russian
            : english;
    }
    catch
    {
        return Application.systemLanguage == SystemLanguage.Russian
            ? russian
            : english;
    }
}

private void LogWarning(string action, Exception ex)
{
    MelonLogger.Warning($"[Crush Crush Cheat Menu] {action}: {ex.GetType().Name}: {ex.Message}");
}

public override void OnGUI()
{
    EnsureUI();
    GUI.depth = -1000;

    if (showMenu)
    {
        windowRect = GUI.Window(777,windowRect,DrawMenu,"",panelStyle);
    }
    DrawGlobalNotification();
    if (showMenu)
    {
        GUI.FocusWindow(777);
    }
}

private void EnsureUI()
{
    if (uiReady)
    return;
    Texture2D lineTex = CreateTexture(new Color(0.11f, 0.11f, 0.11f, 1f));
    lineStyle = new GUIStyle();
    lineStyle.normal.background = lineTex;
    Texture2D content = CreateTexture(new Color(0.075f, 0.075f, 0.075f, 1f));
    contentStyle = new GUIStyle(GUI.skin.box);
    contentStyle.normal.background = content;
    contentStyle.border = new RectOffset(0, 0, 0, 0);
    Texture2D background = CreateTexture(new Color(0.012f, 0.012f, 0.012f, 1f) );
    Texture2D sidebar = CreateTexture(new Color(0.045f, 0.045f, 0.045f, 1f) );
    Texture2D button = CreateTexture(new Color(0.105f, 0.105f, 0.105f, 1f));
    Texture2D buttonHover = CreateTexture(new Color(0.155f, 0.155f, 0.155f, 1f));
    Texture2D active = CreateTexture(new Color(0.205f, 0.205f, 0.205f, 1f));
    Texture2D field = CreateTexture(new Color(0.085f, 0.085f, 0.085f, 1f));
    panelStyle = new GUIStyle(GUI.skin.box);
    panelStyle.normal.background = background;
    panelStyle.border = new RectOffset(0, 0, 0, 0);
    sectionStyle = new GUIStyle(GUI.skin.box);
    sectionStyle.normal.background = sidebar;
    sectionStyle.border = new RectOffset(0, 0, 0, 0);
    titleStyle = new GUIStyle(GUI.skin.label);
    sectionTitleStyle = new GUIStyle(GUI.skin.label);
    sectionTitleStyle.fontSize = 14;
    sectionTitleStyle.fontStyle = FontStyle.Bold;
    sectionTitleStyle.normal.textColor = Color.white;
    titleStyle.fontSize = 18;
    titleStyle.fontStyle = FontStyle.Bold;
    titleStyle.normal.textColor = Color.white;
    labelStyle = new GUIStyle(GUI.skin.label);
    labelStyle.fontSize = 12;
    labelStyle.normal.textColor = new Color(0.78f, 0.78f, 0.78f);
    smallLabelStyle = new GUIStyle(GUI.skin.label);
    smallLabelStyle.fontSize = 10;
    smallLabelStyle.normal.textColor = new Color(0.46f, 0.46f, 0.46f);
    sideButtonStyle = new GUIStyle(GUI.skin.box);
    sideButtonStyle.fontSize = 12;
    sideButtonStyle.fontStyle = FontStyle.Bold;
    sideButtonStyle.alignment = TextAnchor.MiddleLeft;
    sideButtonStyle.padding = new RectOffset(18, 8, 0, 0);
    sideButtonStyle.normal.background = button;
    sideButtonStyle.active.background = buttonHover;
    sideButtonStyle.normal.textColor = new Color(0.72f, 0.72f, 0.72f);
    sideButtonStyle.border = new RectOffset(0, 0, 0, 0);
    activeSideButtonStyle = new GUIStyle(sideButtonStyle);
    activeSideButtonStyle.alignment = TextAnchor.MiddleLeft;
    activeSideButtonStyle.padding = new RectOffset(18, 8, 0, 0);
    activeSideButtonStyle.normal.background = active;
    activeSideButtonStyle.normal.textColor = Color.white;
    buttonStyle = new GUIStyle(GUI.skin.box);
    buttonStyle.hover.background = buttonHover;
    buttonStyle.active.background = buttonHover;
    buttonStyle.fontSize = 12;
    buttonStyle.fontStyle = FontStyle.Bold;
    buttonStyle.alignment = TextAnchor.MiddleCenter;
    buttonStyle.wordWrap = true;
    buttonStyle.padding = new RectOffset(6, 6, 3, 3);
    buttonStyle.normal.background = button;
    buttonStyle.hover.textColor = Color.white;
    buttonStyle.active.textColor = Color.white;
    buttonStyle.normal.textColor = Color.white;
    buttonStyle.hover.textColor = Color.white;
    buttonStyle.border = new RectOffset(0, 0, 0, 0);
    closeButtonStyle = new GUIStyle(buttonStyle);
    closeButtonStyle.fontSize = 16;
    closeButtonStyle.fontStyle = FontStyle.Bold;
    closeButtonStyle.alignment = TextAnchor.MiddleCenter;
    fieldStyle = new GUIStyle(GUI.skin.box);
    fieldStyle.fontSize = 13;
    fieldStyle.normal.background = field;
    fieldStyle.alignment = TextAnchor.MiddleLeft;
    fieldStyle.hover.background = fieldStyle.normal.background;
    fieldStyle.active.background = fieldStyle.normal.background;
    fieldStyle.focused.background = fieldStyle.normal.background;
    fieldStyle.normal.textColor = Color.white;
    fieldStyle.focused.textColor = Color.white;
    fieldStyle.padding = new RectOffset(10, 10, 7, 7);
    fieldStyle.border = new RectOffset(0, 0, 0, 0);
    uiReady = true;
}

private void DrawMenu(int id)
{
    GUI.Box( new Rect(0, 0, windowRect.width, 52), "", sectionStyle);
    GUI.Label(new Rect(24, 9, 320, 26),"CRUSH CRUSH CHEAT MENU",titleStyle);
    GUI.Label(new Rect(25, 31, 160, 16),"v" + BuildInfo.Version,smallLabelStyle);
    if (GUI.Button(new Rect(windowRect.width - 39, 14, 22, 22),"×",closeButtonStyle))
    {
        showMenu = false;
    }
    GUI.Box(new Rect(16, 51, windowRect.width - 32, 1), GUIContent.none, lineStyle);
    tabs = new[]
    {
        T("Overview", "Обзор"),T("Currency", "Валюта"),T("Girls", "Девушки"),T("Stats", "Статы")
    };
    DrawSidebar();
    DrawContent();
    GUI.DragWindow(new Rect(0, 0, windowRect.width - 64, 52));
    posX.Value = windowRect.x;
    posY.Value = windowRect.y;
}

private void DrawSidebar()
{
    GUI.Box(new Rect(14, 64, 150, 312),"",sectionStyle);
    for (int i = 0; i < tabs.Length; i++)
    {
        GUIStyle style =
        selectedTab == i
        ? activeSideButtonStyle
        : sideButtonStyle;

        if (GUI.Button(new Rect(26, 86 + i * 48, 126, 36),tabs[i],style))
        {
            selectedTab = i;
            savedTab.Value = i;
        }
    }
}

private void DrawContent()
{
    GUI.Box(new Rect(176, 64, 490, 312),"",contentStyle);
    switch (selectedTab)
    {
        case 0:
            DrawOverview();
            break;
        case 1:
            DrawCurrency();
            break;
        case 2:
            DrawGirls();
            break;
        case 3:
            DrawStats();
            break;   
    }
}

private void DrawOverview()
{
    GUI.Label(new Rect(196, 82, 260, 28),"CRUSH CRUSH CHEAT MENU",titleStyle);
    GUI.Box(new Rect(196, 122, 448, 1),GUIContent.none,lineStyle);
    GUI.Label(new Rect(206, 146, 180, 24),T("SYSTEM", "СИСТЕМА"),sectionTitleStyle);
    GUI.Label(new Rect(206, 181, 185, 22),"FPS : " + Mathf.Round(1f / deltaTime),labelStyle);
    GUI.Box(new Rect(414, 145, 1, 120),GUIContent.none,lineStyle);
    GUI.Label(new Rect(438, 146, 180, 24),T("SESSION", "СЕССИЯ"),sectionTitleStyle);
    GUI.Label(new Rect(438, 207, 190, 22),T("Phone Skip : ", "Пропуск телефона : ") +
        (autoPhoneSkip
            ? T("Enabled", "Включен")
            : T("Disabled", "Выключен")),
        labelStyle);
    GUI.Label(new Rect(438, 233, 190, 22),T("Game : Crush Crush", "Игра : Crush Crush"),labelStyle);
    GUI.Box(new Rect(196, 286, 448, 1),GUIContent.none,lineStyle);
    GUI.Label(new Rect(206, 308, 180, 24),T("HOTKEYS", "КЛАВИШИ"),sectionTitleStyle);
    GUI.Label(new Rect(206, 337, 200, 22),T("F1 - Toggle Menu", "F1 - Открыть меню"),labelStyle);
    GUI.Label(new Rect(438, 337, 200, 22),T("F6 - Skip Phone", "F6 - Пропуск телефона"),labelStyle);
}

private void ShowNotification(string text)
{
    notificationText = text;
    notificationTimer = 3f;
}

private void DrawGlobalNotification()
{
    if (notificationTimer <= 0f)
    return;
    notificationTimer -= Time.unscaledDeltaTime;
    GUI.Box(new Rect(Screen.width - 340,40,310,50),"",sectionStyle);
    GUI.Label(new Rect(Screen.width - 310,50,290,30),notificationText,labelStyle);
}

private void DrawCurrency()
{
    GUI.Label(new Rect(196, 86, 260, 28),T("Diamonds", "Алмазы"),titleStyle);
    GUI.Label(new Rect(206, 154, 310, 22),T("Amount", "Количество"),labelStyle);
    diamondsInput = GUI.TextField(new Rect(206, 184, 328, 38),diamondsInput,fieldStyle);

    if (GUI.Button(new Rect(546, 184, 96, 38) ,T("Apply", "Применить"),buttonStyle))
    {
        if (int.TryParse(diamondsInput,out int amount))
        {
            Utilities.AwardDiamonds(amount,false);
        }
    }
}

private void DrawGirls()
{
    GUI.Label(new Rect(196, 86, 260, 28),T("Girls", "Девушки"),titleStyle);
    DrawGirlControls();
    DrawJobControls();
}

private void DrawGirlControls()
{
    if (GUI.Button(new Rect(204, 142, 210, 36),T( "Level Up Current Girl", "Повысить уровень девушки" ),buttonStyle))
    {
        if (Girls.CurrentGirl != null)
        {
            Girls.CurrentGirl.AdvanceRelationship();
        }
    }
    if (GUI.Button(new Rect(424, 142, 210, 36),T( "Complete Current Girl", "Завершить девушку" ),buttonStyle))
    {
        if (Girls.CurrentGirl != null)
        {
            try
            {
                for (int i = 0; i < 25; i++)
                {
                    try
                    {
                        Girls.CurrentGirl.AdvanceRelationship();
                    }
                    catch (Exception ex)
                    {
                        LogWarning("Failed to advance current girl while completing", ex);
                        break;
                    }
                }
                ShowNotification(T("Girl completed","Девушка завершена"));
            }
        catch (Exception ex)
        {
            LogWarning("Failed to complete current girl", ex);
        }
        }
    }
    if (GUI.Button(new Rect(204, 192, 210, 36),T("Max Hearts", "Макс сердца"),buttonStyle))
    {
        if (Girls.CurrentGirl != null)
        {
            Girls.CurrentGirl.Hearts = 999999999;
        }
    }
    if (GUI.Button(new Rect(424, 192, 210, 36),T( "Unlock All Girls", "Разблокировать всех" ),buttonStyle))
    {
        Girls girlsInstance = UnityEngine.Object.FindObjectOfType<Girls>();
        if (girlsInstance != null)
        {
            Balance.GirlName newestGirl = System.Enum.GetValues(typeof(Balance.GirlName)).Cast<Balance.GirlName>().Where(g => (int)g < 1000).OrderByDescending(g => (int)g).FirstOrDefault();
            int failedUnlocks = 0;
            Exception lastUnlockError = null;
            for (
                int i = 1;
                i <= (int)newestGirl;
                i++
            )
            {
                try
                {
                    HarmonyLib.Traverse.Create(girlsInstance).Method("UnlockGirl", i).GetValue();
                }
                catch (Exception ex)
                {
                    failedUnlocks++;
                    lastUnlockError = ex;
                }
            }
        if (failedUnlocks > 0 && lastUnlockError != null)
        {
            LogWarning($"Failed to unlock {failedUnlocks} girls", lastUnlockError);
        }
        ShowNotification(T("All girls unlocked","Все девушки разблокированы"));
        }
    }
    if (GUI.Button(new Rect(204, 242, 210, 36),T( "Set All Girls To Lover", "Сделать всех любовницами" ),buttonStyle))
    {
        Balance.GirlName newestGirl =System.Enum.GetValues(typeof(Balance.GirlName)).Cast<Balance.GirlName>().Where(g => (int)g < 1000).OrderByDescending(g => (int)g).FirstOrDefault();
        int failedGirls = 0;
        Exception lastGirlError = null;
        for (
            int i = 1;
            i <= (int)newestGirl;
            i++
        )
        {
            try
            {
                Girl girl = HarmonyLib.Traverse.Create(typeof(Girl)).Method("FindGirl",(Balance.GirlName)i).GetValue<Girl>();

                if (girl != null)
                {
                    HarmonyLib.Traverse.Create(girl).Method("SetLove",Girl.LoveLevel.Lover).GetValue();
                }
            }
            catch (Exception ex)
            {
                failedGirls++;
                lastGirlError = ex;
            }
        }
    if (failedGirls > 0 && lastGirlError != null)
    {
        LogWarning($"Failed to set {failedGirls} girls to Lover", lastGirlError);
    }
    ShowNotification( T( "All girls set to Lover", "Все девушки стали любовницами" ) );
    }
    if (GUI.Button(new Rect(424, 242, 210, 36),
        autoPhoneSkip
            ? T( "Stop Phone Skip", "Остановить пропуск" )
            : T( "Auto Phone Skip", "Авто пропуск телефона" ),
        buttonStyle))
    {
        autoPhoneSkip = !autoPhoneSkip;
        autoPhoneSkipErrorLogged = false;
        savedAutoPhoneSkip.Value = autoPhoneSkip;
    }
}

private void DrawJobControls()
{
    if (GUI.Button(new Rect(204, 310, 210, 36),T( "Max All Jobs", "Макс все работы" ),buttonStyle))
    {
        try
        {
            Job2[] jobs =UnityEngine.Object.FindObjectsOfType<Job2>();
            int count = 0;
            int failedJobs = 0;
            Exception lastJobError = null;
            foreach (Job2 job in jobs)
            {
                try
                {
                    HarmonyLib.Traverse.Create(job).Method("Unlock").GetValue();
                    job.Level =job.MaxLevel;
                    HarmonyLib.Traverse.Create(job).Field("experience").SetValue(0L);
                    if (!job.Gilded)
                    {
                        job.AddMultiplierSilent();
                    }
                    HarmonyLib.Traverse.Create(job).Method("EnableJob").GetValue();
                    HarmonyLib.Traverse.Create(job).Method("UpdateInfo").GetValue();
                    job.StoreState();
                    count++;
                }
                catch (Exception ex)
                {
                    failedJobs++;
                    lastJobError = ex;
                }
            }
if (failedJobs > 0 && lastJobError != null)
{
    LogWarning($"Failed to max {failedJobs} jobs", lastJobError);
}
ShowNotification(T($"Maxed {count} jobs",$"Прокачано работ: {count}"));
        }
        catch (Exception ex)
        {
            LogWarning("Failed to max all jobs", ex);
        }
    }
    if (GUI.Button(new Rect(424, 310, 210, 36),T( "Max Hobby Levels", "Макс уровни хобби" ),buttonStyle))
    {
        try
        {
            int failedHobbies = 0;
            Exception lastHobbyError = null;
            foreach (Hobby2 hobby in UnityEngine.Object.FindObjectsOfType<Hobby2>())
            {
                try
                {
                    hobby.Unlock(false);
                    hobby.Level.Value = Hobby2.MaxLevel;
                    hobby.StoreState();
                    hobby.UpdateText();
                }
                catch (Exception ex)
                {
                    failedHobbies++;
                    lastHobbyError = ex;
                }
            }
        if (failedHobbies > 0 && lastHobbyError != null)
        {
            LogWarning($"Failed to max {failedHobbies} hobbies", lastHobbyError);
        }
        ShowNotification(T( "All hobbies maxed", "Все хобби прокачаны" ));
        }
        catch (Exception ex)
        {
            LogWarning("Failed to max all hobbies", ex);
        }
    }
}

private void DrawStats()
{
    GUI.Label(new Rect(196, 86, 260, 28),T("Stats", "Статы"),titleStyle);
    GUI.Label(new Rect(206, 140, 310, 22),T("Time Scale", "Скорость времени"),labelStyle);
    timeScaleInput = GUI.TextField(new Rect(206, 170, 328, 38),timeScaleInput,fieldStyle);
    if (GUI.Button(new Rect(546, 170, 96, 38),T("Apply", "Применить"),buttonStyle))
    {
        if (float.TryParse(timeScaleInput,out float parsedScale))
        {
            Time.timeScale =Mathf.Clamp(parsedScale,1f,500f);
        }
    }
    GUI.Label(new Rect(206, 240, 310, 22),T("Reset Boost", "Буст сброса"),labelStyle);
    resetBoostInput = GUI.TextField(new Rect(206, 270, 328, 38),resetBoostInput,fieldStyle);
    if (GUI.Button(new Rect(546, 270, 96, 38),T("Apply", "Применить"),buttonStyle))
    {
        if (float.TryParse(resetBoostInput,out float parsedBoost))
        {
            GameState.CurrentState.TimeMultiplier.Value =Mathf.Clamp(parsedBoost,1f,2048f);
        }
    }
    if (GUI.Button(new Rect(206, 330, 436, 36),T("Reset Time Scale","Сбросить скорость времени"),buttonStyle))
    {
        Time.timeScale = 1f;
        timeScaleInput = "1";
    }
}

private Texture2D CreateTexture(Color col)
{
    Texture2D tex = new Texture2D(2, 2);
    Color[] colors = {col, col, col, col};
    tex.SetPixels(colors);
    tex.hideFlags = HideFlags.HideAndDontSave;
    tex.Apply();
    return tex;
}

public override void OnInitializeMelon()
{
    configCategory =MelonPreferences.CreateCategory("CrushCore");
    posX =configCategory.CreateEntry("WindowPosX",170f);
    posY =configCategory.CreateEntry("WindowPosY",90f);
    savedAutoPhoneSkip =configCategory.CreateEntry("AutoPhoneSkip",false);
    savedTab =configCategory.CreateEntry("SelectedTab",0);
    windowRect.x = posX.Value;
    windowRect.y = posY.Value;
    autoPhoneSkip =savedAutoPhoneSkip.Value;
    selectedTab =savedTab.Value;
}
}

public static class RuntimeInstances
{
    public static Cellphone cellphoneInstance;
}

[HarmonyPatch(typeof(Cellphone), "Update")]

public class Cellphone_Update_Patch
{
    [HarmonyPrefix]
    static void Prefix(Cellphone __instance)
    {
        RuntimeInstances.cellphoneInstance = __instance;
    }
}
