using System;
using UnityEngine;

namespace HURTHax
{
    public static class Drawing
    {
        [Flags]
        public enum TextFlags
        {
            TEXT_FLAG_NONE = 0,
            TEXT_FLAG_CENTERED = 1,
            TEXT_FLAG_OUTLINED = 2,
            TEXT_FLAG_DROPSHADOW = 3
        }

        private static Texture2D Texture2D;
        private static Color Texture2DColor;

        public static void DrawString(Vector2 pos, Color color, TextFlags flags, string text)
        {
            var center = (flags & TextFlags.TEXT_FLAG_CENTERED) == TextFlags.TEXT_FLAG_CENTERED;
            if ((flags & TextFlags.TEXT_FLAG_OUTLINED) == TextFlags.TEXT_FLAG_OUTLINED)
            {
                PrivateDrawString(pos + new Vector2(1f, 0f), Color.black, text, center);
                PrivateDrawString(pos + new Vector2(0f, 1f), Color.black, text, center);
                PrivateDrawString(pos + new Vector2(0f, -1f), Color.black, text, center);
            }
            if ((flags & TextFlags.TEXT_FLAG_DROPSHADOW) == TextFlags.TEXT_FLAG_DROPSHADOW)
                PrivateDrawString(pos + new Vector2(1f, 1f), Color.black, text, center);
            PrivateDrawString(pos, color, text, center);
        }


        private static void PrivateDrawString(Vector2 pos, Color color, string text, bool center)
        {
            var style = new GUIStyle(GUI.skin.label) {normal = {textColor = color}, fontSize = 13};
            if (center)
                pos.x -= style.CalcSize(new GUIContent(text)).x / 2f;
            //264 default
            GUI.Label(new Rect(pos.x, pos.y, 264f, 20f), text, style);
        }

        public static Vector2 TextBounds(string text)
        {
            var style = new GUIStyle(GUI.skin.label) {fontSize = 13};
            return style.CalcSize(new GUIContent(text));
        }


        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color)
        {
            GL.Begin(1);
            GL.Color(color);
            GL.Vertex3(pointA.x, pointA.y, 0f);
            GL.Vertex3(pointB.x, pointB.y, 0f);
            GL.End();
        }

        public static void DrawBox(Vector2 pos, Vector2 size, Color color)
        {
            if (Texture2D == null)
                Texture2D = new Texture2D(1, 1);
            if (color != Texture2DColor)
            {
                Texture2D.SetPixel(0, 0, color);
                Texture2D.Apply();
                Texture2DColor = color;
            }
            GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), Texture2D);
        }


        public static void DrawBoxOutlines(Vector2 position, Vector2 size, float borderSize, Color color)
        {
            DrawBox(new Vector2(position.x + borderSize, position.y), new Vector2(size.x - 2f * borderSize, borderSize), color);
            DrawBox(new Vector2(position.x, position.y), new Vector2(borderSize, size.y), color);
            DrawBox(new Vector2(position.x + size.x - borderSize, position.y), new Vector2(borderSize, size.y), color);
            DrawBox(new Vector2(position.x + borderSize, position.y + size.y - borderSize), new Vector2(size.x - 2f * borderSize, borderSize), color);
        }


        public static void DrawEspWindow()
        {
            GUI.color = Color.red;
            GUI.Window(0, new Rect(20f, 20f, 235f, 245f), delegate
            {
                BaseSettings.GetSettings.EspSettings.IsEnabled = GUI.Toggle(new Rect(10f, 20, 130f, 20f), BaseSettings.GetSettings.EspSettings.IsEnabled, "ESP");
                BaseSettings.GetSettings.EspSettings.DrawResouces = GUI.Toggle(new Rect(10f, 40f, 130f, 20f), BaseSettings.GetSettings.EspSettings.DrawResouces, "Draw Resouces");
                BaseSettings.GetSettings.EspSettings.DrawPlayers = GUI.Toggle(new Rect(10f, 60f, 130f, 20f), BaseSettings.GetSettings.EspSettings.DrawPlayers, "Draw Players");
                BaseSettings.GetSettings.EspSettings.DrawLootCrates = GUI.Toggle(new Rect(10f, 80f, 130f, 20f), BaseSettings.GetSettings.EspSettings.DrawLootCrates, "Draw Loot Crates");
                BaseSettings.GetSettings.EspSettings.DrawWrecks = GUI.Toggle(new Rect(10f, 100f, 130f, 20f), BaseSettings.GetSettings.EspSettings.DrawWrecks, "Draw Vehicles Bitch");
                BaseSettings.GetSettings.EspSettings.DrawAnimals = GUI.Toggle(new Rect(10f, 120f, 130f, 20f), BaseSettings.GetSettings.EspSettings.DrawAnimals, "Draw Animals");
                BaseSettings.GetSettings.EspSettings.DrawOwnershipStakes = GUI.Toggle(new Rect(10f, 140f, 130f, 20f), BaseSettings.GetSettings.EspSettings.DrawOwnershipStakes, "Draw Ownership Stakes");
                GUI.Label(new Rect(10f, 160f, 130f, 20f), $"Distance: {Math.Round(BaseSettings.GetSettings.EspSettings.Range)}m");
                BaseSettings.GetSettings.EspSettings.Range = GUI.HorizontalSlider(new Rect(10, 180f, 130f, 20f), BaseSettings.GetSettings.EspSettings.Range, 100f, 500f);
            }, "ESP");
            GUI.color = Color.white;
        }

        public static void DrawAimWindow()
        {
            GUI.color = Color.red;
            GUI.Window(0, new Rect(20f, 20f, 235f, 145f), delegate
            {
                BaseSettings.GetSettings.AimBotSettings.IsEnabled = GUI.Toggle(new Rect(10f, 20, 130f, 20f), BaseSettings.GetSettings.AimBotSettings.IsEnabled, "AIMBOT");
                BaseSettings.GetSettings.AimBotSettings.AimAtPlayers = GUI.Toggle(new Rect(10f, 40f, 130f, 20f), BaseSettings.GetSettings.AimBotSettings.AimAtPlayers, "Aim at players");
                BaseSettings.GetSettings.AimBotSettings.AimAtAnimals = GUI.Toggle(new Rect(10f, 60f, 130f, 20f), BaseSettings.GetSettings.AimBotSettings.AimAtAnimals, "Aim at animals");
            }, "AIMBOT");
            GUI.color = Color.white;
        }
    }
}