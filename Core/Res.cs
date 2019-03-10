using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class Res
    {
        public Audio Audio { get; private set; }
        public Tiles Tiles { get; private set;}
        public SpriteFont Font { get; private set; }
        public SpriteFont Font2 { get; private set; }
        ContentManager Content;


        public string SprGuy = "SprGuy";
        public string SprGrassTiles = "SprGrassTiles";
        public string SprHurdle = "SprHurdle";
        public string SprSun = "SprSun";
        public string SprBackPar = "SprBackPar";
        public string SprCoin = "SprCoin";
        public string SfxJump = "jump";
        public string SfxFail = "fail";
        public string SfxCrash = "crash";
        public string SfxGet = "get";
        public string SfxWhiroop = "whiroop";
        public string SprGuyDown = "SprGuyDown";
        public string SfxWoWoWoWo= "wowowowo";
        public string SprNewRecord = "SprNewRecord";
        public string SprNumberBk = "SprNumberBk";
        public string SprCloud = "SprCloud";
        public string SfxLand = "land";
        public string SfxCoinGet = "coinget";
        public string SfxBlip = "blip";
        public string SprTitle = "SprTitle";
        public string SprBackdrop = "SprBackdrop";
        public string SfxNice = "nice";
        public string SprNice = "SprNice";

        public Res(ContentManager c)
        {
            Content = c;
            Audio = new Audio();
            Tiles = new Tiles();

        }
        public void Load(GraphicsDevice d)
        {
            Font = Content.Load<SpriteFont>("Font");
            Font2 = Content.Load<SpriteFont>("Font2");

            Tiles.Texture = Content.Load<Texture2D>("tiles12x12"); ;
            Tiles.AddSprite(SprCoin, new List<Rectangle>() {
               new Rectangle(3, 0, 1, 1),  new Rectangle(4, 0, 1, 1), new Rectangle(5, 0, 1, 1), new Rectangle(4, 0, 1, 1)}, 0.4f);

            Tiles.AddSprite(SprBackPar, new List<Rectangle>() {
               new Rectangle(0, 9, 2, 3),  new Rectangle(0, 12, 2, 3), new Rectangle(0, 15, 2, 3)}, 0.6f);

            Tiles.AddSprite(SprNewRecord, new List<Rectangle>() {
               new Rectangle(8, 2, 11, 2)}, 0.6f);

            Tiles.AddSprite(SprNice, new List<Rectangle>() {
               new Rectangle(8, 0, 5, 2)}, 0.6f);

            Tiles.AddSprite(SprCloud, new List<Rectangle>() {
               new Rectangle(2,8, 2, 1)}, 0.6f);

            Tiles.AddSprite(SprNumberBk, new List<Rectangle>() {
               new Rectangle(2, 12, 1, 1)}, 0.6f);

            Tiles.AddSprite(SprBackdrop, new List<Rectangle>() {
               new Rectangle(0, 2, 1, 7)}, 0.6f);

            Tiles.AddSprite(SprTitle, new List<Rectangle>() {
               new Rectangle(0, 18, 8, 5)}, 0.6f);

            Tiles.AddSprite(SprSun, new List<Rectangle>() {
               new Rectangle(1, 7, 1, 1),  new Rectangle(1, 8, 1, 1)}, 0.6f);
            Tiles.AddSprite(SprGuy, new List<Rectangle>() {
               new Rectangle(1, 0, 1, 1),  new Rectangle(0, 0, 1, 1), new Rectangle(1, 0, 1, 1), new Rectangle(2, 0, 1, 1) }, 0.6f);
            Tiles.AddSprite(SprGuyDown, new List<Rectangle>() {
               new Rectangle(3, 0, 1, 1)}, 0.0f);

            Tiles.AddSprite(SprHurdle, new List<Rectangle>() {
               new Rectangle(2, 10, 1, 1),  new Rectangle(2, 11, 1, 1), new Rectangle(2, 12, 1, 1),
               new Rectangle(3, 10, 1, 1),  new Rectangle(3, 11, 1, 1), new Rectangle(3, 12, 1, 1),//Green
            }, 0.6f);
            Tiles.AddSprite(SprGrassTiles, new List<Rectangle>() {
                new Rectangle(1, 1, 1, 1),
                new Rectangle(2, 1, 1, 1),
                new Rectangle(3, 1, 1, 1),
                new Rectangle(1, 2, 1, 1),
                new Rectangle(2, 2, 1, 1),
                new Rectangle(3, 2, 1, 1),
                new Rectangle(1, 3, 1, 1),
                new Rectangle(2, 3, 1, 1),
                new Rectangle(3, 3, 1, 1),
            }, 0.6f);

            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxJump));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxFail));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxCrash));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxGet));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxWhiroop));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxWoWoWoWo));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxLand));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxCoinGet));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxBlip));
            Audio.Sounds.Add(Content.Load<SoundEffect>(SfxNice));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxBoom));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxClick1));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxClick2));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxPlace));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxWank));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxChimes));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxChimesFast));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxWarp));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxWarpHi));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxWarpLow));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxDead));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxNextLevel));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxLowThump));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxHiThump));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxMoney1));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxMoney2));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxMoney3));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxMeow));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxCatDie));
            //Audio.Songs.Add(Content.Load<Song>(Resources.Song5));
            //Audio.Songs.Add(Content.Load<Song>(Resources.Song9));
            //Audio.Songs.Add(Content.Load<Song>(Resources.Song0));
            //Audio.Songs.Add(Content.Load<Song>(Resources.SongIntro));
            //Audio.Songs.Add(Content.Load<Song>(Resources.SongBoss));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxHealth));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxSword1));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxSword2));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxWand1));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxWand2));
            //Audio.Sounds.Add(Content.Load<SoundEffect>(Resources.SfxGalaxy));

            //Tiles.S

        }
    }
}
